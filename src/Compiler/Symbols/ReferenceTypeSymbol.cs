namespace Mango.Compiler.Symbols
{
    public sealed class ReferenceTypeSymbol : TypeSymbol
    {
        private readonly TypeSymbol _referencedType;

        internal ReferenceTypeSymbol(TypeSymbol referencedType)
        {
            if (!ValidLocationType(referencedType))
                throw new System.ArgumentException();

            _referencedType = referencedType;
        }

        public sealed override SymbolKind Kind => SymbolKind.ReferenceType;

        public TypeSymbol ReferencedType => _referencedType;

        public override bool Equals(TypeSymbol other) => (object)this == other || other is ReferenceTypeSymbol referenceType && _referencedType == referenceType._referencedType;

        public override int GetHashCode() => Utilities.Hash.Combine(_referencedType, (int)SymbolKind.ReferenceType);
    }
}