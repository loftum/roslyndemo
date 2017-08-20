using Microsoft.CodeAnalysis;

namespace Convenient.Stuff.Models
{
    public class SyntaxTreeModel
    {
        public SyntaxTree Tree { get; }
        public SyntaxNodeModel Root { get; }

        public SyntaxTreeModel(SyntaxTree tree)
        {
            Tree = tree;
            Root = new SyntaxNodeMapper().Map(tree.GetRoot());
        }
    }
}