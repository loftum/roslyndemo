using System.Linq;
using Microsoft.CodeAnalysis;

namespace RoslynDemo.Core.Models.Semantics
{
    public class NamedTypeSymbolModel : TypeSymbolModel
    {
        public string[] MemberNames { get; }

        public NamedTypeSymbolModel(INamedTypeSymbol namedTypeSymbol) : base(namedTypeSymbol)
        {
            MemberNames = namedTypeSymbol.MemberNames.ToArray();
        }
    }
}