using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Mango.Compiler.Symbols
{
    [DebuggerDisplay("{Kind, nq}, Name = {Name}")]
    public abstract class Symbol : IEquatable<Symbol>
    {
        private protected Symbol() { }

        public virtual ModuleSymbol ContainingModule => ContainingSymbol?.ContainingModule;

        public abstract Symbol ContainingSymbol { get; }

        public abstract SymbolKind Kind { get; }

        public virtual string Name => string.Empty;

        public static bool operator !=(Symbol left, Symbol right) => (object)right == null ? (object)left != null : (object)left != right && !right.Equals(left);

        public static bool operator ==(Symbol left, Symbol right) => (object)right == null ? (object)left == null : (object)left == right || right.Equals(left);

        public bool Equals(Symbol other) => Equals((object)other);

        public override bool Equals(object obj) => (object)this == obj;

        public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);
    }
}
