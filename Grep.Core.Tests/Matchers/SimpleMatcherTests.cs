//-----------------------------------------------------------------------
// <copyright file="SimpleMatcherTests.cs" company="Richard Smith">
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

    /// <summary>
    /// Tests for the <see cref="SimpleMatcher"/>.
    /// </summary>
    [TestClass]
    public class SimpleMatcherTests : TestBase
    {
        /// <summary>
        /// Tests an input with a single match.
        /// </summary>
        [TestMethod]
        public void TestSimpleMatch()
        {
            var matcher = new SimpleMatcher("text", false);
            var content = new StringContentProvider("Simple test text content.");

            var matches = matcher.GetMatches(content).Result;

            Assert.AreEqual(1, matches.Count);
            Assert.AreEqual(13, matches[0].Index);
        }

        /// <summary>
        /// Tests a non-matching input.
        /// </summary>
        [TestMethod]
        public void TestSimpleNoMatch()
        {
            var matcher = new SimpleMatcher("Not present", false);
            var content = new StringContentProvider("Simple test text content.");

            var matches = matcher.GetMatches(content).Result;

            Assert.AreEqual(0, matches.Count);
        }

        /// <summary>
        /// Tests an input with multiple matches.
        /// </summary>
        [TestMethod]
        public void TestMultipleSimpleMatch()
        {
            var matcher = new SimpleMatcher("te", false);
            var content = new StringContentProvider("Simple test text content.");

            var matches = matcher.GetMatches(content).Result;

            Assert.AreEqual(3, matches.Count);
            Assert.AreEqual(8, matches[0].Index);
            Assert.AreEqual(13, matches[1].Index);
            Assert.AreEqual(21, matches[2].Index);
        }
    }
}
