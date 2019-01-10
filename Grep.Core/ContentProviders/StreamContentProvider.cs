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
    public class StreamContentProvider : ITextContentProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamContentProvider"/> class.
        /// </summary>
        /// <param name="input">The input stream.</param>
        public StreamContentProvider(Stream input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            this.Content = new StreamReader(input);
        }

        /// <inheritdoc />
        public TextReader Content { get; }
    }
}
