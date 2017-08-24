using System;
using System.CodeDom;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace SolutionWatcher
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("Solution watcher v0.0");
            if (!args.Any())
            {
                PrintUsage();
                return 0;
            }
            try
            {
                using (var source = new CancellationTokenSource())
                {
                    Console.CancelKeyPress += (o, e) =>
                    {
                        source.Cancel();
                    };
                    try
                    {
                        Run(args, source.Token).Wait(source.Token);
                        Console.WriteLine("Bye!");
                    }
                    catch (OperationCanceledException e)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("CANCELLED");
                        Console.ResetColor();
                    }
                    catch (AggregateException a)
                    {
                        var inner = a.InnerException ?? a.InnerExceptions.FirstOrDefault();
                        if (inner != null)
                        {
                            throw inner;
                        }
                        throw;
                    }
                }
            }
            catch (ReflectionTypeLoadException e)
            {
                Console.WriteLine("Dang!");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                foreach (var exception in e.LoaderExceptions)
                {
                    Console.WriteLine(exception);
                    Console.WriteLine();
                }
                return -1;
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Oh, snap!");
                Console.WriteLine();
                Console.WriteLine(e.GetBaseException());
                return -1;
            }
            return 0;
        }

        private static void PrintUsage()
        {
            Console.WriteLine($"{Process.GetCurrentProcess().ProcessName} <solution.sln>");
        }

        private static async Task Run(string[] args, CancellationToken cancellationToken)
        {
            var sln = args[0];
            Console.WriteLine($"Opening {sln}");
            using (var workspace = MSBuildWorkspace.Create())
            {
                workspace.WorkspaceChanged += WorkspaceChanged;
                workspace.DocumentOpened += DocumentOpened;
                workspace.DocumentClosed += DocumentClosed;
                
                var solution = await workspace.OpenSolutionAsync(args[0], cancellationToken);

                Console.WriteLine($"Opened {solution.FilePath}");
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(500, cancellationToken);
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
