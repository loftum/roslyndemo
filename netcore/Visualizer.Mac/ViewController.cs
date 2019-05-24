using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppKit;
using Foundation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynDemo.Core;
using RoslynDemo.Core.Models;
using RoslynDemo.Core.Models.Syntax;
using RoslynDemo.Core.Serializers;

namespace Visualizer.Mac
{
    public partial class ViewController : NSViewController
    {
        private string _syntaxTreeText;
        private string _compilationText;
        private string _syntaxText;
        private string _semanticsText;

        [Export(nameof(SyntaxTreeText))]
        public string SyntaxTreeText
        {
            get => _syntaxTreeText;
            set
            {
                WillChangeValue(nameof(SyntaxTreeText));
                _syntaxTreeText = value;
                DidChangeValue(nameof(SyntaxTreeText));
            }
        }

        [Export(nameof(CompilationText))]
        public string CompilationText
        {
            get => _compilationText;
            set
            {
                WillChangeValue(nameof(CompilationText));
                _compilationText = value;
                DidChangeValue(nameof(CompilationText));
            }
        }

        [Export(nameof(SyntaxText))]
        public string SyntaxText
        {
            get => _syntaxText;
            set
            {
                WillChangeValue(nameof(SyntaxText));
                _syntaxText = value;
                DidChangeValue(nameof(SyntaxText));
            }
        }

        [Export(nameof(SemanticsText))]
        public string SemanticsText
        {
            get => _semanticsText;
            set
            {
                WillChangeValue(nameof(SemanticsText));
                _semanticsText = value;
                DidChangeValue(nameof(SemanticsText));
            }
        }

        private readonly Locked<string> _inputText = new Locked<string>();
        public string InputText
        {
            get => _inputText.Get();
            set => _inputText.Set(value);
        }

        private readonly Locked<bool> _shouldParse = new Locked<bool>();
        public bool ShouldParse
        {
            get => _shouldParse.Get();
            set => _shouldParse.Set(value);
        }


        private readonly Locked<DateTimeOffset> _parseAt = new Locked<DateTimeOffset>();
        public DateTimeOffset ParseAt
        {
            get => _parseAt.Get();
            set => _parseAt.Set(value);
        }

        private readonly Task _parser;

        private static readonly IEnumerable<PortableExecutableReference> References = Assemblies.FromCurrentContext()
            .Append(typeof(Console).Assembly)
            .Append(typeof(object).Assembly)
            .Select(a => MetadataReference.CreateFromFile(a.Location));

        private readonly Locked<VisualizerModel> _m = new Locked<VisualizerModel>();

        public VisualizerModel Model
        {
            get => _m.Get();
            set => _m.Set(value);
        }

        public ViewController(IntPtr handle) : base(handle)
        {
            _parser = new Task(DoParse);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _parser.Start();
            var font = NSFont.FromFontName("Monaco", 12);
            InputBox.Font = font;
            SyntaxTreeBox.Font = font;
            CompilationBox.Font = font;
            EmitBox.Font = font;
            SyntaxBox.Font = font;
            SemanticsBox.Font = font;
            InputBox.TextDidChange += Parse;
            InputBox.DidChangeSelection += UpdateMeta;
            // Do any additional setup after loading the view.
        }

        private void UpdateMeta(object sender, EventArgs e)
        {
            var range = InputBox.SelectedRange;
            if (Model == null || Model.SyntaxTree.Length < range.Location)
            {
                return;
            }

            var meta = Model.GetMetaAt((int)range.Location);
            var syntax = SyntaxMapper.Map(meta.SyntaxNodeOrToken).ToJson(true, true);
            var semantics = meta.Semantics?.ToJson(true, true);
            InvokeOnMainThread(() =>
            {
                SyntaxText = syntax;
                SemanticsText = semantics;
            });
        }

        private void DoParse()
        {
            while (true)
            {
                if (!ShouldParse)
                {
                    Thread.Sleep(100);
                    continue;
                }
                ShouldParse = false;
                while (DateTimeOffset.UtcNow < ParseAt)
                {
                    Thread.Sleep(100);
                }

                var input = InputText;
                Console.WriteLine($"Parsing {input}");
                Model = VisualizerModel.Parse(input);
                var tree = SyntaxMapper.Map(Model.SyntaxTree).ToJson(true, true);
                var compilation = CompilationMapper.Map(Model.Compilation).ToJson(true, true);
                InvokeOnMainThread(() =>
                {
                    SyntaxTreeText = tree;
                    CompilationText = compilation;
                });
            }
        }

        private void Parse(object sender, EventArgs e)
        {
            InputText = InputBox.Value;
            ShouldParse = true;
            ParseAt = DateTimeOffset.UtcNow.AddMilliseconds(100);
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
