#pragma warning disable IDE1006

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Libmango
    {
        internal const int MANGO_IMAGE_MAGIC = 0x00FF;

        internal enum mango_func_attributes
        {
            MANGO_FD_NONE = 0x0,
            MANGO_FD_INIT_LOCALS = 0x1,
        }

        internal enum mango_module_attributes
        {
            MANGO_MD_NONE = 0x0,
            MANGO_MD_EXECUTABLE = 0x2,
        }

        internal enum mango_opcode
        {
            NOP = 0x00,
            BREAK = 0x01,
            HALT = 0x02,

            SWAP = 0x03,
            POP_X32 = 0x04,
            POP_X64 = 0x05,
            DUP_X32 = 0x06,
            DUP_X64 = 0x07,
            OVER = 0x08,
            ROT = 0x09,
            NIP = 0x0A,
            TUCK = 0x0B,

            LDLOC_I8 = 0x0C,
            LDLOC_U8 = 0x0D,
            LDLOC_I16 = 0x0E,
            LDLOC_U16 = 0x0F,
            LDLOC_X32 = 0x10,
            LDLOC_X64 = 0x11,
            LDLOCA = 0x12,
            STLOC_X32 = 0x13,
            STLOC_X64 = 0x14,

            RET = 0x18,
            RET_X32 = 0x19,
            RET_X64 = 0x1A,
            CALLI = 0x1B,
            CALL_S = 0x1C,
            CALL = 0x1D,
            SYSCALL = 0x1E,

            BR_S = 0x20,
            BRFALSE_S = 0x21,
            BRTRUE_S = 0x22,
            BR = 0x23,
            BRFALSE = 0x24,
            BRTRUE = 0x25,

            LDC_I32_M1 = 0x28,
            LDC_I32_0 = 0x29,
            LDC_I32_1 = 0x2A,
            LDC_I32_2 = 0x2B,
            LDC_I32_3 = 0x2C,
            LDC_I32_4 = 0x2D,
            LDC_I32_5 = 0x2E,
            LDC_I32_6 = 0x2F,
            LDC_I32_7 = 0x30,
            LDC_I32_8 = 0x31,
            LDC_I32_S = 0x32,
            LDC_X32 = 0x33,
            LDC_X64 = 0x34,
            LDFTN = 0x35,

            ADD_I32 = 0x40,
            SUB_I32 = 0x41,
            MUL_I32 = 0x42,
            DIV_I32 = 0x43,
            DIV_I32_UN = 0x44,
            REM_I32 = 0x45,
            REM_I32_UN = 0x46,
            NEG_I32 = 0x47,

            SHL_I32 = 0x48,
            SHR_I32 = 0x49,
            SHR_I32_UN = 0x4A,
            AND_I32 = 0x4B,
            OR_I32 = 0x4C,
            XOR_I32 = 0x4D,
            NOT_I32 = 0x4E,

            CEQ_I32 = 0x4F,
            CNE_I32 = 0x50,
            CGT_I32 = 0x51,
            CGT_I32_UN = 0x52,
            CGE_I32 = 0x53,
            CGE_I32_UN = 0x54,
            CLT_I32 = 0x55,
            CLT_I32_UN = 0x56,
            CLE_I32 = 0x57,
            CLE_I32_UN = 0x58,

            CONV_I8_I32 = 0x59,
            CONV_U8_I32 = 0x5A,
            CONV_I16_I32 = 0x5B,
            CONV_U16_I32 = 0x5C,

            NEWOBJ = 0x60,
            NEWARR = 0x61,
            SLICE1 = 0x62,
            SLICE2 = 0x63,

            LDFLD_I8 = 0x68,
            LDFLD_U8 = 0x69,
            LDFLD_I16 = 0x6A,
            LDFLD_U16 = 0x6B,
            LDFLD_X32 = 0x6C,
            LDFLD_X64 = 0x6D,
            LDFLDA = 0x6E,
            STFLD_X8 = 0x6F,
            STFLD_X16 = 0x70,
            STFLD_X32 = 0x71,
            STFLD_X64 = 0x72,

            LDELEM_I8 = 0x7E,
            LDELEM_U8 = 0x7F,
            LDELEM_I16 = 0x80,
            LDELEM_U16 = 0x81,
            LDELEM_X32 = 0x82,
            LDELEM_X64 = 0x83,
            LDELEMA = 0x84,
            STELEM_X8 = 0x85,
            STELEM_X16 = 0x86,
            STELEM_X32 = 0x87,
            STELEM_X64 = 0x88,

            ADD_I64 = 0x90,
            SUB_I64 = 0x91,
            MUL_I64 = 0x92,
            DIV_I64 = 0x93,
            DIV_I64_UN = 0x94,
            REM_I64 = 0x95,
            REM_I64_UN = 0x96,
            NEG_I64 = 0x97,

            SHL_I64 = 0x98,
            SHR_I64 = 0x99,
            SHR_I64_UN = 0x9A,
            AND_I64 = 0x9B,
            OR_I64 = 0x9C,
            XOR_I64 = 0x9D,
            NOT_I64 = 0x9E,

            CEQ_I64 = 0x9F,
            CNE_I64 = 0xA0,
            CGT_I64 = 0xA1,
            CGT_I64_UN = 0xA2,
            CGE_I64 = 0xA3,
            CGE_I64_UN = 0xA4,
            CLT_I64 = 0xA5,
            CLT_I64_UN = 0xA6,
            CLE_I64 = 0xA7,
            CLE_I64_UN = 0xA8,

            CONV_I8_I64 = 0xA9,
            CONV_U8_I64 = 0xAA,
            CONV_I16_I64 = 0xAB,
            CONV_U16_I64 = 0xAC,
            CONV_I32_I64 = 0xAD,
            CONV_U32_I64 = 0xAE,

            CONV_I64_I32 = 0xAF,
            CONV_U64_I32 = 0xB0,
            CONV_I64_F32 = 0xB1,
            CONV_U64_F32 = 0xB2,
            CONV_I64_F64 = 0xB3,
            CONV_U64_F64 = 0xB4,

            ADD_F32 = 0xC0,
            SUB_F32 = 0xC1,
            MUL_F32 = 0xC2,
            DIV_F32 = 0xC3,
            REM_F32 = 0xC4,
            NEG_F32 = 0xC5,

            CEQ_F32 = 0xC6,
            CEQ_F32_UN = 0xC7,
            CNE_F32 = 0xC8,
            CNE_F32_UN = 0xC9,
            CGT_F32 = 0xCA,
            CGT_F32_UN = 0xCB,
            CGE_F32 = 0xCC,
            CGE_F32_UN = 0xCD,
            CLT_F32 = 0xCE,
            CLT_F32_UN = 0xCF,
            CLE_F32 = 0xD0,
            CLE_F32_UN = 0xD1,

            CONV_I8_F32 = 0xD2,
            CONV_U8_F32 = 0xD3,
            CONV_I16_F32 = 0xD4,
            CONV_U16_F32 = 0xD5,
            CONV_I32_F32 = 0xD6,
            CONV_U32_F32 = 0xD7,

            CONV_F32_I32 = 0xD8,
            CONV_F32_I32_UN = 0xD9,
            CONV_F32_I64 = 0xDA,
            CONV_F32_I64_UN = 0xDB,
            CONV_F32_F64 = 0xDC,

            ADD_F64 = 0xE0,
            SUB_F64 = 0xE1,
            MUL_F64 = 0xE2,
            DIV_F64 = 0xE3,
            REM_F64 = 0xE4,
            NEG_F64 = 0xE5,

            CEQ_F64 = 0xE6,
            CEQ_F64_UN = 0xE7,
            CNE_F64 = 0xE8,
            CNE_F64_UN = 0xE9,
            CGT_F64 = 0xEA,
            CGT_F64_UN = 0xEB,
            CGE_F64 = 0xEC,
            CGE_F64_UN = 0xED,
            CLT_F64 = 0xEE,
            CLT_F64_UN = 0xEF,
            CLE_F64 = 0xF0,
            CLE_F64_UN = 0xF1,

            CONV_I8_F64 = 0xF2,
            CONV_U8_F64 = 0xF3,
            CONV_I16_F64 = 0xF4,
            CONV_U16_F64 = 0xF5,
            CONV_I32_F64 = 0xF6,
            CONV_U32_F64 = 0xF7,

            CONV_F64_I32 = 0xF8,
            CONV_F64_I32_UN = 0xF9,
            CONV_F64_I64 = 0xFA,
            CONV_F64_I64_UN = 0xFB,
            CONV_F64_F32 = 0xFC,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct mango_app_info
        {
            internal ushort features;
            internal byte module_count;
            internal byte entry_point_0;
            internal byte entry_point_1;
            internal byte entry_point_2;
            internal byte entry_point_3;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct mango_func_def
        {
            internal byte attributes;
            internal byte max_stack;
            internal byte arg_count;
            internal byte loc_count;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct mango_module_def
        {
            internal ushort magic;
            internal byte attributes;
            internal byte import_count;
            internal byte initializer_0;
            internal byte initializer_1;
            internal byte initializer_2;
            internal byte initializer_3;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
        internal struct mango_module_name
        {
        }
    }
}
