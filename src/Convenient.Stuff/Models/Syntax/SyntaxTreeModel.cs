using Microsoft.CodeAnalysis;

namespace Convenient.Stuff.Models.Syntax
{
    public class SyntaxTreeModel
    {
        public SyntaxTree Tree { get; }
        public SyntaxNodeModel Root { get; }

        public SyntaxTreeModel(SyntaxTree tree)
        {
            Tree = tree;
            Root = SyntaxMapper.Map(tree.GetRoot());
        }
    }
}