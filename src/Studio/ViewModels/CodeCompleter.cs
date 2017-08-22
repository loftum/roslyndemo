using System.Collections.Generic;
using System.Linq;
using Convenient.Stuff.Models.Semantics;
using Convenient.Stuff.Models.Syntax;
using Convenient.Stuff.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;

namespace Studio.ViewModels
{
    public static class CodeCompletionExtensions
    {
        public static CompletionSymbols GetCompletionSymbols(this SemanticModel semantics, SyntaxNode node, INamespaceOrTypeSymbol defaultValue)
        {
            if (node == null)
            {
                return new CompletionSymbols(defaultValue, null);
            }
            var symbolInfo = semantics.GetSymbolInfo(node);

            var symbol = symbolInfo.Symbol;
            var namespaceOrType = symbol as INamespaceOrTypeSymbol;
            if (namespaceOrType != null)
            {
                return new CompletionSymbols(namespaceOrType, null);
            }

            var s = node as LiteralExpressionSyntax;
            

            

            var typeInfo = semantics.GetTypeInfo(node);
            var type = typeInfo.ConvertedType ?? typeInfo.Type;
            return type == null
                ? new CompletionSymbols(defaultValue, null)
                : new CompletionSymbols(type, symbol);
        }
    }

    public struct CompletionSymbols
    {
        public INamespaceOrTypeSymbol NamespaceOrType { get; }
        public ISymbol Symbol { get; }

        public bool SearchForStatic => Symbol == null;

        public CompletionSymbols(INamespaceOrTypeSymbol namespaceOrType, ISymbol symbol)
        {
            NamespaceOrType = namespaceOrType;
            Symbol = symbol;
        }
    }

    public class CodeCompleter
    {
        private readonly IList<ISymbol> _symbols;
        private readonly SemanticModel _semantics;

        public SyntaxNodeModel ContainerNode { get; }
        public SyntaxNodeModel PrefixNode { get; }

        public NamespaceOrTypeSymbolModel NamespaceOrTypeSymbol { get; }
        public SymbolModel Symbol { get; }
        public List<SymbolModel> CompletionCandidates { get; }
        private readonly string _prefix;

        public CodeCompleter(Script script)
        {
            var compilation = script.GetCompilation();
            var tree = compilation.SyntaxTrees.Single();
            var semantics = compilation.GetSemanticModel(tree);

            var nodes = GetNodes(tree.GetRoot().GetMostSpecificNodeOrTokenAt(tree.Length -1));
            _prefix = nodes.Prefix?.GetText().ToString() ?? "";

            var completion = semantics.GetCompletionSymbols(nodes.Container, compilation.GlobalNamespace);
            
            var symbols = semantics.LookupSymbols(tree.Length, completion.NamespaceOrType)
                .Where(s => s.IsStatic == completion.SearchForStatic)
                .ToList();

            _semantics = semantics;
            _symbols = symbols;

            var mapper = new SyntaxNodeMapper();


            

            NamespaceOrTypeSymbol = new NamespaceOrTypeSymbolModel(completion.NamespaceOrType);
            Symbol = completion.Symbol == null ? null : new SymbolModel(completion.Symbol);

            ContainerNode = mapper.Map(nodes.Container);
            PrefixNode = mapper.Map(nodes.Prefix);

            CompletionCandidates = symbols.Select(s => new SymbolModel(s)).ToList();
        }

        public IEnumerable<CompletionData> GetCompletions()
        {
            return _symbols.Where(s => s.Name.StartsWith(_prefix))
                .Select(s => new CompletionData(_prefix, s.Name.Substring(_prefix.Length), $"{s.ToDisplayString(DisplayFormats.ContentFormat)}", $"{s.ToDisplayString(DisplayFormats.DescriptionFormat)}"));
        }

        private static CompletionNodes GetNodes(SyntaxNodeOrToken nodeOrToken)
        {
            SyntaxNodeOrToken dot;
            SyntaxNode prefix = null;

            switch (nodeOrToken.Kind())
            {
                case SyntaxKind.IdentifierName:
                    prefix = nodeOrToken.AsNode();
                    dot = nodeOrToken.GetPreviousSibling();
                    if (dot.Kind() != SyntaxKind.DotToken)
                    {
                        return new CompletionNodes(null, prefix);
                    }
                    break;
                case SyntaxKind.DotToken:
                    dot = nodeOrToken;
                    break;
                default:
                    return new CompletionNodes(null, null);
            }

            var previous = dot.GetPreviousSibling();
            return previous.IsNode ? new CompletionNodes(previous.AsNode(), prefix) : new CompletionNodes(null, prefix);
        }
    }

    public struct CompletionNodes
    {
        public SyntaxNode Container { get; }
        public SyntaxNode Prefix { get; }

        public CompletionNodes(SyntaxNode container, SyntaxNode prefix)
        {
            Container = container;
            Prefix = prefix;
        }
    }
}