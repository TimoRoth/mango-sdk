using System.Collections.Immutable;
using Mango.Compiler.Symbols;

namespace Mango.Compiler.Verification
{
    public abstract class Instruction
    {
        private readonly InstructionKind _kind;
        private readonly ExecutionState _state;

        private protected Instruction(InstructionKind kind, ExecutionState state)
        {
            _kind = kind;
            _state = state;
        }

        public InstructionKind Kind => _kind;

        public ImmutableStack<TypeSymbol> Stack => _state.Stack;
    }
}
