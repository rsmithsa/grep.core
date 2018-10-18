//-----------------------------------------------------------------------
// <copyright file="FileProvider.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Grep.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class FileProvider
    {
        public bool Recurse { get; }
        public bool IgnoreBinary { get; }
        public string ExcludeDirectories { get; }

        public IEnumerable<string> EnumerateFiles(IEnumerable<string> filePatterns)
        {
            throw new NotImplementedException();
        }
    }
}
