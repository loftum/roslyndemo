using Microsoft.CodeAnalysis;

namespace RoslynDemo.Core.Completion
{
    public struct CompletionNodes
    {
        public SyntaxNode Container { get; }
        public SyntaxNode Prefix { get; }

        public CompletionNodes(SyntaxNode container, SyntaxNode prefix)
        {
            Container = container;
            Prefix = prefix;
        }
    }
}