using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Mango.Compiler.Diagnostics;

namespace Mango.Compiler.Analysis
{
    public sealed class DataFlowAnalysis<T>
    {
        private readonly ImmutableArray<DataFlowState<T>> _state;

        internal DataFlowAnalysis(ImmutableArray<DataFlowState<T>> state)
        {
            _state = state;
        }

        public ImmutableArray<DataFlowState<T>> State => _state;

        public IEnumerable<Diagnostic> GetDiagnostics() => _state.SelectMany(s => s.GetDiagnostics());
    }
}
