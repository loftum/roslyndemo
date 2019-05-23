using System;
using System.Collections.Generic;
using System.Linq;
using AppKit;
using Foundation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynDemo.Core;
using RoslynDemo.Core.IO;


namespace Visualizer.Mac
{
    public partial class ViewController : NSViewController
    {
        private static readonly IEnumerable<PortableExecutableReference> References = Assemblies.FromCurrentContext()
            .Append(typeof(Console).Assembly)
            .Append(typeof(object).Assembly)
            .Select(a => MetadataReference.CreateFromFile(a.Location));


        public SyntaxTree SyntaxTree { get; private set; }
        public CSharpCompilation Compilation { get; private set; }

        public ViewController(IntPtr handle) : base(handle)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            InputBox.TextDidChange += Parse;
            // Do any additional setup after loading the view.
        }

        private void Parse(object sender, EventArgs e)
        {
            SyntaxTree = CSharpSyntaxTree.ParseText(InputBox.TextStorage.MutableString.ToString());
            Compilation = CSharpCompilation.Create("hest", new[] { SyntaxTree }, References, new CSharpCompilationOptions(OutputKind.ConsoleApplication));
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
