using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Mango.Compiler.Syntax;

namespace Mango.Compiler.Analysis
{
    internal sealed class InterpreterDataFlowAnalyzer<T> : DataFlowAnalyzer<T>
    {
        private readonly IEqualityComparer<T> _comparer;
        private readonly IInterpreter<T> _interpreter;
        private readonly Dictionary<string, int> _labels;
        private readonly bool _returnsVoid;

        public InterpreterDataFlowAnalyzer(FunctionDeclarationSyntax functionDeclaration, IInterpreter<T> interpreter, IEqualityComparer<T> comparer = null) : base(functionDeclaration)
        {
            _interpreter = interpreter;
            _comparer = comparer ?? EqualityComparer<T>.Default;

            _labels = new Dictionary<string, int>();
            for (var i = 0; i < functionDeclaration.Body.Instructions.Count; i++)
            {
                var instruction = functionDeclaration.Body.Instructions[i];
                while (instruction is LabeledInstructionSyntax label)
                {
                    _labels.Add(label.LabelName, i);
                    instruction = label.LabeledInstruction;
                }
            }

            _returnsVoid = functionDeclaration.ReturnType.Kind == SyntaxKind.VoidType;
        }

        protected override bool Equal(T first, T second)
        {
            return _comparer.Equals(first, second);
        }

        protected override T Merge(T first, T second)
        {
            return _interpreter.Phi(first, second);
        }

        protected override void VisitArgument(int i, ArgumentInstructionSyntax instruction, ImmutableStack<T> stack)
        {
            switch (instruction.Kind)
            {
            case SyntaxKind.Ldarg:
                stack = stack.Push(_interpreter.Ldarg(instruction));
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Ldarga:
                stack = stack.Push(_interpreter.Ldarga(instruction));
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Starg:
                _interpreter.Starg(instruction, stack.Peek());
                stack = stack.Pop();
                BranchTo(i + 1, stack);
                break;

            default:
                throw new Exception();
            }
        }

        protected override void VisitBranch(int i, BranchInstructionSyntax instruction, ImmutableStack<T> stack)
        {
            switch (instruction.Kind)
            {
            case SyntaxKind.Beq:
            case SyntaxKind.Bne_Un:
            case SyntaxKind.Beq_S:
            case SyntaxKind.Bne_Un_S:
                _interpreter.Beq_BneUn(instruction, stack.Pop().Peek(), stack.Peek());
                stack = stack.Pop().Pop();
                BranchTo(_labels[instruction.LabelName], stack);
                BranchTo(i + 1, stack);
                break;

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
                _interpreter.Bge_BgeUn_Bgt_BgtUn_Ble_BleUn_Blt_BltUn(instruction, stack.Pop().Peek(), stack.Peek());
                stack = stack.Pop().Pop();
                BranchTo(_labels[instruction.LabelName], stack);
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Br:
            case SyntaxKind.Br_S:
                _interpreter.Br(instruction);
                BranchTo(_labels[instruction.LabelName], stack);
                break;

            case SyntaxKind.Brfalse:
            case SyntaxKind.Brfalse_S:
                _interpreter.Brfalse(instruction, stack.Peek());
                stack = stack.Pop();
                BranchTo(_labels[instruction.LabelName], stack);
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Brtrue:
            case SyntaxKind.Brtrue_S:
                _interpreter.Brtrue(instruction, stack.Peek());
                stack = stack.Pop();
                BranchTo(_labels[instruction.LabelName], stack);
                BranchTo(i + 1, stack);
                break;

            default:
                throw new Exception();
            }
        }

        protected override void VisitConstant(int i, ConstantInstructionSyntax instruction, ImmutableStack<T> stack)
        {
            switch (instruction.Kind)
            {
            case SyntaxKind.Ldc:
                stack = stack.Push(_interpreter.Ldc(instruction));
                BranchTo(i + 1, stack);
                break;

            default:
                throw new Exception();
            }
        }

        protected override void VisitField(int i, FieldInstructionSyntax instruction, ImmutableStack<T> stack)
        {
            switch (instruction.Kind)
            {
            case SyntaxKind.Ldfld:
                stack = stack.Pop().Push(_interpreter.Ldfld(instruction, stack.Peek()));
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Ldflda:
                stack = stack.Pop().Push(_interpreter.Ldflda(instruction, stack.Peek()));
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Stfld:
                _interpreter.Stfld(instruction, stack.Pop().Peek(), stack.Peek());
                stack = stack.Pop().Pop();
                BranchTo(i + 1, stack);
                break;

            default:
                throw new Exception();
            }
        }

        protected override void VisitFunction(int i, FunctionInstructionSyntax instruction, ImmutableStack<T> stack)
        {
            switch (instruction.Kind)
            {
            case SyntaxKind.Call:
            case SyntaxKind.Syscall:
                var arguments = new T[instruction.ParameterTypes.Count];
                for (var index = arguments.Length - 1; index >= 0; index--)
                    stack = stack.Pop(out arguments[index]);
                if (instruction.ReturnType.Kind == SyntaxKind.VoidType)
                    _interpreter.CallVoid_SyscallVoid(instruction, arguments);
                else
                    stack = stack.Push(_interpreter.Call_Syscall(instruction, arguments));
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Ldftn:
                stack = stack.Push(_interpreter.Ldftn(instruction));
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Newobj:
                arguments = new T[instruction.ParameterTypes.Count];
                for (var index = arguments.Length - 1; index >= 0; index--)
                    stack = stack.Pop(out arguments[index]);
                stack = stack.Push(_interpreter.Newobj(instruction, arguments));
                BranchTo(i + 1, stack);
                break;

            default:
                throw new Exception();
            }
        }

        protected override void VisitLocal(int i, LocalInstructionSyntax instruction, ImmutableStack<T> stack)
        {
            switch (instruction.Kind)
            {
            case SyntaxKind.Ldloc:
                stack = stack.Push(_interpreter.Ldloc(instruction));
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Ldloca:
                stack = stack.Push(_interpreter.Ldloca(instruction));
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Stloc:
                _interpreter.Stloc(instruction, stack.Peek());
                stack = stack.Pop();
                BranchTo(i + 1, stack);
                break;

            default:
                throw new Exception();
            }
        }

        protected override void VisitNone(int i, NoneInstructionSyntax instruction, ImmutableStack<T> stack)
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
                stack = stack.Pop().Pop().Push(_interpreter.Add_Div_DivUn_Mul_Rem_RemUn_Sub(instruction, stack.Pop().Peek(), stack.Peek()));
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.And:
            case SyntaxKind.Or:
            case SyntaxKind.Xor:
                stack = stack.Pop().Pop().Push(_interpreter.And_Or_Xor(instruction, stack.Pop().Peek(), stack.Peek()));
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Break:
                _interpreter.Break(instruction);
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Ceq:
                stack = stack.Pop().Pop().Push(_interpreter.Ceq(instruction, stack.Pop().Peek(), stack.Peek()));
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Cgt:
            case SyntaxKind.Cgt_Un:
            case SyntaxKind.Clt:
            case SyntaxKind.Clt_Un:
                stack = stack.Pop().Pop().Push(_interpreter.Cgt_CgtUn_Clt_CltUn(instruction, stack.Pop().Peek(), stack.Peek()));
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Dup:
                _interpreter.Dup(instruction, stack.Peek());
                stack = stack.Push(stack.Peek()).Push(stack.Peek());
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Ldlen:
                stack = stack.Pop().Push(_interpreter.Ldlen(instruction, stack.Peek()));
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Ldnull:
                stack = stack.Push(_interpreter.Ldnull(instruction));
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Neg:
                stack = stack.Pop().Push(_interpreter.Neg(instruction, stack.Peek()));
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Nop:
                _interpreter.Nop(instruction);
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Not:
                stack = stack.Pop().Push(_interpreter.Not(instruction, stack.Peek()));
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Pop:
                _interpreter.Pop(instruction, stack.Peek());
                stack = stack.Pop();
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Ret:
                if (_returnsVoid)
                {
                    if (!stack.IsEmpty) throw new Exception();
                    _interpreter.Ret(instruction);
                }
                else
                {
                    if (stack.IsEmpty || !stack.Pop().IsEmpty) throw new Exception();
                    _interpreter.Ret(instruction, stack.Peek());
                }
                break;

            case SyntaxKind.Shl:
            case SyntaxKind.Shr:
            case SyntaxKind.Shr_Un:
                stack = stack.Pop().Pop().Push(_interpreter.Shl_Shr_ShrUn(instruction, stack.Pop().Peek(), stack.Peek()));
                BranchTo(i + 1, stack);
                break;

            default:
                throw new Exception();
            }
        }

        protected override void VisitType(int i, TypeInstructionSyntax instruction, ImmutableStack<T> stack)
        {
            switch (instruction.Kind)
            {
            case SyntaxKind.Calli:
                var type = (FunctionTypeSyntax)instruction.Type;
                var arguments = new T[type.ParameterTypes.Count];
                stack = stack.Pop(out var function);
                for (var index = arguments.Length - 1; index >= 0; index--)
                    stack = stack.Pop(out arguments[index]);
                if (type.ReturnType.Kind == SyntaxKind.VoidType)
                    _interpreter.CalliVoid(instruction, arguments, function);
                else
                    stack = stack.Push(_interpreter.Calli(instruction, arguments, function));
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Conv:
            case SyntaxKind.Conv_Un:
                stack = stack.Pop().Push(_interpreter.Conv_ConvUn(instruction, stack.Peek()));
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Ldelem:
                stack = stack.Pop().Pop().Push(_interpreter.Ldelem(instruction, stack.Pop().Peek(), stack.Peek()));
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Ldelema:
                stack = stack.Pop().Pop().Push(_interpreter.Ldelema(instruction, stack.Pop().Peek(), stack.Peek()));
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Ldind:
                stack = stack.Pop().Push(_interpreter.Ldind(instruction, stack.Peek()));
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Ldobj:
                stack = stack.Pop().Push(_interpreter.Ldobj(instruction, stack.Peek()));
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Stelem:
                _interpreter.Stelem(instruction, stack.Pop().Pop().Peek(), stack.Pop().Peek(), stack.Peek());
                stack = stack.Pop().Pop().Pop();
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Stind:
                _interpreter.Stind(instruction, stack.Pop().Peek(), stack.Peek());
                stack = stack.Pop().Pop();
                BranchTo(i + 1, stack);
                break;

            case SyntaxKind.Stobj:
                _interpreter.Stobj(instruction, stack.Pop().Peek(), stack.Peek());
                stack = stack.Pop().Pop();
                BranchTo(i + 1, stack);
                break;

            default:
                throw new Exception();
            }
        }
    }
}
