using Mango.Compiler.Syntax;

namespace Mango.Compiler.Symbols.Source
{
    internal sealed class SourceParameterSymbol : ParameterSymbol
    {
        private readonly SourceFunctionSymbol _containingFunction;
        private readonly ParameterDeclarationSyntax _syntax;
        private readonly TypeSymbol _type;

        internal SourceParameterSymbol(SourceFunctionSymbol containingFunction, ParameterDeclarationSyntax syntax, TypeSymbol type)
        {
            _containingFunction = containingFunction;
            _syntax = syntax;
            _type = type;
        }

        public override Symbol ContainingSymbol => _containingFunction;

        public override string Name => _syntax.ParameterName;

        public override TypeSymbol Type => _type;
    }
}
