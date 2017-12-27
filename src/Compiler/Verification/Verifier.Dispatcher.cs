using System;
using Mango.Compiler.Syntax;

namespace Mango.Compiler.Verification
{
    partial class Verifier
    {
        private readonly struct Dispatcher
        {
            private readonly Converter _converter;

            public Dispatcher(Converter interpreter)
            {
                _converter = interpreter;
            }

            public Instruction Visit(InstructionSyntax instruction, ExecutionState state)
            {
                if (instruction is NoneInstructionSyntax noneInstruction)
                {
                    return VisitNone(noneInstruction, state);
                }
                if (instruction is ConstantInstructionSyntax constantInstruction)
                {
                    return VisitConstant(constantInstruction, state);
                }
                if (instruction is FunctionInstructionSyntax functionInstruction)
                {
                    return VisitFunction(functionInstruction, state);
                }
                if (instruction is TypeInstructionSyntax typeInstruction)
                {
                    return VisitType(typeInstruction, state);
                }
                if (instruction is FieldInstructionSyntax fieldInstruction)
                {
                    return VisitField(fieldInstruction, state);
                }
                if (instruction is ArgumentInstructionSyntax argumentInstruction)
                {
                    return VisitArgument(argumentInstruction, state);
                }
                if (instruction is LocalInstructionSyntax localInstruction)
                {
                    return VisitLocal(localInstruction, state);
                }
                if (instruction is BranchInstructionSyntax branchInstruction)
                {
                    return VisitBranch(branchInstruction, state);
                }
                throw new Exception();
            }

            private Instruction VisitArgument(ArgumentInstructionSyntax instruction, ExecutionState state)
            {
                return _converter.VisitArgument(instruction, state);
            }

            private Instruction VisitBranch(BranchInstructionSyntax instruction, ExecutionState state)
            {
                switch (instruction.Kind)
                {
                case SyntaxKind.Br:
                case SyntaxKind.Br_S:
                    return _converter.VisitBranch(instruction, state);

                case SyntaxKind.Brfalse:
                case SyntaxKind.Brfalse_S:
                case SyntaxKind.Brtrue:
                case SyntaxKind.Brtrue_S:
                    return _converter.VisitBranch(instruction, state, state.Stack.Peek());

                case SyntaxKind.Beq:
                case SyntaxKind.Beq_S:
                case SyntaxKind.Bge:
                case SyntaxKind.Bge_S:
                case SyntaxKind.Bge_Un:
                case SyntaxKind.Bge_Un_S:
                case SyntaxKind.Bgt:
                case SyntaxKind.Bgt_S:
                case SyntaxKind.Bgt_Un:
                case SyntaxKind.Bgt_Un_S:
                case SyntaxKind.Ble:
                case SyntaxKind.Ble_S:
                case SyntaxKind.Ble_Un:
                case SyntaxKind.Ble_Un_S:
                case SyntaxKind.Blt:
                case SyntaxKind.Blt_S:
                case SyntaxKind.Blt_Un:
                case SyntaxKind.Blt_Un_S:
                case SyntaxKind.Bne_Un:
                case SyntaxKind.Bne_Un_S:
                    return _converter.VisitBranch(instruction, state, state.Stack.Pop().Peek(), state.Stack.Peek());

                default:
                    throw new Exception();
                }
            }

            private Instruction VisitConstant(ConstantInstructionSyntax instruction, ExecutionState state)
            {
                return _converter.VisitConstant(instruction, state);
            }

            private Instruction VisitField(FieldInstructionSyntax instruction, ExecutionState state)
            {
                return _converter.VisitField(instruction, state);
            }

            private Instruction VisitFunction(FunctionInstructionSyntax instruction, ExecutionState state)
            {
                return _converter.VisitFunction(instruction, state);
            }

            private Instruction VisitLocal(LocalInstructionSyntax instruction, ExecutionState state)
            {
                return _converter.VisitLocal(instruction, state);
            }

            private Instruction VisitNone(NoneInstructionSyntax instruction, ExecutionState state)
            {
                switch (instruction.Kind)
                {
                case SyntaxKind.Break:
                case SyntaxKind.Ldlen:
                case SyntaxKind.Ldnull:
                case SyntaxKind.Nop:
                    return _converter.VisitNone(instruction, state);

                case SyntaxKind.Ret:
                    return _converter.VisitReturn(instruction, state);

                case SyntaxKind.Dup:
                case SyntaxKind.Neg:
                case SyntaxKind.Not:
                case SyntaxKind.Pop:
                    return _converter.VisitTyped(instruction, state, state.Stack.Peek());

                case SyntaxKind.Shl:
                case SyntaxKind.Shr:
                case SyntaxKind.Shr_Un:
                    return _converter.VisitTyped(instruction, state, state.Stack.Pop().Peek());

                case SyntaxKind.Add:
                case SyntaxKind.And:
                case SyntaxKind.Ceq:
                case SyntaxKind.Cgt:
                case SyntaxKind.Cgt_Un:
                case SyntaxKind.Clt:
                case SyntaxKind.Clt_Un:
                case SyntaxKind.Div:
                case SyntaxKind.Div_Un:
                case SyntaxKind.Mul:
                case SyntaxKind.Or:
                case SyntaxKind.Rem:
                case SyntaxKind.Rem_Un:
                case SyntaxKind.Sub:
                case SyntaxKind.Xor:
                    return _converter.VisitTyped(instruction, state, state.Stack.Pop().Peek(), state.Stack.Peek());

                default:
                    throw new Exception();
                }
            }

            private Instruction VisitType(TypeInstructionSyntax instruction, ExecutionState state)
            {
                switch (instruction.Kind)
                {
                case SyntaxKind.Conv:
                case SyntaxKind.Conv_Un:
                    return _converter.VisitConversion(instruction, state, state.Stack.Peek());

                case SyntaxKind.Calli:
                case SyntaxKind.Ldelem:
                case SyntaxKind.Ldelema:
                case SyntaxKind.Ldind:
                case SyntaxKind.Ldobj:
                case SyntaxKind.Stelem:
                case SyntaxKind.Stind:
                case SyntaxKind.Stobj:
                    return _converter.VisitTyped(instruction, state);

                default:
                    throw new Exception();
                }
            }
        }
    }
}
