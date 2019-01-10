//-----------------------------------------------------------------------
// <copyright file="StringContentProvider.cs" company="Richard Smith">
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
    /// An <see cref="ITextContentProvider"/> which wraps a <see cref="StringReader"/>.
    /// </summary>
    public class StringContentProvider : ITextContentProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringContentProvider"/> class.
        /// </summary>
        /// <param name="text">The text content.</param>
        public StringContentProvider(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            this.Content = new StringReader(text);
        }

        /// <inheritdoc />
        public TextReader Content { get; }
    }
}
