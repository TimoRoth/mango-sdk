using System;
using System.Collections.Immutable;
using Mango.Compiler.Symbols;

namespace Mango.Compiler.Verification
{
    public sealed class VerifiedModule
    {
        private readonly ImmutableArray<VerifiedFunction> _functions;
        private readonly ModuleSymbol _symbol;

        internal VerifiedModule(ModuleSymbol symbol, ImmutableArray<VerifiedFunction> functions)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            _symbol = symbol;
            _functions = functions;
        }

        public ImmutableArray<VerifiedFunction> Functions => _functions;

        public ModuleSymbol Symbol => _symbol;
    }
}
