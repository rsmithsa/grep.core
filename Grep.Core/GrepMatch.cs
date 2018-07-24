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

    public class GrepMatch
    {
        public GrepMatch(int index, int line, string value, string context)
        {
            this.Index = index;
            this.Line = line;
            this.Value = value;
            this.Context = context;
        }

        public int Index { get; }

        public int Line { get; }

        public string Value { get; }

        public string Context { get; }
    }
}
