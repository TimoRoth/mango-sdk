using System;

namespace Mango.Compiler.Symbols
{
    public sealed class ReferenceTypeSymbol : TypeSymbol
    {
        private readonly TypeSymbol _referencedType;
        private readonly TypeLayout _typeLayout;

        internal ReferenceTypeSymbol(TypeSymbol referencedType)
        {
            if (!ValidLocationType(referencedType))
                throw new ArgumentException();

            _referencedType = referencedType;
            _typeLayout = new TypeLayout(4, 4);
        }

        public override Symbol ContainingSymbol => null;

        public override SymbolKind Kind => SymbolKind.ReferenceType;

        public TypeSymbol ReferencedType => _referencedType;

        public override TypeKind TypeKind => TypeKind.Reference;

        public override TypeLayout TypeLayout => _typeLayout;

        public override bool Equals(TypeSymbol other) => (object)this == other || other is ReferenceTypeSymbol referenceType && _referencedType == referenceType._referencedType;

        public override int GetHashCode() => Utilities.Hash.Combine(_referencedType, (int)SymbolKind.ReferenceType);
    }
}
