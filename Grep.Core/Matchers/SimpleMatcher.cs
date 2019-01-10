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

    /// <summary>
    /// An <see cref="ITextMatcher"/> which uses a simple search to find matches.
    /// </summary>
    public class SimpleMatcher : ITextMatcher
    {
        private readonly string textToMatch;
        private readonly bool ignoreCase;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleMatcher"/> class.
        /// </summary>
        /// <param name="textToMatch">The text to match.</param>
        /// <param name="ignoreCase">Specifies case-insensitive matching.</param>
        public SimpleMatcher(string textToMatch, bool ignoreCase)
        {
            if (textToMatch == null)
            {
                throw new ArgumentNullException(nameof(textToMatch));
            }

            this.textToMatch = textToMatch;
            this.ignoreCase = ignoreCase;
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
                    lastMatch = data.IndexOf(this.textToMatch, lastMatch + 1, this.ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture);

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
