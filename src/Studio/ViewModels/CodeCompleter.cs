using System.Collections.Generic;
using System.Linq;
using Convenient.Stuff.Models.Semantics;
using Convenient.Stuff.Models.Syntax;
using Convenient.Stuff.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Scripting;

namespace Studio.ViewModels
{
    public class CodeCompleter
    {
        private SyntaxNode _contextNode;
        private SyntaxNode _prefixNode;
        private SymbolInfo _contextInfo;
        private TypeInfo _contextType;
        private readonly IList<ISymbol> _symbols;
        private readonly SemanticModel _semantics;
        private readonly int _position;

        public SyntaxNodeModel ContextNode { get; }
        public SymbolInfoModel ContextSymbolInfo { get; }
        public TypeInfoModel ContextTypeInfo { get; }
        public SyntaxNodeModel PrefixNode { get; }
        public List<SymbolModel> Symbols { get; }

        private static readonly SymbolDisplayFormat ContentFormat;
        private static readonly SymbolDisplayFormat DescriptionFormat;

        static CodeCompleter()
        {
            ContentFormat = new SymbolDisplayFormat(SymbolDisplayGlobalNamespaceStyle.Omitted,
                SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
                SymbolDisplayGenericsOptions.IncludeTypeParameters,
                SymbolDisplayMemberOptions.IncludeParameters,
                SymbolDisplayDelegateStyle.NameAndParameters,
                SymbolDisplayExtensionMethodStyle.InstanceMethod,
                SymbolDisplayParameterOptions.IncludeName | SymbolDisplayParameterOptions.IncludeType | SymbolDisplayParameterOptions.IncludeOptionalBrackets | SymbolDisplayParameterOptions.IncludeParamsRefOut,
                SymbolDisplayPropertyStyle.NameOnly,
                SymbolDisplayLocalOptions.None,
                SymbolDisplayKindOptions.None,
                SymbolDisplayMiscellaneousOptions.None);

            DescriptionFormat = new SymbolDisplayFormat(SymbolDisplayGlobalNamespaceStyle.Omitted,
                SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
                SymbolDisplayGenericsOptions.IncludeTypeParameters,
                SymbolDisplayMemberOptions.IncludeModifiers | SymbolDisplayMemberOptions.IncludeContainingType | SymbolDisplayMemberOptions.IncludeAccessibility | SymbolDisplayMemberOptions.IncludeConstantValue | SymbolDisplayMemberOptions.IncludeParameters | SymbolDisplayMemberOptions.IncludeType,
                SymbolDisplayDelegateStyle.NameAndParameters,
                SymbolDisplayExtensionMethodStyle.InstanceMethod,
                SymbolDisplayParameterOptions.IncludeName | SymbolDisplayParameterOptions.IncludeType | SymbolDisplayParameterOptions.IncludeOptionalBrackets | SymbolDisplayParameterOptions.IncludeParamsRefOut,
                SymbolDisplayPropertyStyle.NameOnly,
                SymbolDisplayLocalOptions.None,
                SymbolDisplayKindOptions.None,
                SymbolDisplayMiscellaneousOptions.None);

        }

        public CodeCompleter(Script script)
        {
            var compilation = script.GetCompilation();

            var tree = compilation.SyntaxTrees.Single();
            _position = tree.Length;
            GetNodes(tree.GetRoot().GetMostSpecificNodeOrTokenAt(tree.Length -1));

            var semantics = compilation.GetSemanticModel(tree);

            var contextSymbolInfo = semantics.GetSymbolInfo(_contextNode);

            var contextTypeInfo = semantics.GetTypeInfo(_contextNode);
            
            var symbols = semantics.LookupSymbols(tree.Length, contextTypeInfo.Type)
                .Where(s => s.IsStatic == (contextSymbolInfo.Symbol.Kind == SymbolKind.NamedType))
                .ToList();

            _semantics = semantics;
            _contextInfo = contextSymbolInfo;
            _contextType = contextTypeInfo;
            _symbols = symbols;

            var mapper = new SyntaxNodeMapper();
            ContextNode = mapper.Map(_contextNode);
            PrefixNode = mapper.Map(_prefixNode);
            Symbols = symbols.Select(s => new SymbolModel(s)).ToList();
            ContextSymbolInfo = new SymbolInfoModel(contextSymbolInfo);
            ContextTypeInfo = new TypeInfoModel(contextTypeInfo);
        }

        public IEnumerable<CompletionData> GetCompletions()
        {
            var prefix = _prefixNode?.GetText()?.ToString() ?? "";
            return _symbols.Where(s => s.Name.StartsWith(prefix))
                .Select(s => new CompletionData(prefix, s.Name.Substring(prefix.Length), $"{s.ToDisplayString(ContentFormat)}", $"{s.ToDisplayString(DescriptionFormat)}"));
        }

        private void GetNodes(SyntaxNodeOrToken nodeOrToken)
        {
            SyntaxNodeOrToken dot;

            switch (nodeOrToken.Kind())
            {
                case SyntaxKind.IdentifierName:
                    _prefixNode = nodeOrToken.AsNode();
                    dot = nodeOrToken.GetPreviousSibling();
                    if (dot.Kind() != SyntaxKind.DotToken)
                    {
                        return;
                    }
                    break;
                case SyntaxKind.DotToken:
                    dot = nodeOrToken;
                    break;
                default:
                    return;
            }

            var previous = dot.GetPreviousSibling();
            if (previous.IsNode)
            {
                _contextNode = previous.AsNode();
            }
        }
    }
}