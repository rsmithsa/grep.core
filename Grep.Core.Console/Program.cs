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
    using System.IO.MemoryMappedFiles;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Grep.Core.ContentProviders;
    using Grep.Core.Matchers;
    using McMaster.Extensions.CommandLineUtils;

    public class Program
    {
        public static int Main(string[] args)
        {
            return ConfigureApplication().Execute(args);
        }

        private static CommandLineApplication ConfigureApplication()
        {
            var app = new CommandLineApplication();
            app.Name = "grep.core";
            app.Description = "Intelligent grep in .NET Core";

            var regexp = app.Option("-e|--regexp", "Pattern to search for", CommandOptionType.SingleValue).IsRequired();
            var isSimplePattern = app.Option("-F|--fixed-strings", "Interpret pattern as a fixed string not a regular expression", CommandOptionType.NoValue);
            var recurse = app.Option("-r|--recursive", "Read all files under each directory, recursively", CommandOptionType.NoValue);
            var ignoreCase = app.Option("-i|--ignore-case", "Ignore case distinctions in both the pattern and the input files", CommandOptionType.NoValue);
            var listFileMatches = app.Option("-l|--files-with-matches", "Suppress normal output; instead print the name of each input file from which output would normally have been printed", CommandOptionType.NoValue);

            var file = app.Argument("FILE", "Input files to search", multipleValues: true).IsRequired();

            app.HelpOption();
            app.VersionOptionFromAssemblyAttributes(Assembly.GetExecutingAssembly());

            app.OnValidationError(x =>
            {
                Console.Error.WriteLine(x.ErrorMessage);

                Console.WriteLine();
                app.ShowHelp();
            });

            app.OnExecute(() =>
            {
                var sw = Stopwatch.StartNew();

                var matcher = isSimplePattern.HasValue() ? (ITextMatcher)new SimpleMatcher(regexp.Value(), ignoreCase.HasValue()) : (ITextMatcher)new RegexMatcher(regexp.Value(), ignoreCase.HasValue());
                var map = ProcessFiles(file.Values, matcher, recurse.HasValue(), listFileMatches.HasValue()).Result;

                sw.Stop();
                Write($"{map.Values.Count(x => x.matches?.Count > 0)} file(s)", ConsoleColor.Yellow);
                Write(" with ", ConsoleColor.DarkGray);
                Write($"{map.Values.Select(x => x.matches?.Count).Sum()} match(es)", ConsoleColor.Blue);
                WriteLine($" in {map.Count} file(s) in {sw.Elapsed:g}", ConsoleColor.DarkGray);

                if (Debugger.IsAttached)
                {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey(true);
                }
            });

            return app;
        }

        private static Task<ConcurrentDictionary<string, (FileInfo fileInfo, IList<GrepMatch> matches)>> ProcessFiles(IEnumerable<string> filePatterns, ITextMatcher matcher, bool recurse, bool listFileMatches)
        {
            var map = new ConcurrentDictionary<string, (FileInfo fileInfo, IList<GrepMatch> matches)>();
            var tasks = new ConcurrentBag<Task>();
            foreach (var filePattern in filePatterns)
            {
                var pathToSearch = Path.GetDirectoryName(filePattern);
                pathToSearch = string.IsNullOrEmpty(pathToSearch) ? "." : pathToSearch;
                var fileToSearch = Path.GetFileName(filePattern);
                var files = Directory.EnumerateFiles(pathToSearch, fileToSearch, recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                Parallel.ForEach(files, (fileName) =>
                {
                    var info = new FileInfo(fileName);
                    if (info.Length == 0)
                    {
                        map[fileName] = (info, null);
                        return;
                    }

                    var mmf = MemoryMappedFile.CreateFromFile(fileName, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
                    var stream = mmf.CreateViewStream(0, info.Length, MemoryMappedFileAccess.Read);
                    var content = new StreamContentProvider(stream);
                    var task = matcher.GetMatches(content).ContinueWith(matches =>
                    {
                        stream.Close();
                        mmf.Dispose();

                        map[fileName] = (info, matches.Result);
                        if (matches.Result.Count > 0)
                        {
                            lock (Console.Out)
                            {
                                Write(Path.GetRelativePath(pathToSearch, fileName), ConsoleColor.Yellow);
                                Write(" - ", ConsoleColor.DarkGray);
                                Write($"{matches.Result.Count} match(es)", ConsoleColor.Blue);
                                if (listFileMatches == false)
                                {
                                    WriteLine(":", ConsoleColor.DarkGray);
                                    foreach (var match in matches.Result)
                                    {
                                        Write(match.Line.ToString(), ConsoleColor.Blue);
                                        Write(":", ConsoleColor.DarkGray);
                                        Write(match.Index.ToString(), ConsoleColor.Blue);

                                        var preMatch = match.Context.Substring(0, match.Index - 1);
                                        var postMatch = match.Context.Substring(match.Index + match.Value.Length - 1);

                                        Console.Write(" - ");
                                        Write(preMatch.TrimStart(), ConsoleColor.DarkGray);
                                        Write(match.Value, ConsoleColor.Blue);
                                        WriteLine(postMatch.TrimEnd(), ConsoleColor.DarkGray);
                                    }
                                }

                                Console.WriteLine();
                            }
                        }
                    });

                    tasks.Add(task);
                });
            }

            return Task.WhenAll(tasks).ContinueWith(t => map);
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
    }
}