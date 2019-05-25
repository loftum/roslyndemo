using System;

using AppKit;
using Foundation;

namespace Studio.Mac
{
    public partial class ViewController : NSViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var font = NSFont.FromFontName("Monaco", 12);
            InputBox.Font = font;
            InputBox.AutomaticSpellingCorrectionEnabled = false;
            InputBox.AutomaticQuoteSubstitutionEnabled = false;
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }
    }
}
