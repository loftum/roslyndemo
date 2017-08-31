using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Convenient.Stuff;
using Convenient.Stuff.Collections;
using Convenient.Stuff.IO;
using Convenient.Stuff.Models.Syntax;
using Convenient.Stuff.Serializers;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Search;
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
            SearchPanel.Install(SyntaxTree);
            SearchPanel.Install(Compilation);
            SearchPanel.Install(EmitResult);
            Parse();
            UpdateMeta();
            
            Input.TextArea.Caret.PositionChanged += CaretOnPositionChanged;
            Input.Focus();
        }

        private void CaretOnPositionChanged(object sender, EventArgs eventArgs)
        {
            UpdateMeta();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Save();
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
            var meta = Vm.GetMetaAt(index);
            Meta.Text = $"{meta.SyntaxNodeOrToken.Kind()} {meta.SyntaxNodeOrToken}";
            Syntax.Text = SyntaxMapper.Map(meta.SyntaxNodeOrToken).ToJson(true, true);
            Semantics.Text = meta.Semantics?.ToJson(true, true);
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                var completions = Vm.GetCompletions(Input.CaretOffset);
                var completionWindow = new CompletionWindow(Input.TextArea);
                completionWindow.CompletionList.CompletionData.AddRange(completions);
                completionWindow.Show();
                completionWindow.Closed += (o, ea) => completionWindow = null;
                e.Handled = true;
            }
            if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                 Save();
            }
            else if (e.Key == Key.B && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Build();
            }
        }

        private void Save()
        {
            _fileManager.SaveJson(new Data
            {
                Input = Input.Text
            });
        }

        private void Input_OnTextChanged(object sender, EventArgs e)
        {
            Parse();
        }

        private void Parse()
        {
            Vm.Parse(Input.Text);
            SyntaxTree.Text = Vm.GetTreeModel().ToJson(true, true);
            Compilation.Text = Vm.GetCompilationModel().ToJson(true, true);
        }

        private void Build_Click(object sender, RoutedEventArgs e)
        {
            Build();
        }

        private void Build()
        {
            EmitResult.Text = Vm.Emit().ToJson(true, true);
        }
    }
}
