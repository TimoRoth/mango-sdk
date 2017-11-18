namespace Mango.Compiler.Symbols
{
    public abstract class FieldSymbol : Symbol
    {
        private protected FieldSymbol() { }

        public abstract StructuredTypeSymbol ContainingType { get; }

        public abstract int FieldOffset { get; }

        public sealed override SymbolKind Kind => SymbolKind.Field;

        public abstract override string Name { get; }

        public abstract TypeSymbol Type { get; }
    }
}
