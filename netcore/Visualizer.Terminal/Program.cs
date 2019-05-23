using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Visualizer.Terminal
{
    class Program
    {
        private static readonly MetadataReference[] References = AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .ToArray();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Enter statement");
                var read = Console.ReadLine();
                if (string.Equals(read, "quit"))
                {
                    Console.WriteLine("Bye!");
                    return;
                }

                var tree = CSharpSyntaxTree.ParseText(read);
                var compilation = CSharpCompilation.Create("hest", new[] {tree}, References);
                Console.WriteLine(compilation);
            }
            
        }
    }
}
