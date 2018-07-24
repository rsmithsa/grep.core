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

    public class StringContentProvider : ITextContentProvider
    {
        public StringContentProvider(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            this.Content = new StringReader(text);
        }

        public TextReader Content { get; private set; }
    }
}
