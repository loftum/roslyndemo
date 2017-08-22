using Microsoft.CodeAnalysis;

namespace Convenient.Stuff.Models.Semantics
{
    public class TypeInfoModel
    {
        public TypeSymbolModel TypeSymbol { get; }
        public TypeSymbolModel ConvertedType { get; }

        public TypeInfoModel(TypeInfo typeInfo)
        {
            TypeSymbol = new TypeSymbolModel(typeInfo.Type);
            ConvertedType = new TypeSymbolModel(typeInfo.ConvertedType);
        }
    }
}