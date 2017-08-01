using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Editor.Models
{
    public class SyntaxTreeModel
    {
        public SyntaxTree Tree { get; }
        public SyntaxNodeModel Root { get; }

        public SyntaxTreeModel(SyntaxTree tree)
        {
            Tree = tree;
            Root = new SyntaxNodeModel(tree.GetRoot());
        }
    }

    public class SyntaxNodeModel
    {
        public string Type { get; }
        public string Text { get; }
        public List<SyntaxNodeModel> ChildNodes { get; }
        public Dictionary<string, object> Properties { get; }

        private static readonly List<string> IgnoredProperties;

        static SyntaxNodeModel()
        {
            IgnoredProperties = typeof(SyntaxNode).GetProperties()
                .Concat(typeof(CompilationUnitSyntax).GetProperties())
                .Select(p => p.Name)
                .ToList();
        }

        private static bool Allow(PropertyInfo property)
        {
            return !IgnoredProperties.Contains(property.Name) &&
                   !typeof(SyntaxNode).IsAssignableFrom(property.PropertyType);
        }

        public SyntaxNodeModel(SyntaxNode node)
        {
            var type = node.GetType();
            Type = type.Name;
            Text = node.GetText().ToString();

            Properties = type.GetProperties()
                .Where(Allow)
                .ToDictionary(p => p.Name, p => p.GetValue(node));

            ChildNodes = node.ChildNodes().Select(n => new SyntaxNodeModel(n)).ToList();
        }
    }
}