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

        public SyntaxNode Parent { get; internal set; }

        public TNode FirstAncestorOrSelf<TNode>() where TNode : SyntaxNode
        {
            for (var node = this; node != null; node = node.Parent)
            {
                if (node is TNode tnode)
                {
                    return tnode;
                }
            }

            return null;
        }
    }
}
