//-----------------------------------------------------------------------
// <copyright file="IFileProvider.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Grep.Core.FileProviders
{
    using System;
    using System.Collections.Generic;
    using System.IO.Enumeration;
    using System.Text;

    /// <summary>
    /// Interface for a file provider.
    /// </summary>
    public interface IFileProvider
    {
        /// <summary>
        /// Gets the search path.
        /// </summary>
        string SearchPath { get; }

        /// <summary>
        /// Enumerates the files.
        /// </summary>
        /// <returns>A <see cref="FileSystemEnumerable{T}"/> of the file paths and lengths.</returns>
        FileSystemEnumerable<(string Path, long Length)> EnumerateFiles();
    }
}
