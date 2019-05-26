using Microsoft.CodeAnalysis;

namespace RoslynDemo.Core.Completion
{
    public struct CompletionSymbols
    {
        public INamespaceOrTypeSymbol NamespaceOrType { get; }
        public ISymbol Symbol { get; }

        public bool SearchForStatic => Symbol == null;

        public CompletionSymbols(INamespaceOrTypeSymbol namespaceOrType, ISymbol symbol)
        {
            NamespaceOrType = namespaceOrType;
            Symbol = symbol;
        }
    }
}