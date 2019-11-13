//-----------------------------------------------------------------------
// <copyright file="FormattedMatch.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Grep.Core.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// A formatted match based on a <see cref="GrepMatch"/>.
    /// </summary>
    public class FormattedMatch
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedMatch"/> class.
        /// </summary>
        /// <param name="match">The source <see cref="GrepMatch"/>.</param>
        /// <param name="spans">The resulting <see cref="FormattedSpan"/>s.</param>
        public FormattedMatch(GrepMatch match, IList<FormattedSpan> spans)
        {
            this.Match = match;
            this.Spans = spans;
        }

        /// <summary>
        /// Gets the source <see cref="GrepMatch"/>.
        /// </summary>
        public GrepMatch Match { get; }

        /// <summary>
        /// Gets resulting <see cref="FormattedSpan"/>s.
        /// </summary>
        public IList<FormattedSpan> Spans { get; }
    }
}
