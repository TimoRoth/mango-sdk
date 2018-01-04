using System;
using Mango.Compiler.Symbols;

namespace Mango.Compiler.Verification
{
    public sealed class LocalInstruction : Instruction
    {
        private readonly LocalSymbol _local;

        internal LocalInstruction(InstructionKind kind, ExecutionState state, LocalSymbol local) : base(kind, state)
        {
            if (local == null)
            {
                throw new ArgumentNullException(nameof(local));
            }

            _local = local;
        }

        public LocalSymbol Local => _local;
    }
}
