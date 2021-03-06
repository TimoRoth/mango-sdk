using System.Collections.Immutable;

namespace Mango.Compiler.Symbols
{
    public abstract class FunctionSymbol : Symbol
    {
        private protected FunctionSymbol() { }

        public abstract bool IsExtern { get; }

        public sealed override SymbolKind Kind => SymbolKind.Function;

        public abstract ImmutableArray<LabelSymbol> Labels { get; }

        public abstract ImmutableArray<LocalSymbol> Locals { get; }

        public abstract override string Name { get; }

        public abstract ImmutableArray<ParameterSymbol> Parameters { get; }

        public abstract bool ReturnsVoid { get; }

        public abstract TypeSymbol ReturnType { get; }

        public abstract FunctionTypeSymbol Type { get; }

        public abstract int GetSystemCallOrdinal();

        internal abstract LabelSymbol FindLabel(string name);

        internal abstract LocalSymbol FindLocal(string name);

        internal abstract ParameterSymbol FindParameter(string name);
    }
}
