using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace Hestify
{
    using static SyntaxFactory;
    public class Hestifier : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            Console.WriteLine($"Class {node.Identifier.ValueText}");
            var attributes = node.AttributeLists.SelectMany(l => l.Attributes);
            if (attributes.Any(a => a.Name.GetText().ToString() == "Hest"))
            {
                return node;
            }
            Console.WriteLine("Adding [Hest]");
            node = node.WithAttributeLists(node.AttributeLists.Add(
                AttributeList(SingletonSeparatedList<AttributeSyntax>(Attribute(IdentifierName("Hest"))))
                .WithLeadingTrivia(node.GetLeadingTrivia())
                .WithTrailingTrivia(CarriageReturn, LineFeed))
            );
            return node;
        }

        public static async Task Run(string sln, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Hestifying {sln}");
            using (var workspace = MSBuildWorkspace.Create())
            {
                workspace.WorkspaceChanged += WorkspaceChanged;
                workspace.DocumentOpened += DocumentOpened;
                workspace.DocumentClosed += DocumentClosed;

                var solution = await workspace.OpenSolutionAsync(sln, cancellationToken);
                Console.WriteLine($"Solution: {solution.FilePath} ({solution.Projects.Count()} projects)");

                var hestifier = new Hestifier();
                foreach (var projectId in solution.ProjectIds)
                {
                    var project = solution.GetProject(projectId);
                    foreach (var documentId in project.DocumentIds)
                    {
                        var document = project.GetDocument(documentId);
                        var root = await document.GetSyntaxRootAsync(cancellationToken);
                        root = hestifier.Visit(root); // immutable
                        document = document.WithSyntaxRoot(root);  // immutable
                        project = document.Project; // immutable
                    }

                    solution = project.Solution; // immutable
                }
                if (!workspace.TryApplyChanges(solution))
                {
                    Console.WriteLine("Oh, noes.");
                }
            }
        }

        private static void DocumentClosed(object sender, DocumentEventArgs e)
        {
            Console.WriteLine($"Document closed: {e.Document.Name}");
        }

        private static void DocumentOpened(object sender, DocumentEventArgs e)
        {
            Console.WriteLine($"Document opened: {e.Document.Name}");
        }

        private static void WorkspaceChanged(object sender, WorkspaceChangeEventArgs e)
        {
            Console.WriteLine($"Workspace changed: {e.Kind} {e.DocumentId}");
        }
    }
}