using System;
using System.Collections.Generic;
using System.Linq;
using Convenient.Stuff.IO;
using Convenient.Stuff.Models;
using Convenient.Stuff.Models.Emit;
using Convenient.Stuff.Models.Semantics;
using Convenient.Stuff.Models.Syntax;
using Convenient.Stuff.Serializers;
using Convenient.Stuff.Syntax;
using Convenient.Stuff.Wpf;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Visualizer.ViewModels
{
    public struct SyntaxMeta
    {
        public SyntaxNodeOrToken SyntaxNodeOrToken { get; }
        public Dictionary<string, object> Semantics { get; set; }

        public SyntaxMeta(SyntaxNodeOrToken syntaxNodeOrToken, Dictionary<string, object> semantics)
        {
            SyntaxNodeOrToken = syntaxNodeOrToken;
            Semantics = semantics;
        }

        public static SyntaxMeta Empty => new SyntaxMeta(null, null);
    }

    public class MainViewModel : ViewModelBase
    {
        private readonly FileManager _fileManager = new FileManager();

        public SyntaxTree SyntaxTree { get; private set; }
        public CSharpCompilation Compilation { get; private set; }

        public void Parse(string input)
        {
            SyntaxTree = CSharpSyntaxTree.ParseText(input);
            Compilation = CSharpCompilation.Create("hest", new[] { SyntaxTree }, References);
        }

        public SyntaxTreeModel GetTreeModel()
        {
            return SyntaxMapper.Map(SyntaxTree);
        }

        private static readonly MetadataReference[] References = AppDomain.CurrentDomain.GetAssemblies()
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
    }
}