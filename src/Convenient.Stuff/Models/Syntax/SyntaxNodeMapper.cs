using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Convenient.Stuff.Models.Syntax
{
    public class SyntaxNodeMapper
    {
        public SyntaxNodeModel Map(SyntaxNode node)
        {
            return node == null ? null : DoMap((dynamic) node);
        }

        private SyntaxNodeModel DoMap(SyntaxNode node)
        {
            var model = new SyntaxNodeModel
            {
                Type = node.GetType().Name,
                Kind = node.Kind().ToString(),
                Text = node.GetText().ToString(),
                ChildNodes = node.ChildNodes().Select(Map).ToList(),
                ChildTokens = node.ChildTokens().Select(t => new SyntaxTokenModel
                {
                    Kind = t.Kind().ToString(),
                    Text = t.Text
                }).ToList(),
            };
            return model;
        }
    }
}