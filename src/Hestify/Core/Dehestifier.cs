using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hestify.Core
{
    public class Dehestifier : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (!node.HasAttribute("Hest"))
            {
                return node;
            }

            var list = node.AttributeLists;
            var attributes =
                node.AttributeLists.Where(l => l.Attributes.Any(a => a.Name.GetText().ToString() == "Hest")).ToList();
            
            foreach (var attribute in attributes)
            {
                list = list.Remove(attribute);
            }

            Console.WriteLine($"Dehestifying {node.Identifier.ValueText}");
            node = node.WithAttributeLists(list);
            
            return node;
        }
    }
}