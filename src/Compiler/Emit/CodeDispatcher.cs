using System;
using System.Collections.Immutable;
using Mango.Compiler.Symbols;
using Mango.Compiler.Syntax;

namespace Mango.Compiler.Emit
{
    internal readonly struct CodeDispatcher
    {
        private readonly CodeGenerator _interpreter;

        public CodeDispatcher(CodeGenerator interpreter)
        {
            _interpreter = interpreter;
        }

        public ByteCode Visit(int offset, InstructionSyntax instruction, ImmutableStack<TypeSymbol> stack)
        {
            switch (instruction.Kind)
            {
            case SyntaxKind.Ldnull:
            case SyntaxKind.Ret:
            case SyntaxKind.Break:
            case SyntaxKind.Dup:
            case SyntaxKind.Nop:
            case SyntaxKind.Pop:
            case SyntaxKind.Add:
            case SyntaxKind.And:
            case SyntaxKind.Div:
            case SyntaxKind.Div_Un:
            case SyntaxKind.Mul:
            case SyntaxKind.Or:
            case SyntaxKind.Rem:
            case SyntaxKind.Rem_Un:
            case SyntaxKind.Shl:
            case SyntaxKind.Shr:
            case SyntaxKind.Shr_Un:
            case SyntaxKind.Sub:
            case SyntaxKind.Xor:
            case SyntaxKind.Neg:
            case SyntaxKind.Not:
            case SyntaxKind.Ceq:
            case SyntaxKind.Cgt:
            case SyntaxKind.Cgt_Un:
            case SyntaxKind.Clt:
            case SyntaxKind.Clt_Un:
            case SyntaxKind.Ldlen:
                return VisitNone((NoneInstructionSyntax)instruction, stack);

            case SyntaxKind.Ldc:
                return VisitConstant((ConstantInstructionSyntax)instruction, stack);

            case SyntaxKind.Ldftn:
            case SyntaxKind.Call:
            case SyntaxKind.Newobj:
            case SyntaxKind.Syscall:
                return VisitFunction((FunctionInstructionSyntax)instruction, stack);

            case SyntaxKind.Ldind:
            case SyntaxKind.Ldelem:
            case SyntaxKind.Ldelema:
            case SyntaxKind.Stind:
            case SyntaxKind.Stelem:
            case SyntaxKind.Calli:
            case SyntaxKind.Conv:
            case SyntaxKind.Conv_Un:
            case SyntaxKind.Ldobj:
            case SyntaxKind.Stobj:
            case SyntaxKind.Cpobj:
            case SyntaxKind.Initobj:
            case SyntaxKind.Newarr:
                return VisitType((TypeInstructionSyntax)instruction, stack);

            case SyntaxKind.Ldfld:
            case SyntaxKind.Ldflda:
            case SyntaxKind.Stfld:
                return VisitField((FieldInstructionSyntax)instruction, stack);

            case SyntaxKind.Ldarg:
            case SyntaxKind.Ldarga:
            case SyntaxKind.Starg:
                return VisitArgument((ArgumentInstructionSyntax)instruction, stack);

            case SyntaxKind.Ldloc:
            case SyntaxKind.Ldloca:
            case SyntaxKind.Stloc:
                return VisitLocal((LocalInstructionSyntax)instruction, stack);

            case SyntaxKind.Beq:
            case SyntaxKind.Bge:
            case SyntaxKind.Bge_Un:
            case SyntaxKind.Bgt:
            case SyntaxKind.Bgt_Un:
            case SyntaxKind.Ble:
            case SyntaxKind.Ble_Un:
            case SyntaxKind.Blt:
            case SyntaxKind.Blt_Un:
            case SyntaxKind.Bne_Un:
            case SyntaxKind.Br:
            case SyntaxKind.Brfalse:
            case SyntaxKind.Brtrue:
            case SyntaxKind.Beq_S:
            case SyntaxKind.Bge_S:
            case SyntaxKind.Bge_Un_S:
            case SyntaxKind.Bgt_S:
            case SyntaxKind.Bgt_Un_S:
            case SyntaxKind.Ble_S:
            case SyntaxKind.Ble_Un_S:
            case SyntaxKind.Blt_S:
            case SyntaxKind.Blt_Un_S:
            case SyntaxKind.Bne_Un_S:
            case SyntaxKind.Br_S:
            case SyntaxKind.Brfalse_S:
            case SyntaxKind.Brtrue_S:
                return VisitBranch((BranchInstructionSyntax)instruction, stack, offset);

            default:
                throw new InvalidOperationException();
            }
        }

        private ByteCode VisitArgument(ArgumentInstructionSyntax instruction, ImmutableStack<TypeSymbol> stack)
        {
            switch (instruction.Kind)
            {
            case SyntaxKind.Ldarg:
                return _interpreter.Ldarg(instruction, stack);

            case SyntaxKind.Ldarga:
                return _interpreter.Ldarga(instruction, stack);

            case SyntaxKind.Starg:
                return _interpreter.Starg(instruction, stack.Peek(), stack);

            default:
                throw new Exception();
            }
        }

        private ByteCode VisitBranch(BranchInstructionSyntax instruction, ImmutableStack<TypeSymbol> stack, int offset)
        {
            switch (instruction.Kind)
            {
            case SyntaxKind.Beq:
            case SyntaxKind.Bne_Un:
            case SyntaxKind.Beq_S:
            case SyntaxKind.Bne_Un_S:
                return _interpreter.Beq_BneUn(instruction, stack.Pop().Peek(), stack.Peek(), offset);

            case SyntaxKind.Bge:
            case SyntaxKind.Bge_Un:
            case SyntaxKind.Bgt:
            case SyntaxKind.Bgt_Un:
            case SyntaxKind.Ble:
            case SyntaxKind.Ble_Un:
            case SyntaxKind.Blt:
            case SyntaxKind.Blt_Un:
            case SyntaxKind.Bge_S:
            case SyntaxKind.Bge_Un_S:
            case SyntaxKind.Bgt_S:
            case SyntaxKind.Bgt_Un_S:
            case SyntaxKind.Ble_S:
            case SyntaxKind.Ble_Un_S:
            case SyntaxKind.Blt_S:
            case SyntaxKind.Blt_Un_S:
                return _interpreter.Bge_BgeUn_Bgt_BgtUn_Ble_BleUn_Blt_BltUn(instruction, stack.Pop().Peek(), stack.Peek(), offset);

            case SyntaxKind.Br:
            case SyntaxKind.Br_S:
                return _interpreter.Br(instruction, offset);

            case SyntaxKind.Brfalse:
            case SyntaxKind.Brfalse_S:
                return _interpreter.Brfalse(instruction, stack.Peek(), offset);

            case SyntaxKind.Brtrue:
            case SyntaxKind.Brtrue_S:
                return _interpreter.Brtrue(instruction, stack.Peek(), offset);

            default:
                throw new Exception();
            }
        }

        private ByteCode VisitConstant(ConstantInstructionSyntax instruction, ImmutableStack<TypeSymbol> stack)
        {
            switch (instruction.Kind)
            {
            case SyntaxKind.Ldc:
                return _interpreter.Ldc(instruction);

            default:
                throw new Exception();
            }
        }

        private ByteCode VisitField(FieldInstructionSyntax instruction, ImmutableStack<TypeSymbol> stack)
        {
            switch (instruction.Kind)
            {
            case SyntaxKind.Ldfld:
                return _interpreter.Ldfld(instruction, stack.Peek());

            case SyntaxKind.Ldflda:
                return _interpreter.Ldflda(instruction, stack.Peek());

            case SyntaxKind.Stfld:
                return _interpreter.Stfld(instruction, stack.Pop().Peek(), stack.Peek());

            default:
                throw new Exception();
            }
        }

        private ByteCode VisitFunction(FunctionInstructionSyntax instruction, ImmutableStack<TypeSymbol> stack)
        {
            switch (instruction.Kind)
            {
            case SyntaxKind.Call:
                return _interpreter.Call(instruction);

            case SyntaxKind.Ldftn:
                return _interpreter.Ldftn(instruction);

            case SyntaxKind.Newobj:
                return _interpreter.Newobj(instruction);

            case SyntaxKind.Syscall:
                return _interpreter.Syscall(instruction);

            default:
                throw new Exception();
            }
        }

        private ByteCode VisitLocal(LocalInstructionSyntax instruction, ImmutableStack<TypeSymbol> stack)
        {
            switch (instruction.Kind)
            {
            case SyntaxKind.Ldloc:
                return _interpreter.Ldloc(instruction, stack);

            case SyntaxKind.Ldloca:
                return _interpreter.Ldloca(instruction, stack);

            case SyntaxKind.Stloc:
                return _interpreter.Stloc(instruction, stack.Peek(), stack);

            default:
                throw new Exception();
            }
        }

        private ByteCode VisitNone(NoneInstructionSyntax instruction, ImmutableStack<TypeSymbol> stack)
        {
            switch (instruction.Kind)
            {
            case SyntaxKind.Add:
            case SyntaxKind.Div:
            case SyntaxKind.Div_Un:
            case SyntaxKind.Mul:
            case SyntaxKind.Rem:
            case SyntaxKind.Rem_Un:
            case SyntaxKind.Sub:
                return _interpreter.Add_Div_DivUn_Mul_Rem_RemUn_Sub(instruction, stack.Pop().Peek(), stack.Peek());

            case SyntaxKind.And:
            case SyntaxKind.Or:
            case SyntaxKind.Xor:
                return _interpreter.And_Or_Xor(instruction, stack.Pop().Peek(), stack.Peek());

            case SyntaxKind.Break:
                return _interpreter.Break(instruction);

            case SyntaxKind.Ceq:
                return _interpreter.Ceq(instruction, stack.Pop().Peek(), stack.Peek());

            case SyntaxKind.Cgt:
            case SyntaxKind.Cgt_Un:
            case SyntaxKind.Clt:
            case SyntaxKind.Clt_Un:
                return _interpreter.Cgt_CgtUn_Clt_CltUn(instruction, stack.Pop().Peek(), stack.Peek());

            case SyntaxKind.Dup:
                return _interpreter.Dup(instruction, stack.Peek());

            case SyntaxKind.Ldlen:
                return _interpreter.Ldlen(instruction, stack.Peek());

            case SyntaxKind.Ldnull:
                return _interpreter.Ldnull(instruction);

            case SyntaxKind.Neg:
                return _interpreter.Neg(instruction, stack.Peek());

            case SyntaxKind.Nop:
                return _interpreter.Nop(instruction);

            case SyntaxKind.Not:
                return _interpreter.Not(instruction, stack.Peek());

            case SyntaxKind.Pop:
                return _interpreter.Pop(instruction, stack.Peek());

            case SyntaxKind.Ret:
                if (instruction.FirstAncestorOrSelf<FunctionDeclarationSyntax>().ReturnType.Kind == SyntaxKind.VoidType)
                    return _interpreter.Ret(instruction);
                else
                    return _interpreter.Ret(instruction, stack.Peek());

            case SyntaxKind.Shl:
            case SyntaxKind.Shr:
            case SyntaxKind.Shr_Un:
                return _interpreter.Shl_Shr_ShrUn(instruction, stack.Pop().Peek(), stack.Peek());

            default:
                throw new Exception();
            }
        }

        private ByteCode VisitType(TypeInstructionSyntax instruction, ImmutableStack<TypeSymbol> stack)
        {
            switch (instruction.Kind)
            {
            case SyntaxKind.Calli:
                return _interpreter.Calli(instruction, stack.Peek());

            case SyntaxKind.Conv:
            case SyntaxKind.Conv_Un:
                return _interpreter.Conv_ConvUn(instruction, stack.Peek());

            case SyntaxKind.Ldelem:
                return _interpreter.Ldelem(instruction, stack.Pop().Peek(), stack.Peek());

            case SyntaxKind.Ldelema:
                return _interpreter.Ldelema(instruction, stack.Pop().Peek(), stack.Peek());

            case SyntaxKind.Ldind:
                return _interpreter.Ldind(instruction, stack.Peek());

            case SyntaxKind.Ldobj:
                return _interpreter.Ldobj(instruction, stack.Peek());

            case SyntaxKind.Stelem:
                return _interpreter.Stelem(instruction, stack.Pop().Pop().Peek(), stack.Pop().Peek(), stack.Peek());

            case SyntaxKind.Stind:
                return _interpreter.Stind(instruction, stack.Pop().Peek(), stack.Peek());

            case SyntaxKind.Stobj:
                return _interpreter.Stobj(instruction, stack.Pop().Peek(), stack.Peek());

            default:
                throw new Exception();
            }
        }
    }
}
