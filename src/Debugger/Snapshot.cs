using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Mango.Compiler.Symbols;
using static Interop.Libmango;

namespace Mango.Debugger
{
    public sealed class Snapshot
    {
        private readonly byte[] _memory;
        private readonly ImmutableArray<ModuleSymbol> _symbols;

        private ImmutableArray<Module> _modules;
        private ImmutableArray<StackFrame> _stackTrace;

        private Snapshot(byte[] memory, ImmutableArray<ModuleSymbol> symbols = default)
        {
            _memory = memory;
            _symbols = symbols;
        }

        public Module CurrentModule => CurrentStackFrame?.Module;

        public StackFrame CurrentStackFrame => StackTrace.LastOrDefault();

        public Module EntryPointModule => Modules.FirstOrDefault();

        public ReadOnlySpan<byte> MemoryDump => _memory;

        public int MemorySize => _memory.Length;

        public int MemoryUsed => (int)Utilities.GetVM(_memory).heap_used;

        public ImmutableArray<Module> Modules
        {
            get
            {
                if (_modules.IsDefault)
                {
                    ImmutableInterlocked.InterlockedInitialize(ref _modules, Module.CreateModulesFrom(this, _symbols));
                }

                return _modules;
            }
        }

        public int StackSize => Utilities.GetVM(_memory).stack_size * Unsafe.SizeOf<mango_stackval>();

        public ImmutableArray<StackFrame> StackTrace
        {
            get
            {
                if (_stackTrace.IsDefault)
                {
                    ImmutableInterlocked.InterlockedInitialize(ref _stackTrace, StackFrame.CreateStackTraceFrom(_memory, _symbols, this));
                }

                return _stackTrace;
            }
        }

        public int SystemCall => Utilities.GetVM(_memory).syscall;

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
            File.WriteAllBytes(path, _memory);
        }

        public void Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            stream.Write(_memory, 0, _memory.Length);
        }

        internal static Snapshot LoadInternal(byte[] memory, ImmutableArray<ModuleSymbol> symbols)
        {
            if (memory.Length < Unsafe.SizeOf<mango_vm>())
                throw new FormatException();

            ref readonly var vm = ref Utilities.GetVM(memory);

            if (vm.version != MANGO_VERSION_MAJOR ||
                vm.heap_size != memory.Length ||
                vm.heap_used < Unsafe.SizeOf<mango_vm>() ||
                vm.heap_used > memory.Length ||
                vm.modules_created == 0 ||
                vm.modules_imported != vm.modules_created ||
                vm.modules.address == 0 ||
                vm.rp > vm.sp ||
                vm.sp > vm.stack_size)
                throw new FormatException();

            return new Snapshot(memory, symbols);
        }
    }
}
