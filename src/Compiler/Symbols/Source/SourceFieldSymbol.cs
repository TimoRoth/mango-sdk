using Mango.Compiler.Syntax;

namespace Mango.Compiler.Symbols.Source
{
    internal sealed class SourceFieldSymbol : FieldSymbol
    {
        private readonly SourceStructuredTypeSymbol _containingType;
        private readonly int _offset;
        private readonly FieldDeclarationSyntax _syntax;
        private readonly TypeSymbol _type;

        internal SourceFieldSymbol(SourceStructuredTypeSymbol containingType, FieldDeclarationSyntax syntax, TypeSymbol type, int offset)
        {
            _containingType = containingType;
            _syntax = syntax;
            _type = type;
            _offset = offset;
        }

        public override Symbol ContainingSymbol => _containingType;

        public override StructuredTypeSymbol ContainingType => _containingType;

        public override int FieldOffset => _offset;

        public override string Name => _syntax.FieldName;

        public override TypeSymbol Type => _type;
    }
}
