using System;
using System.Runtime.InteropServices;
using Mango.Compiler.Utilities;

namespace Mango.Compiler.Symbols
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct TypeLayout : IEquatable<TypeLayout>
    {
        private readonly int _size;
        private readonly int _alignment;

        public TypeLayout(int size, int alignment)
        {
            if (alignment <= 0 || (alignment & (alignment - 1)) != 0)
                throw new ArgumentOutOfRangeException(nameof(alignment));
            if (size < 0 || (size & (alignment - 1)) != 0)
                throw new ArgumentOutOfRangeException(nameof(size));

            _size = size;
            _alignment = alignment;
        }

        public int Alignment => _alignment;

        public int Size => _size;

        public bool Equals(TypeLayout other)
        {
            return (_size == other._size) && (_alignment == other._alignment);
        }

        public override bool Equals(object obj)
        {
            return (obj is TypeLayout other) && Equals(other);
        }

        public override int GetHashCode()
        {
            return Hash.Combine(_size, _alignment);
        }
    }
}
