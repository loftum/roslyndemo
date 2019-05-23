using Microsoft.CodeAnalysis;

namespace Convenient.Stuff.Models.Syntax
{
    public class SyntaxTreeModel
    {
        public SyntaxTree Tree { get; set; }
        public SyntaxNodeModel Root { get; set; }
    }
}