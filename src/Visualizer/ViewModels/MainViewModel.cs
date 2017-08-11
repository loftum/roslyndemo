using Convenient.Stuff.Serializers;
using Convenient.Stuff.Wpf;
using Microsoft.CodeAnalysis.CSharp;
using Visualizer.Models;

namespace Visualizer.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _output;

        public string Output
        {
            get { return _output; }
            set
            {
                _output = value;
                OnPropertyChanged();
            }
        }

        public void Parse(string inputText)
        {
            var tree = CSharpSyntaxTree.ParseText(inputText);

            Output = new SyntaxTreeModel(tree).ToJson(true, true);
        }

        public void Compile(string inputText)
        {
            var tree = CSharpSyntaxTree.ParseText(inputText);
            var compilation = CSharpCompilation.Create("hest", new[] {tree});
            
            Output = compilation.ToJson(true, true);
        }
    }


}