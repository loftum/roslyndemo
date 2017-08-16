using ICSharpCode.AvalonEdit;

namespace Studio
{
    public static class TextEditorExtensions
    {
        public static string GetSelectedOrAllText(this TextEditor editor)
        {
            var selectedText = editor.SelectedText;
            return string.IsNullOrEmpty(selectedText) ? editor.Text : selectedText;
        }

        public static string GetCurrentStatement(this TextEditor editor)
        {
            var caret = editor.TextArea.Caret;
            if (caret.Offset <= 0)
            {
                return "";
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
            return currentLine;
        }
    }
}