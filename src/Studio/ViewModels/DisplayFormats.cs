using Microsoft.CodeAnalysis;

namespace Studio.ViewModels
{
    public static class DisplayFormats
    {
        public static readonly SymbolDisplayFormat ContentFormat;
        public static readonly SymbolDisplayFormat DescriptionFormat;

        static DisplayFormats()
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
    }
}