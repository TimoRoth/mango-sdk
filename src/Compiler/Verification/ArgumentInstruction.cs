using System;
using Mango.Compiler.Symbols;

namespace Mango.Compiler.Verification
{
    public sealed class ArgumentInstruction : Instruction
    {
        private readonly ParameterSymbol _parameter;

        internal ArgumentInstruction(InstructionKind kind, ExecutionState state, ParameterSymbol parameter) : base(kind, state)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            _parameter = parameter;
        }

        public ParameterSymbol Parameter => _parameter;
    }
}
