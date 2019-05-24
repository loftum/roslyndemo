using Microsoft.CodeAnalysis;

namespace RoslynDemo.Core.Models.Syntax
{
    public class SyntaxTreeModel
    {
        public SyntaxTree Tree { get; set; }
        public SyntaxNodeModel Root { get; set; }
}
}