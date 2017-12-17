using System;
using System.IO;
using System.Runtime.InteropServices;
using static Interop.Libmango;
using static Interop.Libmango.mango_feature_flags;

namespace Mango.Compiler.Emit
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct ByteCode
    {
        private readonly byte _byte0;
        private readonly byte _byte1;
        private readonly byte _byte2;
        private readonly byte _byte3;
        private readonly byte _byte4;
        private readonly byte _byte5;
        private readonly byte _byte6;
        private readonly byte _length;
        private readonly mango_feature_flags _features;

        public ByteCode(mango_opcode opcode) : this()
        {
            _byte0 = (byte)opcode;
            _length = 1;
            _features = GetFeatures(opcode);
        }

        public ByteCode(mango_opcode opcode, byte u8) : this()
        {
            _byte0 = (byte)opcode;
            _byte1 = u8;
            _length = 2;
            _features = GetFeatures(opcode);
        }

        public ByteCode(mango_opcode opcode, sbyte i8) : this(opcode, u8: unchecked((byte)i8))
        {
        }

        public ByteCode(mango_opcode opcode, ushort u16) : this()
        {
            _byte0 = (byte)opcode;
            _byte1 = unchecked((byte)u16);
            _byte2 = unchecked((byte)(u16 >> 8));
            _length = 3;
            _features = GetFeatures(opcode);
        }

        public ByteCode(mango_opcode opcode, short i16) : this(opcode, u16: unchecked((ushort)i16))
        {
        }

        public ByteCode(mango_opcode opcode, uint u32) : this()
        {
            _byte0 = (byte)opcode;
            _byte1 = unchecked((byte)u32);
            _byte2 = unchecked((byte)(u32 >> 8));
            _byte3 = unchecked((byte)(u32 >> 16));
            _byte4 = unchecked((byte)(u32 >> 24));
            _length = 5;
            _features = GetFeatures(opcode);
        }

        public ByteCode(mango_opcode opcode, int i32) : this(opcode, u32: unchecked((uint)i32))
        {
        }

        public ByteCode(mango_opcode opcode, mango_function_token ftn) : this()
        {
            _byte0 = (byte)opcode;
            _byte1 = ftn.module;
            _byte2 = unchecked((byte)ftn.offset);
            _byte3 = unchecked((byte)(ftn.offset >> 8));
            _length = 4;
            _features = GetFeatures(opcode);
        }

        public ByteCode(mango_opcode opcode, sbyte i8, ushort u16) : this()
        {
            _byte0 = (byte)opcode;
            _byte1 = unchecked((byte)i8);
            _byte2 = unchecked((byte)u16);
            _byte3 = unchecked((byte)(u16 >> 8));
            _length = 4;
            _features = GetFeatures(opcode);
        }

        public ByteCode(mango_opcode opcode1, mango_opcode opcode2, sbyte i8) : this()
        {
            _byte0 = (byte)opcode1;
            _byte1 = (byte)opcode2;
            _byte2 = unchecked((byte)i8);
            _length = 3;
            _features = GetFeatures(opcode1) | GetFeatures(opcode2);
        }

        public ByteCode(mango_opcode opcode1, mango_opcode opcode2, short i16) : this()
        {
            _byte0 = (byte)opcode1;
            _byte1 = (byte)opcode2;
            _byte2 = unchecked((byte)i16);
            _byte3 = unchecked((byte)(i16 >> 8));
            _length = 4;
            _features = GetFeatures(opcode1) | GetFeatures(opcode2);
        }

        public ByteCode(mango_opcode opcode1, ushort u16, mango_opcode opcode2, mango_function_token ftn) : this()
        {
            _byte0 = (byte)opcode1;
            _byte1 = unchecked((byte)u16);
            _byte2 = unchecked((byte)(u16 >> 8));
            _byte3 = (byte)opcode2;
            _byte4 = ftn.module;
            _byte5 = unchecked((byte)ftn.offset);
            _byte6 = unchecked((byte)(ftn.offset >> 8));
            _length = 7;
            _features = GetFeatures(opcode1) | GetFeatures(opcode2);
        }

        public mango_feature_flags Features => _features;

        public int Length => _length;

        public int CopyTo(byte[] array, int offset)
        {
            switch (_length)
            {
            case 0:
                return 0;
            case 1:
                array[offset + 0] = _byte0;
                return 1;
            case 2:
                array[offset + 0] = _byte0;
                array[offset + 1] = _byte1;
                return 2;
            case 3:
                array[offset + 0] = _byte0;
                array[offset + 1] = _byte1;
                array[offset + 2] = _byte2;
                return 3;
            case 4:
                array[offset + 0] = _byte0;
                array[offset + 1] = _byte1;
                array[offset + 2] = _byte2;
                array[offset + 3] = _byte3;
                return 4;
            case 5:
                array[offset + 0] = _byte0;
                array[offset + 1] = _byte1;
                array[offset + 2] = _byte2;
                array[offset + 3] = _byte3;
                array[offset + 4] = _byte4;
                return 5;
            case 6:
                array[offset + 0] = _byte0;
                array[offset + 1] = _byte1;
                array[offset + 2] = _byte2;
                array[offset + 3] = _byte3;
                array[offset + 4] = _byte4;
                array[offset + 5] = _byte5;
                return 6;
            case 7:
                array[offset + 0] = _byte0;
                array[offset + 1] = _byte1;
                array[offset + 2] = _byte2;
                array[offset + 3] = _byte3;
                array[offset + 4] = _byte4;
                array[offset + 5] = _byte5;
                array[offset + 6] = _byte6;
                return 7;
            default:
                throw new Exception();
            }
        }

        private static mango_feature_flags GetFeatures(mango_opcode opcode)
        {
            if ((int)opcode < 0x60)
            {
                return 0;
            }
            else if ((int)opcode < 0x90)
            {
                return MANGO_FEATURE_REFS;
            }
            else if ((int)opcode < 0xC0)
            {
                return MANGO_FEATURE_I64;
            }
            else if ((int)opcode < 0xE0)
            {
                return MANGO_FEATURE_F32;
            }
            else
            {
                return MANGO_FEATURE_F64;
            }
        }
    }
}
