#pragma warning disable IDE1006

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Libmango
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct mango_function_token
        {
            internal byte _reserved;
            internal byte module;
            internal ushort offset;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct mango_module
        {
            internal uint _image_0;
            internal uint _image_1;

            internal ushort image_size;

            internal byte name_module;
            internal byte name_index;

            internal byte init_next;
            internal byte init_prev;
            internal byte init_flags;

            internal byte import_count;
            internal mango_ref imports;

            internal uint _reserved_0;

            internal uint _context_0;
            internal uint _context_1;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct mango_ref
        {
            internal int address;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct mango_stack_frame
        {
            internal byte pop;
            internal byte module;
            internal ushort ip;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct mango_stackval
        {
            [FieldOffset(0)]
            internal mango_stack_frame sf;
            [FieldOffset(0)]
            internal mango_function_token ftn;
            [FieldOffset(0)]
            internal int i32;
            [FieldOffset(0)]
            internal uint u32;
            [FieldOffset(0)]
            internal float f32;
            [FieldOffset(0)]
            internal mango_ref @ref;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct mango_vm
        {
            internal byte version;

            internal byte result;
            internal ushort syscall;

            internal uint heap_size;
            internal uint heap_used;

            internal mango_module_name startup_module_name;
            internal mango_ref modules;
            internal byte modules_created;
            internal byte modules_imported;

            internal byte init_head;
            internal byte init_flags;

            internal ushort stack_size;
            internal ushort rp;
            internal ushort sp;
            internal ushort sp_expected;
            internal mango_stack_frame sf;

            internal uint @base;

            internal uint _reserved_0;
            internal uint _reserved_1;

            internal uint _context_0;
            internal uint _context_1;
        }
    }
}
