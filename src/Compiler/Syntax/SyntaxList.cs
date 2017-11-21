using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mango.Compiler.Syntax
{
    public struct SyntaxList<TNode> : IReadOnlyList<TNode> where TNode : SyntaxNode
    {
        private readonly TNode[] _items;

        internal SyntaxList(params TNode[] items)
        {
            _items = items;
        }

        public int Count => _items?.Length ?? 0;

        public TNode this[int index] => _items?[index];

        public IEnumerator<TNode> GetEnumerator() => (_items ?? Enumerable.Empty<TNode>()).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
