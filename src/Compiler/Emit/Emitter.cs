using System;
using System.Collections.Immutable;
using Mango.Compiler.Symbols;
using Mango.Compiler.Syntax;
using Mango.Compiler.Verification;

namespace Mango.Compiler.Emit
{
    internal static partial class Emitter
    {
        public static EmittedModules Build(Compilation compilation)
        {
            if (compilation == null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            return Compile(compilation).Link();
        }

        private static EmittingModules Compile(Compilation compilation)
        {
            var emittingModules = ImmutableDictionary.CreateBuilder<ModuleSymbol, EmittingModule>();

            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                if (syntaxTree.Root is CompilationUnitSyntax compilationUnit)
                {
                    var semanticModel = compilation.GetSemanticModel(syntaxTree);

                    foreach (var moduleDeclaration in compilationUnit.Modules)
                    {
                        var verifiedModule = Verifier.VerifyModule(moduleDeclaration, semanticModel);

                        var emittingModule = new EmittingModule(verifiedModule);

                        emittingModule.Compile();

                        emittingModules.Add(verifiedModule.Symbol, emittingModule);
                    }
                }
            }

            return new EmittingModules(emittingModules.ToImmutable());
        }
    }
}
