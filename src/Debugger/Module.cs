using System.Collections.Immutable;
using Mango.Compiler.Symbols;

namespace Mango.Debugger
{
    public sealed class Module
    {
        private readonly int _index;
        private readonly Snapshot _snapshot;
        private readonly ModuleSymbol _symbol;

        private Module(Snapshot snapshot, int index, ModuleSymbol symbol = null)
        {
            _snapshot = snapshot;
            _index = index;
            _symbol = symbol;
        }

        public ModuleSymbol ModuleSymbol => _symbol;

        internal Snapshot Snapshot => _snapshot;

        internal static ImmutableArray<Module> CreateModulesFrom(Snapshot snapshot, ImmutableArray<ModuleSymbol> symbols = default)
        {
            var count = Utilities.GetVM(snapshot.MemoryDump).modules_imported;
            var builder = ImmutableArray.CreateBuilder<Module>(count);

            for (var i = 0; i < count; i++)
            {
                builder.Add(new Module(snapshot, i, symbols.IsDefault ? null : symbols[i]));
            }

            return builder.MoveToImmutable();
        }
    }
}
