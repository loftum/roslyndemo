using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Convenient.Stuff;
using Convenient.Stuff.IO;
using Convenient.Stuff.Models.Semantics;
using Convenient.Stuff.Models.Syntax;
using Convenient.Stuff.Serializers;
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
            UpdateMeta();
            Input.TextArea.Caret.PositionChanged += CaretOnPositionChanged;
            Input.Focus();
        }

        private void CaretOnPositionChanged(object sender, EventArgs eventArgs)
        {
            UpdateMeta();
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

        private void UpdateMeta()
        {
            var index = Input.CaretOffset;
            UpdateCaret(index);
            Caret.Text = $"Caret: {index}";
            ShowMetaAt(index);
        }

        private void UpdateCaret(int index)
        {
            Caret.Text = $"Caret: {index}";
        }

        private void ShowMetaAt(int index)
        {
            var tree = CSharpSyntaxTree.ParseText(Input.Text);
            SyntaxNode root;
            if (!tree.TryGetRoot(out root))
            {
                return;
            }
            var nodeOrToken = root.GetMostSpecificNodeOrTokenAt(index);

            Meta.Text = $"{nodeOrToken.Kind()} {nodeOrToken}";
            Syntax.Text = SyntaxMapper.Map(nodeOrToken).ToJson(true, true);
            Semantics.Text = GetSemantics(nodeOrToken, index).ToJson(true, true);
        }

        private static readonly IList<MetadataReference> References;

        static MainWindow()
        {
            References = AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .ToArray();
        }

        private static Dictionary<string, object> GetSemantics(SyntaxNodeOrToken nodeOrToken, int index)
        {
            var semantics = CSharpCompilation.Create("hest", new[] { nodeOrToken.SyntaxTree }, References, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .GetSemanticModel(nodeOrToken.SyntaxTree);

            var result = new Dictionary<string, object>();
            var node = nodeOrToken.AsNode();
            if (node != null)
            {
                result["TypeInfo"] = SymbolMapper.Map(semantics.GetTypeInfo(node));
                result["SymbolInfo"] = SymbolMapper.Map(semantics.GetSymbolInfo(node));
            }
            result["EnclosingSymbol"] = SymbolMapper.Map(semantics.GetEnclosingSymbol(index));

            return result;
        }

        private void Input_OnTextChanged(object sender, EventArgs e)
        {
            Vm.Parse(Input.Text);
        }
    }
}
