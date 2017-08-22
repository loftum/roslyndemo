﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Convenient.Stuff.Serializers;
using Convenient.Stuff.Wpf;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Studio.Extensions;

namespace Studio.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ScriptState _scriptState;
        private string _code;

        public string Code
        {
            get { return _code; }
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
            var options = ScriptOptions.Default.WithReferences(AppDomain.CurrentDomain.GetAssemblies());
            var init = string.Join("", RoslynAssemblies.Select(a => $"using {a};"));
            _scriptState = await CSharpScript.RunAsync(init, options, new Interactive(),
                typeof(Interactive));
            Variables.Clear();
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

            var script = _scriptState.Script.ContinueWith(code);
            script.Compile();
            var completer = new CodeCompleter(script);
            Code = completer.ToJson(true, true);
            return completer.GetCompletions();
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
                Code = source.ToString();
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