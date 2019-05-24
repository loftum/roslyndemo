﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace RoslynDemo.Core.Models
{
    public struct SyntaxMeta
    {
        public SyntaxNodeOrToken SyntaxNodeOrToken { get; }
        public Dictionary<string, object> Semantics { get; set; }

        public SyntaxMeta(SyntaxNodeOrToken syntaxNodeOrToken, Dictionary<string, object> semantics)
        {
            SyntaxNodeOrToken = syntaxNodeOrToken;
            Semantics = semantics;
        }

        public static SyntaxMeta Empty => new SyntaxMeta(null, null);
    }
}