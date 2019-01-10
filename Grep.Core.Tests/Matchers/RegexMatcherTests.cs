//-----------------------------------------------------------------------
// <copyright file="RegexMatcherTests.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Grep.Core.Tests.Matchers
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Grep.Core.ContentProviders;
    using Grep.Core.Matchers;
    using Xunit;

    /// <summary>
    /// Tests for the <see cref="RegexMatcher"/>.
    /// </summary>
    public class RegexMatcherTests : TestBase
    {
        /// <summary>
        /// Tests an input with a single match.
        /// </summary>
        [Fact]
        public void TestRegexMatch()
        {
            var matcher = new RegexMatcher("tex[Tt]", false);
            var content = new StringContentProvider("Simple test text content.");

            var matches = matcher.GetMatches(content).Result;

            Assert.Equal(1, matches.Count);
            Assert.Equal(13, matches[0].Index);
        }

        /// <summary>
        /// Tests a non-matching input.
        /// </summary>
        [Fact]
        public void TestRegexNoMatch()
        {
            var matcher = new RegexMatcher("Not pr.*nt", false);
            var content = new StringContentProvider("Simple test text content.");

            var matches = matcher.GetMatches(content).Result;

            Assert.Equal(0, matches.Count);
        }

        /// <summary>
        /// Tests an input with multiple matches.
        /// </summary>
        [Fact]
        public void TestMultipleRegexMatch()
        {
            var matcher = new RegexMatcher("te[sxn]", false);
            var content = new StringContentProvider("Simple test text content.");

            var matches = matcher.GetMatches(content).Result;

            Assert.Equal(3, matches.Count);
            Assert.Equal(8, matches[0].Index);
            Assert.Equal(13, matches[1].Index);
            Assert.Equal(21, matches[2].Index);
        }
    }
}
