﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;

namespace Hestify.Core
{
    public class Hestification
    {
        public static async Task Run(string sln, bool revert, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Hestifying {sln}");
            using (var workspace = MSBuildWorkspace.Create())
            {
                workspace.WorkspaceChanged += WorkspaceChanged;
                workspace.DocumentOpened += DocumentOpened;
                workspace.DocumentClosed += DocumentClosed;

                var solution = await workspace.OpenSolutionAsync(sln, cancellationToken);
                Console.WriteLine($"Solution: {solution.FilePath} ({solution.Projects.Count()} projects)");

                var hestifier = revert ? (CSharpSyntaxRewriter) new Dehestifier() : new Hestifier();
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