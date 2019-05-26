using Microsoft.CodeAnalysis;

namespace RoslynDemo.Core.Completion
{
    public static class CodeCompletionExtensions
    {
        public static CompletionSymbols GetCompletionSymbols(this SemanticModel semantics, SyntaxNode node)
        {
            if (node == null)
            {
                return semantics.GetDefault();
            }
            var symbolInfo = semantics.GetSymbolInfo(node);

            var symbol = symbolInfo.Symbol;
            var namespaceOrType = symbol as INamespaceOrTypeSymbol;
            if (namespaceOrType != null)
            {
                return new CompletionSymbols(namespaceOrType, null);
            }

            var typeInfo = semantics.GetTypeInfo(node);
            var type = typeInfo.ConvertedType ?? typeInfo.Type;

            return type != null ? new CompletionSymbols(type, symbol) : semantics.GetDefault();
        }

        private static CompletionSymbols GetDefault(this SemanticModel semantics)
        {
            return new CompletionSymbols(semantics.Compilation.GlobalNamespace, null);
        }
    }
}