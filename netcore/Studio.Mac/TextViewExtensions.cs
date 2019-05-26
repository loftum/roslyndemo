using AppKit;

namespace Studio.Mac
{
    public static class TextViewExtensions
    {
        public static string GetSelectedText(this NSTextView textView)
        {
            var range = textView.GetSelectedRange();
            var length = (int)range.Length;
            if (length == 0)
            {
                return textView.Value;
            }
            return textView.Value.Substring((int)range.Location, length);
        }
    }
}
