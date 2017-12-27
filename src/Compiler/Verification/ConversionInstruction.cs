using System;
using Mango.Compiler.Symbols;

namespace Mango.Compiler.Verification
{
    public sealed class ConversionInstruction : Instruction
    {
        private readonly TypeSymbol _fromType;
        private readonly TypeSymbol _toType;

        internal ConversionInstruction(InstructionKind kind, ExecutionState state, TypeSymbol toType, TypeSymbol fromType) : base(kind, state)
        {
            if (toType == null)
            {
                throw new ArgumentNullException(nameof(toType));
            }
            if (fromType == null)
            {
                throw new ArgumentNullException(nameof(fromType));
            }

            _toType = toType;
            _fromType = fromType;
        }

        public TypeSymbol FromType => _fromType;

        public TypeSymbol ToType => _toType;
    }
}
