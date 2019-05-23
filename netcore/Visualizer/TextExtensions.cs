using ICSharpCode.AvalonEdit;

namespace Visualizer
{
    public static class TextExtensions
    {
        public static int GetIndexAt(this string text, TextViewPosition position)
        {
            var index = -1;
            var line = 1;
            var column = 0;
            using (var enumerator = text.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    index++;
                    switch (enumerator.Current)
                    {
                        case '\n':
                            line++;
                            column = 0;
                            break;
                        default:
                            column++;
                            break;
                    }
                    if (line >= position.Line && column >= position.Column)
                    {
                        break;
                    }
                }
            }
            return index;
        }
    }
}