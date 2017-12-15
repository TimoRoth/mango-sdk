using System.Collections.Immutable;

namespace Mango.Compiler.Symbols
{
    public abstract class ModuleSymbol : Symbol
    {
        private protected ModuleSymbol() { }

        public abstract ImmutableArray<FunctionSymbol> Functions { get; }

        public sealed override SymbolKind Kind => SymbolKind.Module;

        public abstract override string Name { get; }

        public abstract ImmutableArray<StructuredTypeSymbol> Types { get; }

        internal abstract FunctionSymbol FindFunction(string name);

        internal abstract StructuredTypeSymbol FindType(string name);
    }
}
