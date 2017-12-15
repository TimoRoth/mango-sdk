using System.Collections.Immutable;

namespace Mango.Compiler.Symbols
{
    public abstract class StructuredTypeSymbol : TypeSymbol
    {
        private protected StructuredTypeSymbol() { }

        public abstract ImmutableArray<FieldSymbol> Fields { get; }

        public sealed override SymbolKind Kind => SymbolKind.StructuredType;

        public abstract override string Name { get; }

        public sealed override TypeKind TypeKind => TypeKind.Structured;

        internal abstract FieldSymbol FindField(string name);
    }
}
