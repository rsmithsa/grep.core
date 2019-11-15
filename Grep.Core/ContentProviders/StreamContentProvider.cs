//-----------------------------------------------------------------------
// <copyright file="StreamContentProvider.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Grep.Core.ContentProviders
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// An <see cref="ITextContentProvider"/> which wraps a <see cref="StreamReader"/>.
    /// </summary>
    public sealed class StreamContentProvider : ITextContentProvider, IDisposable
    {
        private readonly long contentLength;

        private bool? isBinary;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamContentProvider"/> class.
        /// </summary>
        /// <param name="input">The input stream.</param>
        public StreamContentProvider(Stream input)
            : this(input, input?.Length ?? 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamContentProvider"/> class.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="contentLength">The content length in the stream.</param>
        public StreamContentProvider(Stream input, long contentLength)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            this.Content = new StreamReader(input);
            this.contentLength = contentLength;
        }

        /// <inheritdoc />
        public TextReader Content { get; }

        /// <inheritdoc />
        public bool IsBinary
        {
            get
            {
                if (this.isBinary.HasValue)
                {
                    return this.isBinary.Value;
                }

                this.isBinary = this.IsStreamReaderBinary((StreamReader)this.Content, this.contentLength);

                return this.isBinary.Value;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Content?.Dispose();
        }

        private bool IsStreamReaderBinary(StreamReader streamReader, long fileLength)
        {
            var originalPosition = streamReader.BaseStream.Position;

            // Heuristic for binary files
            Span<byte> buffer = stackalloc byte[128];
            streamReader.BaseStream.Read(buffer);

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

            streamReader.BaseStream.Position = originalPosition;

            return false;
        }
    }
}
