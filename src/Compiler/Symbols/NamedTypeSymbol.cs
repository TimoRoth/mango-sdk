namespace Mango.Compiler.Symbols
{
    public abstract class NamedTypeSymbol : TypeSymbol
    {
        private protected NamedTypeSymbol() { }

        public sealed override SymbolKind Kind => SymbolKind.NamedType;

        public abstract override string Name { get; }
    }
}
