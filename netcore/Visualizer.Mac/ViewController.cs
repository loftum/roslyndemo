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
using RoslynDemo.Core.IO;
using RoslynDemo.Core.Models;
using RoslynDemo.Core.Models.Syntax;
using RoslynDemo.Core.Serializers;

namespace Visualizer.Mac
{
    public partial class ViewController : NSViewController
    {
        private readonly FileManager _fileManager = new FileManager("out");
        private string _syntaxTreeText;
        private string _compilationText;
        private string _syntaxText;
        private string _semanticsText;
        private string caretText;
        private string _emitText;

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

        [Export(nameof(EmitText))]
        public string EmitText
        {
            get => _emitText;
            set
            {
                WillChangeValue(nameof(EmitText));
                _emitText = value;
                DidChangeValue(nameof(EmitText));
            }
        }

        [Export(nameof(CaretText))]
        public string CaretText
        {
            get => caretText;
            set
            {
                WillChangeValue(nameof(CaretText));
                caretText = value;
                DidChangeValue(nameof(CaretText));
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
            InputBox.AutomaticSpellingCorrectionEnabled = false;
            InputBox.AutomaticQuoteSubstitutionEnabled = false;
            SyntaxTreeBox.Font = font;
            CompilationBox.Font = font;
            EmitBox.Font = font;
            SyntaxBox.Font = font;
            SemanticsBox.Font = font;
            SyntaxText = "";
            SemanticsText = "";
            SyntaxTreeText = "";
            EmitText = "";
            InputBox.TextDidChange += Parse;
            InputBox.DidChangeSelection += UpdateMeta;
            UpdateMeta(this, EventArgs.Empty);
            // Do any additional setup after loading the view.
        }

        private void UpdateMeta(object sender, EventArgs e)
        {
            var caret = InputBox.GetCaretPosition();
            InvokeOnMainThread(() => CaretText = $"Index: {caret.Location} ({caret.Row}, {caret.Column})");
            if (Model == null)
            {
                return;
            }

            var meta = Model.GetMetaAt(caret.Location);
            var syntax = SyntaxMapper.Map(meta.SyntaxNodeOrToken).ToPrettyJson();
            var semantics = meta.Semantics?.ToPrettyJson();
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
                var tree = SyntaxMapper.Map(Model.SyntaxTree).ToPrettyJson();
                var compilation = CompilationMapper.Map(Model.Compilation).ToPrettyJson();
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

        partial void Emit(NSObject sender)
        {
            if (Model == null)
            {
                InvokeOnMainThread(() => EmitText = "Successfully emitted nothing into the void of zilch. It is dangerous to walk in the nil. Take this ASCII sword:\n\n ()///}============>\n\n(You haven't written any code yet.)");
                return;
            }
            _fileManager.CleanFolder();
            var programPath = _fileManager.ToFullPath("program.dll");
            Console.WriteLine($"Writing {programPath}");
            var result = Model.Compilation.Emit(programPath);
            _fileManager.SaveJson(RuntimeConfig.Generate(), "program.runtimeconfig.json");
            var emitText = CompilationMapper.Map(result).ToPrettyJson();
            InvokeOnMainThread(() => EmitText = $"{emitText}\n\nEmitted to {programPath}");
        }

        //public override NSObject RepresentedObject
        //{
        //    get
        //    {
        //        return base.RepresentedObject;
        //    }
        //    set
        //    {
        //        base.RepresentedObject = value;
        //        // Update the view, if already loaded.
        //    }
        //}
    }
}
