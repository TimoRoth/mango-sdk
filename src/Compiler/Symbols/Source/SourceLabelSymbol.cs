using Mango.Compiler.Syntax;

namespace Mango.Compiler.Symbols.Source
{
    internal sealed class SourceLabelSymbol : LabelSymbol
    {
        private readonly SourceFunctionSymbol _containingFunction;
        private readonly LabeledInstructionSyntax _syntax;

        public SourceLabelSymbol(SourceFunctionSymbol containingFunction, LabeledInstructionSyntax syntax)
        {
            _containingFunction = containingFunction;
            _syntax = syntax;
        }

        public override Symbol ContainingSymbol => _containingFunction;

        public override string Name => _syntax.LabelName;
    }
}
