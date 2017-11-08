using System.Collections.Immutable;

namespace Mango.Compiler.Symbols
{
    public sealed class FunctionTypeSymbol : TypeSymbol
    {
        private readonly ImmutableArray<TypeSymbol> _parameterTypes;
        private readonly TypeSymbol _returnType;

        internal FunctionTypeSymbol(TypeSymbol returnType, ImmutableArray<TypeSymbol> parameterTypes)
        {
            if (!ValidReturnType(returnType))
                throw new System.ArgumentException();
            foreach (var parameterType in parameterTypes)
                if (!ValidLocationType(parameterType))
                    throw new System.ArgumentException();

            _returnType = returnType;
            _parameterTypes = parameterTypes;
        }

        public sealed override SymbolKind Kind => SymbolKind.FunctionType;

        public ImmutableArray<TypeSymbol> ParameterTypes => _parameterTypes;

        public bool ReturnsVoid => _returnType.SpecialType == SpecialType.Void;

        public TypeSymbol ReturnType => _returnType;

        public override bool Equals(TypeSymbol other) => (object)this == other || other is FunctionTypeSymbol functionType && _returnType == functionType._returnType && _parameterTypes.Length == functionType._parameterTypes.Length && System.Linq.Enumerable.SequenceEqual(_parameterTypes, functionType._parameterTypes);

        public override int GetHashCode() => Utilities.Hash.CombineValues(_parameterTypes, Utilities.Hash.Combine(_returnType, (int)SymbolKind.FunctionType));
    }
}
