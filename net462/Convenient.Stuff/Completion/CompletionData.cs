using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace Convenient.Stuff.Completion
{
    public class CompletionData : ICompletionData
    {
        private readonly string _prefix;
        private readonly string _completion;

        public ImageSource Image => null;
        public string Text => $"{_prefix}{_completion}";
        public object Content { get; }
        public object Description { get; }
        public double Priority { get; set; }

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, _completion);
            
        }

        public CompletionData(string prefix, string completion, string content, string description)
        {
            _prefix = prefix;
            _completion = completion;
            Content = content;
            Description = description;
        }

        public CompletionData(string prefix, string completion)
        {
            _prefix = prefix;
            _completion = completion;
            Content = Text;
            Description = Text;
        }
    }
}