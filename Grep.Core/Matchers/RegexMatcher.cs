//-----------------------------------------------------------------------
// <copyright file="RegexMatcher.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Grep.Core.Matchers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Grep.Core.ContentProviders;

    public class RegexMatcher : ITextMatcher
    {
        private readonly Regex regex;

        public RegexMatcher(string pattern, bool ignoreCase)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            this.regex = new Regex(pattern, RegexOptions.Compiled | (ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None));
        }

        /// <inheritdoc />
        public async Task<IList<GrepMatch>> GetMatches(ITextContentProvider content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            var result = new List<GrepMatch>();

            int line = 0;
            for (string data = await content.Content.ReadLineAsync(); data != null; data = await content.Content.ReadLineAsync())
            {
                line++;

                foreach (Match match in this.regex.Matches(data))
                {
                    result.Add(new GrepMatch(match.Index + 1, line, match.Value, data));
                }
            }

            return result;
        }
    }
}
