using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using RoslynDemo.Core.Collections;
using RoslynDemo.Core.Completion;
using RoslynDemo.Core.Studio;
using Studio.Avalon;
using Studio.Wpf;

namespace Studio.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly CSharpScripter _scripter = new CSharpScripter();

        private string _code;

        public string Code
        {
            get => _code;
            set { _code = value; OnPropertyChanged(); }
        }

        public ObservableCollection<VariableModel> Variables { get; } = new ObservableCollection<VariableModel>();

        public IEnumerable<CompletionData> GetCompletions(string code)
        {
            return _scripter.GetCompletions(code).Select(ToCompletionData);
        }

        private static CompletionData ToCompletionData(CompletionItem item)
        {
            return new CompletionData(item.Prefix, item.Completion, item.Content, item.Description);
        }

        public async Task<object> Evaluate(string code)
        {
            var ret = await _scripter.Evaluate(code);
            Variables.Clear();
            Variables.AddRange(_scripter.ScriptState.Variables.Select(v => new VariableModel(v)));
            var source = await _scripter.SyntaxTree.GetTextAsync();
            Code = source.ToString();
            return ret;
        }
    }
}