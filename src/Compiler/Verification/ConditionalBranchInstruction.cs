using System;
using Mango.Compiler.Symbols;

namespace Mango.Compiler.Verification
{
    public sealed class ConditionalBranchInstruction : Instruction
    {
        private readonly LabelSymbol _label;
        private readonly TypeSymbol _type;

        internal ConditionalBranchInstruction(InstructionKind kind, ExecutionState state, TypeSymbol type, LabelSymbol label) : base(kind, state)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (label == null)
            {
                throw new ArgumentNullException(nameof(label));
            }

            _type = type;
            _label = label;
        }

        public LabelSymbol Label => _label;

        public TypeSymbol Type => _type;
    }
}
