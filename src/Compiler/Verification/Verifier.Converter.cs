using System;
using Mango.Compiler.Binding;
using Mango.Compiler.Symbols;
using Mango.Compiler.Syntax;
using static Mango.Compiler.Analysis.Verification;

namespace Mango.Compiler.Verification
{
    partial class Verifier
    {
        private readonly struct Converter
        {
            private readonly Binder _binder;
            private readonly FunctionSymbol _function;

            public Converter(Binder binder, FunctionSymbol function)
            {
                _binder = binder;
                _function = function;
            }

            public Instruction VisitArgument(ArgumentInstructionSyntax instruction, ExecutionState state)
            {
                var kind = Convert(instruction.Kind);
                var parameter = _binder.BindParameter(instruction) ?? throw new Exception();

                return new ArgumentInstruction(kind, state, parameter);
            }

            public Instruction VisitBranch(BranchInstructionSyntax instruction, ExecutionState state)
            {
                var kind = Convert(instruction.Kind);
                var label = _binder.BindLabel(instruction) ?? throw new Exception();

                return new UnconditionalBranchInstruction(kind, state, label);
            }

            public Instruction VisitBranch(BranchInstructionSyntax instruction, ExecutionState state, TypeSymbol value)
            {
                var kind = Convert(instruction.Kind);
                var label = _binder.BindLabel(instruction) ?? throw new Exception();

                return new ConditionalBranchInstruction(kind, state, value, label);
            }

            public Instruction VisitBranch(BranchInstructionSyntax instruction, ExecutionState state, TypeSymbol value1, TypeSymbol value2)
            {
                if (value1 != value2) throw new Exception();

                var kind = Convert(instruction.Kind);
                var label = _binder.BindLabel(instruction) ?? throw new Exception();

                return new ConditionalBranchInstruction(kind, state, value1, label);
            }

            public Instruction VisitConstant(ConstantInstructionSyntax instruction, ExecutionState state)
            {
                var kind = Convert(instruction.Kind);
                var type = _binder.BindType(instruction) ?? throw new Exception();

                return new ConstantInstruction(kind, state, type, instruction.ConstantValue);
            }

            public Instruction VisitConversion(TypeInstructionSyntax instruction, ExecutionState state, TypeSymbol value)
            {
                var kind = Convert(instruction.Kind);
                var type = _binder.BindType(instruction) ?? throw new Exception();

                return new ConversionInstruction(kind, state, type, value);
            }

            public Instruction VisitField(FieldInstructionSyntax instruction, ExecutionState state)
            {
                var kind = Convert(instruction.Kind);
                var field = _binder.BindField(instruction) ?? throw new Exception();

                return new FieldInstruction(kind, state, field);
            }

            public Instruction VisitFunction(FunctionInstructionSyntax instruction, ExecutionState state)
            {
                var kind = Convert(instruction.Kind);
                var function = _binder.BindFunction(instruction) ?? throw new Exception();

                return new FunctionInstruction(kind, state, function);
            }

            public Instruction VisitLocal(LocalInstructionSyntax instruction, ExecutionState state)
            {
                var kind = Convert(instruction.Kind);
                var local = _binder.BindLocal(instruction) ?? throw new Exception();

                return new LocalInstruction(kind, state, local);
            }

            public Instruction VisitNone(NoneInstructionSyntax instruction, ExecutionState state)
            {
                var kind = Convert(instruction.Kind);

                return new NoneInstruction(kind, state);
            }

            public Instruction VisitReturn(NoneInstructionSyntax instruction, ExecutionState state)
            {
                var kind = Convert(instruction.Kind);

                if (_function.ReturnsVoid)
                {
                    return new NoneInstruction(kind, state);
                }
                else
                {
                    return new TypedInstruction(kind, state, GetIntermediateType(_function.ReturnType));
                }
            }

            public Instruction VisitTyped(TypeInstructionSyntax instruction, ExecutionState state)
            {
                var kind = Convert(instruction.Kind);
                var type = _binder.BindType(instruction) ?? throw new Exception();

                return new TypedInstruction(kind, state, type);
            }

            public Instruction VisitTyped(NoneInstructionSyntax instruction, ExecutionState state, TypeSymbol value)
            {
                var kind = Convert(instruction.Kind);

                return new TypedInstruction(kind, state, value);
            }

            public Instruction VisitTyped(NoneInstructionSyntax instruction, ExecutionState state, TypeSymbol value1, TypeSymbol value2)
            {
                if (value1 != value2) throw new Exception();

                var kind = Convert(instruction.Kind);

                return new TypedInstruction(kind, state, value1);
            }

            private static InstructionKind Convert(SyntaxKind kind)
            {
                switch (kind)
                {
                case SyntaxKind.Add: return InstructionKind.Add;
                case SyntaxKind.And: return InstructionKind.And;
                case SyntaxKind.Beq: return InstructionKind.Beq;
                case SyntaxKind.Beq_S: return InstructionKind.Beq_S;
                case SyntaxKind.Bge: return InstructionKind.Bge;
                case SyntaxKind.Bge_S: return InstructionKind.Bge_S;
                case SyntaxKind.Bge_Un: return InstructionKind.Bge_Un;
                case SyntaxKind.Bge_Un_S: return InstructionKind.Bge_Un_S;
                case SyntaxKind.Bgt: return InstructionKind.Bgt;
                case SyntaxKind.Bgt_S: return InstructionKind.Bgt_S;
                case SyntaxKind.Bgt_Un: return InstructionKind.Bgt_Un;
                case SyntaxKind.Bgt_Un_S: return InstructionKind.Bgt_Un_S;
                case SyntaxKind.Ble: return InstructionKind.Ble;
                case SyntaxKind.Ble_S: return InstructionKind.Ble_S;
                case SyntaxKind.Ble_Un: return InstructionKind.Ble_Un;
                case SyntaxKind.Ble_Un_S: return InstructionKind.Ble_Un_S;
                case SyntaxKind.Blt: return InstructionKind.Blt;
                case SyntaxKind.Blt_S: return InstructionKind.Blt_S;
                case SyntaxKind.Blt_Un: return InstructionKind.Blt_Un;
                case SyntaxKind.Blt_Un_S: return InstructionKind.Blt_Un_S;
                case SyntaxKind.Bne_Un: return InstructionKind.Bne_Un;
                case SyntaxKind.Bne_Un_S: return InstructionKind.Bne_Un_S;
                case SyntaxKind.Br: return InstructionKind.Br;
                case SyntaxKind.Br_S: return InstructionKind.Br_S;
                case SyntaxKind.Break: return InstructionKind.Break;
                case SyntaxKind.Brfalse: return InstructionKind.Brfalse;
                case SyntaxKind.Brfalse_S: return InstructionKind.Brfalse_S;
                case SyntaxKind.Brtrue: return InstructionKind.Brtrue;
                case SyntaxKind.Brtrue_S: return InstructionKind.Brtrue_S;
                case SyntaxKind.Call: return InstructionKind.Call;
                case SyntaxKind.Calli: return InstructionKind.Calli;
                case SyntaxKind.Ceq: return InstructionKind.Ceq;
                case SyntaxKind.Cgt: return InstructionKind.Cgt;
                case SyntaxKind.Cgt_Un: return InstructionKind.Cgt_Un;
                case SyntaxKind.Clt: return InstructionKind.Clt;
                case SyntaxKind.Clt_Un: return InstructionKind.Clt_Un;
                case SyntaxKind.Conv: return InstructionKind.Conv;
                case SyntaxKind.Conv_Un: return InstructionKind.Conv_Un;
                case SyntaxKind.Cpobj: return InstructionKind.Cpobj;
                case SyntaxKind.Div: return InstructionKind.Div;
                case SyntaxKind.Div_Un: return InstructionKind.Div_Un;
                case SyntaxKind.Dup: return InstructionKind.Dup;
                case SyntaxKind.Initobj: return InstructionKind.Initobj;
                case SyntaxKind.Ldarg: return InstructionKind.Ldarg;
                case SyntaxKind.Ldarga: return InstructionKind.Ldarga;
                case SyntaxKind.Ldc: return InstructionKind.Ldc;
                case SyntaxKind.Ldelem: return InstructionKind.Ldelem;
                case SyntaxKind.Ldelema: return InstructionKind.Ldelema;
                case SyntaxKind.Ldfld: return InstructionKind.Ldfld;
                case SyntaxKind.Ldflda: return InstructionKind.Ldflda;
                case SyntaxKind.Ldftn: return InstructionKind.Ldftn;
                case SyntaxKind.Ldind: return InstructionKind.Ldind;
                case SyntaxKind.Ldlen: return InstructionKind.Ldlen;
                case SyntaxKind.Ldloc: return InstructionKind.Ldloc;
                case SyntaxKind.Ldloca: return InstructionKind.Ldloca;
                case SyntaxKind.Ldnull: return InstructionKind.Ldnull;
                case SyntaxKind.Ldobj: return InstructionKind.Ldobj;
                case SyntaxKind.Mul: return InstructionKind.Mul;
                case SyntaxKind.Neg: return InstructionKind.Neg;
                case SyntaxKind.Newarr: return InstructionKind.Newarr;
                case SyntaxKind.Newobj: return InstructionKind.Newobj;
                case SyntaxKind.Nop: return InstructionKind.Nop;
                case SyntaxKind.Not: return InstructionKind.Not;
                case SyntaxKind.Or: return InstructionKind.Or;
                case SyntaxKind.Pop: return InstructionKind.Pop;
                case SyntaxKind.Rem: return InstructionKind.Rem;
                case SyntaxKind.Rem_Un: return InstructionKind.Rem_Un;
                case SyntaxKind.Ret: return InstructionKind.Ret;
                case SyntaxKind.Shl: return InstructionKind.Shl;
                case SyntaxKind.Shr: return InstructionKind.Shr;
                case SyntaxKind.Shr_Un: return InstructionKind.Shr_Un;
                case SyntaxKind.Starg: return InstructionKind.Starg;
                case SyntaxKind.Stelem: return InstructionKind.Stelem;
                case SyntaxKind.Stfld: return InstructionKind.Stfld;
                case SyntaxKind.Stind: return InstructionKind.Stind;
                case SyntaxKind.Stloc: return InstructionKind.Stloc;
                case SyntaxKind.Stobj: return InstructionKind.Stobj;
                case SyntaxKind.Sub: return InstructionKind.Sub;
                case SyntaxKind.Syscall: return InstructionKind.Syscall;
                case SyntaxKind.Xor: return InstructionKind.Xor;
                default: throw new ArgumentException();
                }
            }
        }
    }
}
