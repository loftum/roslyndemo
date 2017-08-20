using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace Studio.ViewModels
{
    public class CompletionData : ICompletionData
    {
        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Completion);
        }

        public CompletionData(string prefix, string completion, string description)
        {
            Prefix = prefix;
            Completion = completion;
            Description = description;
        }

        public CompletionData(string prefix, string completion)
        {
            Prefix = prefix;
            Completion = completion;
            Description = Text;
        }

        public string Prefix { get; }
        public string Completion { get; }

        public ImageSource Image => null;
        public string Text => $"{Prefix}{Completion}";
        public object Content => Text;
        public object Description { get; }
        public double Priority { get; set; }
    }
}