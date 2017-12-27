using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mango.Compiler.Emit;
using static Interop.Libmango;
using static Interop.Libmango.mango_result;

namespace Mango.Debugger
{
    public sealed class Process : IDisposable
    {
        private readonly SafeMangoHandle _handle;
        private readonly ImmutableArray<EmittedModule>.Builder _symbols;

        private Process(SafeMangoHandle handle)
        {
            _handle = handle;
            _symbols = ImmutableArray.CreateBuilder<EmittedModule>();
        }

        public static Process Create(int heapSize, int stackSize)
        {
            if (!Utilities.PlatformSupported)
                throw new PlatformNotSupportedException();
            if (stackSize < 0 || stackSize > ushort.MaxValue * Unsafe.SizeOf<mango_stackval>())
                throw new ArgumentOutOfRangeException(nameof(stackSize));
            if (heapSize < stackSize || heapSize - stackSize < Unsafe.SizeOf<mango_vm>())
                throw new ArgumentOutOfRangeException(nameof(heapSize));

            var handle = (SafeMangoHandle)null;
            var addedRef = false;
            var success = false;
            try
            {
                handle = new SafeMangoHandle(Marshal.AllocHGlobal(heapSize), heapSize);
                handle.DangerousAddRef(ref addedRef);

                unsafe
                {
                    Unsafe.InitBlockUnaligned(handle.DangerousGetHandle().ToPointer(), 0xCD, (uint)heapSize);
                }

                if (mango_version_major() != MANGO_VERSION_MAJOR ||
                    mango_version_minor() != MANGO_VERSION_MINOR)
                    throw new NotSupportedException();

                var vm = mango_initialize(handle, (UIntPtr)heapSize, (UIntPtr)stackSize, IntPtr.Zero);
                if (vm != handle.DangerousGetHandle())
                    throw new NotSupportedException();

                success = true;
                return new Process(handle);
            }
            finally
            {
                if (addedRef)
                    handle.DangerousRelease();
                if (!success)
                    handle.Dispose();
            }
        }

        public byte[] CreateMemoryDump()
        {
            return _handle.ToArray();
        }

        public Snapshot CreateSnapshot()
        {
            return Snapshot.LoadInternal(_handle.ToArray(), _symbols.ToImmutableArray());
        }

        public void Dispose()
        {
            _handle.Dispose();
        }

        public int GetHeapSize()
        {
            return (int)mango_heap_size(_handle);
        }

        public int GetHeapUsed()
        {
            return (int)mango_heap_size(_handle) - (int)mango_heap_available(_handle);
        }

        public int GetStackSize()
        {
            return (int)mango_stack_size(_handle);
        }

        public ImmutableArray<StackFrame> GetStackTrace()
        {
            var handle = _handle;
            var addedRef = false;
            var success = false;
            try
            {
                handle.DangerousAddRef(ref addedRef);

                Span<byte> memory;
                unsafe
                {
                    memory = new Span<byte>(handle.DangerousGetHandle().ToPointer(), handle.Length);
                }

                var stackTrace = StackFrame.CreateStackTraceFrom(memory, _symbols.ToImmutableArray());
                success = true;
                return stackTrace;
            }
            finally
            {
                if (addedRef)
                    handle.DangerousRelease();
                if (!success)
                    handle.Dispose();
            }
        }

        public int GetSystemCall()
        {
            return mango_syscall(_handle);
        }

        public void ImportModule(ReadOnlySpan<byte> name, ReadOnlySpan<byte> image, EmittedModule symbol = null)
        {
            if (name.Length != Unsafe.SizeOf<mango_module_name>())
                throw new ArgumentException();
            if (image.Length > ushort.MaxValue)
                throw new ArgumentException();

            Span<byte> unmanagedMemory;
            unsafe
            {
                unmanagedMemory = new Span<byte>(Marshal.AllocHGlobal(image.Length).ToPointer(), image.Length);
            }

            image.CopyTo(unmanagedMemory);

            var handle = _handle;
            var addedRef = false;
            var success = false;
            try
            {
                handle.DangerousAddRef(ref addedRef);

                var error = mango_module_import(
                    handle,
                    ref MemoryMarshal.GetReference(name),
                    ref MemoryMarshal.GetReference(unmanagedMemory),
                    (UIntPtr)unmanagedMemory.Length,
                    IntPtr.Zero);

                if (error != MANGO_E_SUCCESS)
                    throw new Exception("Error: " + error);

                success = true;
            }
            finally
            {
                if (addedRef)
                    handle.DangerousRelease();
                if (!success)
                    handle.Dispose();
            }

            _symbols.Add(symbol);
        }

        public ErrorCode Run()
        {
            return (ErrorCode)mango_run(_handle);
        }
    }
}
