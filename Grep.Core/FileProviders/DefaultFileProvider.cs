//-----------------------------------------------------------------------
// <copyright file="DefaultFileProvider.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Grep.Core.FileProviders
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Enumeration;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The default <see cref="IFileProvider"/> which provides files to be searched.
    /// </summary>
    public class DefaultFileProvider : IFileProvider
    {
        private readonly string searchFile;
        private readonly bool recurse;
        private readonly Regex? excludeDir;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultFileProvider"/> class.
        /// </summary>
        /// <param name="filePattern">The file pattern.</param>
        /// <param name="recurse">Specifies recursive file enumeration.</param>
        /// <param name="excludedDirPattern">The excluded directory pattern.</param>
        public DefaultFileProvider(string? filePattern, bool recurse, string? excludedDirPattern)
        {
            if (filePattern == null)
            {
                throw new ArgumentNullException(nameof(filePattern));
            }

            this.SearchPath = Path.GetDirectoryName(filePattern);
            this.SearchPath = string.IsNullOrEmpty(this.SearchPath) ? "." : this.SearchPath;

            this.searchFile = Path.GetFileName(filePattern);
            this.recurse = recurse;
            this.excludeDir = string.IsNullOrEmpty(excludedDirPattern) ? null : new Regex(excludedDirPattern, RegexOptions.Compiled);
        }

        /// <inheritdoc />
        public string SearchPath { get; }

        /// <inheritdoc />
        public FileSystemEnumerable<(string Path, long Length)> EnumerateFiles()
        {
            var options = new EnumerationOptions()
                              {
                                  RecurseSubdirectories = this.recurse,
                                  AttributesToSkip = 0,
                              };

            FileSystemEnumerable<(string Path, long Length)> files;
            if (this.excludeDir == null)
            {
                files = new FileSystemEnumerable<(string Path, long Length)>(this.SearchPath, (ref FileSystemEntry entry) => (entry.ToFullPath(), entry.Length), options)
                            {
                                ShouldIncludePredicate = (ref FileSystemEntry entry) => !entry.IsDirectory && FileSystemName.MatchesSimpleExpression(this.searchFile, entry.FileName),
                            };
            }
            else
            {
                files = new FileSystemEnumerable<(string Path, long Length)>(this.SearchPath, (ref FileSystemEntry entry) => (entry.ToFullPath(), entry.Length), options)
                            {
                                ShouldIncludePredicate = (ref FileSystemEntry entry) => !entry.IsDirectory && FileSystemName.MatchesSimpleExpression(this.searchFile, entry.FileName) && !this.excludeDir.IsMatch(entry.Directory.ToString()),
                            };
            }

            return files;
        }
    }
}
