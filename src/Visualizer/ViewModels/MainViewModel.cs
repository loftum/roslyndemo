using System;
using System.Linq;
using Convenient.Stuff.IO;
using Convenient.Stuff.Models.Syntax;
using Convenient.Stuff.Serializers;
using Convenient.Stuff.Wpf;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Visualizer.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly FileManager _fileManager = new FileManager();

        public SyntaxTree SyntaxTree { get; private set; }
        protected CSharpCompilation Compilation { get; private set; }

        public void Parse(string input)
        {
            SyntaxTree = CSharpSyntaxTree.ParseText(input);
            Compilation = CSharpCompilation.Create("hest", new[] { SyntaxTree }, References);
        }

        public string GetTree()
        {
            return new SyntaxTreeModel(SyntaxTree).ToJson(true, true);
        }

        private static readonly MetadataReference[] References = AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .ToArray();

        public string GetCompilation()
        {
            return Compilation.ToJson(true, true);
        }

        public string Emit()
        {
            var result = Compilation.Emit(_fileManager.ToFullPath("program.exe"), _fileManager.ToFullPath("program.pdb"));
            return result.ToJson(true, true);
        }
    }
}