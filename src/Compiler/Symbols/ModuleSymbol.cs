using System.Collections.Immutable;

namespace Mango.Compiler.Symbols
{
    public abstract class ModuleSymbol : Symbol
    {
        private protected ModuleSymbol() { }

        public abstract FunctionSymbol EntryPoint { get; }

        public abstract ImmutableArray<FunctionSymbol> Functions { get; }

        public abstract ImmutableArray<ModuleSymbol> Imports { get; }

        public sealed override SymbolKind Kind => SymbolKind.Module;

        public abstract override string Name { get; }

        public abstract ImmutableArray<StructuredTypeSymbol> Types { get; }

        internal ImmutableHashSet<ModuleSymbol> CreateImportsClosure()
        {
            var closure = ImmutableHashSet.CreateBuilder<ModuleSymbol>();

            var queue = ImmutableQueue<ModuleSymbol>.Empty;

            if (closure.Add(this))
            {
                queue = queue.Enqueue(this);
            }

            while (!queue.IsEmpty)
            {
                queue = queue.Dequeue(out var module);

                foreach (var import in module.Imports)
                {
                    if (closure.Add(import))
                    {
                        queue = queue.Enqueue(import);
                    }
                }
            }

            return closure.ToImmutable();
        }

        internal abstract FunctionSymbol FindFunction(string name, TypeSymbol returnType, params TypeSymbol[] parameterTypes);

        internal abstract StructuredTypeSymbol FindType(string name);
    }
}
