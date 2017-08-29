using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Convenient.Stuff.Models.Emit
{
    public class EmitResultModel
    {
        public bool Success { get; set; }
        public List<DiagnosticModel> Diagnostics { get; set; } = new List<DiagnosticModel>();
    }

    public class DiagnosticModel
    {
        
    }

    public class EmitMapper
    {
        public static EmitResultModel Map(EmitResult result)
        {
            return new EmitResultModel
            {
                Success = result.Success,
                Diagnostics = result.Diagnostics.Select(Map).ToList()
            };
        }

        public static DiagnosticModel Map(Diagnostic diagnostic)
        {
            
            return new DiagnosticModel
            {

            };
        }
    }
}