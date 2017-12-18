#pragma warning disable IDE1006

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Libmango
    {
        internal const int MANGO_VERSION_MAJOR = 1;
        internal const int MANGO_VERSION_MINOR = 0;

        internal enum mango_alloc_flags
        {
            MANGO_ALLOC_ZERO_MEMORY = 0x8,
        }

        [Flags]
        internal enum mango_feature_flags
        {
            MANGO_FEATURE_I64 = 0x10,
            MANGO_FEATURE_F32 = 0x20,
            MANGO_FEATURE_F64 = 0x40,
            MANGO_FEATURE_REFS = 0x80,
        }

        internal enum mango_result
        {
            MANGO_E_SUCCESS = 0,
            MANGO_E_ARGUMENT = 64,
            MANGO_E_ARGUMENT_NULL = 65,
            MANGO_E_INVALID_OPERATION = 66,
            MANGO_E_NOT_SUPPORTED = 67,
            MANGO_E_NOT_IMPLEMENTED = 68,
            MANGO_E_BAD_IMAGE_FORMAT = 69,
            MANGO_E_OUT_OF_MEMORY = 80,
            MANGO_E_SECURITY = 81,
            MANGO_E_STACK_OVERFLOW = 82,
            MANGO_E_STACK_IMBALANCE = 83,
            MANGO_E_ARITHMETIC = 84,
            MANGO_E_DIVIDE_BY_ZERO = 85,
            MANGO_E_INDEX_OUT_OF_RANGE = 86,
            MANGO_E_INVALID_PROGRAM = 87,
            MANGO_E_NULL_REFERENCE = 88,
            MANGO_E_BREAKPOINT = 110,
            MANGO_E_TIMEOUT = 111,
            MANGO_E_SYSCALL = 112,
        }

        [DllImport(Libraries.Libmango, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr mango_context(
            SafeMangoHandle vm);

        [DllImport(Libraries.Libmango, CallingConvention = CallingConvention.Cdecl)]
        internal static extern mango_result mango_error(
            SafeMangoHandle vm,
            mango_result error);

        [DllImport(Libraries.Libmango, CallingConvention = CallingConvention.Cdecl)]
        internal static extern mango_feature_flags mango_features();

        [DllImport(Libraries.Libmango, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr mango_heap_alloc(
            SafeMangoHandle vm,
            UIntPtr count,
            UIntPtr size,
            UIntPtr alignment,
            mango_alloc_flags flags);

        [DllImport(Libraries.Libmango, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr mango_heap_available(
            SafeMangoHandle vm);

        [DllImport(Libraries.Libmango, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr mango_heap_size(
            SafeMangoHandle vm);

        [DllImport(Libraries.Libmango, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr mango_initialize(
            SafeMangoHandle address,
            UIntPtr heap_size,
            UIntPtr stack_size,
            IntPtr context);

        [DllImport(Libraries.Libmango, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr mango_module_context(
            SafeMangoHandle vm);

        [DllImport(Libraries.Libmango, CallingConvention = CallingConvention.Cdecl)]
        internal static extern mango_result mango_module_import(
            SafeMangoHandle vm,
            ref byte name,
            ref byte image,
            UIntPtr size,
            IntPtr context);

        [DllImport(Libraries.Libmango, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr mango_module_missing(
            SafeMangoHandle vm);

        [DllImport(Libraries.Libmango, CallingConvention = CallingConvention.Cdecl)]
        internal static extern mango_result mango_run(
            SafeMangoHandle vm);

        [DllImport(Libraries.Libmango, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr mango_stack_alloc(
            SafeMangoHandle vm,
            UIntPtr size,
            mango_alloc_flags flags);

        [DllImport(Libraries.Libmango, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr mango_stack_available(
            SafeMangoHandle vm);

        [DllImport(Libraries.Libmango, CallingConvention = CallingConvention.Cdecl)]
        internal static extern mango_result mango_stack_free(
            SafeMangoHandle vm,
            UIntPtr size);

        [DllImport(Libraries.Libmango, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr mango_stack_size(
            SafeMangoHandle vm);

        [DllImport(Libraries.Libmango, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr mango_stack_top(
            SafeMangoHandle vm);

        [DllImport(Libraries.Libmango, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mango_syscall(
            SafeMangoHandle vm);

        [DllImport(Libraries.Libmango, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mango_version_major();

        [DllImport(Libraries.Libmango, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mango_version_minor();
    }
}
