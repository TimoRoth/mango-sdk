using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Interop.Libmango;

namespace Mango.Debugger
{
    internal static class Utilities
    {
        internal static bool PlatformSupported => BitConverter.IsLittleEndian && IntPtr.Size == 8;

        internal static ReadOnlySpan<mango_stackval> GetEvaluationStack(ReadOnlySpan<byte> memory, in mango_vm vm)
        {
            return MemoryMarshal.Cast<byte, mango_stackval>(memory.Slice(Unsafe.SizeOf<mango_vm>() + vm.sp * Unsafe.SizeOf<mango_stackval>(), (vm.stack_size - vm.sp) * Unsafe.SizeOf<mango_stackval>()));
        }

        internal static ReadOnlySpan<mango_module> GetModules(ReadOnlySpan<byte> memory, in mango_vm vm)
        {
            return MemoryMarshal.Cast<byte, mango_module>(memory.Slice(vm.modules.address, vm.modules_imported * Unsafe.SizeOf<mango_module>()));
        }

        internal static ReadOnlySpan<mango_stack_frame> GetReturnStack(ReadOnlySpan<byte> memory, in mango_vm vm)
        {
            return MemoryMarshal.Cast<byte, mango_stack_frame>(memory.Slice(Unsafe.SizeOf<mango_vm>(), vm.rp * Unsafe.SizeOf<mango_stack_frame>()));
        }

        internal static ref readonly mango_vm GetVM(ReadOnlySpan<byte> memory)
        {
            return ref MemoryMarshal.GetReference(MemoryMarshal.Cast<byte, mango_vm>(memory.Slice(0, Unsafe.SizeOf<mango_vm>())));
        }

        internal static void SanitizeMemory(Span<byte> memory)
        {
            ref var vm = ref MemoryMarshal.GetReference(MemoryMarshal.Cast<byte, mango_vm>(memory.Slice(0, Unsafe.SizeOf<mango_vm>())));
            var modules = MemoryMarshal.Cast<byte, mango_module>(memory.Slice(vm.modules.address, vm.modules_imported * Unsafe.SizeOf<mango_module>()));

            vm._reserved_0 = 0;
            vm._reserved_1 = 0;
            vm._context_0 = 0;
            vm._context_1 = 0;

            for (var i = 0; i < modules.Length; i++)
            {
                modules[i]._image_0 = 0;
                modules[i]._image_1 = 0;
                modules[i]._reserved_0 = 0;
                modules[i]._context_0 = 0;
                modules[i]._context_1 = 0;
            }
        }
    }
}
