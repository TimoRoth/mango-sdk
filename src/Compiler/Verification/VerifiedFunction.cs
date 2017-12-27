using System;
using System.Collections.Immutable;
using Mango.Compiler.Symbols;

namespace Mango.Compiler.Verification
{
    public sealed class VerifiedFunction
    {
        private readonly ImmutableArray<Instruction> _instructions;
        private readonly ImmutableDictionary<LabelSymbol, int> _labels;
        private readonly FunctionSymbol _symbol;

        internal VerifiedFunction(FunctionSymbol symbol, ImmutableArray<Instruction> instructions, ImmutableDictionary<LabelSymbol, int> labels)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            _symbol = symbol;
            _instructions = instructions;
            _labels = labels;
        }

        public ImmutableArray<Instruction> Instructions => _instructions;

        public ImmutableDictionary<LabelSymbol, int> Labels => _labels;

        public FunctionSymbol Symbol => _symbol;
    }
}
