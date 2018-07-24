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

    public interface ITextContentProvider
    {
        TextReader Content { get; }
    }
}
