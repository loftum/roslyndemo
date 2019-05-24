// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Visualizer.Mac
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSTextView InputBox { get; set; }

		[Outlet]
		AppKit.NSTextView SyntaxTreeBox { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (InputBox != null) {
				InputBox.Dispose ();
				InputBox = null;
			}

			if (SyntaxTreeBox != null) {
				SyntaxTreeBox.Dispose ();
				SyntaxTreeBox = null;
			}
		}
	}
}
