using System.Collections.Immutable;

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
    }
}
