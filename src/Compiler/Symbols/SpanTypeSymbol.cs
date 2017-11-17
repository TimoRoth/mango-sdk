namespace Mango.Compiler.Symbols
{
    public sealed class SpanTypeSymbol : TypeSymbol
    {
        private readonly TypeSymbol _elementType;
        private readonly TypeLayout _typeLayout;

        internal SpanTypeSymbol(TypeSymbol elementType)
        {
            if (!ValidLocationType(elementType))
                throw new System.ArgumentException();

            _elementType = elementType;
            _typeLayout = new TypeLayout(8, 4);
        }

        public TypeSymbol ElementType => _elementType;

        public sealed override SymbolKind Kind => SymbolKind.SpanType;

        public override TypeLayout TypeLayout => _typeLayout;

        public override Symbol ContainingSymbol => null;

        public override bool Equals(TypeSymbol other) => (object)this == other || other is SpanTypeSymbol spanType && _elementType == spanType.ElementType;

        public override int GetHashCode() => Utilities.Hash.Combine(_elementType, (int)SymbolKind.SpanType);
    }
}
