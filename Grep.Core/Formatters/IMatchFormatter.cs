//-----------------------------------------------------------------------
// <copyright file="IMatchFormatter.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Grep.Core.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Interface for a match formatter matcher which formats <see cref="GrepMatch"/>s.
    /// </summary>
    public interface IMatchFormatter
    {
        /// <summary>
        /// Formats the <paramref name="match"/>.
        /// </summary>
        /// <param name="match">The <see cref="GrepMatch"/> to format.</param>
        /// <returns>The formatted match.</returns>
        FormattedMatch FormatMatch(GrepMatch match);
    }
}
