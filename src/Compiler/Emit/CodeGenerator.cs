using System;
using System.Collections.Immutable;
using System.Linq;
using Mango.Compiler.Binding;
using Mango.Compiler.Symbols;
using Mango.Compiler.Syntax;
using static Interop.Libmango;
using static Interop.Libmango.mango_opcode;
using static Mango.Compiler.Analysis.Verification;

namespace Mango.Compiler.Emit
{
    internal readonly struct CodeGenerator
    {
        private readonly Binder _binder;
        private readonly CompiledModules _compiledModules;
        private readonly FunctionDeclarationSyntax _functionDeclaration;

        public CodeGenerator(Binder binder, FunctionDeclarationSyntax functionDeclaration, CompiledModules compiledModules = null)
        {
            _binder = binder;
            _functionDeclaration = functionDeclaration;
            _compiledModules = compiledModules;
        }

        public ByteCode Add_Div_DivUn_Mul_Rem_RemUn_Sub(NoneInstructionSyntax instruction, TypeSymbol value1, TypeSymbol value2)
        {
            if (value1 != value2) throw new Exception();

            switch (instruction.Kind)
            {
            case SyntaxKind.Add:
                return new ByteCode(Select(value1, ADD_I32, ADD_I64, ADD_F32, ADD_F64));
            case SyntaxKind.Div:
                return new ByteCode(Select(value1, DIV_I32, DIV_I64, DIV_F32, DIV_F64));
            case SyntaxKind.Div_Un:
                return new ByteCode(Select(value1, DIV_I32_UN, DIV_I64_UN, DIV_F32, DIV_F64));
            case SyntaxKind.Mul:
                return new ByteCode(Select(value1, MUL_I32, MUL_I64, MUL_F32, MUL_F64));
            case SyntaxKind.Rem:
                return new ByteCode(Select(value1, REM_I32, REM_I64, REM_F32, REM_F64));
            case SyntaxKind.Rem_Un:
                return new ByteCode(Select(value1, REM_I32_UN, REM_I64_UN, REM_F32, REM_F64));
            case SyntaxKind.Sub:
                return new ByteCode(Select(value1, SUB_I32, SUB_I64, SUB_F32, SUB_F64));
            default:
                throw new Exception();
            }
        }

        public ByteCode And_Or_Xor(NoneInstructionSyntax instruction, TypeSymbol value1, TypeSymbol value2)
        {
            if (value1 != value2) throw new Exception();

            switch (instruction.Kind)
            {
            case SyntaxKind.And:
                return new ByteCode(Select(value1, AND_I32, AND_I64));
            case SyntaxKind.Or:
                return new ByteCode(Select(value1, OR_I32, OR_I64));
            case SyntaxKind.Xor:
                return new ByteCode(Select(value1, XOR_I32, XOR_I64));
            default:
                throw new Exception();
            }
        }

        public ByteCode Beq_BneUn(BranchInstructionSyntax instruction, TypeSymbol value1, TypeSymbol value2, int offset)
        {
            if (value1 != value2) throw new Exception();

            var label = _binder.BindLabel(instruction);

            switch (instruction.Kind)
            {
            case SyntaxKind.Beq:
                return Branch(Select(value1, CEQ_I32, CEQ_I64, CEQ_F32, CEQ_F64, CEQ_I32), BRTRUE, offset, _compiledModules, label);
            case SyntaxKind.Bne_Un:
                return Branch(Select(value1, CEQ_I32, CEQ_I64, CEQ_F32, CEQ_F64, CEQ_I32), BRFALSE, offset, _compiledModules, label);
            case SyntaxKind.Beq_S:
                return BranchShort(Select(value1, CEQ_I32, CEQ_I64, CEQ_F32, CEQ_F64, CEQ_I32), BRTRUE_S, offset, _compiledModules, label);
            case SyntaxKind.Bne_Un_S:
                return BranchShort(Select(value1, CEQ_I32, CEQ_I64, CEQ_F32, CEQ_F64, CEQ_I32), BRFALSE_S, offset, _compiledModules, label);
            default:
                throw new Exception();
            }
        }

        public ByteCode Bge_BgeUn_Bgt_BgtUn_Ble_BleUn_Blt_BltUn(BranchInstructionSyntax instruction, TypeSymbol value1, TypeSymbol value2, int offset)
        {
            if (value1 != value2) throw new Exception();

            var label = _binder.BindLabel(instruction);

            switch (instruction.Kind)
            {
            case SyntaxKind.Bge:
                return Branch(Select(value1, CGE_I32, CGE_I64, CGE_F32, CGE_F64), BRTRUE, offset, _compiledModules, label);
            case SyntaxKind.Bge_Un:
                return Branch(Select(value1, CGE_I32_UN, CGE_I64_UN, CGE_F32_UN, CGE_F64_UN), BRTRUE, offset, _compiledModules, label);
            case SyntaxKind.Bgt:
                return Branch(Select(value1, CGT_I32, CGT_I64, CGT_F32, CGT_F64), BRTRUE, offset, _compiledModules, label);
            case SyntaxKind.Bgt_Un:
                return Branch(Select(value1, CGT_I32_UN, CGT_I64_UN, CGT_F32_UN, CGT_F64_UN), BRTRUE, offset, _compiledModules, label);
            case SyntaxKind.Ble:
                return Branch(Select(value1, CLE_I32, CLE_I64, CLE_F32, CLE_F64), BRTRUE, offset, _compiledModules, label);
            case SyntaxKind.Ble_Un:
                return Branch(Select(value1, CLE_I32_UN, CLE_I64_UN, CLE_F32_UN, CLE_F64_UN), BRTRUE, offset, _compiledModules, label);
            case SyntaxKind.Blt:
                return Branch(Select(value1, CLT_I32, CLT_I64, CLT_F32, CLT_F64), BRTRUE, offset, _compiledModules, label);
            case SyntaxKind.Blt_Un:
                return Branch(Select(value1, CLT_I32_UN, CLT_I64_UN, CLT_F32_UN, CLT_F64_UN), BRTRUE, offset, _compiledModules, label);
            case SyntaxKind.Bge_S:
                return BranchShort(Select(value1, CGE_I32, CGE_I64, CGE_F32, CGE_F64), BRTRUE_S, offset, _compiledModules, label);
            case SyntaxKind.Bge_Un_S:
                return BranchShort(Select(value1, CGE_I32_UN, CGE_I64_UN, CGE_F32_UN, CGE_F64_UN), BRTRUE_S, offset, _compiledModules, label);
            case SyntaxKind.Bgt_S:
                return BranchShort(Select(value1, CGT_I32, CGT_I64, CGT_F32, CGT_F64), BRTRUE_S, offset, _compiledModules, label);
            case SyntaxKind.Bgt_Un_S:
                return BranchShort(Select(value1, CGT_I32_UN, CGT_I64_UN, CGT_F32_UN, CGT_F64_UN), BRTRUE_S, offset, _compiledModules, label);
            case SyntaxKind.Ble_S:
                return BranchShort(Select(value1, CLE_I32, CLE_I64, CLE_F32, CLE_F64), BRTRUE_S, offset, _compiledModules, label);
            case SyntaxKind.Ble_Un_S:
                return BranchShort(Select(value1, CLE_I32_UN, CLE_I64_UN, CLE_F32_UN, CLE_F64_UN), BRTRUE_S, offset, _compiledModules, label);
            case SyntaxKind.Blt_S:
                return BranchShort(Select(value1, CLT_I32, CLT_I64, CLT_F32, CLT_F64), BRTRUE_S, offset, _compiledModules, label);
            case SyntaxKind.Blt_Un_S:
                return BranchShort(Select(value1, CLT_I32_UN, CLT_I64_UN, CLT_F32_UN, CLT_F64_UN), BRTRUE_S, offset, _compiledModules, label);
            default:
                throw new Exception();
            }
        }

        public ByteCode Br(BranchInstructionSyntax instruction, int offset)
        {
            var label = _binder.BindLabel(instruction);

            switch (instruction.Kind)
            {
            case SyntaxKind.Br:
                return Branch(BR, offset, _compiledModules, label);
            case SyntaxKind.Br_S:
                return BranchShort(BR_S, offset, _compiledModules, label);
            default:
                throw new Exception();
            }
        }

        public ByteCode Break(NoneInstructionSyntax instruction)
        {
            return new ByteCode(BREAK);
        }

        public ByteCode Brfalse(BranchInstructionSyntax instruction, TypeSymbol value, int offset)
        {
            var label = _binder.BindLabel(instruction);

            switch (instruction.Kind)
            {
            case SyntaxKind.Brfalse:
                return Branch(BRFALSE, offset, _compiledModules, label);
            case SyntaxKind.Brfalse_S:
                return BranchShort(BRFALSE_S, offset, _compiledModules, label);
            default:
                throw new Exception();
            }
        }

        public ByteCode Brtrue(BranchInstructionSyntax instruction, TypeSymbol value, int offset)
        {
            var label = _binder.BindLabel(instruction);

            switch (instruction.Kind)
            {
            case SyntaxKind.Brtrue:
                return Branch(BRTRUE, offset, _compiledModules, label);
            case SyntaxKind.Brtrue_S:
                return BranchShort(BRTRUE_S, offset, _compiledModules, label);
            default:
                throw new Exception();
            }
        }

        public ByteCode Call(FunctionInstructionSyntax instruction)
        {
            var function = _binder.BindFunction(instruction) ?? throw new Exception();
            if (function.IsExtern) throw new Exception();
            var token = GetFunctionToken(function, _binder.BindFunction(_functionDeclaration).ContainingModule, _compiledModules);

            switch (instruction.Kind)
            {
            case SyntaxKind.Call:
                return new ByteCode(CALL, ftn: token);
            default:
                throw new Exception();
            }
        }

        public ByteCode Calli(TypeInstructionSyntax instruction, TypeSymbol function)
        {
            var instructionType = (FunctionTypeSymbol)_binder.BindType(instruction);

            if (!VerifierAssignableTo(function, instructionType)) throw new Exception();

            switch (instruction.Kind)
            {
            case SyntaxKind.Calli:
                return new ByteCode(CALLI);
            default:
                throw new Exception();
            }
        }

        public ByteCode Ceq(NoneInstructionSyntax instruction, TypeSymbol value1, TypeSymbol value2)
        {
            if (value1 != value2) throw new Exception();

            switch (instruction.Kind)
            {
            case SyntaxKind.Ceq:
                return new ByteCode(Select(value1, CEQ_I32, CEQ_I64, CEQ_F32, CEQ_F64, CEQ_I32));
            default:
                throw new Exception();
            }
        }

        public ByteCode Cgt_CgtUn_Clt_CltUn(NoneInstructionSyntax instruction, TypeSymbol value1, TypeSymbol value2)
        {
            if (value1 != value2) throw new Exception();

            switch (instruction.Kind)
            {
            case SyntaxKind.Cgt:
                return new ByteCode(Select(value1, CGT_I32, CGT_I64, CGT_F32, CGT_F64));
            case SyntaxKind.Cgt_Un:
                return new ByteCode(Select(value1, CGT_I32_UN, CGT_I64_UN, CGT_F32_UN, CGT_F64_UN));
            case SyntaxKind.Clt:
                return new ByteCode(Select(value1, CLT_I32, CLT_I64, CLT_F32, CLT_F64));
            case SyntaxKind.Clt_Un:
                return new ByteCode(Select(value1, CLT_I32_UN, CLT_I64_UN, CLT_F32_UN, CLT_F64_UN));
            default:
                throw new Exception();
            }
        }

        public ByteCode Conv_ConvUn(TypeInstructionSyntax instruction, TypeSymbol value)
        {
            var instructionType = _binder.BindType(instruction);

            switch (instructionType.SpecialType)
            {
            case SpecialType.Int8:
                switch (instruction.Kind)
                {
                case SyntaxKind.Conv:
                case SyntaxKind.Conv_Un:
                    return new ByteCode(Select(value, CONV_I8_I32, CONV_I8_I64, CONV_I8_F32, CONV_I8_F64));
                default:
                    throw new Exception();
                }

            case SpecialType.Int16:
                switch (instruction.Kind)
                {
                case SyntaxKind.Conv:
                case SyntaxKind.Conv_Un:
                    return new ByteCode(Select(value, CONV_I16_I32, CONV_I16_I64, CONV_I16_F32, CONV_I16_F64));
                default:
                    throw new Exception();
                }

            case SpecialType.Int32:
                switch (instruction.Kind)
                {
                case SyntaxKind.Conv:
                case SyntaxKind.Conv_Un:
                    return new ByteCode(Select(value, NOP, CONV_I32_I64, CONV_I32_F32, CONV_I32_F64));
                default:
                    throw new Exception();
                }

            case SpecialType.Int64:
                switch (instruction.Kind)
                {
                case SyntaxKind.Conv:
                case SyntaxKind.Conv_Un:
                    return new ByteCode(Select(value, CONV_I64_I32, NOP, CONV_I64_F32, CONV_I64_F64));
                default:
                    throw new Exception();
                }

            case SpecialType.UInt8:
                switch (instruction.Kind)
                {
                case SyntaxKind.Conv:
                case SyntaxKind.Conv_Un:
                    return new ByteCode(Select(value, CONV_U8_I32, CONV_U8_I64, CONV_U8_F32, CONV_U8_F64));
                default:
                    throw new Exception();
                }

            case SpecialType.UInt16:
                switch (instruction.Kind)
                {
                case SyntaxKind.Conv:
                case SyntaxKind.Conv_Un:
                    return new ByteCode(Select(value, CONV_U16_I32, CONV_U16_I64, CONV_U16_F32, CONV_U16_F64));
                default:
                    throw new Exception();
                }

            case SpecialType.UInt32:
                switch (instruction.Kind)
                {
                case SyntaxKind.Conv:
                case SyntaxKind.Conv_Un:
                    return new ByteCode(Select(value, NOP, CONV_U32_I64, CONV_U32_F32, CONV_U32_F64));
                default:
                    throw new Exception();
                }

            case SpecialType.UInt64:
                switch (instruction.Kind)
                {
                case SyntaxKind.Conv:
                case SyntaxKind.Conv_Un:
                    return new ByteCode(Select(value, CONV_U64_I32, NOP, CONV_U64_F32, CONV_U64_F64));
                default:
                    throw new Exception();
                }

            case SpecialType.Float32:
                switch (instruction.Kind)
                {
                case SyntaxKind.Conv:
                    return new ByteCode(Select(value, CONV_F32_I32, CONV_F32_I64, NOP, CONV_F32_F64));
                case SyntaxKind.Conv_Un:
                    return new ByteCode(Select(value, CONV_F32_I32_UN, CONV_F32_I64_UN, NOP, CONV_F32_F64));
                default:
                    throw new Exception();
                }

            case SpecialType.Float64:
                switch (instruction.Kind)
                {
                case SyntaxKind.Conv:
                    return new ByteCode(Select(value, CONV_F64_I32, CONV_F64_I64, CONV_F64_F32, NOP));
                case SyntaxKind.Conv_Un:
                    return new ByteCode(Select(value, CONV_F64_I32_UN, CONV_F64_I64_UN, CONV_F64_F32, NOP));
                default:
                    throw new Exception();
                }

            default:
                throw new Exception();
            }
        }

        public ByteCode Cpobj(TypeInstructionSyntax instruction, TypeSymbol destination, TypeSymbol source)
        {
            var destinationType = GetReferencedType(destination);
            var instructionType = _binder.BindType(instruction);
            var sourceType = GetReferencedType(source);

            if (!VerifierAssignableTo(sourceType, instructionType)) throw new Exception();
            if (!VerifierAssignableTo(instructionType, destinationType)) throw new Exception();

            switch (instruction.Kind)
            {
            case SyntaxKind.Cpobj:
                throw new NotImplementedException();
            default:
                throw new Exception();
            }
        }

        public ByteCode Dup(NoneInstructionSyntax instruction, TypeSymbol value)
        {
            switch (instruction.Kind)
            {
            case SyntaxKind.Dup:
                return new ByteCode(Select(value, DUP_X32, DUP_X64, DUP_X32, DUP_X64, DUP_X32));
            default:
                throw new Exception();
            }
        }

        public ByteCode Initobj(TypeInstructionSyntax instruction, TypeSymbol destination)
        {
            var instructionType = _binder.BindType(instruction);
            var destinationType = GetReferencedType(destination);

            if (!VerifierAssignableTo(instructionType, destinationType)) throw new Exception();

            switch (instruction.Kind)
            {
            case SyntaxKind.Initobj:
                throw new NotImplementedException();
            default:
                throw new Exception();
            }
        }

        public ByteCode Ldarg(ArgumentInstructionSyntax instruction, ImmutableStack<TypeSymbol> stack)
        {
            var parameter = _binder.BindParameter(instruction);
            var slot = FindStackSlot(stack, parameter);

            switch (instruction.Kind)
            {
            case SyntaxKind.Ldarg:
                if (slot < byte.MinValue || slot > byte.MaxValue) throw new Exception();
                return new ByteCode(Select(parameter.Type, LDLOC_I8, LDLOC_I16, LDLOC_X32, LDLOC_X64, LDLOC_U8, LDLOC_U16, LDLOC_X32, LDLOC_X64, LDLOC_X32, LDLOC_X64, LDLOC_X32), u8: (byte)slot);
            default:
                throw new Exception();
            }
        }

        public ByteCode Ldarga(ArgumentInstructionSyntax instruction, ImmutableStack<TypeSymbol> stack)
        {
            var parameter = _binder.BindParameter(instruction);
            var slot = FindStackSlot(stack, parameter);

            switch (instruction.Kind)
            {
            case SyntaxKind.Ldarga:
                if (slot < byte.MinValue || slot > byte.MaxValue) throw new Exception();
                return new ByteCode(LDLOCA, u8: (byte)slot);
            default:
                throw new Exception();
            }
        }

        public ByteCode Ldc(ConstantInstructionSyntax instruction)
        {
            var type = _binder.BindType(instruction);

            if (type.SpecialType != SpecialType.Int32) throw new Exception();

            switch (instruction.Kind)
            {
            case SyntaxKind.Ldc:
                return new ByteCode(LDC_X32, i32: instruction.ConstantValue);
            default:
                throw new Exception();
            }
        }

        public ByteCode Ldelem(TypeInstructionSyntax instruction, TypeSymbol array, TypeSymbol index)
        {
            if (index.SpecialType != SpecialType.Int32) throw new Exception();

            var instructionType = _binder.BindType(instruction);
            var elementType = GetElementType(array);

            if (!VerifierAssignableTo(elementType, instructionType)) throw new Exception();

            switch (instruction.Kind)
            {
            case SyntaxKind.Ldelem:
                return new ByteCode(Select(instructionType, LDELEM_I8, LDELEM_I16, LDELEM_X32, LDELEM_X64, LDELEM_U8, LDELEM_U16, LDELEM_X32, LDELEM_X64, LDELEM_X32, LDELEM_X64, LDELEM_X32));
            default:
                throw new Exception();
            }
        }

        public ByteCode Ldelema(TypeInstructionSyntax instruction, TypeSymbol array, TypeSymbol index)
        {
            if (index.SpecialType != SpecialType.Int32) throw new Exception();

            var instructionType = _binder.BindType(instruction);
            var elementType = GetElementType(array);

            if (!VerifierAssignableTo(elementType, instructionType)) throw new Exception();

            var size = instructionType.TypeLayout.Size;

            switch (instruction.Kind)
            {
            case SyntaxKind.Ldelema:
                if (size < ushort.MinValue || size > ushort.MaxValue) throw new Exception();
                return new ByteCode(LDELEMA, u16: (ushort)size);
            default:
                throw new Exception();
            }
        }

        public ByteCode Ldfld(FieldInstructionSyntax instruction, TypeSymbol obj)
        {
            var field = _binder.BindField(instruction);

            if (!VerifierAssignableTo(obj, field.ContainingType)) throw new Exception();

            var offset = field.FieldOffset;

            switch (instruction.Kind)
            {
            case SyntaxKind.Ldfld:
                if (offset < ushort.MinValue || offset > ushort.MaxValue) throw new Exception();
                return new ByteCode(Select(field.Type, LDFLD_I8, LDFLD_I16, LDFLD_X32, LDFLD_X64, LDFLD_U8, LDFLD_U16, LDFLD_X32, LDFLD_X64, LDFLD_X32, LDFLD_X64, LDFLD_X32), u16: (ushort)offset);
            default:
                throw new Exception();
            }
        }

        public ByteCode Ldflda(FieldInstructionSyntax instruction, TypeSymbol obj)
        {
            var field = _binder.BindField(instruction);

            if (!VerifierAssignableTo(obj, field.ContainingType)) throw new Exception();

            var offset = field.FieldOffset;

            switch (instruction.Kind)
            {
            case SyntaxKind.Ldflda:
                if (offset < ushort.MinValue || offset > ushort.MaxValue) throw new Exception();
                return new ByteCode(LDFLDA, u16: (ushort)offset);
            default:
                throw new Exception();
            }
        }

        public ByteCode Ldftn(FunctionInstructionSyntax instruction)
        {
            var function = _binder.BindFunction(instruction) ?? throw new Exception();
            if (function.IsExtern) throw new Exception();
            var token = GetFunctionToken(function, _binder.BindFunction(_functionDeclaration).ContainingModule, _compiledModules);

            switch (instruction.Kind)
            {
            case SyntaxKind.Ldftn:
                return new ByteCode(LDFTN, ftn: token);
            default:
                throw new Exception();
            }
        }

        public ByteCode Ldind(TypeInstructionSyntax instruction, TypeSymbol address)
        {
            var instructionType = _binder.BindType(instruction);
            var sourceType = GetReferencedType(address);

            if (!VerifierAssignableTo(sourceType, instructionType)) throw new Exception();

            switch (instruction.Kind)
            {
            case SyntaxKind.Ldind:
                return new ByteCode(Select(instructionType, LDFLD_I8, LDFLD_I16, LDFLD_X32, LDFLD_X64, LDFLD_U8, LDFLD_U16, LDFLD_X32, LDFLD_X64, LDFLD_X32, LDFLD_X64, LDFLD_X32), u16: 0);
            default:
                throw new Exception();
            }
        }

        public ByteCode Ldlen(NoneInstructionSyntax instruction, TypeSymbol array)
        {
            var elementType = GetElementType(array);

            switch (instruction.Kind)
            {
            case SyntaxKind.Ldlen:
                return new ByteCode(POP_X32);
            default:
                throw new Exception();
            }
        }

        public ByteCode Ldloc(LocalInstructionSyntax instruction, ImmutableStack<TypeSymbol> stack)
        {
            var local = _binder.BindLocal(instruction);
            var slot = FindStackSlot(stack, local);

            switch (instruction.Kind)
            {
            case SyntaxKind.Ldloc:
                if (slot < byte.MinValue || slot > byte.MaxValue) throw new Exception();
                return new ByteCode(Select(local.Type, LDLOC_I8, LDLOC_I16, LDLOC_X32, LDLOC_X64, LDLOC_U8, LDLOC_U16, LDLOC_X32, LDLOC_X64, LDLOC_X32, LDLOC_X64, LDLOC_X32), u8: (byte)slot);
            default:
                throw new Exception();
            }
        }

        public ByteCode Ldloca(LocalInstructionSyntax instruction, ImmutableStack<TypeSymbol> stack)
        {
            var local = _binder.BindLocal(instruction);
            var slot = FindStackSlot(stack, local);

            switch (instruction.Kind)
            {
            case SyntaxKind.Ldloca:
                if (slot < byte.MinValue || slot > byte.MaxValue) throw new Exception();
                return new ByteCode(LDLOCA, u8: (byte)slot);
            default:
                throw new Exception();
            }
        }

        public ByteCode Ldnull(NoneInstructionSyntax instruction)
        {
            switch (instruction.Kind)
            {
            case SyntaxKind.Ldnull:
                return new ByteCode(LDC_I32_0);
            default:
                throw new Exception();
            }
        }

        public ByteCode Ldobj(TypeInstructionSyntax instruction, TypeSymbol source)
        {
            var instructionType = _binder.BindType(instruction);
            var sourceType = GetReferencedType(source);

            if (!VerifierAssignableTo(sourceType, instructionType)) throw new Exception();

            throw new NotSupportedException();
        }

        public ByteCode Neg(NoneInstructionSyntax instruction, TypeSymbol value)
        {
            switch (instruction.Kind)
            {
            case SyntaxKind.Neg:
                return new ByteCode(Select(value, NEG_I32, NEG_I64, NEG_F32, NEG_F64));
            default:
                throw new Exception();
            }
        }

        public ByteCode Newarr(TypeInstructionSyntax instruction, TypeSymbol length)
        {
            if (length.SpecialType != SpecialType.Int32) throw new Exception();

            var instructionType = _binder.BindType(instruction);

            var size = instructionType.TypeLayout.Size;

            switch (instruction.Kind)
            {
            case SyntaxKind.Newarr:
                if (size < ushort.MinValue || size > ushort.MaxValue) throw new Exception();
                return new ByteCode(NEWARR, u16: (ushort)size);
            default:
                throw new Exception();
            }
        }

        public ByteCode Newobj(FunctionInstructionSyntax instruction)
        {
            var function = _binder.BindFunction(instruction) ?? throw new Exception();
            if (function.IsExtern) throw new Exception();
            var size = function.ReturnType.TypeLayout.Size;
            var token = GetFunctionToken(function, _binder.BindFunction(_functionDeclaration).ContainingModule, _compiledModules);

            switch (instruction.Kind)
            {
            case SyntaxKind.Call:
                if (size < ushort.MinValue || size > ushort.MaxValue) throw new Exception();
                return new ByteCode(NEWOBJ, u16: (ushort)size, CALL, ftn: token);
            default:
                throw new Exception();
            }
        }

        public ByteCode Nop(NoneInstructionSyntax instruction)
        {
            switch (instruction.Kind)
            {
            case SyntaxKind.Nop:
                return new ByteCode(NOP);
            default:
                throw new Exception();
            }
        }

        public ByteCode Not(NoneInstructionSyntax instruction, TypeSymbol value)
        {
            switch (instruction.Kind)
            {
            case SyntaxKind.Not:
                return new ByteCode(Select(value, NOT_I32, NOT_I64));
            default:
                throw new Exception();
            }
        }

        public ByteCode Pop(NoneInstructionSyntax instruction, TypeSymbol value)
        {
            switch (instruction.Kind)
            {
            case SyntaxKind.Pop:
                return new ByteCode(Select(value, POP_X32, POP_X64, POP_X32, POP_X64, POP_X32));
            default:
                throw new Exception();
            }
        }

        public ByteCode Ret(NoneInstructionSyntax instruction)
        {
            var returnType = _binder.BindFunction(_functionDeclaration).ReturnType;

            if (returnType.SpecialType != SpecialType.Void) throw new Exception();

            switch (instruction.Kind)
            {
            case SyntaxKind.Ret:
                return new ByteCode(RET);
            default:
                throw new Exception();
            }
        }

        public ByteCode Ret(NoneInstructionSyntax instruction, TypeSymbol value)
        {
            var returnType = _binder.BindFunction(_functionDeclaration).ReturnType;

            if (returnType.SpecialType == SpecialType.Void) throw new Exception();
            if (!VerifierAssignableTo(value, returnType)) throw new Exception();

            switch (instruction.Kind)
            {
            case SyntaxKind.Ret:
                return new ByteCode(Select(value, RET_X32, RET_X64, RET_X32, RET_X64, RET_X32));
            default:
                throw new Exception();
            }
        }

        public ByteCode Shl_Shr_ShrUn(NoneInstructionSyntax instruction, TypeSymbol value, TypeSymbol amount)
        {
            if (amount.SpecialType != SpecialType.Int32) throw new Exception();

            switch (instruction.Kind)
            {
            case SyntaxKind.Shl:
                return new ByteCode(Select(value, SHL_I32, SHL_I64));
            case SyntaxKind.Shr:
                return new ByteCode(Select(value, SHR_I32, SHR_I64));
            case SyntaxKind.Shr_Un:
                return new ByteCode(Select(value, SHR_I32_UN, SHR_I64_UN));
            default:
                throw new Exception();
            }
        }

        public ByteCode Starg(ArgumentInstructionSyntax instruction, TypeSymbol value, ImmutableStack<TypeSymbol> stack)
        {
            var parameter = _binder.BindParameter(instruction);
            var slot = FindStackSlot(stack, parameter);

            if (!VerifierAssignableTo(value, parameter.Type)) throw new Exception();

            switch (instruction.Kind)
            {
            case SyntaxKind.Starg:
                if (slot < byte.MinValue || slot > byte.MaxValue) throw new Exception();
                return new ByteCode(Select(parameter.Type, STLOC_X32, STLOC_X64, STLOC_X32, STLOC_X64, STLOC_X32), u8: (byte)slot);
            default:
                throw new Exception();
            }
        }

        public ByteCode Stelem(TypeInstructionSyntax instruction, TypeSymbol array, TypeSymbol index, TypeSymbol value)
        {
            if (index.SpecialType != SpecialType.Int32) throw new Exception();

            var instructionType = _binder.BindType(instruction);
            var elementType = GetElementType(array);

            if (!VerifierAssignableTo(value, instructionType)) throw new Exception();
            if (!VerifierAssignableTo(instructionType, elementType)) throw new Exception();

            switch (instruction.Kind)
            {
            case SyntaxKind.Stelem:
                return new ByteCode(Select(instructionType, STELEM_X8, STELEM_X16, STELEM_X32, STELEM_X64, STELEM_X8, STELEM_X16, STELEM_X32, STELEM_X64, STELEM_X32, STELEM_X64, STELEM_X32));
            default:
                throw new Exception();
            }
        }

        public ByteCode Stfld(FieldInstructionSyntax instruction, TypeSymbol obj, TypeSymbol value)
        {
            var field = _binder.BindField(instruction);

            if (!VerifierAssignableTo(obj, field.ContainingType)) throw new Exception();
            if (!VerifierAssignableTo(value, field.Type)) throw new Exception();

            var offset = _binder.BindField(instruction).FieldOffset;

            switch (instruction.Kind)
            {
            case SyntaxKind.Stfld:
                if (offset < ushort.MinValue || offset > ushort.MaxValue) throw new Exception();
                return new ByteCode(Select(field.Type, STFLD_X8, STFLD_X16, STFLD_X32, STFLD_X64, STFLD_X8, STFLD_X16, STFLD_X32, STFLD_X64, STFLD_X32, STFLD_X64, STFLD_X32), u16: (ushort)offset);
            default:
                throw new Exception();
            }
        }

        public ByteCode Stind(TypeInstructionSyntax instruction, TypeSymbol address, TypeSymbol value)
        {
            var destinationType = GetReferencedType(address);
            var instructionType = _binder.BindType(instruction);
            var sourceType = value;

            if (!VerifierAssignableTo(sourceType, instructionType)) throw new Exception();
            if (!VerifierAssignableTo(instructionType, destinationType)) throw new Exception();

            switch (instruction.Kind)
            {
            case SyntaxKind.Stind:
                return new ByteCode(Select(instructionType, STFLD_X8, STFLD_X16, STFLD_X32, STFLD_X64, STFLD_X8, STFLD_X16, STFLD_X32, STFLD_X64, STFLD_X32, STFLD_X64, STFLD_X32), u16: 0);
            default:
                throw new Exception();
            }
        }

        public ByteCode Stloc(LocalInstructionSyntax instruction, TypeSymbol value, ImmutableStack<TypeSymbol> stack)
        {
            var local = _binder.BindLocal(instruction);
            var slot = FindStackSlot(stack, local);

            if (!VerifierAssignableTo(value, local.Type)) throw new Exception();

            switch (instruction.Kind)
            {
            case SyntaxKind.Stloc:
                if (slot < byte.MinValue || slot > byte.MaxValue) throw new Exception();
                return new ByteCode(Select(local.Type, STLOC_X32, STLOC_X64, STLOC_X32, STLOC_X64, STLOC_X32), u8: (byte)slot);
            default:
                throw new Exception();
            }
        }

        public ByteCode Stobj(TypeInstructionSyntax instruction, TypeSymbol destination, TypeSymbol source)
        {
            var destinationType = GetReferencedType(destination);
            var instructionType = _binder.BindType(instruction);
            var sourceType = source;

            if (!VerifierAssignableTo(sourceType, instructionType)) throw new Exception();
            if (!VerifierAssignableTo(instructionType, destinationType)) throw new Exception();

            throw new NotSupportedException();
        }

        public ByteCode Syscall(FunctionInstructionSyntax instruction)
        {
            var function = _binder.BindFunction(instruction) ?? throw new Exception();
            var adjustment = GetStackAdjustment(function);
            var ordinal = function.GetSystemCallOrdinal();

            if (!function.IsExtern) throw new Exception();

            switch (instruction.Kind)
            {
            case SyntaxKind.Syscall:
                if (adjustment < sbyte.MinValue || adjustment > sbyte.MaxValue) throw new Exception();
                if (ordinal < ushort.MinValue || ordinal > ushort.MaxValue) throw new Exception();
                return new ByteCode(SYSCALL, i8: (sbyte)adjustment, u16: (ushort)ordinal);
            default:
                throw new Exception();
            }
        }

        private static ByteCode Branch(mango_opcode opcode, int position, CompiledModules compiledModules, LabelSymbol label)
        {
            var target = compiledModules != null ? compiledModules.GetCompiledModuleFromSymbol(label.ContainingModule).GetLabelOffset(label) - (position + new ByteCode(opcode, i16: 0).Length) : 0;
            if (target < short.MinValue || target > short.MaxValue) throw new Exception();
            return new ByteCode(opcode, i16: (short)target);
        }

        private static ByteCode Branch(mango_opcode opcode1, mango_opcode opcode2, int position, CompiledModules compiledModules, LabelSymbol label)
        {
            var target = compiledModules != null ? compiledModules.GetCompiledModuleFromSymbol(label.ContainingModule).GetLabelOffset(label) - (position + new ByteCode(opcode1, opcode2, i16: 0).Length) : 0;
            if (target < short.MinValue || target > short.MaxValue) throw new Exception();
            return new ByteCode(opcode1, opcode2, i16: (short)target);
        }

        private static ByteCode BranchShort(mango_opcode opcode, int position, CompiledModules compiledModules, LabelSymbol label)
        {
            var target = compiledModules != null ? compiledModules.GetCompiledModuleFromSymbol(label.ContainingModule).GetLabelOffset(label) - (position + new ByteCode(opcode, i8: 0).Length) : 0;
            if (target < sbyte.MinValue || target > sbyte.MaxValue) throw new Exception();
            return new ByteCode(opcode, i8: (sbyte)target);
        }

        private static ByteCode BranchShort(mango_opcode opcode1, mango_opcode opcode2, int position, CompiledModules compiledModules, LabelSymbol label)
        {
            var target = compiledModules != null ? compiledModules.GetCompiledModuleFromSymbol(label.ContainingModule).GetLabelOffset(label) - (position + new ByteCode(opcode1, opcode2, i8: 0).Length) : 0;
            if (target < sbyte.MinValue || target > sbyte.MaxValue) throw new Exception();
            return new ByteCode(opcode1, opcode2, i8: (sbyte)target);
        }

        private static int FindStackSlot(ImmutableStack<TypeSymbol> stack, LocalSymbol local)
        {
            var function = (FunctionSymbol)local.ContainingSymbol;
            var slot = 0;
            foreach (var type in stack)
                slot += (type.TypeLayout.Size + 3) / 4;
            foreach (var l in function.Locals.Reverse())
                if (l == local) return slot; else slot += (l.Type.TypeLayout.Size + 3) / 4;
            throw new Exception();
        }

        private static int FindStackSlot(ImmutableStack<TypeSymbol> stack, ParameterSymbol parameter)
        {
            var function = (FunctionSymbol)parameter.ContainingSymbol;
            var slot = 0;
            foreach (var type in stack)
                slot += (type.TypeLayout.Size + 3) / 4;
            foreach (var local in function.Locals)
                slot += (local.Type.TypeLayout.Size + 3) / 4;
            foreach (var p in function.Parameters.Reverse())
                if (p == parameter) return slot; else slot += (p.Type.TypeLayout.Size + 3) / 4;
            throw new Exception();
        }

        private static TypeSymbol GetElementType(TypeSymbol type)
        {
            return type is SpanTypeSymbol span ? span.ElementType : throw new Exception();
        }

        private static mango_function_token GetFunctionToken(FunctionSymbol function, ModuleSymbol currentModule, CompiledModules compiledModules)
        {
            const int CurrentModuleSentinel = byte.MaxValue;

            var moduleIndex = 0;
            var functionOffset = 0;

            if (compiledModules != null)
            {
                if (function.ContainingModule == currentModule)
                {
                    moduleIndex = CurrentModuleSentinel;
                }
                else
                {
                    moduleIndex = compiledModules.GetCompiledModuleFromSymbol(currentModule).GetReferencedModuleIndex(function.ContainingModule);
                    if (moduleIndex == CurrentModuleSentinel) throw new Exception();
                }

                functionOffset = compiledModules.GetCompiledModuleFromSymbol(function.ContainingModule).GetFunctionOffset(function);
            }

            if (moduleIndex < byte.MinValue || moduleIndex > byte.MaxValue) throw new Exception();
            if (functionOffset < ushort.MinValue || functionOffset > ushort.MaxValue) throw new Exception();

            return new mango_function_token { module = (byte)moduleIndex, offset = (ushort)functionOffset };
        }

        private static TypeSymbol GetReferencedType(TypeSymbol type)
        {
            return type is ReferenceTypeSymbol reference ? reference.ReferencedType : throw new Exception();
        }

        private static int GetStackAdjustment(FunctionSymbol function)
        {
            var adjustment = 0;
            foreach (var parameter in function.Parameters)
                adjustment += (parameter.Type.TypeLayout.Size + 3) / 4;
            if (!function.ReturnsVoid)
                adjustment -= (function.ReturnType.TypeLayout.Size + 3) / 4;
            return adjustment;
        }

        private static mango_opcode Select(TypeSymbol type, mango_opcode i32, mango_opcode i64)
        {
            return type.SpecialType == SpecialType.Int32 ? i32 :
                   type.SpecialType == SpecialType.Int64 ? i64 :
                   throw new NotSupportedException();
        }

        private static mango_opcode Select(TypeSymbol type, mango_opcode i32, mango_opcode i64, mango_opcode reference)
        {
            return type.SpecialType == SpecialType.Int32 ? i32 :
                   type.SpecialType == SpecialType.Int64 ? throw new NotSupportedException() :
                   type.TypeKind == TypeKind.Reference ? reference :
                   throw new NotSupportedException();
        }

        private static mango_opcode Select(TypeSymbol type, mango_opcode i32, mango_opcode i64, mango_opcode f32, mango_opcode f64)
        {
            return type.SpecialType == SpecialType.Int32 ? i32 :
                   type.SpecialType == SpecialType.Int64 ? i64 :
                   type.SpecialType == SpecialType.Float32 ? f32 :
                   type.SpecialType == SpecialType.Float64 ? f64 :
                   throw new NotSupportedException();
        }

        private static mango_opcode Select(TypeSymbol type, mango_opcode i32, mango_opcode i64, mango_opcode f32, mango_opcode f64, mango_opcode reference)
        {
            return type.SpecialType == SpecialType.Int32 ? i32 :
                   type.SpecialType == SpecialType.Int64 ? i64 :
                   type.SpecialType == SpecialType.Float32 ? f32 :
                   type.SpecialType == SpecialType.Float64 ? f64 :
                   type.TypeKind == TypeKind.Reference ? reference :
                   throw new NotSupportedException();
        }

        private static mango_opcode Select(TypeSymbol type, mango_opcode i8, mango_opcode i16, mango_opcode i32, mango_opcode i64, mango_opcode u8, mango_opcode u16, mango_opcode u32, mango_opcode u64, mango_opcode f32, mango_opcode f64, mango_opcode reference)
        {
            return type.SpecialType == SpecialType.Int8 ? i8 :
                   type.SpecialType == SpecialType.Int16 ? i16 :
                   type.SpecialType == SpecialType.Int32 ? i32 :
                   type.SpecialType == SpecialType.Int64 ? i64 :
                   type.SpecialType == SpecialType.UInt8 ? u8 :
                   type.SpecialType == SpecialType.UInt16 ? u16 :
                   type.SpecialType == SpecialType.UInt32 ? u32 :
                   type.SpecialType == SpecialType.UInt64 ? u64 :
                   type.SpecialType == SpecialType.Float32 ? f32 :
                   type.SpecialType == SpecialType.Float64 ? f64 :
                   type.TypeKind == TypeKind.Reference ? reference :
                   throw new NotSupportedException();
        }
    }
}
