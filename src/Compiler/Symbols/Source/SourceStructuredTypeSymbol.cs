using System;
using System.Collections.Immutable;
using System.Threading;
using Mango.Compiler.Syntax;

namespace Mango.Compiler.Symbols.Source
{
    internal sealed class SourceStructuredTypeSymbol : StructuredTypeSymbol
    {
        private readonly SourceModuleSymbol _containingModule;
        private readonly TypeDeclarationSyntax _syntax;

        private LazyTypeInfo _typeInfo;

        public SourceStructuredTypeSymbol(SourceModuleSymbol containingModule, TypeDeclarationSyntax syntax)
        {
            _containingModule = containingModule;
            _syntax = syntax;
        }

        public override ModuleSymbol ContainingModule => _containingModule;

        public override Symbol ContainingSymbol => _containingModule;

        public override ImmutableArray<FieldSymbol> Fields => GetTypeInfo().Fields;

        public override string Name => _syntax.TypeName;

        public override TypeLayout TypeLayout => GetTypeInfo().TypeLayout;

        internal override FieldSymbol FindField(string name)
        {
            foreach (var field in Fields)
            {
                if (field.Name == name)
                {
                    return field;
                }
            }

            return null;
        }

        private LazyTypeInfo GetTypeInfo()
        {
            if (_typeInfo == null)
            {
                if (_syntax.Fields.Count == 0)
                {
                    var typeLayout = new TypeLayout(1, 1);

                    Interlocked.CompareExchange(ref _typeInfo, new LazyTypeInfo(ImmutableArray<FieldSymbol>.Empty, typeLayout), null);
                }
                else
                {
                    var offset = 0;
                    var alignment = 0;
                    var binder = DeclaringCompilation.Binder;
                    var fields = ImmutableArray.CreateBuilder<FieldSymbol>(_syntax.Fields.Count);

                    foreach (var syntax in _syntax.Fields)
                    {
                        var fieldType = binder.BindType(syntax.FieldType);
                        if (!ValidLocationType(fieldType))
                            throw new Exception();
                        offset = (offset + (fieldType.TypeLayout.Alignment - 1)) & ~(fieldType.TypeLayout.Alignment - 1);
                        fields.Add(new SourceFieldSymbol(this, syntax, fieldType, offset));
                        offset += fieldType.TypeLayout.Size;
                        if (alignment < fieldType.TypeLayout.Alignment)
                            alignment = fieldType.TypeLayout.Alignment;
                    }

                    var typeLayout = new TypeLayout((offset + (alignment - 1)) & ~(alignment - 1), alignment);

                    Interlocked.CompareExchange(ref _typeInfo, new LazyTypeInfo(fields.MoveToImmutable(), typeLayout), null);
                }
            }

            return _typeInfo;
        }

        private class LazyTypeInfo
        {
            private readonly ImmutableArray<FieldSymbol> _fields;
            private readonly TypeLayout _typeLayout;

            public LazyTypeInfo(ImmutableArray<FieldSymbol> fields, TypeLayout typeLayout)
            {
                _fields = fields;
                _typeLayout = typeLayout;
            }

            public ImmutableArray<FieldSymbol> Fields => _fields;

            public TypeLayout TypeLayout => _typeLayout;
        }
    }
}
