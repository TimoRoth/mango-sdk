using System.Collections.Immutable;
using Mango.Compiler.Syntax;

namespace Mango.Compiler.Symbols.Source
{
    internal sealed class SourceApplicationSymbol : ApplicationSymbol
    {
        private readonly Compilation _compilation;

        private ImmutableArray<ModuleSymbol> _modules;

        internal SourceApplicationSymbol(Compilation compilation)
        {
            _compilation = compilation;
        }

        public override ApplicationSymbol ContainingApplication => null;

        public override ModuleSymbol ContainingModule => null;

        public override Symbol ContainingSymbol => null;

        public override ImmutableArray<ModuleSymbol> Modules => GetModules();

        public override string Name => _compilation.ApplicationName;

        internal override Compilation DeclaringCompilation => _compilation;

        internal override ModuleSymbol FindModule(string name)
        {
            foreach (var module in Modules)
            {
                if (module.Name == name)
                {
                    return module;
                }
            }

            return null;
        }

        private ImmutableArray<ModuleSymbol> GetModules()
        {
            if (_modules.IsDefault)
            {
                var count = 0;

                foreach (var syntaxTree in _compilation.SyntaxTrees)
                {
                    if (syntaxTree.Root is CompilationUnitSyntax compilationUnit)
                    {
                        foreach (var syntax in compilationUnit.Modules)
                        {
                            count++;
                        }
                    }
                }

                var modules = ImmutableArray.CreateBuilder<ModuleSymbol>(count);

                foreach (var syntaxTree in _compilation.SyntaxTrees)
                {
                    if (syntaxTree.Root is CompilationUnitSyntax compilationUnit)
                    {
                        foreach (var syntax in compilationUnit.Modules)
                        {
                            modules.Add(new SourceModuleSymbol(this, syntax));
                        }
                    }
                }

                ImmutableInterlocked.InterlockedInitialize(ref _modules, modules.MoveToImmutable());
            }

            return _modules;
        }
    }
}
