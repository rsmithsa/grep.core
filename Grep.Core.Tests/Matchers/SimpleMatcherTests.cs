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

    [TestClass]
    public class SimpleMatcherTests : TestBase
    {
        [TestMethod]
        public void TestSimpleMatch()
        {
            var matcher = new SimpleMatcher("text");
            var content = new StringContentProvider("Simple test text content.");

            var matches = matcher.GetMatches(content).Result;

            Assert.AreEqual(1, matches.Count);
            Assert.AreEqual(13, matches[0].Index);
        }

        [TestMethod]
        public void TestSimpleNoMatch()
        {
            var matcher = new SimpleMatcher("Not present");
            var content = new StringContentProvider("Simple test text content.");

            var matches = matcher.GetMatches(content).Result;

            Assert.AreEqual(0, matches.Count);
        }

        [TestMethod]
        public void TestMultipleSimpleMatch()
        {
            var matcher = new SimpleMatcher("te");
            var content = new StringContentProvider("Simple test text content.");

            var matches = matcher.GetMatches(content).Result;

            Assert.AreEqual(3, matches.Count);
            Assert.AreEqual(8, matches[0].Index);
            Assert.AreEqual(13, matches[1].Index);
            Assert.AreEqual(21, matches[2].Index);
        }
    }
}
