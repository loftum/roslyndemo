using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using RoslynDemo.Core.Collections;
using RoslynDemo.Core.Serializers;
using Studio.Completion;
using Studio.Extensions;
using Studio.Wpf;

namespace Studio.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ScriptState _scriptState;
        private string _code;

        public string Code
        {
            get => _code;
            set { _code = value; OnPropertyChanged(); }
        }

        public ObservableCollection<VariableModel> Variables { get; } = new ObservableCollection<VariableModel>();

        public MainViewModel()
        {
            Reset().Wait();
        }

        private static readonly string[] RoslynAssemblies = {
            "Microsoft.CodeAnalysis",
            "Microsoft.CodeAnalysis.CSharp",
            "Microsoft.CodeAnalysis.Scripting",
            "Microsoft.CodeAnalysis.CSharp.Scripting"
        };

        public async Task Reset()
        {
            var options = ScriptOptions.Default.WithReferences(AppDomain.CurrentDomain.GetAssemblies())
                .WithImports(RoslynAssemblies);
            _scriptState = await CSharpScript.RunAsync("", options, new Interactive(),
                typeof(Interactive));
            Variables.Clear();
        }

        protected CSharpCompilation Compilation => (CSharpCompilation)_scriptState.Script.GetCompilation();
        protected CSharpSyntaxTree SyntaxTree => (CSharpSyntaxTree)Compilation.SyntaxTrees.Single();

        public IEnumerable<CompletionData> GetCompletions(string code)
        {
            var script = _scriptState.Script.ContinueWith(code);
            script.Compile();
            var compilation = script.GetCompilation();
            var tree = compilation.SyntaxTrees.Single();
            var completer = new CodeCompleter(tree, compilation, tree.Length - 1);
            Code = completer.ToJson(true, true);
            return completer.GetCompletions();
        }

        public async Task<object> Evaluate(string code)
        {
            try
            {
                _scriptState = await _scriptState.ContinueWithAsync(code);
                Variables.Clear();
                Variables.AddRange(_scriptState.Variables.Select(v => new VariableModel(v)));
                return _scriptState.ReturnValue ?? _scriptState.Exception;
            }
            catch (CompilationErrorException e)
            {
                return e;
            }
            catch (Exception e)
            {
                return e;
            }
            finally
            {
                var source = await SyntaxTree.GetTextAsync();
                Code = source.ToString();
            }
        }
    }
}