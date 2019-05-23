using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynDemo.Core;
using RoslynDemo.Core.IO;
using RoslynDemo.Core.Models;
using RoslynDemo.Core.Models.Emit;
using RoslynDemo.Core.Models.Semantics;
using RoslynDemo.Core.Models.Syntax;
using RoslynDemo.Core.Syntax;
using Visualizer.Completion;
using Visualizer.Wpf;

namespace Visualizer.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly FileManager _fileManager = new FileManager("out");

        public SyntaxTree SyntaxTree { get; private set; }
        public CSharpCompilation Compilation { get; private set; }

        public void Parse(string input)
        {
            SyntaxTree = CSharpSyntaxTree.ParseText(input, CSharpParseOptions.Default);
            Compilation = CSharpCompilation.Create("hest", new[] { SyntaxTree }, References, new CSharpCompilationOptions(OutputKind.ConsoleApplication));
        }

        public SyntaxTreeModel GetTreeModel()
        {
            return SyntaxMapper.Map(SyntaxTree);
        }

        private static readonly IEnumerable<PortableExecutableReference> References = Assemblies.FromCurrentContext()
            .Append(typeof(Console).Assembly)
            .Append(typeof(object).Assembly)
            .Select(a => MetadataReference.CreateFromFile(a.Location));

        public CSharpCompilationModel GetCompilationModel()
        {
            return CompilationMapper.Map(Compilation);
        }

        public EmitResultModel Emit()
        {
            _fileManager.CleanFolder();
            var result = Compilation.Emit(_fileManager.ToFullPath("program.dll"), _fileManager.ToFullPath("program.pdb"));
            _fileManager.SaveJson(RuntimeConfig.Generate(), "program.runtimeconfig.json");
            return CompilationMapper.Map(result);
        }

        public SyntaxMeta GetMetaAt(int index)
        {
            if (!SyntaxTree.TryGetRoot(out var root))
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

        public IEnumerable<CompletionData> GetCompletions(int location)
        {
            var completer = new CodeCompleter(SyntaxTree, Compilation, location);
            return completer.GetCompletions();
        }
    }

    public static class RuntimeConfig
    {
        public static Dictionary<string, object> Generate()
        {
            var version = "2.2.0";// Environment.Version;
            //var version = Environment.Version;

            return new Dictionary<string, object>
            {
                ["runtimeOptions"] = new Dictionary<string, object>
                {
                    ["tfm"] = $"netcoreapp{version}",
                    ["framework"] = new Dictionary<string, object>
                    {
                        ["name"] = "Microsoft.NETCore.App",
                        ["version"] = $"{version}"
                    }
                }
            };
        }
    }
}