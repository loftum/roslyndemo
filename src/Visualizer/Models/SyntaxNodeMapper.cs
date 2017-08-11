using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Visualizer.Models
{
    public class SyntaxNodeMapper
    {
        public SyntaxNodeModel Map(SyntaxNode node)
        {
            return DoMap((dynamic) node);
        }

        private SyntaxNodeModel DoMap(SyntaxNode node)
        {
            var model = new SyntaxNodeModel
            {
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