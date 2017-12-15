using System;
using System.Collections.Immutable;
using Mango.Compiler.Symbols;
using Mango.Compiler.Symbols.Source;
using Mango.Compiler.Syntax;

namespace Mango.Compiler.Binding
{
    internal sealed class Binder
    {
        private readonly ApplicationSymbol _applicationSymbol;

        public Binder(Compilation compilation)
        {
            _applicationSymbol = new SourceApplicationSymbol(compilation);
        }

        public ApplicationSymbol BindApplication()
        {
            return _applicationSymbol;
        }

        public FieldSymbol BindField(FieldDeclarationSyntax syntax)
        {
            return BindType((TypeDeclarationSyntax)syntax.Parent)?.FindField(syntax.FieldName);
        }

        public FieldSymbol BindField(FieldInstructionSyntax syntax)
        {
            return BindType(syntax.ContainingType)?.FindField(syntax.FieldName);
        }

        public FunctionSymbol BindFunction(FunctionDeclarationSyntax syntax)
        {
            return BindModule((ModuleDeclarationSyntax)syntax.Parent)?.FindFunction(syntax.FunctionName);
        }

        public FunctionSymbol BindFunction(FunctionInstructionSyntax syntax)
        {
            if (syntax.ModuleName == null)
            {
                return BindModule(syntax.FirstAncestorOrSelf<ModuleDeclarationSyntax>())?.FindFunction(syntax.FunctionName);
            }
            else
            {
                return _applicationSymbol.FindModule(syntax.ModuleName)?.FindFunction(syntax.FunctionName);
            }
        }

        public LabelSymbol BindLabel(LabeledInstructionSyntax syntax)
        {
            return BindFunction(syntax.FirstAncestorOrSelf<FunctionDeclarationSyntax>())?.FindLabel(syntax.LabelName);
        }

        public LabelSymbol BindLabel(BranchInstructionSyntax syntax)
        {
            return BindFunction(syntax.FirstAncestorOrSelf<FunctionDeclarationSyntax>())?.FindLabel(syntax.LabelName);
        }

        public LocalSymbol BindLocal(LocalDeclarationSyntax syntax)
        {
            return BindFunction(syntax.FirstAncestorOrSelf<FunctionDeclarationSyntax>())?.FindLocal(syntax.LocalName);
        }

        public LocalSymbol BindLocal(LocalInstructionSyntax syntax)
        {
            return BindFunction(syntax.FirstAncestorOrSelf<FunctionDeclarationSyntax>())?.FindLocal(syntax.LocalName);
        }

        public ModuleSymbol BindModule(ModuleDeclarationSyntax syntax)
        {
            return _applicationSymbol.FindModule(syntax.ModuleName);
        }

        public ParameterSymbol BindParameter(ParameterDeclarationSyntax syntax)
        {
            return BindFunction(syntax.FirstAncestorOrSelf<FunctionDeclarationSyntax>())?.FindParameter(syntax.ParameterName);
        }

        public ParameterSymbol BindParameter(ArgumentInstructionSyntax syntax)
        {
            return BindFunction(syntax.FirstAncestorOrSelf<FunctionDeclarationSyntax>())?.FindParameter(syntax.ParameterName);
        }

        public StructuredTypeSymbol BindType(TypeDeclarationSyntax syntax)
        {
            return BindModule((ModuleDeclarationSyntax)syntax.Parent)?.FindType(syntax.TypeName);
        }

        public TypeSymbol BindType(ConstantInstructionSyntax syntax)
        {
            return BindType(syntax.ConstantType);
        }

        public TypeSymbol BindType(TypeInstructionSyntax syntax)
        {
            return BindType(syntax.Type);
        }

        public StructuredTypeSymbol BindType(StructuredTypeSyntax syntax)
        {
            if (syntax.ModuleName == null)
            {
                return BindModule(syntax.FirstAncestorOrSelf<ModuleDeclarationSyntax>())?.FindType(syntax.TypeName);
            }
            else
            {
                return _applicationSymbol.FindModule(syntax.ModuleName)?.FindType(syntax.TypeName);
            }
        }

        public TypeSymbol BindType(TypeSyntax syntax)
        {
            switch (syntax.Kind)
            {
            case SyntaxKind.ArrayType when syntax is ArrayTypeSyntax array:
                return new ArrayTypeSymbol(BindType(array.ElementType), array.Length);

            case SyntaxKind.BoolType:
                return SpecialTypeSymbol.GetSpecialType(SpecialType.Bool);

            case SyntaxKind.Float32Type:
                return SpecialTypeSymbol.GetSpecialType(SpecialType.Float32);

            case SyntaxKind.Float64Type:
                return SpecialTypeSymbol.GetSpecialType(SpecialType.Float64);

            case SyntaxKind.FunctionType when syntax is FunctionTypeSyntax function:
                var returnType = BindType(function.ReturnType);
                var parameterTypes = ImmutableArray.CreateBuilder<TypeSymbol>(function.ParameterTypes.Count);
                foreach (var parameterType in function.ParameterTypes)
                {
                    parameterTypes.Add(BindType(parameterType));
                }
                return new FunctionTypeSymbol(returnType, parameterTypes.MoveToImmutable());

            case SyntaxKind.Int16Type:
                return SpecialTypeSymbol.GetSpecialType(SpecialType.Int16);

            case SyntaxKind.Int32Type:
                return SpecialTypeSymbol.GetSpecialType(SpecialType.Int32);

            case SyntaxKind.Int64Type:
                return SpecialTypeSymbol.GetSpecialType(SpecialType.Int64);

            case SyntaxKind.Int8Type:
                return SpecialTypeSymbol.GetSpecialType(SpecialType.Int8);

            case SyntaxKind.StructuredType when syntax is StructuredTypeSyntax structure:
                return BindType(structure);

            case SyntaxKind.ReferenceType when syntax is ReferenceTypeSyntax reference:
                return new ReferenceTypeSymbol(BindType(reference.ReferencedType));

            case SyntaxKind.SpanType when syntax is SpanTypeSyntax span:
                return new SpanTypeSymbol(BindType(span.ElementType));

            case SyntaxKind.UInt16Type:
                return SpecialTypeSymbol.GetSpecialType(SpecialType.UInt16);

            case SyntaxKind.UInt32Type:
                return SpecialTypeSymbol.GetSpecialType(SpecialType.UInt32);

            case SyntaxKind.UInt64Type:
                return SpecialTypeSymbol.GetSpecialType(SpecialType.UInt64);

            case SyntaxKind.UInt8Type:
                return SpecialTypeSymbol.GetSpecialType(SpecialType.UInt8);

            case SyntaxKind.VoidType:
                return SpecialTypeSymbol.GetSpecialType(SpecialType.Void);

            default:
                throw new Exception();
            }
        }
    }
}
