using System;
using Mango.Compiler.Symbols;

namespace Mango.Compiler.Verification
{
    public sealed class UnconditionalBranchInstruction : Instruction
    {
        private readonly LabelSymbol _label;

        internal UnconditionalBranchInstruction(InstructionKind kind, ExecutionState state, LabelSymbol label) : base(kind, state)
        {
            if (label == null)
            {
                throw new ArgumentNullException(nameof(label));
            }

            _label = label;
        }

        public LabelSymbol Label => _label;
    }
}
