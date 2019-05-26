using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RoslynDemo.Core.Studio;

namespace Studio.Terminal
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("Studio Terminal");
            using (var source = new CancellationTokenSource())
            {
                Console.CancelKeyPress += (s, e) => source.Cancel();
                try
                {
                    RunAsync(args, source.Token).Wait(source.Token);
                    return 0;
                }
                catch (AggregateException a)
                {
                    Console.WriteLine(a.InnerException);
                    return -1;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return -1;
                }
            }
        }

        private static async Task RunAsync(string[] args, CancellationToken token)
        {
            var scripter = new CSharpScripter();

            while (true)
            {
                Console.Write("> ");
                var read = Console.ReadLine();
                switch (read)
                {
                    case string whitespace when string.IsNullOrWhiteSpace(whitespace):
                        break;
                    case "quit":
                    case "exit":
                        return;
                    case "vars":
                        Console.WriteLine("Variables:");
                        foreach (var variable in scripter.ScriptState.Variables.Select(v => new VariableModel(v)))
                        {
                            Console.WriteLine(variable);
                        }
                        break;

                    default:
                        var result = await scripter.EvaluateAsync(read);
                        Console.WriteLine(result.ToResultString());
                        break;
                }
            }
        }
    }
}
