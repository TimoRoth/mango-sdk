using System.Diagnostics;

namespace Mango.Compiler.Symbols
{
    internal sealed class SpecialTypeSymbol : TypeSymbol
    {
        private const int SpecialTypeCount = 14;

        private static readonly TypeSymbol[] s_specialTypes = new TypeSymbol[SpecialTypeCount]
        {
            null,
            new SpecialTypeSymbol("null", SpecialType.Null, 4, 4),
            new SpecialTypeSymbol("void", SpecialType.Void, 0, 1),
            new SpecialTypeSymbol("bool", SpecialType.Bool, 1, 1),
            new SpecialTypeSymbol("i8", SpecialType.Int8, 1, 1),
            new SpecialTypeSymbol("i16", SpecialType.Int16, 2, 2),
            new SpecialTypeSymbol("i32", SpecialType.Int32, 4, 4),
            new SpecialTypeSymbol("i64", SpecialType.Int64, 8, 8),
            new SpecialTypeSymbol("u8", SpecialType.UInt8, 1, 1),
            new SpecialTypeSymbol("u16", SpecialType.UInt16, 2, 2),
            new SpecialTypeSymbol("u32", SpecialType.UInt32, 4, 4),
            new SpecialTypeSymbol("u64", SpecialType.UInt64, 8, 8),
            new SpecialTypeSymbol("f32", SpecialType.Float32, 4, 4),
            new SpecialTypeSymbol("f64", SpecialType.Float64, 8, 8),
        };

        private readonly string _name;
        private readonly SpecialType _specialType;
        private readonly TypeLayout _typeLayout;

        private SpecialTypeSymbol(string name, SpecialType specialType, int size, int alignment)
        {
            Debug.Assert(name != null);
            Debug.Assert(unchecked((uint)specialType < SpecialTypeCount));

            _name = name;
            _specialType = specialType;
            _typeLayout = new TypeLayout(size, alignment);
        }

        public override Symbol ContainingSymbol => null;

        public override SymbolKind Kind => SymbolKind.SpecialType;

        public override string Name => _name;

        public override SpecialType SpecialType => _specialType;

        public override TypeKind TypeKind => TypeKind.Special;

        public override TypeLayout TypeLayout => _typeLayout;

        internal static TypeSymbol GetSpecialType(SpecialType specialType) => unchecked((uint)specialType < SpecialTypeCount ? s_specialTypes[(int)specialType] : null);
    }
}
