namespace Mango.Compiler.Symbols
{
    public abstract class FieldSymbol : Symbol
    {
        private protected FieldSymbol() { }

        public abstract int FieldOffset { get; }

        public abstract TypeSymbol FieldType { get; }

        public sealed override SymbolKind Kind => SymbolKind.Field;

        public abstract override string Name { get; }
    }
}
