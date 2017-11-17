using System.Collections.Immutable;

namespace Mango.Compiler.Symbols
{
    public abstract class NamedTypeSymbol : TypeSymbol
    {
        private protected NamedTypeSymbol() { }

        public abstract ImmutableArray<FieldSymbol> Fields { get; }

        public sealed override SymbolKind Kind => SymbolKind.NamedType;

        public abstract override string Name { get; }
    }
}
