namespace Mango.Compiler.Symbols
{
    public abstract class TypeSymbol : Symbol, System.IEquatable<TypeSymbol>
    {
        private protected TypeSymbol() { }

        public virtual SpecialType SpecialType => SpecialType.None;

        public abstract TypeLayout TypeLayout { get; }

        public virtual bool Equals(TypeSymbol other) => (object)this == other;

        public sealed override bool Equals(object obj) => (obj is TypeSymbol other) && Equals(other);

        public override int GetHashCode() => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this);

        public override string ToString() =>
            this is SpecialTypeSymbol special ? special.Name :
            this is ReferenceTypeSymbol reference ? reference.ReferencedType + "&" :
            this is ArrayTypeSymbol array ? array.ElementType + "[" + array.Length + "]" :
            this is SpanTypeSymbol span ? span.ElementType + "[]" :
            this is FunctionTypeSymbol func ? func.ReturnType + "(" + string.Join(", ", func.ParameterTypes) + ")" :
            this is StructuredTypeSymbol named ? named.Name :
            Kind.ToString();

        internal protected static bool ValidLocationType(TypeSymbol type) => type != null && type.SpecialType != SpecialType.Void;

        internal protected static bool ValidReturnType(TypeSymbol type) => type != null;
    }
}
