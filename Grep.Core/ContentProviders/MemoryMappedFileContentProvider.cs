//-----------------------------------------------------------------------
// <copyright file="MemoryMappedFileContentProvider.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Grep.Core.ContentProviders
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.MemoryMappedFiles;
    using System.Text;

    /// <summary>
    /// An <see cref="ITextContentProvider"/> which wraps a <see cref="StreamReader"/>.
    /// </summary>
    public sealed class MemoryMappedFileContentProvider : ITextContentProvider, IDisposable
    {
        private readonly MemoryMappedFile mmf;
        private readonly MemoryMappedViewStream stream;
        private readonly StreamContentProvider streamContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryMappedFileContentProvider"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="fileLength">The file length.</param>
        public MemoryMappedFileContentProvider(string filePath, long fileLength)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            this.mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
            this.stream = this.mmf.CreateViewStream(0, fileLength, MemoryMappedFileAccess.Read);
            this.streamContent = new StreamContentProvider(this.stream, fileLength);
        }

        /// <inheritdoc />
        public TextReader Content => this.streamContent.Content;

        /// <inheritdoc />
        public bool IsBinary => this.streamContent.IsBinary;

        /// <inheritdoc />
        public void Dispose()
        {
            this.streamContent?.Dispose();
            this.stream?.Dispose();
            this.mmf?.Dispose();
        }
    }
}
