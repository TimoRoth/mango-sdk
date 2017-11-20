using System;
using System.Collections.Immutable;
using System.Linq;

namespace Mango.Compiler.Symbols
{
    public sealed class FunctionTypeSymbol : TypeSymbol
    {
        private readonly ImmutableArray<TypeSymbol> _parameterTypes;
        private readonly TypeSymbol _returnType;
        private readonly TypeLayout _typeLayout;

        internal FunctionTypeSymbol(TypeSymbol returnType, ImmutableArray<TypeSymbol> parameterTypes)
        {
            if (!ValidReturnType(returnType))
                throw new ArgumentException();
            foreach (var parameterType in parameterTypes)
                if (!ValidLocationType(parameterType))
                    throw new ArgumentException();

            _returnType = returnType;
            _parameterTypes = parameterTypes;
            _typeLayout = new TypeLayout(4, 4);
        }

        public override Symbol ContainingSymbol => null;

        public override SymbolKind Kind => SymbolKind.FunctionType;

        public ImmutableArray<TypeSymbol> ParameterTypes => _parameterTypes;

        public bool ReturnsVoid => _returnType.SpecialType == SpecialType.Void;

        public TypeSymbol ReturnType => _returnType;

        public override TypeKind TypeKind => TypeKind.Function;

        public override TypeLayout TypeLayout => _typeLayout;

        public override bool Equals(TypeSymbol other) => (object)this == other || other is FunctionTypeSymbol functionType && _returnType == functionType._returnType && _parameterTypes.Length == functionType._parameterTypes.Length && _parameterTypes.SequenceEqual(functionType._parameterTypes);

        public override int GetHashCode() => Utilities.Hash.CombineValues(_parameterTypes, Utilities.Hash.Combine(_returnType, (int)SymbolKind.FunctionType));
    }
}
