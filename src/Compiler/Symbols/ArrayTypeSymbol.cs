using System;

namespace Mango.Compiler.Symbols
{
    public sealed class ArrayTypeSymbol : TypeSymbol
    {
        private readonly TypeSymbol _elementType;
        private readonly int _length;
        private readonly TypeLayout _typeLayout;

        internal ArrayTypeSymbol(TypeSymbol elementType, int length)
        {
            if (!ValidLocationType(elementType))
                throw new ArgumentException();
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            _elementType = elementType;
            _length = length;
            _typeLayout = new TypeLayout(checked(length * elementType.TypeLayout.Size), elementType.TypeLayout.Alignment);
        }

        public override Symbol ContainingSymbol => null;

        public TypeSymbol ElementType => _elementType;

        public override SymbolKind Kind => SymbolKind.ArrayType;

        public int Length => _length;

        public override TypeKind TypeKind => TypeKind.Array;

        public override TypeLayout TypeLayout => _typeLayout;

        public override bool Equals(TypeSymbol other) => (object)this == other || other is ArrayTypeSymbol arrayType && _length == arrayType._length && _elementType == arrayType._elementType;

        public override int GetHashCode() => Utilities.Hash.Combine(_elementType, Utilities.Hash.Combine(_length, (int)SymbolKind.ArrayType));
    }
}
