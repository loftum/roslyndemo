using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Convenient.Stuff.Wpf;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Studio.ViewModels
{
    public class VariableModel
    {
        public Type Type { get; }
        public string Name { get; }
        public object Value { get; }

        public VariableModel(Type type, string name, object value)
        {
            Type = type;
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Type.GetFriendlyName()} {Name} {Value ?? "null"}";
        }
    }

    public class Interactive
    {
        public string GetHelp()
        {
            return "help";
        }
    }

    public class MainViewModel : ViewModelBase
    {
        private CSharpSyntaxTree _syntaxTree;
        private CSharpCompilation _compilation;
        private ScriptState _script;

        public ObservableCollection<VariableModel> Variables { get; } = new ObservableCollection<VariableModel>();

        public MainViewModel()
        {
            _syntaxTree = (CSharpSyntaxTree) CSharpSyntaxTree.ParseText("");
            _compilation = CSharpCompilation.Create("hest");
            Reset().Wait();
        }

        public async Task Reset()
        {
            _script = await CSharpScript.RunAsync("", ScriptOptions.Default, new Interactive(), typeof(Interactive));
            Variables.Clear();
        }

        public IList<CompletionData> GetCompletions(string text)
        {
            var tree = (CSharpSyntaxTree) CSharpSyntaxTree.ParseText(text);
            var compilation = _compilation.AddSyntaxTrees(tree);
            var semantics = compilation.GetSemanticModel(tree);
            var symbols = semantics.LookupSymbols(text.Length - 1);
            return symbols.Select(s => new CompletionData("", s.Name)).ToList();
        }

        public async Task<object> Evaluate(string code)
        {
            try
            {
                _script = await _script.ContinueWithAsync(code);

                var newVariables = _script.Variables.Where(v => Variables.All(e => !AreEqual(v, e)));
                Variables.AddRange(newVariables.Select(v => new VariableModel(v.Type, v.Name, v.Value)));

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

        private static bool AreEqual(ScriptVariable second, VariableModel first)
        {
            return first == null
                ? second == null
                : first.Name == second.Name && first.Type == second.Type && first.Value == second.Value;
        }
    }
}