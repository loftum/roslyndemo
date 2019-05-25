using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using RoslynDemo.Core;
using RoslynDemo.Core.IO;
using RoslynDemo.Core.Models;
using RoslynDemo.Core.Models.Emit;
using RoslynDemo.Core.Models.Syntax;
using Visualizer.Completion;
using Visualizer.Wpf;

namespace Visualizer.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly FileManager _fileManager = new FileManager("out");

        private VisualizerModel _model;

        public void Parse(string input)
        {
            _model = VisualizerModel.Parse(input);
        }

        public SyntaxTreeModel GetTreeModel()
        {
            return SyntaxMapper.Map(_model.SyntaxTree);
        }

        public CSharpCompilationModel GetCompilationModel()
        {
            return CompilationMapper.Map(_model.Compilation);
        }

        public EmitResultModel Emit()
        {
            _fileManager.CleanFolder();
            var result = _model.Compilation.Emit(_fileManager.ToFullPath("program.dll"), _fileManager.ToFullPath("program.pdb"));
            _fileManager.SaveJson(RuntimeConfig.Generate(), "program.runtimeconfig.json");
            return CompilationMapper.Map(result);
        }

        public SyntaxMeta GetMetaAt(int index) => _model.GetMetaAt(index);

        public IEnumerable<CompletionData> GetCompletions(int location)
        {
            var completer = new CodeCompleter(_model.SyntaxTree, _model.Compilation, location);
            return completer.GetCompletions();
        }
    }
}