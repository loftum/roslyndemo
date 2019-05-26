using System;
using Foundation;
using AppKit;

namespace Studio.Mac
{
    public partial class EditorTextView : NSTextView
    {
        // Called when created from unmanaged code
        public EditorTextView(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public EditorTextView(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
            AutomaticTextReplacementEnabled = false;
            AutomaticQuoteSubstitutionEnabled = false;
            Font = NSFont.FromFontName("Monaco", 12);
        }
    }
}
