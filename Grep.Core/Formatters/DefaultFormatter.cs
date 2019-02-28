//-----------------------------------------------------------------------
// <copyright file="DefaultFormatter.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Grep.Core.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The default <see cref="IMatchFormatter"/> which provides generic match formatting.
    /// </summary>
    public class DefaultFormatter : IMatchFormatter
    {
        /// <inheritdoc />
        public FormattedMatch FormatMatch(GrepMatch match)
        {
            var preMatch = match.Context.Substring(0, match.Index - 1).TrimStart();
            var postMatch = match.Context.Substring(match.Index + match.Value.Length - 1).TrimEnd();

            return new FormattedMatch(
                match,
                new List<FormattedSpan>
                    {
                        new FormattedSpan(preMatch, ConsoleColor.DarkGray),
                        new FormattedSpan(match.Value, ConsoleColor.Blue),
                        new FormattedSpan(postMatch, ConsoleColor.DarkGray),
                    });
        }
    }
}
