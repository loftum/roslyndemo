using Convenient.Stuff.Models.Syntax;
using Convenient.Stuff.Serializers;
using Convenient.Stuff.Wpf;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

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

        public SyntaxTree SyntaxTree { get; private set; }

        public void Parse(string input)
        {
            SyntaxTree = CSharpSyntaxTree.ParseText(input);
        }

        public string GetTree()
        {
            return new SyntaxTreeModel(SyntaxTree).ToJson(true, true);
        }

        public string GetCompilation()
        {
            var compilation = CSharpCompilation.Create("hest", new[] {SyntaxTree});
            return compilation.ToJson(true, true);
        }
    }
}