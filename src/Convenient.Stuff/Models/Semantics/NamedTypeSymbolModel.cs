using System.Linq;
using Microsoft.CodeAnalysis;

namespace Convenient.Stuff.Models.Semantics
{
    public class NamedTypeSymbolModel : TypeSymbolModel
    {
        public TypeKind TypeKind { get; }
        public string[] MemberNames { get; }

        public NamedTypeSymbolModel(INamedTypeSymbol namedTypeSymbol) : base(namedTypeSymbol)
        {
            TypeKind = namedTypeSymbol.TypeKind;
            MemberNames = namedTypeSymbol.MemberNames.ToArray();
        }
    }
}