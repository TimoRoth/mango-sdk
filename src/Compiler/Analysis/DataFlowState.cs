using System;
using System.Collections.Immutable;

namespace Mango.Compiler.Analysis
{
    public struct DataFlowState<T> : IEquatable<DataFlowState<T>>
    {
        private readonly ImmutableStack<T> _stack;

        internal DataFlowState(ImmutableStack<T> stack)
        {
            _stack = stack;
        }

        public ImmutableStack<T> Stack => _stack;

        public static bool operator !=(DataFlowState<T> left, DataFlowState<T> right) => !left.Equals(right);

        public static bool operator ==(DataFlowState<T> left, DataFlowState<T> right) => left.Equals(right);

        public bool Equals(DataFlowState<T> other) => _stack == other._stack;

        public override bool Equals(object obj) => obj is DataFlowState<T> other && Equals(other);

        public override int GetHashCode() => _stack.GetHashCode();
    }
}
