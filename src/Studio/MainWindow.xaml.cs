using System;
using System.ComponentModel;
using System.Windows.Input;
using Convenient.Stuff;
using Convenient.Stuff.ConsoleRedirect;
using Convenient.Stuff.IO;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Search;
using Studio.Extensions;
using Studio.ViewModels;

namespace Studio
{
    public partial class MainWindow
    {
        private readonly FileManager _fileManager = new FileManager();

        protected MainViewModel Vm => (MainViewModel) DataContext;

        public MainWindow()
        {
            InitializeComponent();
            SearchPanel.Install(Input);
            ConsoleOut.Writer.Add(new DocumentTextWriter(Console.Document, 0), Dispatcher);
            Input.Focus();
        }

        private async void Input_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                var statement = Input.GetSelectedOrAllText();
                var result = await Vm.Evaluate(statement.Text);
                Output.Text = result.ToResultString();
                Input.Focus();
            }
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                var statement = Input.GetCurrentStatement();
                var completions = Vm.GetCompletions(statement.Text);

                var completionWindow = new CompletionWindow(Input.TextArea);
                completionWindow.CompletionList.CompletionData.AddRange(completions);
                completionWindow.Show();
                completionWindow.Closed += (o, ea) => completionWindow = null;
                e.Handled = true;
            }
            else if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Save();
            }
        }

        private void Console_ScrollToEnd(object sender, EventArgs e)
        {
            var textBox = sender as TextEditor;
            textBox?.ScrollToEnd();
        }

        private void Save()
        {
            _fileManager.SaveJson(new Data
            {
                Input = Input.Text
            });
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
    }
}
