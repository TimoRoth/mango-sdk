using System;

namespace Mango.Compiler.Symbols
{
    public sealed class SpanTypeSymbol : TypeSymbol
    {
        private readonly TypeSymbol _elementType;
        private readonly TypeLayout _typeLayout;

        internal SpanTypeSymbol(TypeSymbol elementType)
        {
            if (!ValidLocationType(elementType))
                throw new ArgumentException();

            _elementType = elementType;
            _typeLayout = new TypeLayout(8, 4);
        }

        public override Symbol ContainingSymbol => null;

        public TypeSymbol ElementType => _elementType;

        public override SymbolKind Kind => SymbolKind.SpanType;

        public override TypeKind TypeKind => TypeKind.Span;

        public override TypeLayout TypeLayout => _typeLayout;

        public override bool Equals(TypeSymbol other) => (object)this == other || other is SpanTypeSymbol spanType && _elementType == spanType.ElementType;

        public override int GetHashCode() => Utilities.Hash.Combine(_elementType, (int)SymbolKind.SpanType);
    }
}
