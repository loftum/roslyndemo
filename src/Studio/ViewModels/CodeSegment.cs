namespace Studio.ViewModels
{
    public struct CodeSegment
    {
        public int Start { get; }
        public int End { get; }
        public int Length { get; }
        public string Text { get; }

        public CodeSegment(int start, string text) : this()
        {
            text = text ?? "";
            Start = start;
            Length = text.Length;
            End = start + text.Length;
            Text = text;
        }

        public static CodeSegment Empty => new CodeSegment(0, "");
    }
}