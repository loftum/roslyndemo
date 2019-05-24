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
using RoslynDemo.Core.Models.Syntax;
using RoslynDemo.Core.Serializers;

namespace Visualizer.Mac
{
    public class Locked<T>
    {
        private readonly object _lock = new object();
        private T _value;

        public T Get()
        {
            lock(_lock)
            {
                return _value;
            }
        }

        public void Set(T value)
        {
            lock(_lock)
            {
                _value = value;
            }
        }

        public static implicit operator T (Locked<T> locked)
        {
            return locked.Get();
        }

    }

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
            InputBox.TextDidChange += Parse;
            // Do any additional setup after loading the view.
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
                SyntaxTree = CSharpSyntaxTree.ParseText(input);
                Compilation = CSharpCompilation.Create("hest", new[] { SyntaxTree }, References, new CSharpCompilationOptions(OutputKind.ConsoleApplication));
                var treeModel = SyntaxMapper.Map(SyntaxTree);
                InvokeOnMainThread(() => SyntaxTreeText = treeModel.ToJson(true, true));
            }
        }

        private void Parse(object sender, EventArgs e)
        {
            InputText = InputBox.Value;
            ShouldParse = true;
            ParseAt = DateTimeOffset.UtcNow.AddMilliseconds(300);
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
