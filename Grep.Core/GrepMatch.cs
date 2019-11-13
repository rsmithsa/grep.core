//-----------------------------------------------------------------------
// <copyright file="GrepMatch.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Grep.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Match detail for a match in grep.core.
    /// </summary>
    public class GrepMatch
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrepMatch"/> class.
        /// </summary>
        /// <param name="index">The character index of the match.</param>
        /// <param name="line">The line number of the match.</param>
        /// <param name="value">The match value.</param>
        /// <param name="context">The match context.</param>
        public GrepMatch(int index, int line, string value, string context)
        {
            this.Index = index;
            this.Line = line;
            this.Value = value;
            this.Context = context;
        }

        /// <summary>
        /// Gets the character index of the match.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Gets the line number of the match.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Gets the value of the match.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets the context of the match.
        /// </summary>
        public string Context { get; }
    }
}
