namespace Mango.Compiler.Symbols
{
    public abstract class LocalSymbol : Symbol
    {
        private protected LocalSymbol() { }

        public sealed override SymbolKind Kind => SymbolKind.Local;

        public abstract override string Name { get; }

        public abstract TypeSymbol Type { get; }
    }
}
