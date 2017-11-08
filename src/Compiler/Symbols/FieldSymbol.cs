namespace Mango.Compiler.Symbols.Source
{
    public abstract class FieldSymbol : Symbol
    {
        private protected FieldSymbol() { }

        public sealed override SymbolKind Kind => SymbolKind.Field;

        public abstract override string Name { get; }

        public abstract TypeSymbol Type { get; }
    }
}
