using System.Linq;
using Microsoft.CodeAnalysis;

namespace RoslynDemo.Core.Models.Semantics
{
    public class SymbolInfoModel
    {
        public SymbolModel Symbol { get; }
        public CandidateReason CandidateReason { get; }
        public SymbolModel[] CandidateSymbols { get; }

        public SymbolInfoModel(SymbolInfo symbolInfo)
        {
            Symbol = symbolInfo.Symbol == null ? null : new SymbolModel(symbolInfo.Symbol);
            CandidateReason = symbolInfo.CandidateReason;
            CandidateSymbols = symbolInfo.CandidateSymbols.Select(s => new SymbolModel(s)).ToArray();
        }
    }
}