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

    /// <summary>
    /// A formatted span of text.
    /// </summary>
    public class FormattedSpan
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedSpan"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="colour">The text colour.</param>
        public FormattedSpan(string text, ConsoleColor colour)
        {
            this.Text = text;
            this.Colour = colour;
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the text colour.
        /// </summary>
        public ConsoleColor Colour { get; }
    }
}