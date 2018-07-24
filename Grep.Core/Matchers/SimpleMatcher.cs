//-----------------------------------------------------------------------
// <copyright file="SimpleMatcher.cs" company="Richard Smith">
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

    public class SimpleMatcher : ITextMatcher
    {
        private readonly string textToMatch;

        public SimpleMatcher(string textToMatch)
        {
            if (textToMatch == null)
            {
                throw new ArgumentNullException(nameof(textToMatch));
            }

            this.textToMatch = textToMatch;
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

                var lastMatch = -1;
                do
                {
                    lastMatch = data.IndexOf(this.textToMatch, lastMatch + 1);

                    if (lastMatch >= 0)
                    {
                        result.Add(new GrepMatch(lastMatch + 1, line, this.textToMatch, data));
                    }
                }
                while (lastMatch > 0 && lastMatch < data.Length);
            }

            return result;
        }
    }
}
