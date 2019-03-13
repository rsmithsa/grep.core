//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Grep.Core.Console
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Enumeration;
    using System.IO.MemoryMappedFiles;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Grep.Core.ContentProviders;
    using Grep.Core.Formatters;
    using Grep.Core.Matchers;
    using McMaster.Extensions.CommandLineUtils;

    /// <summary>
    /// grep.core's console application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point for grep.core's console application.
        /// </summary>
        /// <param name="args">Command line parameters.</param>
        /// <returns>The return code.</returns>
        public static int Main(string[] args)
        {
            return ConfigureApplication().Execute(args);
        }

        private static CommandLineApplication ConfigureApplication()
        {
            var app = new CommandLineApplication();
            app.Name = "grep.core";
            app.Description = "Intelligent grep in .NET Core";

            app.HelpOption();
            app.ThrowOnUnexpectedArgument = false;
            app.MakeSuggestionsInErrorMessage = true;
            app.VersionOptionFromAssemblyAttributes(Assembly.GetExecutingAssembly());

            var regexp = app.Option("-e|--regexp", "Pattern to search for", CommandOptionType.SingleValue).IsRequired();
            var isSimplePattern = app.Option("-F|--fixed-strings", "Interpret pattern as a fixed string not a regular expression", CommandOptionType.NoValue);
            var recurse = app.Option("-r|--recursive", "Read all files under each directory, recursively", CommandOptionType.NoValue);
            var ignoreCase = app.Option("-i|--ignore-case", "Ignore case distinctions in both the pattern and the input files", CommandOptionType.NoValue);
            var listFileMatches = app.Option("-l|--files-with-matches", $"Suppress normal output; instead print the name of each input file{Environment.NewLine}from which output would normally have been printed", CommandOptionType.NoValue);
            var ignoreBinary = app.Option("-I", "Process a binary file as if it did not contain matching data", CommandOptionType.NoValue);
            var excludeDir = app.Option("--exclude-dir", "Exclude directories matching the pattern from recursive searches", CommandOptionType.SingleValue);

            var file = app.Argument("FILE", "Input files to search", multipleValues: true).IsRequired();

            app.OnExecute(async () =>
            {
                var sw = Stopwatch.StartNew();

                var matcher = isSimplePattern.HasValue() ? (ITextMatcher)new SimpleMatcher(regexp.Value(), ignoreCase.HasValue()) : (ITextMatcher)new RegexMatcher(regexp.Value(), ignoreCase.HasValue());

                var formatter = new DefaultFormatter();

                var results = new ResultInfo();

                var printTask = PrintResults(results, formatter, listFileMatches.HasValue());

                var processTask = ProcessFiles(file.Values, matcher, results, recurse.HasValue(), ignoreBinary.HasValue(), excludeDir.Value());

                await Task.WhenAll(printTask, processTask).ContinueWith(t =>
                {
                    sw.Stop();
                    Write($"{results.MatchedFiles} file(s)", ConsoleColor.Yellow);
                    Write(" with ", ConsoleColor.DarkGray);
                    Write($"{results.TotalMatches} match(es)", ConsoleColor.Blue);
                    WriteLine($" in {results.TotalFiles} file(s) in {sw.Elapsed:g}", ConsoleColor.DarkGray);

                    if (Debugger.IsAttached)
                    {
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                    }
                });
            });

            return app;
        }

        private static Task PrintResults(ResultInfo results, IMatchFormatter formatter, bool listFileMatches)
        {
            return Task.Run(() =>
            {
                while (!results.Results.IsCompleted)
                {
                    try
                    {
                        var data = results.Results.Take();

                        lock (Console.Out)
                        {
                            if (data.error != null)
                            {
                                Write(data.fileName, ConsoleColor.Red);
                                Write(" - ", ConsoleColor.DarkGray);
                                Write(data.error, ConsoleColor.DarkGray);
                            }
                            else
                            {
                                results.MatchedFiles++;
                                results.TotalMatches += data.matches.Count;
                                Write(data.fileName, ConsoleColor.Yellow);
                                Write(" - ", ConsoleColor.DarkGray);
                                Write($"{data.matches.Count} match(es)", ConsoleColor.Blue);
                                if (!listFileMatches)
                                {
                                    WriteLine(":", ConsoleColor.DarkGray);
                                    foreach (var match in data.matches)
                                    {
                                        var formatted = formatter.FormatMatch(match);
                                        Write(formatted);
                                    }
                                }
                            }

                            Console.WriteLine();
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        // Take() was called on a completed collection.
                        // We will break out on the next iteration.
                    }
                }
            });
        }

        private static Task ProcessFiles(IEnumerable<string> filePatterns, ITextMatcher matcher, ResultInfo results, bool recurse, bool ignoreBinary, string excludeDir)
        {
            var tasks = new ConcurrentBag<Task>();

            var dirRegex = string.IsNullOrEmpty(excludeDir) ? null : new Regex(excludeDir, RegexOptions.Compiled);

            foreach (var filePattern in filePatterns)
            {
                var (pathToSearch, files) = FileProvider.EnumerateFiles(filePattern, recurse, dirRegex);

                Parallel.ForEach(files, (fileInfo) =>
                {
                    Interlocked.Increment(ref results.TotalFiles);
                    if (fileInfo.Length == 0)
                    {
                        return;
                    }

                    MemoryMappedFile mmf;
                    try
                    {
                        mmf = MemoryMappedFile.CreateFromFile(fileInfo.Path, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
                    }
                    catch (IOException ex)
                    {
                        results.Results.Add((Path.GetRelativePath(pathToSearch, fileInfo.Path), null, ex.Message));
                        return;
                    }

                    MemoryMappedViewStream stream;
                    try
                    {
                        stream = mmf.CreateViewStream(0, fileInfo.Length, MemoryMappedFileAccess.Read);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        results.Results.Add((Path.GetRelativePath(pathToSearch, fileInfo.Path), null, ex.Message));
                        return;
                    }

                    if (ignoreBinary && FileProvider.IsBinary(stream, fileInfo.Length))
                    {
                        return;
                    }

                    var content = new StreamContentProvider(stream);

                    var task = matcher.GetMatches(content).ContinueWith(matches =>
                    {
                        stream.Dispose();
                        mmf.Dispose();

                        if (matches.Result.Count > 0)
                        {
                            results.Results.Add((Path.GetRelativePath(pathToSearch, fileInfo.Path), matches.Result, null));
                        }
                    });

                    tasks.Add(task);
                });
            }

            return Task.WhenAll(tasks).ContinueWith(t =>
            {
                results.Results.CompleteAdding();
                return;
            });
        }

        private static void Write(FormattedMatch formattedMatch)
        {
            Write(formattedMatch.Match.Line.ToString(), ConsoleColor.Blue);
            Write(":", ConsoleColor.DarkGray);
            Write(formattedMatch.Match.Index.ToString(), ConsoleColor.Blue);

            Console.Write(" - ");

            foreach (var formattedSpan in formattedMatch.Spans)
            {
                Write(formattedSpan.Text, formattedSpan.Colour);
            }

            Console.WriteLine();
        }

        private static void Write(string text, ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            Console.Write(text);
            Console.ResetColor();
        }

        private static void WriteLine(string text, ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private class ResultInfo
        {
#pragma warning disable SA1401 // Fields should be private
            public int MatchedFiles = 0;
            public int TotalMatches = 0;
            public int TotalFiles = 0;
#pragma warning restore SA1401 // Fields should be private

            public BlockingCollection<(string fileName, IList<GrepMatch> matches, string error)> Results { get; } = new BlockingCollection<(string fileName, IList<GrepMatch> matches, string error)>();
        }
    }
}