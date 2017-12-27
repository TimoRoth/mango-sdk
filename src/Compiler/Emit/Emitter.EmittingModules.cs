using System;
using System.Collections.Immutable;
using Mango.Compiler.Symbols;

namespace Mango.Compiler.Emit
{
    internal static partial class Emitter
    {
        private sealed class EmittingModules
        {
            private readonly ImmutableDictionary<ModuleSymbol, EmittingModule> _modules;

            private EmittedModules _result;

            public EmittingModules(ImmutableDictionary<ModuleSymbol, EmittingModule> modules)
            {
                if (modules == null)
                {
                    throw new ArgumentNullException(nameof(modules));
                }

                _modules = modules;
            }

            public EmittedModules Link()
            {
                if (_result != null)
                {
                    return _result;
                }

                var modules = ImmutableArray.CreateBuilder<EmittedModule>(_modules.Count);

                foreach (var item in _modules.Values)
                {
                    modules.Add(item.Link(this));
                }

                return _result = new EmittedModules(modules.MoveToImmutable());
            }

            public EmittedModule Link(ModuleSymbol module)
            {
                if (module == null)
                {
                    throw new ArgumentNullException(nameof(module));
                }

                return _modules[module].Link(this);
            }

            internal int GetFunctionOffset(FunctionSymbol function) => _modules[function.ContainingModule].GetFunctionOffset(function);
        }
    }
}
