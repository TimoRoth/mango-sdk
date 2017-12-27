using System.Collections.Immutable;
using Mango.Compiler.Symbols;

namespace Mango.Compiler.Verification
{
    internal struct ExecutionState
    {
        private readonly ImmutableList<TypeSymbol> _locals;
        private readonly ImmutableList<TypeSymbol> _parameters;
        private readonly ImmutableStack<TypeSymbol> _stack;

        public ExecutionState(ImmutableList<TypeSymbol> parameters, ImmutableList<TypeSymbol> locals, ImmutableStack<TypeSymbol> stack)
        {
            _parameters = parameters;
            _locals = locals;
            _stack = stack;
        }

        public ImmutableList<TypeSymbol> Locals => _locals;

        public ImmutableList<TypeSymbol> Parameters => _parameters;

        public ImmutableStack<TypeSymbol> Stack => _stack;
    }
}
