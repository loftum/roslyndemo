using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Convenient.Stuff.ConsoleRedirect;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Search;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Studio.ViewModels;

namespace Studio
{
    public partial class MainWindow
    {
        protected MainViewModel Vm => (MainViewModel) DataContext;

        public MainWindow()
        {
            InitializeComponent();
            SearchPanel.Install(Input);
            ConsoleOut.Writer.Add(new DocumentTextWriter(Console.Document, 0), Dispatcher);
        }

        private async void Input_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                var statement = Input.GetSelectedOrAllText();
                var result = await Vm.Evaluate(statement.Text);
                Output.Text = result?.ToString();
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
        }

        private void Console_ScrollToEnd(object sender, EventArgs e)
        {
            var textBox = sender as TextEditor;
            textBox?.ScrollToEnd();
        }
    }

    public static class ListExtensions
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
        }
    }

    public class CompletionData : ICompletionData
    {
        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Completion);
        }

        public CompletionData(string prefix, string completion)
        {
            Prefix = prefix;
            Completion = completion;
        }

        public string Prefix { get; }
        public string Completion { get; }

        public ImageSource Image => null;
        public string Text => $"{Prefix}{Completion}";
        public object Content => Text;
        public object Description => Text;
        public double Priority { get; set; }
    }
}
