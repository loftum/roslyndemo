using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Convenient.Stuff;
using Convenient.Stuff.IO;
using Convenient.Stuff.Syntax;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Search;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Visualizer.ViewModels;

namespace Visualizer
{
    public partial class MainWindow
    {
        private readonly FileManager _fileManager = new FileManager();
        protected MainViewModel Vm => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
            SearchPanel.Install(Input);
            SearchPanel.Install(Output);
            Input.TextArea.Caret.PositionChanged += CaretOnPositionChanged;
        }

        private void CaretOnPositionChanged(object sender, EventArgs eventArgs)
        {
            var index = Input.CaretOffset;
            Caret.Text = $"Caret: {index}";
            ShowMetaAt(index);
        }

        private void Parse_Click(object sender, RoutedEventArgs e)
        {
            Output.Text = Vm.GetTree();
        }

        private void Compile_Click(object sender, RoutedEventArgs e)
        {
            Output.Text = Vm.GetCompilation();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _fileManager.SaveJson(new Data
            {
                Input = Input.Text
            });

            base.OnClosing(e);
        }

        protected override void OnInitialized(EventArgs e)
        {
            var data = _fileManager.LoadJson<Data>() ?? new Data();
            Input.Text = data.Input;
            base.OnInitialized(e);
        }

        private void Input_OnMouseHover(object sender, MouseEventArgs e)
        {
            var input = (TextEditor) sender;
            var position = input.GetPositionFromPoint(e.GetPosition((IInputElement)sender));
            
            if (position == null)
            {
                return;
            }
            
            var index = input.Text.GetIndexAt(position.Value);
            if (index < 0)
            {
                return;
            }
            ShowMetaAt(index);
        }

        private void ShowMetaAt(int index)
        {
            var tree = CSharpSyntaxTree.ParseText(Input.Text);
            SyntaxNode root;
            if (!tree.TryGetRoot(out root))
            {
                return;
            }
            var node = root.GetMostSpecificNodeOrTokenAt(index);

            Meta.Text = $"{node.Kind()} {node}";
        }

        private void Input_OnTextChanged(object sender, EventArgs e)
        {
            Vm.Parse(Input.Text);
        }
    }
}
