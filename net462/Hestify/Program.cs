using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Hestify.Core;

namespace Hestify
{
    class Program
    {
        static int Main(string[] args)
        {
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
                        var sln = args[0];
                        var remove = args.Length > 1 && args[1].StartsWith("-r");
                        Hestification.Run(sln, remove, source.Token).Wait(source.Token);
                        Console.WriteLine("Bye!");
                    }
                    catch (OperationCanceledException e)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("CANCELLED");
                        Console.ResetColor();
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
            Console.WriteLine($"{Process.GetCurrentProcess().ProcessName} <solution.sln> [-r(evert)]");
        }
    }
}