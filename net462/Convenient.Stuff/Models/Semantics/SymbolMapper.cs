using Microsoft.CodeAnalysis;

namespace Convenient.Stuff.Models.Semantics
{
    public class SymbolMapper
    {
        public static TypeInfoModel Map(TypeInfo typeInfo)
        {
            return new TypeInfoModel(typeInfo);
        }

        public static SymbolInfoModel Map(SymbolInfo symbolInfo)
        {
            return new SymbolInfoModel(symbolInfo);
        }

        public static SymbolModel Map(ISymbol symbol)
        {
            return symbol == null ? null : new SymbolModel(symbol);
        }
    }
}