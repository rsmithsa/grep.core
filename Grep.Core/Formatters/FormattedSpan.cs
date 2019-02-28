//-----------------------------------------------------------------------
// <copyright file="FormattedSpan.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Grep.Core.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class FormattedSpan
    {
        public FormattedSpan(string text, ConsoleColor colour)
        {
            this.Text = text;
            this.Colour = colour;
        }

        public string Text { get; }

        public ConsoleColor Colour { get; }
    }
}