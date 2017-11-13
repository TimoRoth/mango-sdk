namespace Mango.Compiler.Syntax
{
    public abstract class SyntaxNode
    {
        private readonly SyntaxKind _kind;

        private protected SyntaxNode(SyntaxKind kind)
        {
            _kind = kind;
        }

        public SyntaxKind Kind => _kind;
    }
}
