using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Mime;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Convenient.Stuff.IO;
using ICSharpCode.AvalonEdit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Visualizer.ViewModels;

namespace Visualizer
{
    public static class TextExtensions
    {
        public static int GetIndexAt(this string text, TextViewPosition position)
        {
            var index = -1;
            var line = 1;
            var column = 0;
            using (var enumerator = text.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    index++;
                    switch (enumerator.Current)
                    {
                        case '\r':
                            break;
                        case '\n':
                            line++;
                            column = 1;
                            break;
                        default:
                            column++;
                            break;
                    }
                    if (line == position.Line && column == position.Column)
                    {
                        break;
                    }
                }
            }
            return index;
        }
    }

    public partial class MainWindow
    {
        private readonly FileManager _fileManager = new FileManager();
        protected MainViewModel Vm => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Parse_Click(object sender, RoutedEventArgs e)
        {
            Vm.Parse(Input.Text);
        }

        private void Compile_Click(object sender, RoutedEventArgs e)
        {
            Vm.Compile(Input.Text);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _fileManager.SaveJson(new Data
            {
                Input = Input.Text
            });

            base.OnClosing(e);
        }

        protected override void OnInitialized(EventArgs e)
        {
            var data = _fileManager.LoadJson<Data>() ?? new Data();
            Input.Text = data.Input;
            base.OnInitialized(e);
        }

        private void Input_OnMouseHover(object sender, MouseEventArgs e)
        {
            var input = (TextEditor) sender;
            var position = input.GetPositionFromPoint(e.GetPosition((IInputElement)sender));
            Meta.Text = position.ToString();
            if (position == null)
            {
                return;
            }
            
            var index = input.Text.GetIndexAt(position.Value);
            var caret = input.CaretOffset;
            if (index < 0)
            {
                return;
            }
            var tree = CSharpSyntaxTree.ParseText(Input.Text);
            SyntaxNode root;
            if (!tree.TryGetRoot(out root))
            {
                return;
            }
            var node = root.GetMostSpecificNodeAt(index);

            Meta.Text = $"{node.Kind()} {node} pos={index} caret={caret}";
        }
    }

    public static class SyntaxNodeExtensions
    {
        public static SyntaxNodeOrToken GetMostSpecificNodeAt(this SyntaxNode root, int position)
        {
            SyntaxNodeOrToken specific = root;
            foreach (var node in root.DescendantNodesAndTokens())
            {
                if (node.FullSpan.Covers(position) && node.FullSpan.IsMoreSpecificThan(specific.FullSpan))
                {
                    specific = node;
                }
            }
            return specific;
        }

        public static bool Covers(this TextSpan span, int position)
        {
            return span.Start <= position && span.End >= position;
        }

        public static bool IsMoreSpecificThan(this TextSpan span, TextSpan other)
        {
            return span.Start >= other.Start && span.End <= other.End;
        }
    }

    public class Data
    {
        public string Input { get; set; }
    }

    public class WindowSettings
    {
        public double Top { get; set; }
        public double Left { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public WindowSettings()
        {
            Top = 0;
            Left = 0;
            Width = 800;
            Height = 600;
        }
    }
}
