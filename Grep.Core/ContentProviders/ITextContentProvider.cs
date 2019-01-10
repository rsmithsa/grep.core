//-----------------------------------------------------------------------
// <copyright file="ITextContentProvider.cs" company="Richard Smith">
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
    /// Interface for a text content provider.
    /// </summary>
    public interface ITextContentProvider
    {
        /// <summary>
        /// Gets the text content.
        /// </summary>
        TextReader Content { get; }
    }
}
