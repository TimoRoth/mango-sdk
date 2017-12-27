using System;
using Mango.Compiler.Symbols;

namespace Mango.Compiler.Verification
{
    public sealed class TypedInstruction : Instruction
    {
        private readonly TypeSymbol _type;

        internal TypedInstruction(InstructionKind kind, ExecutionState state, TypeSymbol type) : base(kind, state)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _type = type;
        }

        public TypeSymbol Type => _type;
    }
}
