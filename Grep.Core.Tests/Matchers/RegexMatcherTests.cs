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
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RegexMatcherTests : TestBase
    {
        [TestMethod]
        public void TestRegexMatch()
        {
            var matcher = new RegexMatcher("tex[Tt]", false);
            var content = new StringContentProvider("Simple test text content.");

            var matches = matcher.GetMatches(content).Result;

            Assert.AreEqual(1, matches.Count);
            Assert.AreEqual(13, matches[0].Index);
        }

        [TestMethod]
        public void TestRegexNoMatch()
        {
            var matcher = new RegexMatcher("Not pr.*nt", false);
            var content = new StringContentProvider("Simple test text content.");

            var matches = matcher.GetMatches(content).Result;

            Assert.AreEqual(0, matches.Count);
        }

        [TestMethod]
        public void TestMultipleRegexMatch()
        {
            var matcher = new RegexMatcher("te[sxn]", false);
            var content = new StringContentProvider("Simple test text content.");

            var matches = matcher.GetMatches(content).Result;

            Assert.AreEqual(3, matches.Count);
            Assert.AreEqual(8, matches[0].Index);
            Assert.AreEqual(13, matches[1].Index);
            Assert.AreEqual(21, matches[2].Index);
        }
    }
}
