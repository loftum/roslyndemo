using Convenient.Stuff.Models.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace Convenient.Stuff.Models
{
    public class CSharpCompilationModel
    {
        public string Language { get; set; }
        public LanguageVersion LanguageVersion { get; set; }
        public DiagnosticModel[] Diagnoscics { get; set; } = new DiagnosticModel[0];
        public SyntaxTreeModel[] SyntaxTrees { get; set; } = new SyntaxTreeModel[0];
        public MetadataReferencesModel[] References { get; set; } = new MetadataReferencesModel[0];
    }
}