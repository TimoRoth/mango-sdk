namespace Mango.Compiler.Symbols
{
    public abstract class ParameterSymbol : Symbol
    {
        private protected ParameterSymbol() { }

        public sealed override SymbolKind Kind => SymbolKind.Parameter;

        public abstract override string Name { get; }

        public abstract TypeSymbol Type { get; }
    }
}
