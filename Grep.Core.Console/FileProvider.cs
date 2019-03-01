//-----------------------------------------------------------------------
// <copyright file="FileProvider.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Grep.Core.Console
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Enumeration;
    using System.IO.MemoryMappedFiles;
    using System.Reflection.Metadata.Ecma335;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Grep.Core.ContentProviders;

    public static class FileProvider
    {
        public static (string SearchPath, FileSystemEnumerable<(string Path, long Length)> Files) EnumerateFiles(string filePattern, bool recurse, Regex dirRegex)
        {
            var pathToSearch = Path.GetDirectoryName(filePattern);
            pathToSearch = string.IsNullOrEmpty(pathToSearch) ? "." : pathToSearch;
            var fileToSearch = Path.GetFileName(filePattern);

            var options = new EnumerationOptions()
            {
                RecurseSubdirectories = recurse,
                AttributesToSkip = 0,
            };

            FileSystemEnumerable<(string Path, long Length)> files;
            if (dirRegex == null)
            {
                files = new FileSystemEnumerable<(string Path, long Length)>(pathToSearch, (ref FileSystemEntry entry) => (entry.ToFullPath(), entry.Length), options)
                {
                    ShouldIncludePredicate = (ref FileSystemEntry entry) => !entry.IsDirectory && FileSystemName.MatchesSimpleExpression(fileToSearch, entry.FileName),
                };
            }
            else
            {
                files = new FileSystemEnumerable<(string Path, long Length)>(pathToSearch, (ref FileSystemEntry entry) => (entry.ToFullPath(), entry.Length), options)
                {
                    ShouldIncludePredicate = (ref FileSystemEntry entry) => !entry.IsDirectory && FileSystemName.MatchesSimpleExpression(fileToSearch, entry.FileName) && !dirRegex.IsMatch(entry.Directory.ToString()),
                };
            }

            return (pathToSearch, files);
        }

        public static bool IsBinary(Stream stream, long fileLength)
        {
            var originalPosition = stream.Position;

            // Heuristic for binary files
            Span<byte> buffer = stackalloc byte[128];
            stream.Read(buffer);

            for (int i = 0; i < buffer.Length; i++)
            {
                if (i == fileLength)
                {
                    return false;
                }

                if (buffer[i] == 0)
                {
                    // Assume file with NUL is binary;
                    return true;
                }
            }

            stream.Position = originalPosition;

            return false;
        }
    }
}
