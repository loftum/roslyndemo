using System;
using System.CodeDom;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.MSBuild;

namespace SolutionWatcher
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("Solution watcher started");
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
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("CANCELLED");
                        Console.ResetColor();
                    };
                    Run(args, source.Token).Wait(source.Token);
                    
                    Console.WriteLine("Bye!");
                    return 0;
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Oh, snap!");
                Console.WriteLine();
                Console.WriteLine(e.GetBaseException());
                return -1;
            }

        }

        private static void PrintUsage()
        {
            Console.WriteLine($"{Process.GetCurrentProcess().ProcessName} <solution.sln>");
        }

        private static async Task Run(string[] args, CancellationToken cancellationToken)
        {
            var sln = args[0];
            Console.WriteLine($"Opening {sln}");
            var workspace = MSBuildWorkspace.Create();
            //var solution = await workspace.OpenSolutionAsync(args[0], cancellationToken);
            //Console.WriteLine($"Opened {solution.FilePath}");
        }
    }
}
