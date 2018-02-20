using System;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Mango.Compiler.Symbols;

namespace Mango.Debugger
{
    public readonly struct TypedValue
    {
        private readonly int _offset;
        private readonly Snapshot _snapshot;
        private readonly TypeSymbol _type;

        internal TypedValue(int offset, TypeSymbol type, Snapshot snapshot)
        {
            _offset = offset;
            _type = type;
            _snapshot = snapshot;
        }

        public int MemoryOffset => _offset;

        public TypeSymbol Type => _type;

        public object Value
        {
            get
            {
                if (_snapshot != null && _type != null)
                {
                    switch (_type.SpecialType)
                    {
                    case SpecialType.Bool:
                        return _snapshot.MemoryDump[_offset] != 0;
                    case SpecialType.Int8:
                        return MemoryMarshal.Cast<byte, sbyte>(_snapshot.MemoryDump.Slice(_offset, sizeof(sbyte)))[0];
                    case SpecialType.Int16:
                        return MemoryMarshal.Cast<byte, short>(_snapshot.MemoryDump.Slice(_offset, sizeof(short)))[0];
                    case SpecialType.Int32:
                        return MemoryMarshal.Cast<byte, int>(_snapshot.MemoryDump.Slice(_offset, sizeof(int)))[0];
                    case SpecialType.Int64:
                        return MemoryMarshal.Cast<byte, long>(_snapshot.MemoryDump.Slice(_offset, sizeof(long)))[0];
                    case SpecialType.UInt8:
                        return MemoryMarshal.Cast<byte, byte>(_snapshot.MemoryDump.Slice(_offset, sizeof(byte)))[0];
                    case SpecialType.UInt16:
                        return MemoryMarshal.Cast<byte, ushort>(_snapshot.MemoryDump.Slice(_offset, sizeof(ushort)))[0];
                    case SpecialType.UInt32:
                        return MemoryMarshal.Cast<byte, uint>(_snapshot.MemoryDump.Slice(_offset, sizeof(uint)))[0];
                    case SpecialType.UInt64:
                        return MemoryMarshal.Cast<byte, ulong>(_snapshot.MemoryDump.Slice(_offset, sizeof(ulong)))[0];
                    case SpecialType.Float32:
                        return MemoryMarshal.Cast<byte, float>(_snapshot.MemoryDump.Slice(_offset, sizeof(float)))[0];
                    case SpecialType.Float64:
                        return MemoryMarshal.Cast<byte, double>(_snapshot.MemoryDump.Slice(_offset, sizeof(double)))[0];
                    }
                }

                return null;
            }
        }

        public ImmutableDictionary<FieldSymbol, TypedValue> GetFields()
        {
            var type = _type as StructuredTypeSymbol;
            if (type == null)
                return null;

            var offset = _offset;
            var builder = ImmutableDictionary.CreateBuilder<FieldSymbol, TypedValue>();

            foreach (var field in type.Fields)
            {
                builder.Add(field, new TypedValue(offset + field.FieldOffset, field.Type, _snapshot));
            }

            return builder.ToImmutable();
        }
    }
}
