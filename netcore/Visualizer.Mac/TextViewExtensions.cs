using AppKit;

namespace Visualizer.Mac
{
    public static class TextViewExtensions
    {
        public static CaretPosition GetCaretPosition(this NSTextView view)
        {
            var range = view.GetSelectedRange();
            var text = view.Value;
            var location = (int)range.Location;
            var row = 1;
            var column = 1;
            for (var ii = 0; ii < location; ii++)
            {
                switch (text[ii])
                {
                    case '\n':
                        row++;
                        column = 1;
                        break;
                    case '\r':
                        break;
                    default:
                        column++;
                        break;
                }
            }
            return new CaretPosition(location, row, column);
        }
    }
}
