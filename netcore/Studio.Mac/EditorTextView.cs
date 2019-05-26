using System;
using System.Collections.Generic;
using System.Linq;
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

        }

        public override void KeyDown(NSEvent theEvent)
        {
            base.KeyDown(theEvent);
            Console.WriteLine($"KeyDown: {theEvent.KeyCode}");
        }
    }
}
