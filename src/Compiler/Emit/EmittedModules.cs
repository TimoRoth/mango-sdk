using System.Collections.Immutable;

namespace Mango.Compiler.Emit
{
    public sealed class EmittedModules
    {
        private readonly ImmutableArray<EmittedModule> _modules;

        internal EmittedModules(ImmutableArray<EmittedModule> modules)
        {
            _modules = modules;
        }

        public ImmutableArray<EmittedModule> Modules => _modules;
    }
}
