using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hestify.Core
{
    public static class NodeExtensions
    {
        public static bool HasAttribute(this ClassDeclarationSyntax node, string attributeName)
        {
            var attributes = node.AttributeLists.SelectMany(l => l.Attributes);
            return attributes.Any(a => a.Name.GetText().ToString() == attributeName);
        }
    }
}