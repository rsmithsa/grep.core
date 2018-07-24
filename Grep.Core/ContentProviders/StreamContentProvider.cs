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

    public class StreamContentProvider : ITextContentProvider
    {
        public StreamContentProvider(Stream input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            this.Content = new StreamReader(input);
        }

        public TextReader Content { get; private set; }
    }
}
