using ICSharpCode.AvalonEdit;

namespace Studio.Avalon
{
    public static class TextEditorExtensions
    {
        public static CodeSegment GetSelectedOrAllText(this TextEditor editor)
        {
            return editor.SelectionLength > 0
                ? new CodeSegment(editor.SelectionStart, editor.SelectedText)
                : new CodeSegment(0, editor.Text);
        }

        public static CodeSegment GetCurrentStatement(this TextEditor editor)
        {
            
            var caret = editor.TextArea.Caret;
            if (caret.Offset <= 0)
            {
                return CodeSegment.Empty;
            }
            var start = caret.Offset <= 0 ? 0 : caret.Offset - 1;
            var lineFeeds = 0;
            while (start > 0)
            {
                var c = editor.Text[start];
                if (c == '\n')
                {
                    if (lineFeeds > 0)
                    {
                        break;
                    }
                    lineFeeds++;
                }
                else if (c != '\r')
                {
                    lineFeeds = 0;
                }
                start--;
            }
            var currentLine = editor.Text.Substring(start, caret.Offset - start).Trim();
            return new CodeSegment(start, currentLine);
        }
    }
}