using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Convenient.Stuff.Models;
using Convenient.Stuff.Syntax;
using Convenient.Stuff.Wpf;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using Studio.Extensions;

namespace Studio.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ScriptState _scriptState;
        private string _source;

        public string Source
        {
            get { return _source; }
            set { _source = value; OnPropertyChanged(); }
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
            var options = ScriptOptions.Default.WithReferences(AppDomain.CurrentDomain.GetAssemblies());
            var init = string.Join("", RoslynAssemblies.Select(a => $"using {a};"));
            _scriptState = await CSharpScript.RunAsync(init, options, new Interactive(),
                typeof(Interactive));
            Variables.Clear();
            var source = await SyntaxTree.GetTextAsync();
            Source = source.ToString();
        }

        protected CSharpCompilation Compilation => (CSharpCompilation)_scriptState.Script.GetCompilation();
        protected CSharpSyntaxTree SyntaxTree => (CSharpSyntaxTree)Compilation.SyntaxTrees.Single();

        public IEnumerable<CompletionData> GetCompletions(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return typeof(Interactive).GetMethodsPropertiesAndFields()
                    .Select(m => new CompletionData("", m.Name));
            }
            var tree = CSharpSyntaxTree.ParseText(code);

            var compilation = Compilation.ReplaceSyntaxTree(SyntaxTree, tree);

            var semantics = compilation.GetSemanticModel(tree);
            var symbol = semantics.GetEnclosingSymbol(code.Length);
            
            
            var symbols = semantics.LookupSymbols(code.Length);
            return symbols.Select(s => new CompletionData("", s. Name, $"{s.Kind} {s.GetType().Name} {s.Name}"));
        }

        public async Task<object> Evaluate(string code)
        {
            try
            {
                _scriptState = await _scriptState.ContinueWithAsync(code);

                var newVariables = _scriptState.Variables.Where(v => Variables.All(e => !AreEqual(v, e)));
                Variables.AddRange(newVariables.Select(v => new VariableModel(v.Type, v.Name, v.Value)));

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
                Source = source.ToString();
            }
        }

        private static bool AreEqual(ScriptVariable second, VariableModel first)
        {
            return first == null
                ? second == null
                : first.Name == second.Name && first.Type == second.Type && first.Value == second.Value;
        }
    }
}