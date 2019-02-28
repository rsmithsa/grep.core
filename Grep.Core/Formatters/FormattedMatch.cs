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

    public class FormattedMatch
    {
        public FormattedMatch(GrepMatch match, IList<FormattedSpan> spans)
        {
            this.Match = match;
            this.Spans = spans;
        }

        public GrepMatch Match { get; }

        public IList<FormattedSpan> Spans { get; }
    }
}
