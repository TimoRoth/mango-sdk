using System;
using System.Collections.Immutable;
using Mango.Compiler.Syntax;

namespace Mango.Compiler.Symbols.Source
{
    internal sealed class SourceModuleSymbol : ModuleSymbol
    {
        private readonly SourceApplicationSymbol _containingApplication;
        private readonly ModuleDeclarationSyntax _syntax;

        private ImmutableArray<FunctionSymbol> _functions;
        private ImmutableArray<ModuleSymbol> _imports;
        private ImmutableArray<StructuredTypeSymbol> _types;

        internal SourceModuleSymbol(SourceApplicationSymbol containingApplication, ModuleDeclarationSyntax syntax)
        {
            _containingApplication = containingApplication;
            _syntax = syntax;
        }

        public override ApplicationSymbol ContainingApplication => _containingApplication;

        public override ModuleSymbol ContainingModule => null;

        public override Symbol ContainingSymbol => _containingApplication;

        internal override Compilation DeclaringCompilation => _containingApplication.DeclaringCompilation;

        public override ImmutableArray<FunctionSymbol> Functions => GetFunctions();

        public override ImmutableArray<ModuleSymbol> Imports => GetImports();

        public override string Name => _syntax.ModuleName;

        public override ImmutableArray<StructuredTypeSymbol> Types => GetTypes();

        internal override FunctionSymbol FindFunction(string name)
        {
            foreach (var function in Functions)
            {
                if (function.Name == name)
                {
                    return function;
                }
            }

            return null;
        }

        internal override StructuredTypeSymbol FindType(string name)
        {
            foreach (var type in Types)
            {
                if (type.Name == name)
                {
                    return type;
                }
            }

            return null;
        }

        private ImmutableArray<FunctionSymbol> GetFunctions()
        {
            if (_functions.IsDefault)
            {
                var count = 0;

                foreach (var syntax in _syntax.Members)
                {
                    if (syntax.Kind == SyntaxKind.FunctionDeclaration)
                    {
                        count++;
                    }
                }

                var binder = DeclaringCompilation.Binder;
                var functions = ImmutableArray.CreateBuilder<FunctionSymbol>(count);

                foreach (var syntax in _syntax.Members)
                {
                    if (syntax.Kind == SyntaxKind.FunctionDeclaration)
                    {
                        var functionSyntax = (FunctionDeclarationSyntax)syntax;
                        var returnType = binder.BindType(functionSyntax.ReturnType);
                        if (!TypeSymbol.ValidReturnType(returnType))
                            throw new Exception();
                        functions.Add(new SourceFunctionSymbol(this, functionSyntax, returnType));
                    }
                }

                ImmutableInterlocked.InterlockedInitialize(ref _functions, functions.MoveToImmutable());
            }

            return _functions;
        }

        private ImmutableArray<ModuleSymbol> GetImports()
        {
            if (_imports.IsDefault)
            {
                var binder = DeclaringCompilation.Binder;
                var imports = ImmutableArray.CreateBuilder<ModuleSymbol>(_syntax.Imports.Count);

                foreach (var syntax in _syntax.Imports)
                {
                    var importedModule = binder.BindModule(syntax);
                    if (importedModule == null)
                        throw new Exception();
                    imports.Add(importedModule);
                }

                ImmutableInterlocked.InterlockedInitialize(ref _imports, imports.MoveToImmutable());
            }

            return _imports;
        }

        private ImmutableArray<StructuredTypeSymbol> GetTypes()
        {
            if (_types.IsDefault)
            {
                var count = 0;

                foreach (var syntax in _syntax.Members)
                {
                    if (syntax.Kind == SyntaxKind.TypeDeclaration)
                    {
                        count++;
                    }
                }

                var types = ImmutableArray.CreateBuilder<StructuredTypeSymbol>(count);

                foreach (var syntax in _syntax.Members)
                {
                    if (syntax.Kind == SyntaxKind.TypeDeclaration)
                    {
                        types.Add(new SourceStructuredTypeSymbol(this, (TypeDeclarationSyntax)syntax));
                    }
                }

                ImmutableInterlocked.InterlockedInitialize(ref _types, types.MoveToImmutable());
            }

            return _types;
        }
    }
}
