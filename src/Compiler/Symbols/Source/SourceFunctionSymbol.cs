using System;
using System.Collections.Immutable;
using System.Threading;
using Mango.Compiler.Syntax;

namespace Mango.Compiler.Symbols.Source
{
    internal sealed class SourceFunctionSymbol : FunctionSymbol
    {
        private readonly SourceModuleSymbol _containingModule;
        private readonly TypeSymbol _returnType;
        private readonly FunctionDeclarationSyntax _syntax;
        private readonly int? _systemCallOrdinal;

        private ImmutableArray<LabelSymbol> _labels;
        private ImmutableArray<LocalSymbol> _locals;
        private ImmutableArray<ParameterSymbol> _parameters;
        private FunctionTypeSymbol _type;

        internal SourceFunctionSymbol(SourceModuleSymbol containingModule, FunctionDeclarationSyntax syntax, TypeSymbol returnType)
        {
            _containingModule = containingModule;
            _syntax = syntax;
            _returnType = returnType;
            _systemCallOrdinal = syntax.SystemCallOrdinal;
        }

        public override ModuleSymbol ContainingModule => _containingModule;

        public override Symbol ContainingSymbol => _containingModule;

        public override bool IsExtern => _systemCallOrdinal.HasValue;

        public override ImmutableArray<LabelSymbol> Labels => GetLabels();

        public override ImmutableArray<LocalSymbol> Locals => GetLocals();

        public override string Name => _syntax.FunctionName;

        public override ImmutableArray<ParameterSymbol> Parameters => GetParameters();

        public override bool ReturnsVoid => _returnType.SpecialType == SpecialType.Void;

        public override TypeSymbol ReturnType => _returnType;

        public override FunctionTypeSymbol Type => GetFunctionType();

        public override int GetSystemCallOrdinal() => _systemCallOrdinal.Value;

        internal override LabelSymbol FindLabel(string name)
        {
            foreach (var label in Labels)
            {
                if (label.Name == name)
                {
                    return label;
                }
            }

            return null;
        }

        internal override LocalSymbol FindLocal(string name)
        {
            foreach (var local in Locals)
            {
                if (local.Name == name)
                {
                    return local;
                }
            }

            return null;
        }

        internal override ParameterSymbol FindParameter(string name)
        {
            foreach (var parameter in Parameters)
            {
                if (parameter.Name == name)
                {
                    return parameter;
                }
            }

            return null;
        }

        private FunctionTypeSymbol GetFunctionType()
        {
            if (_type == null)
            {
                var binder = DeclaringCompilation.Binder;
                var parameterTypes = ImmutableArray.CreateBuilder<TypeSymbol>(_syntax.Parameters.Count);

                foreach (var syntax in _syntax.Parameters)
                {
                    var parameterType = binder.BindType(syntax.ParameterType);
                    if (!TypeSymbol.ValidLocationType(parameterType))
                        throw new Exception();
                    parameterTypes.Add(parameterType);
                }

                Interlocked.CompareExchange(ref _type, new FunctionTypeSymbol(_returnType, parameterTypes.MoveToImmutable()), null);
            }

            return _type;
        }

        private ImmutableArray<LabelSymbol> GetLabels()
        {
            if (_labels.IsDefault)
            {
                if (_syntax.Body != null)
                {
                    var count = 0;

                    foreach (var instruction in _syntax.Body.Instructions)
                    {
                        var i = instruction;
                        while (i.Kind == SyntaxKind.LabeledInstruction)
                        {
                            var syntax = (LabeledInstructionSyntax)i;
                            count++;
                            i = syntax.LabeledInstruction;
                        }
                    }

                    var labels = ImmutableArray.CreateBuilder<LabelSymbol>(count);

                    foreach (var instruction in _syntax.Body.Instructions)
                    {
                        var i = instruction;
                        while (i.Kind == SyntaxKind.LabeledInstruction)
                        {
                            var syntax = (LabeledInstructionSyntax)i;
                            labels.Add(new SourceLabelSymbol(this, syntax));
                            i = syntax.LabeledInstruction;
                        }
                    }

                    ImmutableInterlocked.InterlockedInitialize(ref _labels, labels.MoveToImmutable());
                }
                else
                {
                    ImmutableInterlocked.InterlockedInitialize(ref _labels, ImmutableArray<LabelSymbol>.Empty);
                }
            }

            return _labels;
        }

        private ImmutableArray<LocalSymbol> GetLocals()
        {
            if (_locals.IsDefault)
            {
                if (_syntax.Body != null)
                {
                    var binder = DeclaringCompilation.Binder;
                    var locals = ImmutableArray.CreateBuilder<LocalSymbol>(_syntax.Body.Locals.Count);

                    foreach (var syntax in _syntax.Body.Locals)
                    {
                        var localType = binder.BindType(syntax.LocalType);
                        if (!TypeSymbol.ValidLocationType(localType))
                            throw new Exception();
                        locals.Add(new SourceLocalSymbol(this, syntax, localType));
                    }

                    ImmutableInterlocked.InterlockedInitialize(ref _locals, locals.MoveToImmutable());
                }
                else
                {
                    ImmutableInterlocked.InterlockedInitialize(ref _locals, ImmutableArray<LocalSymbol>.Empty);
                }
            }

            return _locals;
        }

        private ImmutableArray<ParameterSymbol> GetParameters()
        {
            if (_parameters.IsDefault)
            {
                var binder = DeclaringCompilation.Binder;
                var parameters = ImmutableArray.CreateBuilder<ParameterSymbol>(_syntax.Parameters.Count);

                foreach (var syntax in _syntax.Parameters)
                {
                    var parameterType = binder.BindType(syntax.ParameterType);
                    if (!TypeSymbol.ValidLocationType(parameterType))
                        throw new Exception();
                    parameters.Add(new SourceParameterSymbol(this, syntax, parameterType));
                }

                ImmutableInterlocked.InterlockedInitialize(ref _parameters, parameters.MoveToImmutable());
            }

            return _parameters;
        }
    }
}
