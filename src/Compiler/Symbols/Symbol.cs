using System.Diagnostics;

namespace Mango.Compiler.Symbols
{
    [DebuggerDisplay("{Kind, nq}, Name = {Name}")]
    public abstract class Symbol : System.IEquatable<Symbol>
    {
        private protected Symbol() { }

        public abstract SymbolKind Kind { get; }

        public virtual string Name => string.Empty;

        public static bool operator !=(Symbol left, Symbol right) => (object)right == null ? (object)left != null : (object)left != right && !right.Equals(left);

        public static bool operator ==(Symbol left, Symbol right) => (object)right == null ? (object)left == null : (object)left == right || right.Equals(left);

        public bool Equals(Symbol other) => Equals((object)other);

        public override bool Equals(object obj) => (object)this == obj;

        public override int GetHashCode() => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this);
    }
}
