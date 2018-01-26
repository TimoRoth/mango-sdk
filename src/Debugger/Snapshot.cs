using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mango.Compiler.Emit;
using Mango.Compiler.Symbols;
using static Interop.Libmango;

namespace Mango.Debugger
{
    public sealed class Snapshot
    {
        private readonly ReadOnlyMemory<byte> _memory;
        private readonly ImmutableArray<EmittedModule> _symbols;

        private ImmutableArray<StackFrame> _stackTrace;

        private Snapshot(ReadOnlyMemory<byte> memory, ImmutableArray<EmittedModule> symbols = default)
        {
            _memory = memory;
            _symbols = symbols;
        }

        public StackFrame CurrentStackFrame => StackTrace.LastOrDefault();

        public EmittedModule EntryPointModule => _symbols.IsDefaultOrEmpty ? null : _symbols[0];

        public int HeapSize => (int)Utilities.GetVM(_memory.Span).heap_size;

        public int HeapUsed => (int)Utilities.GetVM(_memory.Span).heap_used;

        public ReadOnlySpan<byte> MemoryDump => _memory.Span;

        public ImmutableArray<EmittedModule> Modules => _symbols;

        public int StackSize => Utilities.GetVM(_memory.Span).stack_size * Unsafe.SizeOf<mango_stackval>();

        public ImmutableArray<StackFrame> StackTrace => GetStackTrace();

        public int SystemCall => Utilities.GetVM(_memory.Span).syscall;

        public static Snapshot Load(string path)
        {
            if (!Utilities.PlatformSupported)
                throw new PlatformNotSupportedException();

            return LoadInternal(File.ReadAllBytes(path), default);
        }

        public static Snapshot Load(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!Utilities.PlatformSupported)
                throw new PlatformNotSupportedException();

            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return LoadInternal(memoryStream.ToArray(), default);
        }

        public static Snapshot Load(ReadOnlySpan<byte> memoryDump)
        {
            if (!Utilities.PlatformSupported)
                throw new PlatformNotSupportedException();

            return LoadInternal(memoryDump.ToArray(), default);
        }

        public ImmutableStack<TypedValue> GetEvaluationStack(StackFrame stackFrame)
        {
            if (stackFrame == null)
                throw new ArgumentNullException(nameof(stackFrame));
            if (stackFrame.Snapshot != this)
                throw new ArgumentException();

            return stackFrame.GetEvaluationStack();
        }

        public ImmutableDictionary<LocalSymbol, TypedValue> GetLocals(StackFrame stackFrame)
        {
            if (stackFrame == null)
                throw new ArgumentNullException(nameof(stackFrame));
            if (stackFrame.Snapshot != this)
                throw new ArgumentException();

            return stackFrame.GetLocals();
        }

        public ImmutableDictionary<ParameterSymbol, TypedValue> GetParameters(StackFrame stackFrame)
        {
            if (stackFrame == null)
                throw new ArgumentNullException(nameof(stackFrame));
            if (stackFrame.Snapshot != this)
                throw new ArgumentException();

            return stackFrame.GetParameters();
        }

        public void Save(string path)
        {
            if (!MemoryMarshal.TryGetArray(_memory, out var array))
                throw new InvalidOperationException();

            File.WriteAllBytes(path, array.Array);
        }

        public void Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!MemoryMarshal.TryGetArray(_memory, out var array))
                throw new InvalidOperationException();

            stream.Write(array.Array, array.Offset, array.Count);
        }

        internal static Snapshot LoadInternal(ReadOnlyMemory<byte> memory, ImmutableArray<EmittedModule> symbols)
        {
            if (memory.Length < Unsafe.SizeOf<mango_vm>())
                throw new FormatException();

            ref readonly var vm = ref Utilities.GetVM(memory.Span);

            if (vm.version != MANGO_VERSION_MAJOR ||
                vm.heap_size < memory.Length ||
                vm.heap_used > vm.heap_size ||
                vm.heap_used < Unsafe.SizeOf<mango_vm>() ||
                vm.heap_used > memory.Length ||
                vm.modules.address == 0 ||
                vm.modules_created == 0 ||
                vm.modules_imported != vm.modules_created ||
                vm.rp > vm.sp ||
                vm.sp > vm.stack_size||
                vm.@base != 0) // TODO: 32-bit snapshots
                throw new FormatException();

            return new Snapshot(memory.Slice(0, (int)vm.heap_used), symbols);
        }

        private ImmutableArray<StackFrame> GetStackTrace()
        {
            if (_stackTrace.IsDefault)
            {
                ImmutableInterlocked.InterlockedInitialize(ref _stackTrace, StackFrame.CreateStackTraceFrom(_memory.Span, _symbols, this));
            }

            return _stackTrace;
        }
    }
}
