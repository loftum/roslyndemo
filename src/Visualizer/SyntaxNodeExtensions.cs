using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Visualizer
{
    public static class SyntaxNodeExtensions
    {
        public static SyntaxNodeOrToken GetMostSpecificNodeAt(this SyntaxNode root, int position)
        {
            SyntaxNodeOrToken specific = root;
            foreach (var node in root.DescendantNodesAndTokens())
            {
                if (node.FullSpan.Covers(position) && node.FullSpan.IsMoreSpecificThan(specific.FullSpan))
                {
                    specific = node;
                }
            }
            return specific;
        }

        public static bool Covers(this TextSpan span, int position)
        {
            return span.Start <= position && span.End > position;
        }

        public static bool IsMoreSpecificThan(this TextSpan span, TextSpan other)
        {
            return span.Start > other.Start && span.End <= other.End ||
                   span.Start >= other.Start && span.End < other.End;
        }
    }
}