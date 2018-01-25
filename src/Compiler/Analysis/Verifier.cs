using System;
using Mango.Compiler.Binding;
using Mango.Compiler.Symbols;
using Mango.Compiler.Syntax;
using static Mango.Compiler.Analysis.Verification;

namespace Mango.Compiler.Analysis
{
    public class Verifier : IInterpreter<TypeSymbol>
    {
        private readonly Binder _binder;
        private readonly FunctionDeclarationSyntax _functionDeclaration;

        public Verifier(FunctionDeclarationSyntax functionDeclaration, Compilation compilation)
        {
            _functionDeclaration = functionDeclaration;
            _binder = compilation.Binder;
        }

        public TypeSymbol Add_Div_DivUn_Mul_Rem_RemUn_Sub(NoneInstructionSyntax instruction, TypeSymbol value1, TypeSymbol value2)
        {
            return IsNumeric(value1) && value1 == value2 ? value1 : throw new Exception();
        }

        public TypeSymbol And_Or_Xor(NoneInstructionSyntax instruction, TypeSymbol value1, TypeSymbol value2)
        {
            return IsInteger(value1) && value1 == value2 ? value1 : throw new Exception();
        }

        public void Beq_BneUn(BranchInstructionSyntax instruction, TypeSymbol value1, TypeSymbol value2)
        {
            if (!(IsNumericOrReference(value1) && value1 == value2)) throw new Exception();
        }

        public void Bge_BgeUn_Bgt_BgtUn_Ble_BleUn_Blt_BltUn(BranchInstructionSyntax instruction, TypeSymbol value1, TypeSymbol value2)
        {
            if (!(IsNumeric(value1) && value1 == value2)) throw new Exception();
        }

        public void Br(BranchInstructionSyntax instruction)
        {
        }

        public void Break(NoneInstructionSyntax instruction)
        {
        }

        public void Brfalse(BranchInstructionSyntax instruction, TypeSymbol value)
        {
            if (!IsIntegerOrReference(value)) throw new Exception();
        }

        public void Brtrue(BranchInstructionSyntax instruction, TypeSymbol value)
        {
            if (!IsIntegerOrReference(value)) throw new Exception();
        }

        public TypeSymbol Call_Syscall(FunctionInstructionSyntax instruction, TypeSymbol[] arguments)
        {
            var function = _binder.BindFunction(instruction) ?? throw new Exception();
            var parameters = function.Parameters;
            var returnType = function.ReturnType;

            if (function.ReturnsVoid) throw new Exception();
            if (arguments.Length != parameters.Length) throw new Exception();
            for (var i = 0; i < parameters.Length; i++)
                if (!VerifierAssignableTo(arguments[i], parameters[i].Type)) throw new Exception();

            return GetIntermediateType(returnType);
        }

        public void CallVoid_SyscallVoid(FunctionInstructionSyntax instruction, TypeSymbol[] arguments)
        {
            var function = _binder.BindFunction(instruction) ?? throw new Exception();
            var parameters = function.Parameters;

            if (!function.ReturnsVoid) throw new Exception();
            if (arguments.Length != parameters.Length) throw new Exception();
            for (var i = 0; i < parameters.Length; i++)
                if (!VerifierAssignableTo(arguments[i], parameters[i].Type)) throw new Exception();
        }

        public TypeSymbol Calli(TypeInstructionSyntax instruction, TypeSymbol[] arguments, TypeSymbol function)
        {
            var instructionType = (FunctionTypeSymbol)_binder.BindType(instruction) ?? throw new Exception();
            var parameterTypes = instructionType.ParameterTypes;
            var returnType = instructionType.ReturnType;

            if (instructionType.ReturnsVoid) throw new Exception();
            if (arguments.Length != parameterTypes.Length) throw new Exception();
            for (var i = 0; i < parameterTypes.Length; i++)
                if (!VerifierAssignableTo(arguments[i], parameterTypes[i])) throw new Exception();
            if (!VerifierAssignableTo(function, instructionType)) throw new Exception();

            return GetIntermediateType(returnType);
        }

        public void CalliVoid(TypeInstructionSyntax instruction, TypeSymbol[] arguments, TypeSymbol function)
        {
            var instructionType = (FunctionTypeSymbol)_binder.BindType(instruction) ?? throw new Exception();
            var parameterTypes = instructionType.ParameterTypes;

            if (!instructionType.ReturnsVoid) throw new Exception();
            if (arguments.Length != parameterTypes.Length) throw new Exception();
            for (var i = 0; i < parameterTypes.Length; i++)
                if (!VerifierAssignableTo(arguments[i], parameterTypes[i])) throw new Exception();
            if (!VerifierAssignableTo(function, instructionType)) throw new Exception();
        }

        public TypeSymbol Ceq(NoneInstructionSyntax instruction, TypeSymbol value1, TypeSymbol value2)
        {
            return IsNumericOrReference(value1) && value1 == value2 ? SpecialTypeSymbol.GetSpecialType(SpecialType.Int32) : throw new Exception();
        }

        public TypeSymbol Cgt_CgtUn_Clt_CltUn(NoneInstructionSyntax instruction, TypeSymbol value1, TypeSymbol value2)
        {
            return IsNumeric(value1) && value1 == value2 ? SpecialTypeSymbol.GetSpecialType(SpecialType.Int32) : throw new Exception();
        }

        public TypeSymbol Conv_ConvUn(TypeInstructionSyntax instruction, TypeSymbol value)
        {
            if (!IsNumeric(value)) throw new Exception();

            var instructionType = _binder.BindType(instruction);

            switch (instructionType.SpecialType)
            {
            case SpecialType.Int8:
            case SpecialType.Int16:
            case SpecialType.Int32:
            case SpecialType.Int64:
            case SpecialType.UInt8:
            case SpecialType.UInt16:
            case SpecialType.UInt32:
            case SpecialType.UInt64:
            case SpecialType.Float32:
            case SpecialType.Float64:
                break;
            default:
                throw new Exception();
            }

            return GetIntermediateType(instructionType);
        }

        public void Cpobj(TypeInstructionSyntax instruction, TypeSymbol destination, TypeSymbol source)
        {
            var destinationType = GetReferencedType(destination);
            var instructionType = _binder.BindType(instruction);
            var sourceType = GetReferencedType(source);

            if (!VerifierAssignableTo(sourceType, instructionType)) throw new Exception();
            if (!VerifierAssignableTo(instructionType, destinationType)) throw new Exception();
        }

        public void Dup(NoneInstructionSyntax instruction, TypeSymbol value)
        {
        }

        public void Initobj(TypeInstructionSyntax instruction, TypeSymbol destination)
        {
            var instructionType = _binder.BindType(instruction);
            var destinationType = GetReferencedType(destination);

            if (!VerifierAssignableTo(instructionType, destinationType)) throw new Exception();
        }

        public TypeSymbol Ldarg(ArgumentInstructionSyntax instruction)
        {
            var parameter = _binder.BindParameter(instruction);

            return GetIntermediateType(parameter.Type);
        }

        public TypeSymbol Ldarga(ArgumentInstructionSyntax instruction)
        {
            var parameter = _binder.BindParameter(instruction);

            return GetIntermediateType(new ReferenceTypeSymbol(parameter.Type));
        }

        public TypeSymbol Ldc(ConstantInstructionSyntax instruction)
        {
            var type = _binder.BindType(instruction);

            if (type.SpecialType != SpecialType.Int32 &&
                type.SpecialType != SpecialType.Int64) throw new Exception();

            return GetIntermediateType(type);
        }

        public TypeSymbol Ldelem(TypeInstructionSyntax instruction, TypeSymbol array, TypeSymbol index)
        {
            if (index.SpecialType != SpecialType.Int32) throw new Exception();

            var instructionType = _binder.BindType(instruction);
            var elementType = GetElementType(array);

            if (!VerifierAssignableTo(elementType, instructionType)) throw new Exception();

            return GetIntermediateType(instructionType);
        }

        public TypeSymbol Ldelema(TypeInstructionSyntax instruction, TypeSymbol array, TypeSymbol index)
        {
            if (index.SpecialType != SpecialType.Int32) throw new Exception();

            var instructionType = _binder.BindType(instruction);
            var elementType = GetElementType(array);

            if (!VerifierAssignableTo(elementType, instructionType)) throw new Exception();

            return GetIntermediateType(new ReferenceTypeSymbol(instructionType));
        }

        public TypeSymbol Ldfld(FieldInstructionSyntax instruction, TypeSymbol obj)
        {
            var field = _binder.BindField(instruction);

            if (!VerifierAssignableTo(obj, field.ContainingType)) throw new Exception();

            return GetIntermediateType(field.Type);
        }

        public TypeSymbol Ldflda(FieldInstructionSyntax instruction, TypeSymbol obj)
        {
            var field = _binder.BindField(instruction);

            if (!VerifierAssignableTo(obj, field.ContainingType)) throw new Exception();

            return GetIntermediateType(new ReferenceTypeSymbol(field.Type));
        }

        public TypeSymbol Ldftn(FunctionInstructionSyntax instruction)
        {
            var function = _binder.BindFunction(instruction) ?? throw new Exception();

            return GetIntermediateType(function.Type);
        }

        public TypeSymbol Ldind(TypeInstructionSyntax instruction, TypeSymbol address)
        {
            var instructionType = _binder.BindType(instruction);
            var sourceType = GetReferencedType(address);

            if (!VerifierAssignableTo(sourceType, instructionType)) throw new Exception();

            return GetIntermediateType(instructionType);
        }

        public TypeSymbol Ldlen(NoneInstructionSyntax instruction, TypeSymbol array)
        {
            var elementType = GetElementType(array);

            return SpecialTypeSymbol.GetSpecialType(SpecialType.Int32);
        }

        public TypeSymbol Ldloc(LocalInstructionSyntax instruction)
        {
            var local = _binder.BindLocal(instruction);

            return GetIntermediateType(local.Type);
        }

        public TypeSymbol Ldloca(LocalInstructionSyntax instruction)
        {
            var local = _binder.BindLocal(instruction);

            return GetIntermediateType(new ReferenceTypeSymbol(local.Type));
        }

        public TypeSymbol Ldnull(NoneInstructionSyntax instruction)
        {
            return SpecialTypeSymbol.GetSpecialType(SpecialType.Null);
        }

        public TypeSymbol Ldobj(TypeInstructionSyntax instruction, TypeSymbol source)
        {
            var instructionType = _binder.BindType(instruction);
            var sourceType = GetReferencedType(source);

            if (!VerifierAssignableTo(sourceType, instructionType)) throw new Exception();

            return GetIntermediateType(instructionType);
        }

        public TypeSymbol Neg(NoneInstructionSyntax instruction, TypeSymbol value)
        {
            return IsNumeric(value) ? value : throw new Exception();
        }

        public TypeSymbol Newarr(TypeInstructionSyntax instruction, TypeSymbol length)
        {
            if (length.SpecialType != SpecialType.Int32) throw new Exception();

            var instructionType = _binder.BindType(instruction);

            return GetIntermediateType(new SpanTypeSymbol(instructionType));
        }

        public TypeSymbol Newobj(FunctionInstructionSyntax instruction, TypeSymbol[] arguments)
        {
            var function = _binder.BindFunction(instruction) ?? throw new Exception();
            var parameters = function.Parameters;
            var returnType = function.ReturnType;

            if (parameters.Length == 0) throw new Exception();
            if (parameters[0].Type.TypeKind != TypeKind.Reference) throw new Exception();
            if (arguments.Length != parameters.Length) throw new Exception();
            for (var i = 0; i < parameters.Length; i++)
                if (!VerifierAssignableTo(arguments[i], parameters[i].Type)) throw new Exception();

            return GetIntermediateType(returnType);
        }

        public void Nop(NoneInstructionSyntax instruction)
        {
        }

        public TypeSymbol Not(NoneInstructionSyntax instruction, TypeSymbol value)
        {
            return IsInteger(value) ? value : throw new Exception();
        }

        public TypeSymbol Phi(TypeSymbol value1, TypeSymbol value2)
        {
            return Merge(value1, value2);
        }

        public void Pop(NoneInstructionSyntax instruction, TypeSymbol value)
        {
        }

        public void Ret(NoneInstructionSyntax instruction)
        {
            var returnType = _binder.BindFunction(_functionDeclaration).ReturnType;

            if (returnType.SpecialType != SpecialType.Void) throw new Exception();
        }

        public void Ret(NoneInstructionSyntax instruction, TypeSymbol value)
        {
            var returnType = _binder.BindFunction(_functionDeclaration).ReturnType;

            if (returnType.SpecialType == SpecialType.Void) throw new Exception();
            if (!VerifierAssignableTo(value, returnType)) throw new Exception();

            if (returnType.TypeKind == TypeKind.Reference) throw new NotSupportedException(); // requires lifetimes
            if (returnType.TypeKind == TypeKind.Span) throw new NotSupportedException(); // requires lifetimes
        }

        public TypeSymbol Shl_Shr_ShrUn(NoneInstructionSyntax instruction, TypeSymbol value, TypeSymbol amount)
        {
            if (amount.SpecialType != SpecialType.Int32) throw new Exception();

            return IsInteger(value) ? value : throw new Exception();
        }

        public void Starg(ArgumentInstructionSyntax instruction, TypeSymbol value)
        {
            var parameter = _binder.BindParameter(instruction);

            if (!VerifierAssignableTo(value, parameter.Type)) throw new Exception();
        }

        public void Stelem(TypeInstructionSyntax instruction, TypeSymbol array, TypeSymbol index, TypeSymbol value)
        {
            if (index.SpecialType != SpecialType.Int32) throw new Exception();

            var instructionType = _binder.BindType(instruction);
            var elementType = GetElementType(array);

            if (!VerifierAssignableTo(value, instructionType)) throw new Exception();
            if (!VerifierAssignableTo(instructionType, elementType)) throw new Exception();

            if (instructionType.TypeKind == TypeKind.Reference) throw new NotSupportedException(); // requires lifetimes
            if (instructionType.TypeKind == TypeKind.Span) throw new NotSupportedException(); // requires lifetimes
        }

        public void Stfld(FieldInstructionSyntax instruction, TypeSymbol obj, TypeSymbol value)
        {
            var field = _binder.BindField(instruction);

            if (!VerifierAssignableTo(obj, field.ContainingType)) throw new Exception();
            if (!VerifierAssignableTo(value, field.Type)) throw new Exception();

            if (field.Type.TypeKind == TypeKind.Reference) throw new NotSupportedException(); // requires lifetimes
            if (field.Type.TypeKind == TypeKind.Span) throw new NotSupportedException(); // requires lifetimes
        }

        public void Stind(TypeInstructionSyntax instruction, TypeSymbol address, TypeSymbol value)
        {
            var destinationType = GetReferencedType(address);
            var instructionType = _binder.BindType(instruction);
            var sourceType = value;

            if (!VerifierAssignableTo(sourceType, instructionType)) throw new Exception();
            if (!VerifierAssignableTo(instructionType, destinationType)) throw new Exception();

            if (instructionType.TypeKind == TypeKind.Reference) throw new NotSupportedException(); // requires lifetimes
            if (instructionType.TypeKind == TypeKind.Span) throw new NotSupportedException(); // requires lifetimes
        }

        public void Stloc(LocalInstructionSyntax instruction, TypeSymbol value)
        {
            var local = _binder.BindLocal(instruction);

            if (!VerifierAssignableTo(value, local.Type)) throw new Exception();
        }

        public void Stobj(TypeInstructionSyntax instruction, TypeSymbol destination, TypeSymbol source)
        {
            var destinationType = GetReferencedType(destination);
            var instructionType = _binder.BindType(instruction);
            var sourceType = source;

            if (!VerifierAssignableTo(sourceType, instructionType)) throw new Exception();
            if (!VerifierAssignableTo(instructionType, destinationType)) throw new Exception();

            if (instructionType.TypeKind == TypeKind.Reference) throw new NotSupportedException(); // requires lifetimes
            if (instructionType.TypeKind == TypeKind.Span) throw new NotSupportedException(); // requires lifetimes
        }

        private static TypeSymbol GetElementType(TypeSymbol value)
        {
            return value is ArrayTypeSymbol array ? array.ElementType :
                   value is SpanTypeSymbol span ? span.ElementType :
                   value is ReferenceTypeSymbol reference && reference.ReferencedType is ArrayTypeSymbol referencedArray ? referencedArray.ElementType : throw new Exception();
        }

        private static TypeSymbol GetReferencedType(TypeSymbol value)
        {
            return value is ReferenceTypeSymbol reference ? reference.ReferencedType : throw new Exception();
        }

        private static bool IsInteger(TypeSymbol type)
        {
            return type.SpecialType == SpecialType.Int32 ||
                   type.SpecialType == SpecialType.Int64;
        }

        private static bool IsIntegerOrReference(TypeSymbol type)
        {
            return type.SpecialType == SpecialType.Int32 ||
                   type.SpecialType == SpecialType.Int64 ||
                   type.TypeKind == TypeKind.Reference;
        }

        private static bool IsNumeric(TypeSymbol type)
        {
            return type.SpecialType == SpecialType.Int32 ||
                   type.SpecialType == SpecialType.Int64 ||
                   type.SpecialType == SpecialType.Float32 ||
                   type.SpecialType == SpecialType.Float64;
        }

        private static bool IsNumericOrReference(TypeSymbol type)
        {
            return type.SpecialType == SpecialType.Int32 ||
                   type.SpecialType == SpecialType.Int64 ||
                   type.SpecialType == SpecialType.Float32 ||
                   type.SpecialType == SpecialType.Float64 ||
                   type.TypeKind == TypeKind.Reference;
        }
    }
}
