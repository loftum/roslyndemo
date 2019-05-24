using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppKit;
using CoreGraphics;
using CoreText;
using Foundation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynDemo.Core;
using RoslynDemo.Core.Models.Syntax;
using RoslynDemo.Core.Serializers;

namespace Visualizer.Mac
{
    public partial class ViewController : NSViewController
    {
        private string _syntaxTreeText;
        [Export("SyntaxTreeText")]
        public string SyntaxTreeText
        {
            get => _syntaxTreeText;
            set
            {
                WillChangeValue("SyntaxTreeText");
                _syntaxTreeText = value;
                DidChangeValue("SyntaxTreeText");
            }
        }

        private DateTimeOffset _parseAt;
        private bool _doParse;
        private readonly Task _parser;

        private static readonly IEnumerable<PortableExecutableReference> References = Assemblies.FromCurrentContext()
            .Append(typeof(Console).Assembly)
            .Append(typeof(object).Assembly)
            .Select(a => MetadataReference.CreateFromFile(a.Location));


        public SyntaxTree SyntaxTree { get; private set; }
        public CSharpCompilation Compilation { get; private set; }

        public ViewController(IntPtr handle) : base(handle)
        {
            _parser = new Task(DoParse);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _parser.Start();
            SyntaxTreeBox.TextStorage.SetString(new NSAttributedString("Hello", StringAttributes));
            InputBox.TextDidChange += Parse;
            // Do any additional setup after loading the view.
        }

        private static readonly CTStringAttributes StringAttributes = new CTStringAttributes
        {
            ForegroundColor = new CGColor(1.0f, 1.0f, 1.0f)
        };

        private void DoParse()
        {
            while (true)
            {
                if (!_doParse)
                {
                    Thread.Sleep(500);
                    continue;
                }
                _doParse = false;
                while (_parseAt > DateTimeOffset.UtcNow)
                {
                    Thread.Sleep(500);
                }

                Console.WriteLine("hello 1");

                SyntaxTree = CSharpSyntaxTree.ParseText(InputBox.Value);
                Console.WriteLine("hello 2");
                Compilation = CSharpCompilation.Create("hest", new[] { SyntaxTree }, References, new CSharpCompilationOptions(OutputKind.ConsoleApplication));
                Console.WriteLine("hello 3");
                var treeModel = SyntaxMapper.Map(SyntaxTree);
                SyntaxTreeText = treeModel.ToJson(true, true);
            }
        }

        private void Parse(object sender, EventArgs e)
        {
            _doParse = true;
            _parseAt = DateTimeOffset.UtcNow.AddSeconds(1);
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
