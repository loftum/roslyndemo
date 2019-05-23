using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
        private readonly FileManager _fileManager = new FileManager();

        public SyntaxTree SyntaxTree { get; private set; }
        public CSharpCompilation Compilation { get; private set; }

        public void Parse(string input)
        {
            SyntaxTree = CSharpSyntaxTree.ParseText(input, CSharpParseOptions.Default);
            Compilation = CSharpCompilation.Create("hest", new[] { SyntaxTree }, References);

        }

        public SyntaxTreeModel GetTreeModel()
        {
            return SyntaxMapper.Map(SyntaxTree);
        }

        private static readonly PortableExecutableReference[] References = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .ToArray();

        public CSharpCompilationModel GetCompilationModel()
        {
            return CompilationMapper.Map(Compilation);
        }

        public EmitResultModel Emit()
        {
            var result = Compilation.Emit(_fileManager.ToFullPath("program.exe"), _fileManager.ToFullPath("program.pdb"));
            return CompilationMapper.Map(result);
        }

        public SyntaxMeta GetMetaAt(int index)
        {
            SyntaxNode root;
            if (!SyntaxTree.TryGetRoot(out root))
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
}