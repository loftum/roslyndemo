using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Convenient.Stuff.Wpf;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Studio.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private CSharpSyntaxTree _syntaxTree;
        private CSharpCompilation _compilation;
        private ScriptState _script;

        public MainViewModel()
        {
            _syntaxTree = (CSharpSyntaxTree) CSharpSyntaxTree.ParseText("");
            _compilation = CSharpCompilation.Create("hest");
            var task = CSharpScript.RunAsync("");
            task.Wait();
            _script = task.Result;
        }

        public IList<CompletionData> GetCompletions(string text)
        {
            var tree = (CSharpSyntaxTree) CSharpSyntaxTree.ParseText(text);
            var compilation = _compilation.AddSyntaxTrees(tree);
            var semantics = compilation.GetSemanticModel(tree);
            var symbols = semantics.LookupSymbols(text.Length - 1);
            return symbols.Select(s => new CompletionData("", s.Name)).ToList();
        }

        public async Task Reset()
        {
            _script = await CSharpScript.RunAsync("");
        }

        public async Task<object> Evaluate(string code)
        {
            try
            {
                _script = await _script.ContinueWithAsync(code);
                return _script.ReturnValue ?? _script.Exception;
            }
            catch (CompilationErrorException e)
            {
                return e;
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }
}