using System;

using AppKit;
using Foundation;
using Microsoft.CodeAnalysis.Scripting;

namespace Studio.Mac
{
    public partial class ViewController : NSViewController
    {
        private ScriptState _scriptState;

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
            NSEvent.AddLocalMonitorForEventsMatchingMask(NSEventMask.KeyDown, HandleKeyDown);
        }

        private NSEvent HandleKeyDown(NSEvent theEvent)
        {
            var key = (NSKey)theEvent.KeyCode;
            switch (key)
            {
                case NSKey.F5:
                    return null;
                default:
                    return theEvent;
            }
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
