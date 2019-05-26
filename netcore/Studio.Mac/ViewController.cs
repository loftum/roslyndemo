using System;
using System.Threading.Tasks;
using AppKit;
using Foundation;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using RoslynDemo.Core;
using RoslynDemo.Core.Studio;

namespace Studio.Mac
{
    public partial class ViewController : NSViewController
    {
        private CSharpScripter _scripter = new CSharpScripter();
        private string _outputText;

        [Export(nameof(OutputText))]
        public string OutputText
        {
            get => _outputText;
            set
            {
                WillChangeValue(nameof(OutputText));
                _outputText = value;
                DidChangeValue(nameof(OutputText));
            }
        }

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
                    EvaluateAsync().Wait();
                    return null;
                default:
                    return theEvent;
            }
        }

        private async Task EvaluateAsync()
        {
            var code = InputBox.GetSelectedText();
            var result = await _scripter.EvaluateAsync(code);
            
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
