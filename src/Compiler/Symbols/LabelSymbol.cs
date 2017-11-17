namespace Mango.Compiler.Symbols
{
    public abstract class LabelSymbol : Symbol
    {
        private protected LabelSymbol() { }

        public sealed override SymbolKind Kind => SymbolKind.Label;

        public abstract override string Name { get; }
    }
}
