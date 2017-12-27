using System;
using Mango.Compiler.Symbols;

namespace Mango.Compiler.Verification
{
    public sealed class ConstantInstruction : Instruction
    {
        private readonly TypeSymbol _type;
        private readonly object _value;

        internal ConstantInstruction(InstructionKind kind, ExecutionState state, TypeSymbol type, object value) : base(kind, state)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            _type = type;
            _value = value;
        }

        public TypeSymbol Type => _type;

        public object Value => _value;
    }
}
