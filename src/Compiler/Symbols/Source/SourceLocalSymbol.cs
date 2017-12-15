using Mango.Compiler.Syntax;

namespace Mango.Compiler.Symbols.Source
{
    internal sealed class SourceLocalSymbol : LocalSymbol
    {
        private readonly SourceFunctionSymbol _containingFunction;
        private readonly LocalDeclarationSyntax _syntax;
        private readonly TypeSymbol _type;

        internal SourceLocalSymbol(SourceFunctionSymbol containingFunction, LocalDeclarationSyntax syntax, TypeSymbol type)
        {
            _containingFunction = containingFunction;
            _syntax = syntax;
            _type = type;
        }

        public override Symbol ContainingSymbol => _containingFunction;

        public override string Name => _syntax.LocalName;

        public override TypeSymbol Type => _type;
    }
}
