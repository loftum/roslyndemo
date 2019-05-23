using Microsoft.CodeAnalysis.CSharp;
using RoslynDemo.Core.Models.Syntax;

namespace RoslynDemo.Core.Models
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