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

    /// <summary>
    /// Interface for a text matcher which provides matches from an <see cref="ITextContentProvider"/>.
    /// </summary>
    public interface ITextMatcher
    {
        /// <summary>
        /// Gets all the matches in the <paramref name="content"/>.
        /// </summary>
        /// <param name="content">The <see cref="ITextContentProvider"/> to search for matches.</param>
        /// <returns>The list of matches.</returns>
        Task<IList<GrepMatch>> GetMatches(ITextContentProvider content);
    }
}
