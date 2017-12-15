using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Mango.Compiler.Symbols;
using static Interop.Libmango;

namespace Mango.Debugger
{
    public sealed class StackFrame
    {
        private readonly mango_stack_frame _frame;
        private readonly StackFrame _next;
        private readonly Snapshot _snapshot;

        //private readonly Instruction _instruction;
        //private readonly int _localsOffset;
        //private readonly int _parametersOffset;
        //private readonly int _stackOffset;

        private StackFrame(mango_stack_frame frame, StackFrame next, Snapshot snapshot = null)
        {
            _frame = frame;
            _next = next;
            _snapshot = snapshot;
        }

        /*
        private StackFrame(mango_stack_frame frame, StackFrame next, Snapshot snapshot, Instruction instruction, int stackOffset, int localsOffset, int parametersOffset)
        {
            _frame = frame;
            _next = next;
            _instruction = instruction;
            _snapshot = snapshot;
            _stackOffset = stackOffset;
            _localsOffset = localsOffset;
            _parametersOffset = parametersOffset;
        }
        */

        public int ByteCodeOffset => _frame.ip;

        public Module Module => _snapshot?.Modules[_frame.module];

        public int ModuleIndex => _frame.module;

        public StackFrame Next => _next;

        //public Instruction CurrentInstruction => _instruction;
        public FunctionSymbol FunctionSymbol => null; //_instruction?.ContainingFunction;
        public ModuleSymbol ModuleSymbol => null; //_instruction?.ContainingFunction?.ContainingModule;

        internal Snapshot Snapshot => _snapshot;

        internal static ImmutableArray<StackFrame> CreateStackTraceFrom(Span<byte> memory, ImmutableArray<ModuleSymbol> symbols = default, Snapshot snapshot = null)
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
                        /*
                        var instruction = ...;
                        var function = instruction.ContainingFunction;
                        var parametersOffset = offset;
                        foreach (var parameter in function.Parameters)
                            parametersOffset += (parameter.Type.TypeLayout.Size + 3) & ~3;
                        var localsOffset = offset;
                        foreach (var local in function.Locals)
                            offset -= (local.Type.TypeLayout.Size + 3) & ~3;
                        foreach (var type in instruction.StackBefore)
                            offset -= (type.TypeLayout.Size + 3) & ~3;
                        var stackOffset = offset;
                        builder.Add(currentStackFrame = new StackFrame(sf, currentStackFrame, snapshot, instruction, stackOffset, localsOffset, parametersOffset));
                        continue;
                        */
                    }
                }

                builder.Add(currentStackFrame = new StackFrame(sf, currentStackFrame, snapshot));
            }

            return builder.MoveToImmutable();
        }

        internal ImmutableStack<TypedValue> GetEvaluationStack()
        {
            //if (_snapshot == null || _instruction == null)
            return null;

            /*
            return GetEvaluationStack(_stackOffset, _instruction.StackBefore, _snapshot);
            */
        }

        internal ImmutableDictionary<LocalSymbol, TypedValue> GetLocals()
        {
            //if (_snapshot == null || _instruction == null)
            return null;

            /*
            var offset = _localsOffset;
            var builder = ImmutableDictionary.CreateBuilder<LocalSymbol, TypedValue>();

            foreach (var local in _instruction.ContainingFunction.Locals)
            {
                offset -= (local.Type.Layout.Size + 3) & ~3;
                var s = offset.ToString("X8");
                builder.Add(local, new TypedValue(offset, local.Type, _snapshot));
            }

            return builder.ToImmutable();
            */
        }

        internal ImmutableDictionary<ParameterSymbol, TypedValue> GetParameters()
        {
            //if (_snapshot == null || _instruction == null)
            return null;

            /*
            var offset = _parametersOffset;
            var builder = ImmutableDictionary.CreateBuilder<ParameterSymbol, TypedValue>();

            foreach (var parameter in _instruction.ContainingFunction.Parameters)
            {
                offset -= (parameter.Type.Layout.Size + 3) & ~3;
                builder.Add(parameter, new TypedValue(offset, parameter.Type, _snapshot));
            }

            return builder.ToImmutable();
            */
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
