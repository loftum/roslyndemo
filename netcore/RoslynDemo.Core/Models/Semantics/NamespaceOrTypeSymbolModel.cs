﻿using Microsoft.CodeAnalysis;

namespace RoslynDemo.Core.Models.Semantics
{
    public class NamespaceOrTypeSymbolModel : SymbolModel
    {
        public bool IsType { get; }
        public bool IsNamespace { get; }

        public NamespaceOrTypeSymbolModel(INamespaceOrTypeSymbol namespaceOrType) : base(namespaceOrType)
        {
            if (namespaceOrType == null)
            {
                return;
            }
            IsType = namespaceOrType.IsType;
            IsNamespace = namespaceOrType.IsNamespace;
        }
    }
}