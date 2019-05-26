namespace RoslynDemo.Core.Completion
{
    public class CompletionItem
    {
        public string Prefix { get; }
        public string Completion { get; }

        public string Text => $"{Prefix}{Completion}";
        public string Content { get; }
        public string Description { get; }

        public CompletionItem(string prefix, string completion, string content, string description)
        {
            Prefix = prefix;
            Completion = completion;
            Content = content;
            Description = description;
        }

        public CompletionItem(string prefix, string completion)
        {
            Prefix = prefix;
            Completion = completion;
            Content = Text;
            Description = Text;
        }
    }
}