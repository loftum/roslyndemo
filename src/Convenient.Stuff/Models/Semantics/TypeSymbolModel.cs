using Microsoft.CodeAnalysis;

namespace Convenient.Stuff.Models.Semantics
{
    public class TypeSymbolModel
    {
        public NamedTypeSymbolModel BaseType { get; }
        public string Name { get; }
        public SpecialType SpecialType { get; }


        public TypeSymbolModel(ITypeSymbol typeSymbol)
        {
            Name = typeSymbol.Name;
            SpecialType = typeSymbol.SpecialType;
            BaseType = typeSymbol.BaseType == null ? null : new NamedTypeSymbolModel(typeSymbol.BaseType);
        }
    }
}