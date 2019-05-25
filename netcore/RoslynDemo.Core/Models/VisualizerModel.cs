using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynDemo.Core.Models.Semantics;
using RoslynDemo.Core.Syntax;

namespace RoslynDemo.Core.Models
{
    public class VisualizerModel
    {
        private static readonly IEnumerable<PortableExecutableReference> References = Assemblies.FromCurrentContext()
            .Append(typeof(Console).Assembly)
            .Append(typeof(object).Assembly)
            .Select(a => MetadataReference.CreateFromFile(a.Location));

        public SyntaxTree SyntaxTree { get; }
        public CSharpCompilation Compilation { get; }

        public VisualizerModel(string input)
        {
            SyntaxTree = CSharpSyntaxTree.ParseText(input, CSharpParseOptions.Default);
            Compilation = CSharpCompilation.Create("hest", new[] { SyntaxTree }, References, new CSharpCompilationOptions(OutputKind.ConsoleApplication));
        }

        public SyntaxMeta GetMetaAt(int index)
        {
            if (!SyntaxTree.TryGetRoot(out var root) || index < 0 || index >= SyntaxTree.Length)
            {
                return SyntaxMeta.Empty;
            }
            var nodeOrToken = root.GetMostSpecificNodeOrTokenAt(index);
            var semantics = GetSemantics(nodeOrToken, index);
            return new SyntaxMeta(nodeOrToken, semantics);
        }

        private Dictionary<string, object> GetSemantics(SyntaxNodeOrToken nodeOrToken, int index)
        {
            var semantics = Compilation.GetSemanticModel(SyntaxTree);

            var result = new Dictionary<string, object>();
            var node = nodeOrToken.AsNode();
            if (node != null)
            {
                result["TypeInfo"] = SymbolMapper.Map(semantics.GetTypeInfo(node));
                result["SymbolInfo"] = SymbolMapper.Map(semantics.GetSymbolInfo(node));
            }
            result["EnclosingSymbol"] = SymbolMapper.Map(semantics.GetEnclosingSymbol(index));

            return result;
        }

        public static VisualizerModel Parse(string input) => new VisualizerModel(input);
    }
}