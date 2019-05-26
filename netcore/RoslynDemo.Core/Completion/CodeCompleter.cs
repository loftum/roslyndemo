using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynDemo.Core.Models.Semantics;
using RoslynDemo.Core.Models.Syntax;
using RoslynDemo.Core.Syntax;

namespace RoslynDemo.Core.Completion
{
    public class CodeCompleter
    {
        private readonly IList<ISymbol> _symbols;

        public SyntaxNodeModel ContainerNode { get; }
        public SyntaxNodeModel PrefixNode { get; }

        public NamespaceOrTypeSymbolModel NamespaceOrTypeSymbol { get; }
        public SymbolModel Symbol { get; }
        public List<SymbolModel> CompletionCandidates { get; }
        private readonly string _prefix;

        public CodeCompleter(SyntaxTree tree, Compilation compilation, int location)
        {
            var semantics = compilation.GetSemanticModel(tree);
            
            var nodes = GetNodes(tree.GetRoot().GetMostSpecificNodeOrTokenAt(location));
            _prefix = nodes.Prefix?.GetText().ToString() ?? "";
            
            var completion = semantics.GetCompletionSymbols(nodes.Container);
            
            var symbols = semantics.LookupSymbols(tree.Length, completion.NamespaceOrType)
                .Where(s => s.IsStatic == completion.SearchForStatic)
                .ToList();

            _symbols = symbols;

            NamespaceOrTypeSymbol = new NamespaceOrTypeSymbolModel(completion.NamespaceOrType);
            Symbol = completion.Symbol == null ? null : new SymbolModel(completion.Symbol);

            ContainerNode = SyntaxMapper.Map(nodes.Container);
            PrefixNode = SyntaxMapper.Map(nodes.Prefix);

            CompletionCandidates = symbols.Select(s => new SymbolModel(s)).ToList();
        }

        public IEnumerable<CompletionItem> GetCompletions()
        {
            return Enumerable.Where<ISymbol>(_symbols, s => s.Name.StartsWith(_prefix))
                .Select(s => new CompletionItem(_prefix,
                s.ToDisplayString(DisplayFormats.CompletionFormat).Substring(_prefix.Length),
                s.ToDisplayString(DisplayFormats.ContentFormat),
                s.ToDisplayString(DisplayFormats.DescriptionFormat))
                );
        }

        private static CompletionNodes GetNodes(SyntaxNodeOrToken nodeOrToken)
        {
            SyntaxNodeOrToken dot;
            SyntaxNode prefix = null;

            if (nodeOrToken.IsNode)
            {
                prefix = nodeOrToken.AsNode();
                dot = nodeOrToken.GetPreviousSibling();
                if (dot.Kind() != SyntaxKind.DotToken)
                {
                    return new CompletionNodes(null, prefix);
                }
            }
            else
            {
                if (nodeOrToken.Kind() != SyntaxKind.DotToken)
                {
                    return new CompletionNodes(null, prefix);
                }
                dot = nodeOrToken;
            }
            
            var previous = dot.GetPreviousSibling();
            return previous.IsNode ? new CompletionNodes(previous.AsNode(), prefix) : new CompletionNodes(null, prefix);
        }
    }
}