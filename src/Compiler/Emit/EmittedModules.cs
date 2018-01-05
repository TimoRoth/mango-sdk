using System;
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

        public EmittedModule GetModuleByFingerprint(ReadOnlySpan<byte> fingerprint)
        {
            foreach (var item in _modules)
            {
                if (item.Name.SequenceEqual(fingerprint))
                {
                    return item;
                }
            }

            return null;
        }

        public EmittedModule GetModuleByName(string name)
        {
            foreach (var item in _modules)
            {
                if (item.Symbol.Name == name)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
