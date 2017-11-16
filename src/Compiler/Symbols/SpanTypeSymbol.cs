namespace Mango.Compiler.Symbols
{
    public sealed class SpanTypeSymbol : TypeSymbol
    {
        private readonly TypeSymbol _elementType;

        internal SpanTypeSymbol(TypeSymbol elementType)
        {
            if (!ValidLocationType(elementType))
                throw new System.ArgumentException();

            _elementType = elementType;
        }

        public TypeSymbol ElementType => _elementType;

        public sealed override SymbolKind Kind => SymbolKind.SpanType;

        public override bool Equals(TypeSymbol other) => (object)this == other || other is SpanTypeSymbol spanType && _elementType == spanType.ElementType;

        public override int GetHashCode() => Utilities.Hash.Combine(_elementType, (int)SymbolKind.SpanType);
    }
}
