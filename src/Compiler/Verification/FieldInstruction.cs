using System;
using Mango.Compiler.Symbols;

namespace Mango.Compiler.Verification
{
    public sealed class FieldInstruction : Instruction
    {
        private readonly FieldSymbol _field;

        internal FieldInstruction(InstructionKind kind, ExecutionState state, FieldSymbol field) : base(kind, state)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            _field = field;
        }

        public FieldSymbol Field => _field;
    }
}
