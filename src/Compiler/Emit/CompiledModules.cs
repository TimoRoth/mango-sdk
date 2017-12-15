using System.Collections.Generic;
using System.Collections.Immutable;
using Mango.Compiler.Symbols;
using Mango.Compiler.Syntax;

namespace Mango.Compiler.Emit
{
    public sealed class CompiledModules : IReadOnlyCollection<CompiledModule>
    {
        private readonly ImmutableDictionary<ModuleSymbol, CompiledModule> _compiledModules;

        private CompiledModules(ImmutableDictionary<ModuleSymbol, CompiledModule> compiledModules)
        {
            _compiledModules = compiledModules;
        }

        public int Count => _compiledModules.Count;

        public void Emit(string path)
        {
            foreach (var compiledModule in _compiledModules.Values)
            {
                compiledModule.Emit(path);
            }
        }

        public IEnumerator<CompiledModule> GetEnumerator() => _compiledModules.Values.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        internal static CompiledModules Create(Compilation compilation)
        {
            var builder = ImmutableDictionary.CreateBuilder<ModuleSymbol, CompiledModule>();

            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                if (syntaxTree.Root is CompilationUnitSyntax compilationUnitSyntax)
                {
                    var semanticModel = compilation.GetSemanticModel(syntaxTree);

                    foreach (var moduleDeclaration in compilationUnitSyntax.Modules)
                    {
                        builder.Add(semanticModel.GetDeclaredSymbol(moduleDeclaration), CompiledModule.Create(moduleDeclaration, semanticModel));
                    }
                }
            }

            var result = new CompiledModules(builder.ToImmutable());

            foreach (var compiledModule in builder.Values)
            {
                compiledModule.Link(result);
            }

            return result;
        }

        internal CompiledModule GetCompiledModuleFromSymbol(ModuleSymbol module) => _compiledModules[module];
    }
}
