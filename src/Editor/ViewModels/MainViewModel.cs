using Convenient.Stuff.Serializers;
using Convenient.Stuff.Wpf;
using Editor.Models;
using Microsoft.CodeAnalysis.CSharp;

namespace Editor.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _syntaxTree;

        public string SyntaxTree
        {
            get { return _syntaxTree; }
            set
            {
                _syntaxTree = value;
                OnPropertyChanged();
            }
        }

        public void Parse(string inputText)
        {
            var tree = CSharpSyntaxTree.ParseText(inputText);
            
            SyntaxTree = new SyntaxTreeModel(tree).ToJson(true, true);
        }
    }
}