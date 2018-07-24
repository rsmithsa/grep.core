//-----------------------------------------------------------------------
// <copyright file="ITextMatcher.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Grep.Core.Matchers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Grep.Core.ContentProviders;

    public interface ITextMatcher
    {
        Task<IList<GrepMatch>> GetMatches(ITextContentProvider content);
    }
}
