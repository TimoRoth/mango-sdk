using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Mango.Compiler.Emit;
using Mango.Compiler.Symbols;
using Mango.Compiler.Verification;
using static Interop.Libmango;

namespace Mango.Debugger
{
    public sealed class StackFrame
    {
        private readonly mango_stack_frame _frame;
        private readonly VerifiedFunction _function;
        private readonly Instruction _instruction;
        private readonly int _localsOffset;
        private readonly StackFrame _next;
        private readonly int _parametersOffset;
        private readonly Snapshot _snapshot;
        private readonly int _stackOffset;

        private StackFrame(mango_stack_frame frame, StackFrame next, Snapshot snapshot)
        {
            _frame = frame;
            _next = next;
            _snapshot = snapshot;
        }

        private StackFrame(mango_stack_frame frame, StackFrame next, Snapshot snapshot, VerifiedFunction function)
        {
            _frame = frame;
            _next = next;
            _snapshot = snapshot;
            _function = function;
        }

        private StackFrame(mango_stack_frame frame, StackFrame next, Snapshot snapshot, VerifiedFunction function, Instruction instruction, int stackOffset, int localsOffset, int parametersOffset)
        {
            _frame = frame;
            _next = next;
            _snapshot = snapshot;
            _function = function;
            _instruction = instruction;
            _stackOffset = stackOffset;
            _localsOffset = localsOffset;
            _parametersOffset = parametersOffset;
        }

        public int ByteCodeOffset => _frame.ip;

        public Instruction CurrentInstruction => _instruction;

        public FunctionSymbol FunctionSymbol => _function?.Symbol;

        public int ModuleIndex => _frame.module;

        public ModuleSymbol ModuleSymbol => _function?.Symbol.ContainingModule;

        public StackFrame Next => _next;

        public VerifiedFunction VerifiedFunction => _function;

        internal Snapshot Snapshot => _snapshot;

        internal static ImmutableArray<StackFrame> CreateStackTraceFrom(ReadOnlySpan<byte> memory, ImmutableArray<EmittedModule> symbols = default, Snapshot snapshot = null)
        {
            ref readonly var vm = ref Utilities.GetVM(memory);
            var returnStack = Utilities.GetReturnStack(memory, in vm);
            var builder = ImmutableArray.CreateBuilder<StackFrame>(returnStack.Length);
            var offset = Unsafe.SizeOf<mango_vm>() + vm.stack_size * Unsafe.SizeOf<mango_stackval>();
            var currentStackFrame = (StackFrame)null;

            for (var i = 0; i < returnStack.Length; i++)
            {
                var sf = (i < returnStack.Length - 1) ? returnStack[i + 1] : vm.sf;

                if (!symbols.IsDefault)
                {
                    var symbol = symbols[sf.module];
                    if (symbol != null)
                    {
                        var function = symbol.GetFunctionFromOffset(sf.ip);
                        if (function != null)
                        {
                            var instruction = symbol.GetInstructionFromOffset(sf.ip);
                            if (instruction != null)
                            {
                                var parametersOffset = offset;
                                foreach (var parameter in function.Symbol.Parameters)
                                    parametersOffset += (parameter.Type.TypeLayout.Size + 3) & ~3;
                                var localsOffset = offset;
                                foreach (var local in function.Symbol.Locals)
                                    offset -= (local.Type.TypeLayout.Size + 3) & ~3;
                                foreach (var type in instruction.Stack)
                                    offset -= (type.TypeLayout.Size + 3) & ~3;
                                var stackOffset = offset;
                                builder.Add(currentStackFrame = new StackFrame(sf, currentStackFrame, snapshot, function, instruction, stackOffset, localsOffset, parametersOffset));
                                continue;
                            }
                            else
                            {
                                builder.Add(currentStackFrame = new StackFrame(sf, currentStackFrame, snapshot, function));
                                continue;
                            }
                        }
                    }
                }

                builder.Add(currentStackFrame = new StackFrame(sf, currentStackFrame, snapshot));
            }

            return builder.MoveToImmutable();
        }

        internal ImmutableStack<TypedValue> GetEvaluationStack()
        {
            if (_instruction == null)
            {
                return null;
            }

            return GetEvaluationStack(_stackOffset, _instruction.Stack, _snapshot);
        }

        internal ImmutableDictionary<LocalSymbol, TypedValue> GetLocals()
        {
            if (_function == null || _function.Symbol.Locals == null)
            {
                return null;
            }

            var builder = ImmutableDictionary.CreateBuilder<LocalSymbol, TypedValue>();
            var offset = _localsOffset;

            foreach (var local in _function.Symbol.Locals)
            {
                offset -= (local.Type.TypeLayout.Size + 3) & ~3;
                builder.Add(local, new TypedValue(offset, local.Type, _snapshot));
            }

            return builder.ToImmutable();
        }

        internal ImmutableDictionary<ParameterSymbol, TypedValue> GetParameters()
        {
            if (_function == null)
            {
                return null;
            }

            var builder = ImmutableDictionary.CreateBuilder<ParameterSymbol, TypedValue>();
            var offset = _parametersOffset;

            foreach (var parameter in _function.Symbol.Parameters)
            {
                offset -= (parameter.Type.TypeLayout.Size + 3) & ~3;
                builder.Add(parameter, new TypedValue(offset, parameter.Type, _snapshot));
            }

            return builder.ToImmutable();
        }

        private static ImmutableStack<TypedValue> GetEvaluationStack(int offset, ImmutableStack<TypeSymbol> types, Snapshot snapshot)
        {
            if (types.IsEmpty)
            {
                return ImmutableStack<TypedValue>.Empty;
            }
            else
            {
                return GetEvaluationStack(offset + ((types.Peek().TypeLayout.Size + 3) & ~3), types.Pop(), snapshot).Push(new TypedValue(offset, types.Peek(), snapshot));
            }
        }
    }
}
