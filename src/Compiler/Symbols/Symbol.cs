using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Mango.Compiler.Symbols.Source;

namespace Mango.Compiler.Symbols
{
    [DebuggerDisplay("{Kind, nq}, Name = {Name}")]
    public abstract class Symbol : IEquatable<Symbol>
    {
        private protected Symbol() { }

        public virtual ApplicationSymbol ContainingApplication => ContainingSymbol?.ContainingApplication;

        public virtual ModuleSymbol ContainingModule => ContainingSymbol?.ContainingModule;

        public abstract Symbol ContainingSymbol { get; }

        internal virtual Compilation DeclaringCompilation => (ContainingModule as SourceModuleSymbol)?.DeclaringCompilation;

        public abstract SymbolKind Kind { get; }

        public virtual string Name => string.Empty;

        public static bool operator !=(Symbol left, Symbol right) => (object)right == null ? (object)left != null : (object)left != right && !right.Equals(left);

        public static bool operator ==(Symbol left, Symbol right) => (object)right == null ? (object)left == null : (object)left == right || right.Equals(left);

        public bool Equals(Symbol other) => Equals((object)other);

        public override bool Equals(object obj) => (object)this == obj;

        public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);
    }
}
