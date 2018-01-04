using System;
using Mango.Compiler.Symbols;

namespace Mango.Compiler.Verification
{
    public sealed class FunctionInstruction : Instruction
    {
        private readonly FunctionSymbol _function;

        internal FunctionInstruction(InstructionKind kind, ExecutionState state, FunctionSymbol function) : base(kind, state)
        {
            if (function == null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            _function = function;
        }

        public FunctionSymbol Function => _function;
    }
}
