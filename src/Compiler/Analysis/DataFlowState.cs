using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Mango.Compiler.Diagnostics;
using Mango.Compiler.Utilities;

namespace Mango.Compiler.Analysis
{
    public struct DataFlowState<T> : IEquatable<DataFlowState<T>>
    {
        private readonly ImmutableList<Diagnostic> _diagnostics;
        private readonly ImmutableStack<T> _stack;

        internal DataFlowState(ImmutableStack<T> stack)
        {
            _stack = stack;
            _diagnostics = null;
        }

        private DataFlowState(ImmutableStack<T> stack, ImmutableList<Diagnostic> diagnostics)
        {
            _stack = stack;
            _diagnostics = diagnostics;
        }

        public ImmutableStack<T> Stack => _stack;

        public static bool operator !=(DataFlowState<T> left, DataFlowState<T> right) => !left.Equals(right);

        public static bool operator ==(DataFlowState<T> left, DataFlowState<T> right) => left.Equals(right);

        public bool Equals(DataFlowState<T> other) => _stack == other._stack && _diagnostics == other._diagnostics;

        public override bool Equals(object obj) => obj is DataFlowState<T> other && Equals(other);

        public IEnumerable<Diagnostic> GetDiagnostics() => _diagnostics ?? ImmutableList<Diagnostic>.Empty;

        public override int GetHashCode() => Hash.Combine(Stack, Hash.Combine(_diagnostics, 0));

        internal DataFlowState<T> AddError(Diagnostic diagnostic) => new DataFlowState<T>(_stack, (_diagnostics ?? ImmutableList<Diagnostic>.Empty).Add(diagnostic));
    }
}
