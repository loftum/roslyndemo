using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using RoslynDemo.Core.Completion;
using RoslynDemo.Core.Serializers;

namespace RoslynDemo.Core.Studio
{
    public class CSharpScripter
    {
        public ScriptState ScriptState { get; private set; }
        public CSharpCompilation Compilation => (CSharpCompilation)ScriptState.Script.GetCompilation();
        public CSharpSyntaxTree SyntaxTree => (CSharpSyntaxTree)Compilation.SyntaxTrees.Single();

        public CSharpScripter()
        {
            Reset().Wait();
        }

        public async Task Reset()
        {
            var options = ScriptOptions.Default.WithReferences(AppDomain.CurrentDomain.GetAssemblies())
                .WithImports(Assemblies.RoslynNamespaces);
            ScriptState = await CSharpScript.RunAsync("", options, new Interactive(),
                typeof(Interactive));
        }

        public async Task<object> Evaluate(string code)
        {
            try
            {
                ScriptState = await ScriptState.ContinueWithAsync(code);
                return ScriptState.ReturnValue ?? ScriptState.Exception;
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

        public IEnumerable<CompletionItem> GetCompletions(string code)
        {
            var script = ScriptState.Script.ContinueWith(code);
            script.Compile();
            var compilation = script.GetCompilation();
            var tree = compilation.SyntaxTrees.Single();
            var completer = new CodeCompleter(tree, compilation, tree.Length - 1);
            return completer.GetCompletions();
        }
    }
}