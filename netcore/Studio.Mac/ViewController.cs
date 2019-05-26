using System;
using System.Threading.Tasks;
using AppKit;
using Foundation;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using RoslynDemo.Core;

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

        private async Task Reset()
        {
            var options = ScriptOptions.Default.WithReferences(Assemblies.FromCurrentContext())
                .WithImports(Assemblies.RoslynNamespaces);
            //_scriptState = await CSharpScript.RunAsync("", options, new Interacti)
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
