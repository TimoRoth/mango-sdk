using System;
using Mango.Compiler.Symbols;
using Mango.Compiler.Verification;
using static Interop.Libmango;
using static Interop.Libmango.mango_opcode;
using static Mango.Compiler.Emit.CodeGeneration;

namespace Mango.Compiler.Emit
{
    partial class Emitter
    {
        private readonly struct Generator
        {
            private readonly EmittingModule _emittingModule;
            private readonly EmittingModules _emittingModules;
            private readonly VerifiedFunction _function;

            public Generator(VerifiedFunction function, EmittingModule emittingModule, EmittingModules emittingModules)
            {
                _function = function;
                _emittingModule = emittingModule;
                _emittingModules = emittingModules;
            }

            public ByteCode VisitArgument(ArgumentInstruction instruction)
            {
                var slot = CalculateStackSlot(instruction.Stack, instruction.Parameter);

                if (slot < byte.MinValue || slot > byte.MaxValue) throw new Exception();

                switch (instruction.Kind)
                {
                case InstructionKind.Ldarg:
                    return new ByteCode(Select(instruction.Parameter.Type, LDLOC_I8, LDLOC_I16, LDLOC_X32, LDLOC_X64, LDLOC_U8, LDLOC_U16, LDLOC_X32, LDLOC_X64, LDLOC_X32, LDLOC_X64, LDLOC_X32), u8: (byte)slot);
                case InstructionKind.Ldarga:
                    return new ByteCode(LDLOCA, u8: (byte)slot);
                case InstructionKind.Starg:
                    return new ByteCode(Select(instruction.Parameter.Type, STLOC_X32, STLOC_X64, STLOC_X32, STLOC_X64, STLOC_X32), u8: (byte)slot);
                default:
                    throw new Exception();
                }
            }

            public ByteCode VisitConditionalBranch(ConditionalBranchInstruction instruction, int offset)
            {
                switch (instruction.Kind)
                {
                case InstructionKind.Beq:
                    return GetLongBranch(Select(instruction.Type, CEQ_I32, CEQ_I64, CEQ_F32, CEQ_F64, CEQ_I32), BRTRUE, offset, instruction.Label);
                case InstructionKind.Bge:
                    return GetLongBranch(Select(instruction.Type, CGE_I32, CGE_I64, CGE_F32, CGE_F64), BRTRUE, offset, instruction.Label);
                case InstructionKind.Bge_Un:
                    return GetLongBranch(Select(instruction.Type, CGE_I32_UN, CGE_I64_UN, CGE_F32_UN, CGE_F64_UN), BRTRUE, offset, instruction.Label);
                case InstructionKind.Bgt:
                    return GetLongBranch(Select(instruction.Type, CGT_I32, CGT_I64, CGT_F32, CGT_F64), BRTRUE, offset, instruction.Label);
                case InstructionKind.Bgt_Un:
                    return GetLongBranch(Select(instruction.Type, CGT_I32_UN, CGT_I64_UN, CGT_F32_UN, CGT_F64_UN), BRTRUE, offset, instruction.Label);
                case InstructionKind.Ble:
                    return GetLongBranch(Select(instruction.Type, CLE_I32, CLE_I64, CLE_F32, CLE_F64), BRTRUE, offset, instruction.Label);
                case InstructionKind.Ble_Un:
                    return GetLongBranch(Select(instruction.Type, CLE_I32_UN, CLE_I64_UN, CLE_F32_UN, CLE_F64_UN), BRTRUE, offset, instruction.Label);
                case InstructionKind.Blt:
                    return GetLongBranch(Select(instruction.Type, CLT_I32, CLT_I64, CLT_F32, CLT_F64), BRTRUE, offset, instruction.Label);
                case InstructionKind.Blt_Un:
                    return GetLongBranch(Select(instruction.Type, CLT_I32_UN, CLT_I64_UN, CLT_F32_UN, CLT_F64_UN), BRTRUE, offset, instruction.Label);
                case InstructionKind.Bne_Un:
                    return GetLongBranch(Select(instruction.Type, CEQ_I32, CEQ_I64, CEQ_F32, CEQ_F64, CEQ_I32), BRFALSE, offset, instruction.Label);
                case InstructionKind.Brfalse:
                    return GetLongBranch(Select(instruction.Type, BRFALSE), offset, instruction.Label);
                case InstructionKind.Brtrue:
                    return GetLongBranch(Select(instruction.Type, BRTRUE), offset, instruction.Label);

                case InstructionKind.Beq_S:
                    return GetShortBranch(Select(instruction.Type, CEQ_I32, CEQ_I64, CEQ_F32, CEQ_F64, CEQ_I32), BRTRUE_S, offset, instruction.Label);
                case InstructionKind.Bge_S:
                    return GetShortBranch(Select(instruction.Type, CGE_I32, CGE_I64, CGE_F32, CGE_F64), BRTRUE_S, offset, instruction.Label);
                case InstructionKind.Bge_Un_S:
                    return GetShortBranch(Select(instruction.Type, CGE_I32_UN, CGE_I64_UN, CGE_F32_UN, CGE_F64_UN), BRTRUE_S, offset, instruction.Label);
                case InstructionKind.Bgt_S:
                    return GetShortBranch(Select(instruction.Type, CGT_I32, CGT_I64, CGT_F32, CGT_F64), BRTRUE_S, offset, instruction.Label);
                case InstructionKind.Bgt_Un_S:
                    return GetShortBranch(Select(instruction.Type, CGT_I32_UN, CGT_I64_UN, CGT_F32_UN, CGT_F64_UN), BRTRUE_S, offset, instruction.Label);
                case InstructionKind.Ble_S:
                    return GetShortBranch(Select(instruction.Type, CLE_I32, CLE_I64, CLE_F32, CLE_F64), BRTRUE_S, offset, instruction.Label);
                case InstructionKind.Ble_Un_S:
                    return GetShortBranch(Select(instruction.Type, CLE_I32_UN, CLE_I64_UN, CLE_F32_UN, CLE_F64_UN), BRTRUE_S, offset, instruction.Label);
                case InstructionKind.Blt_S:
                    return GetShortBranch(Select(instruction.Type, CLT_I32, CLT_I64, CLT_F32, CLT_F64), BRTRUE_S, offset, instruction.Label);
                case InstructionKind.Blt_Un_S:
                    return GetShortBranch(Select(instruction.Type, CLT_I32_UN, CLT_I64_UN, CLT_F32_UN, CLT_F64_UN), BRTRUE_S, offset, instruction.Label);
                case InstructionKind.Bne_Un_S:
                    return GetShortBranch(Select(instruction.Type, CEQ_I32, CEQ_I64, CEQ_F32, CEQ_F64, CEQ_I32), BRFALSE_S, offset, instruction.Label);
                case InstructionKind.Brfalse_S:
                    return GetShortBranch(Select(instruction.Type, BRFALSE_S), offset, instruction.Label);
                case InstructionKind.Brtrue_S:
                    return GetShortBranch(Select(instruction.Type, BRTRUE_S), offset, instruction.Label);

                default:
                    throw new Exception();
                }
            }

            public ByteCode VisitConstant(ConstantInstruction instruction)
            {
                switch (instruction.Type.SpecialType)
                {
                case SpecialType.Int32:
                    switch (instruction.Kind)
                    {
                    case InstructionKind.Ldc:
                        return new ByteCode(LDC_X32, i32: (int)instruction.Value);
                    default:
                        throw new Exception();
                    }

                default:
                    throw new Exception();
                }
            }

            public ByteCode VisitConversion(ConversionInstruction instruction)
            {
                switch (instruction.ToType.SpecialType)
                {
                case SpecialType.Int8:
                    switch (instruction.Kind)
                    {
                    case InstructionKind.Conv:
                    case InstructionKind.Conv_Un:
                        return new ByteCode(Select(instruction.FromType, CONV_I8_I32, CONV_I8_I64, CONV_I8_F32, CONV_I8_F64));
                    default:
                        throw new Exception();
                    }

                case SpecialType.Int16:
                    switch (instruction.Kind)
                    {
                    case InstructionKind.Conv:
                    case InstructionKind.Conv_Un:
                        return new ByteCode(Select(instruction.FromType, CONV_I16_I32, CONV_I16_I64, CONV_I16_F32, CONV_I16_F64));
                    default:
                        throw new Exception();
                    }

                case SpecialType.Int32:
                    switch (instruction.Kind)
                    {
                    case InstructionKind.Conv:
                    case InstructionKind.Conv_Un:
                        return new ByteCode(Select(instruction.FromType, NOP, CONV_I32_I64, CONV_I32_F32, CONV_I32_F64));
                    default:
                        throw new Exception();
                    }

                case SpecialType.Int64:
                    switch (instruction.Kind)
                    {
                    case InstructionKind.Conv:
                    case InstructionKind.Conv_Un:
                        return new ByteCode(Select(instruction.FromType, CONV_I64_I32, NOP, CONV_I64_F32, CONV_I64_F64));
                    default:
                        throw new Exception();
                    }

                case SpecialType.UInt8:
                    switch (instruction.Kind)
                    {
                    case InstructionKind.Conv:
                    case InstructionKind.Conv_Un:
                        return new ByteCode(Select(instruction.FromType, CONV_U8_I32, CONV_U8_I64, CONV_U8_F32, CONV_U8_F64));
                    default:
                        throw new Exception();
                    }

                case SpecialType.UInt16:
                    switch (instruction.Kind)
                    {
                    case InstructionKind.Conv:
                    case InstructionKind.Conv_Un:
                        return new ByteCode(Select(instruction.FromType, CONV_U16_I32, CONV_U16_I64, CONV_U16_F32, CONV_U16_F64));
                    default:
                        throw new Exception();
                    }

                case SpecialType.UInt32:
                    switch (instruction.Kind)
                    {
                    case InstructionKind.Conv:
                    case InstructionKind.Conv_Un:
                        return new ByteCode(Select(instruction.FromType, NOP, CONV_U32_I64, CONV_U32_F32, CONV_U32_F64));
                    default:
                        throw new Exception();
                    }

                case SpecialType.UInt64:
                    switch (instruction.Kind)
                    {
                    case InstructionKind.Conv:
                    case InstructionKind.Conv_Un:
                        return new ByteCode(Select(instruction.FromType, CONV_U64_I32, NOP, CONV_U64_F32, CONV_U64_F64));
                    default:
                        throw new Exception();
                    }

                case SpecialType.Float32:
                    switch (instruction.Kind)
                    {
                    case InstructionKind.Conv:
                        return new ByteCode(Select(instruction.FromType, CONV_F32_I32, CONV_F32_I64, NOP, CONV_F32_F64));
                    case InstructionKind.Conv_Un:
                        return new ByteCode(Select(instruction.FromType, CONV_F32_I32_UN, CONV_F32_I64_UN, NOP, CONV_F32_F64));
                    default:
                        throw new Exception();
                    }

                case SpecialType.Float64:
                    switch (instruction.Kind)
                    {
                    case InstructionKind.Conv:
                        return new ByteCode(Select(instruction.FromType, CONV_F64_I32, CONV_F64_I64, CONV_F64_F32, NOP));
                    case InstructionKind.Conv_Un:
                        return new ByteCode(Select(instruction.FromType, CONV_F64_I32_UN, CONV_F64_I64_UN, CONV_F64_F32, NOP));
                    default:
                        throw new Exception();
                    }

                default:
                    throw new Exception();
                }
            }

            public ByteCode VisitField(FieldInstruction instruction)
            {
                var offset = instruction.Field.FieldOffset;

                if (offset < ushort.MinValue || offset > ushort.MaxValue) throw new Exception();

                switch (instruction.Kind)
                {
                case InstructionKind.Ldfld:
                    return new ByteCode(Select(instruction.Field.Type, LDFLD_I8, LDFLD_I16, LDFLD_X32, LDFLD_X64, LDFLD_U8, LDFLD_U16, LDFLD_X32, LDFLD_X64, LDFLD_X32, LDFLD_X64, LDFLD_X32), u16: (ushort)offset);
                case InstructionKind.Ldflda:
                    return new ByteCode(LDFLDA, u16: (ushort)offset);
                case InstructionKind.Stfld:
                    return new ByteCode(Select(instruction.Field.Type, STFLD_X8, STFLD_X16, STFLD_X32, STFLD_X64, STFLD_X8, STFLD_X16, STFLD_X32, STFLD_X64, STFLD_X32, STFLD_X64, STFLD_X32), u16: (ushort)offset);
                default:
                    throw new Exception();
                }
            }

            public ByteCode VisitFunction(FunctionInstruction instruction)
            {
                var functionToken = GetFunctionToken(instruction.Function);

                switch (instruction.Kind)
                {
                case InstructionKind.Call when instruction.Function.ContainingModule == _function.Symbol.ContainingModule:
                    return new ByteCode(CALL_S, u16: functionToken.offset);
                case InstructionKind.Call:
                    return new ByteCode(CALL, ftn: functionToken);
                case InstructionKind.Ldftn:
                    return new ByteCode(LDFTN, ftn: functionToken);
                case InstructionKind.Syscall when instruction.Function.ContainingModule == _function.Symbol.ContainingModule:
                    return GetSystemCall(instruction);
                case InstructionKind.Syscall:
                    return new ByteCode(CALL, ftn: functionToken);
                default:
                    throw new Exception();
                }
            }

            public ByteCode VisitLocal(LocalInstruction instruction)
            {
                var slot = CalculateStackSlot(instruction.Stack, instruction.Local);

                if (slot < byte.MinValue || slot > byte.MaxValue) throw new Exception();

                switch (instruction.Kind)
                {
                case InstructionKind.Ldloc:
                    return new ByteCode(Select(instruction.Local.Type, LDLOC_I8, LDLOC_I16, LDLOC_X32, LDLOC_X64, LDLOC_U8, LDLOC_U16, LDLOC_X32, LDLOC_X64, LDLOC_X32, LDLOC_X64, LDLOC_X32), u8: (byte)slot);
                case InstructionKind.Ldloca:
                    return new ByteCode(LDLOCA, u8: (byte)slot);
                case InstructionKind.Stloc:
                    return new ByteCode(Select(instruction.Local.Type, STLOC_X32, STLOC_X64, STLOC_X32, STLOC_X64, STLOC_X32), u8: (byte)slot);
                default:
                    throw new Exception();
                }
            }

            public ByteCode VisitNone(NoneInstruction instruction)
            {
                switch (instruction.Kind)
                {
                case InstructionKind.Break:
                    return new ByteCode(BREAK);
                case InstructionKind.Ldlen:
                    return new ByteCode(POP_X32);
                case InstructionKind.Ldnull:
                    return new ByteCode(LDC_I32_0);
                case InstructionKind.Nop:
                    return new ByteCode(NOP);
                case InstructionKind.Ret:
                    return new ByteCode(RET);
                default:
                    throw new Exception();
                }
            }

            public ByteCode VisitTyped(TypedInstruction instruction)
            {
                var size = instruction.Type.TypeLayout.Size;

                if (size < ushort.MinValue || size > ushort.MaxValue) throw new Exception();

                switch (instruction.Kind)
                {
                case InstructionKind.Dup:
                    return new ByteCode(Select(instruction.Type, DUP_X32, DUP_X64, DUP_X32, DUP_X64, DUP_X32));
                case InstructionKind.Pop:
                    return new ByteCode(Select(instruction.Type, POP_X32, POP_X64, POP_X32, POP_X64, POP_X32));
                case InstructionKind.Ret:
                    return new ByteCode(Select(instruction.Type, RET_X32, RET_X64, RET_X32, RET_X64, RET_X32));

                case InstructionKind.Add:
                    return new ByteCode(Select(instruction.Type, ADD_I32, ADD_I64, ADD_F32, ADD_F64));
                case InstructionKind.Div:
                    return new ByteCode(Select(instruction.Type, DIV_I32, DIV_I64, DIV_F32, DIV_F64));
                case InstructionKind.Div_Un:
                    return new ByteCode(Select(instruction.Type, DIV_I32_UN, DIV_I64_UN, DIV_F32, DIV_F64));
                case InstructionKind.Mul:
                    return new ByteCode(Select(instruction.Type, MUL_I32, MUL_I64, MUL_F32, MUL_F64));
                case InstructionKind.Rem:
                    return new ByteCode(Select(instruction.Type, REM_I32, REM_I64, REM_F32, REM_F64));
                case InstructionKind.Rem_Un:
                    return new ByteCode(Select(instruction.Type, REM_I32_UN, REM_I64_UN, REM_F32, REM_F64));
                case InstructionKind.Sub:
                    return new ByteCode(Select(instruction.Type, SUB_I32, SUB_I64, SUB_F32, SUB_F64));

                case InstructionKind.Neg:
                    return new ByteCode(Select(instruction.Type, NEG_I32, NEG_I64, NEG_F32, NEG_F64));

                case InstructionKind.And:
                    return new ByteCode(Select(instruction.Type, AND_I32, AND_I64));
                case InstructionKind.Or:
                    return new ByteCode(Select(instruction.Type, OR_I32, OR_I64));
                case InstructionKind.Xor:
                    return new ByteCode(Select(instruction.Type, XOR_I32, XOR_I64));

                case InstructionKind.Shl:
                    return new ByteCode(Select(instruction.Type, SHL_I32, SHL_I64));
                case InstructionKind.Shr:
                    return new ByteCode(Select(instruction.Type, SHR_I32, SHR_I64));
                case InstructionKind.Shr_Un:
                    return new ByteCode(Select(instruction.Type, SHR_I32_UN, SHR_I64_UN));

                case InstructionKind.Not:
                    return new ByteCode(Select(instruction.Type, NOT_I32, NOT_I64));

                case InstructionKind.Ceq:
                    return new ByteCode(Select(instruction.Type, CEQ_I32, CEQ_I64, CEQ_F32, CEQ_F64, CEQ_I32));
                case InstructionKind.Cgt:
                    return new ByteCode(Select(instruction.Type, CGT_I32, CGT_I64, CGT_F32, CGT_F64));
                case InstructionKind.Cgt_Un:
                    return new ByteCode(Select(instruction.Type, CGT_I32_UN, CGT_I64_UN, CGT_F32_UN, CGT_F64_UN));
                case InstructionKind.Clt:
                    return new ByteCode(Select(instruction.Type, CLT_I32, CLT_I64, CLT_F32, CLT_F64));
                case InstructionKind.Clt_Un:
                    return new ByteCode(Select(instruction.Type, CLT_I32_UN, CLT_I64_UN, CLT_F32_UN, CLT_F64_UN));

                case InstructionKind.Calli:
                    if (instruction.Type.Kind != SymbolKind.FunctionType) throw new Exception();
                    return new ByteCode(CALLI);

                case InstructionKind.Ldind:
                    return new ByteCode(Select(instruction.Type, LDFLD_I8, LDFLD_I16, LDFLD_X32, LDFLD_X64, LDFLD_U8, LDFLD_U16, LDFLD_X32, LDFLD_X64, LDFLD_X32, LDFLD_X64, LDFLD_X32), u16: 0);
                case InstructionKind.Stind:
                    return new ByteCode(Select(instruction.Type, STFLD_X8, STFLD_X16, STFLD_X32, STFLD_X64, STFLD_X8, STFLD_X16, STFLD_X32, STFLD_X64, STFLD_X32, STFLD_X64, STFLD_X32), u16: 0);

                case InstructionKind.Ldelem:
                    return new ByteCode(Select(instruction.Type, LDELEM_I8, LDELEM_I16, LDELEM_X32, LDELEM_X64, LDELEM_U8, LDELEM_U16, LDELEM_X32, LDELEM_X64, LDELEM_X32, LDELEM_X64, LDELEM_X32));
                case InstructionKind.Ldelema:
                    return new ByteCode(LDELEMA, u16: (ushort)size);
                case InstructionKind.Stelem:
                    return new ByteCode(Select(instruction.Type, STELEM_X8, STELEM_X16, STELEM_X32, STELEM_X64, STELEM_X8, STELEM_X16, STELEM_X32, STELEM_X64, STELEM_X32, STELEM_X64, STELEM_X32));

                case InstructionKind.Newarr:
                    return new ByteCode(NEWARR, u16: (ushort)size);

                default:
                    throw new Exception();
                }
            }

            public ByteCode VisitUnconditionalBranch(UnconditionalBranchInstruction instruction, int offset)
            {
                switch (instruction.Kind)
                {
                case InstructionKind.Br:
                    return GetLongBranch(BR, offset, instruction.Label);
                case InstructionKind.Br_S:
                    return GetShortBranch(BR_S, offset, instruction.Label);
                default:
                    throw new Exception();
                }
            }

            private mango_function_token GetFunctionToken(FunctionSymbol function)
            {
                if (_emittingModules == null) return default;

                var moduleIndex = GetImportIndex(function);
                var functionOffset = _emittingModules.GetFunctionOffset(function);

                if (moduleIndex < byte.MinValue || moduleIndex > byte.MaxValue) throw new Exception();
                if (functionOffset < ushort.MinValue || functionOffset > ushort.MaxValue) throw new Exception();

                return new mango_function_token { module = (byte)moduleIndex, offset = (ushort)functionOffset };
            }

            private int GetImportIndex(FunctionSymbol function)
            {
                const int CurrentModuleSentinel = byte.MaxValue;

                if (function.ContainingModule == _function.Symbol.ContainingModule)
                {
                    return CurrentModuleSentinel;
                }
                else
                {
                    var index = _function.Symbol.ContainingModule.Imports.IndexOf(function.ContainingModule);
                    if (index < 0) throw new Exception();                       // module not imported
                    if (index >= CurrentModuleSentinel) throw new Exception();  // too many imports
                    return index;
                }
            }

            private ByteCode GetLongBranch(mango_opcode opcode, int offset, LabelSymbol label)
            {
                var target = _emittingModules == null ? default : _emittingModule.GetLabelOffset(_function, label) - (offset + new ByteCode(opcode, i16: 0).Length);
                if (target < short.MinValue || target > short.MaxValue) throw new Exception();
                return new ByteCode(opcode, i16: (short)target);
            }

            private ByteCode GetLongBranch(mango_opcode opcode1, mango_opcode opcode2, int offset, LabelSymbol label)
            {
                var target = _emittingModules == null ? default : _emittingModule.GetLabelOffset(_function, label) - (offset + new ByteCode(opcode1, opcode2, i16: 0).Length);
                if (target < short.MinValue || target > short.MaxValue) throw new Exception();
                return new ByteCode(opcode1, opcode2, i16: (short)target);
            }

            private ByteCode GetShortBranch(mango_opcode opcode, int offset, LabelSymbol label)
            {
                var target = _emittingModules == null ? default : _emittingModule.GetLabelOffset(_function, label) - (offset + new ByteCode(opcode, i8: 0).Length);
                if (target < sbyte.MinValue || target > sbyte.MaxValue) throw new Exception();
                return new ByteCode(opcode, i8: (sbyte)target);
            }

            private ByteCode GetShortBranch(mango_opcode opcode1, mango_opcode opcode2, int offset, LabelSymbol label)
            {
                var target = _emittingModules == null ? default : _emittingModule.GetLabelOffset(_function, label) - (offset + new ByteCode(opcode1, opcode2, i8: 0).Length);
                if (target < sbyte.MinValue || target > sbyte.MaxValue) throw new Exception();
                return new ByteCode(opcode1, opcode2, i8: (sbyte)target);
            }

            private ByteCode GetSystemCall(FunctionInstruction instruction)
            {
                var returnSlotCount = CalculateReturnsSlotCount(instruction.Function);
                var parametersSlotCount = CalculateParametersSlotCount(instruction.Function);
                var adjustment = parametersSlotCount - returnSlotCount;
                var ordinal = instruction.Function.GetSystemCallOrdinal();

                if (adjustment < sbyte.MinValue || adjustment > sbyte.MaxValue) throw new Exception();
                if (ordinal < ushort.MinValue || ordinal > ushort.MaxValue) throw new Exception();

                return new ByteCode(SYSCALL, i8: (sbyte)adjustment, u16: (ushort)ordinal);
            }
        }
    }
}
