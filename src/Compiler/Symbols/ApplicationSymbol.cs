using System.Collections.Immutable;

namespace Mango.Compiler.Symbols
{
    public abstract class ApplicationSymbol : Symbol
    {
        private protected ApplicationSymbol() { }

        public sealed override SymbolKind Kind => SymbolKind.Application;

        public abstract ImmutableArray<ModuleSymbol> Modules { get; }

        internal abstract ModuleSymbol FindModule(string name);
    }
}
