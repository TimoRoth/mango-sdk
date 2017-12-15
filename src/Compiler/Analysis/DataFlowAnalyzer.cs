using System;
using System.Collections.Immutable;
using Mango.Compiler.Syntax;

namespace Mango.Compiler.Analysis
{
    internal abstract class DataFlowAnalyzer<T>
    {
        private readonly FunctionDeclarationSyntax _functionDeclaration;

        private readonly int[] _queue;
        private readonly bool[] _queued;
        private readonly ImmutableArray<DataFlowState<T>>.Builder _state;

        private int _top;

        public DataFlowAnalyzer(FunctionDeclarationSyntax functionDeclaration)
        {
            _functionDeclaration = functionDeclaration;

            _state = ImmutableArray.CreateBuilder<DataFlowState<T>>(functionDeclaration.Body.Instructions.Count);
            _queued = new bool[functionDeclaration.Body.Instructions.Count];
            _queue = new int[functionDeclaration.Body.Instructions.Count];
        }

        public DataFlowAnalysis<T> Analyze()
        {
            var state = _state;
            var instructions = _functionDeclaration.Body.Instructions;

            _state.Count = instructions.Count;

            BranchTo(0, ImmutableStack<T>.Empty);

            while (_top > 0)
            {
                var i = _queue[--_top];
                _queued[i] = false;

                var stack = state[i].Stack;
                var instruction = instructions[i];

                while (instruction is LabeledInstructionSyntax label)
                {
                    instruction = label.LabeledInstruction;
                }

                Visit(i, instruction, stack);
            }

            return new DataFlowAnalysis<T>(_state.MoveToImmutable());
        }

        protected void AddError(int target, Diagnostics.Diagnostic diagnostic)
        {
            _state[target] = _state[target].AddError(diagnostic);
        }

        protected void BranchTo(int target, ImmutableStack<T> stack)
        {
            var existing = _state[target];

            if (existing.Stack == null || existing.Stack != (stack = Merge(existing.Stack, stack)))
            {
                _state[target] = new DataFlowState<T>(stack);
                if (!_queued[target])
                {
                    _queue[_top++] = target;
                    _queued[target] = true;
                }
            }
        }

        protected abstract bool Equal(T first, T second);

        protected abstract T Merge(T first, T second);

        protected abstract void VisitArgument(int i, ArgumentInstructionSyntax instruction, ImmutableStack<T> stack);

        protected abstract void VisitBranch(int i, BranchInstructionSyntax instruction, ImmutableStack<T> stack);

        protected abstract void VisitConstant(int i, ConstantInstructionSyntax instruction, ImmutableStack<T> stack);

        protected abstract void VisitField(int i, FieldInstructionSyntax instruction, ImmutableStack<T> stack);

        protected abstract void VisitFunction(int i, FunctionInstructionSyntax instruction, ImmutableStack<T> stack);

        protected abstract void VisitLocal(int i, LocalInstructionSyntax instruction, ImmutableStack<T> stack);

        protected abstract void VisitNone(int i, NoneInstructionSyntax instruction, ImmutableStack<T> stack);

        protected abstract void VisitType(int i, TypeInstructionSyntax instruction, ImmutableStack<T> stack);

        private ImmutableStack<T> Merge(ImmutableStack<T> first, ImmutableStack<T> second)
        {
            if (first.IsEmpty && second.IsEmpty)
                return first;
            else if (first.IsEmpty || second.IsEmpty)
                throw new Exception();
            var xs = first.Pop();
            var ys = second.Pop();
            var zs = Merge(xs, ys);
            var x = first.Peek();
            var y = second.Peek();
            var z = Merge(x, y);
            if (zs != xs || !Equal(z, x))
                return zs.Push(z);
            else
                return first;
        }

        private void Visit(int i, InstructionSyntax instruction, ImmutableStack<T> stack)
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
                VisitNone(i, (NoneInstructionSyntax)instruction, stack);
                break;

            case SyntaxKind.Ldc:
                VisitConstant(i, (ConstantInstructionSyntax)instruction, stack);
                break;

            case SyntaxKind.Ldftn:
            case SyntaxKind.Syscall:
            case SyntaxKind.Call:
            case SyntaxKind.Newobj:
                VisitFunction(i, (FunctionInstructionSyntax)instruction, stack);
                break;

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
                VisitType(i, (TypeInstructionSyntax)instruction, stack);
                break;

            case SyntaxKind.Ldfld:
            case SyntaxKind.Ldflda:
            case SyntaxKind.Stfld:
                VisitField(i, (FieldInstructionSyntax)instruction, stack);
                break;

            case SyntaxKind.Ldarg:
            case SyntaxKind.Ldarga:
            case SyntaxKind.Starg:
                VisitArgument(i, (ArgumentInstructionSyntax)instruction, stack);
                break;

            case SyntaxKind.Ldloc:
            case SyntaxKind.Ldloca:
            case SyntaxKind.Stloc:
                VisitLocal(i, (LocalInstructionSyntax)instruction, stack);
                break;

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
                VisitBranch(i, (BranchInstructionSyntax)instruction, stack);
                break;

            default:
                throw new InvalidOperationException();
            }
        }
    }
}
