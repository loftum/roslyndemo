using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Editor.Models
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

    public class SyntaxNodeMapper
    {
        public SyntaxNodeModel Map(SyntaxNode node)
        {
            return DoMap((dynamic) node);
        }

        private SyntaxNodeModel DoMap(SyntaxNode other)
        {
            var model = new SyntaxNodeModel
            {
                Type = other.GetType().Name,
                Text = other.GetText().ToString(),
                ChildNodes = other.ChildNodes().Select(Map).ToList(),
                ChildTokens = other.ChildTokens().Select(t => new SyntaxTokenModel
                {
                    Span = t.Span,
                    Kind = t.Kind().ToString(),
                    Text = t.Text
                }).ToList(),
                Span = other.Span
            };
            return model;
        }
    }

    public class SyntaxTokenModel
    {
        public string Kind { get; set; }
        public string Text { get; set; }
        public TextSpan Span { get; set; }
    }

    public class SyntaxNodeModel
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public List<SyntaxNodeModel> ChildNodes { get; set; }
        public List<SyntaxTokenModel> ChildTokens { get; set; }
        public TextSpan Span { get; set; }

        public SyntaxNodeModel()
        {
            ChildNodes = new List<SyntaxNodeModel>();
            ChildTokens = new List<SyntaxTokenModel>();
        }
    }
}