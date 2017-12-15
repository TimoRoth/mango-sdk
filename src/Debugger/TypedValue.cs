using System;
using System.Collections.Immutable;
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
                        return _snapshot.MemoryDump.Slice(_offset, sizeof(sbyte)).NonPortableCast<byte, sbyte>()[0];
                    case SpecialType.Int16:
                        return _snapshot.MemoryDump.Slice(_offset, sizeof(short)).NonPortableCast<byte, short>()[0];
                    case SpecialType.Int32:
                        return _snapshot.MemoryDump.Slice(_offset, sizeof(int)).NonPortableCast<byte, int>()[0];
                    case SpecialType.Int64:
                        return _snapshot.MemoryDump.Slice(_offset, sizeof(long)).NonPortableCast<byte, long>()[0];
                    case SpecialType.UInt8:
                        return _snapshot.MemoryDump[_offset];
                    case SpecialType.UInt16:
                        return _snapshot.MemoryDump.Slice(_offset, sizeof(ushort)).NonPortableCast<byte, ushort>()[0];
                    case SpecialType.UInt32:
                        return _snapshot.MemoryDump.Slice(_offset, sizeof(uint)).NonPortableCast<byte, uint>()[0];
                    case SpecialType.UInt64:
                        return _snapshot.MemoryDump.Slice(_offset, sizeof(ulong)).NonPortableCast<byte, ulong>()[0];
                    case SpecialType.Float32:
                        return _snapshot.MemoryDump.Slice(_offset, sizeof(float)).NonPortableCast<byte, float>()[0];
                    case SpecialType.Float64:
                        return _snapshot.MemoryDump.Slice(_offset, sizeof(double)).NonPortableCast<byte, double>()[0];
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
