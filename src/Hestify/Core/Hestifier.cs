using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hestify.Core
{
    public class Hestifier : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (node.HasAttribute("Hest"))
            {
                return node;
            }
            Console.WriteLine($"Hestifying {node.Identifier.ValueText}");
            
            node = node.AddAttributeLists(
                SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList<AttributeSyntax>(SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("Hest"))))
                .WithLeadingTrivia(node.GetLeadingTrivia())
                .WithTrailingTrivia(SyntaxFactory.CarriageReturn, SyntaxFactory.LineFeed)
            );
            return node;
        }
    }
}