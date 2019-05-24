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
		AppKit.NSTextView CompilationBox { get; set; }

		[Outlet]
		AppKit.NSTextView EmitBox { get; set; }

		[Outlet]
		AppKit.NSTextView InputBox { get; set; }

		[Outlet]
		AppKit.NSTextView SemanticsBox { get; set; }

		[Outlet]
		AppKit.NSTextView SyntaxBox { get; set; }

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

			if (CompilationBox != null) {
				CompilationBox.Dispose ();
				CompilationBox = null;
			}

			if (EmitBox != null) {
				EmitBox.Dispose ();
				EmitBox = null;
			}

			if (SyntaxBox != null) {
				SyntaxBox.Dispose ();
				SyntaxBox = null;
			}

			if (SemanticsBox != null) {
				SemanticsBox.Dispose ();
				SemanticsBox = null;
			}
		}
	}
}
