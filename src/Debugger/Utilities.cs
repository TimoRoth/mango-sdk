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
            return memory.Slice(Unsafe.SizeOf<mango_vm>() + vm.sp * Unsafe.SizeOf<mango_stackval>(), (vm.stack_size - vm.sp) * Unsafe.SizeOf<mango_stackval>()).NonPortableCast<byte, mango_stackval>();
        }

        internal static ReadOnlySpan<mango_module> GetModules(ReadOnlySpan<byte> memory, in mango_vm vm)
        {
            return memory.Slice(vm.modules.address, vm.modules_imported * Unsafe.SizeOf<mango_module>()).NonPortableCast<byte, mango_module>();
        }

        internal static ReadOnlySpan<mango_stack_frame> GetReturnStack(ReadOnlySpan<byte> memory, in mango_vm vm)
        {
            return memory.Slice(Unsafe.SizeOf<mango_vm>(), vm.rp * Unsafe.SizeOf<mango_stack_frame>()).NonPortableCast<byte, mango_stack_frame>();
        }

        internal static ref readonly mango_vm GetVM(ReadOnlySpan<byte> memory)
        {
            return ref MemoryMarshal.GetReference(memory.Slice(0, Unsafe.SizeOf<mango_vm>()).NonPortableCast<byte, mango_vm>());
        }

        internal static void SanitizeMemory(Span<byte> memory)
        {
            var vm = memory.Slice(0, Unsafe.SizeOf<mango_vm>()).NonPortableCast<byte, mango_vm>();
            var modules = memory.Slice(vm[0].modules.address, vm[0].modules_imported * Unsafe.SizeOf<mango_module>()).NonPortableCast<byte, mango_module>();

            vm[0]._reserved_0 = 0;
            vm[0]._reserved_1 = 0;
            vm[0]._context_0 = 0;
            vm[0]._context_1 = 0;

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
