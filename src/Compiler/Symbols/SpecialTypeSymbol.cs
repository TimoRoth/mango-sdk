namespace Mango.Compiler.Symbols
{
    internal sealed class SpecialTypeSymbol : NamedTypeSymbol
    {
        private const int SpecialTypeCount = 13;

        private static readonly NamedTypeSymbol[] s_specialTypes = new NamedTypeSymbol[SpecialTypeCount]
        {
            null,
            new SpecialTypeSymbol("void", SpecialType.Void),
            new SpecialTypeSymbol("bool", SpecialType.Bool),
            new SpecialTypeSymbol("i8", SpecialType.Int8),
            new SpecialTypeSymbol("i16", SpecialType.Int16),
            new SpecialTypeSymbol("i32", SpecialType.Int32),
            new SpecialTypeSymbol("i64", SpecialType.Int64),
            new SpecialTypeSymbol("u8", SpecialType.UInt8),
            new SpecialTypeSymbol("u16", SpecialType.UInt16),
            new SpecialTypeSymbol("u32", SpecialType.UInt32),
            new SpecialTypeSymbol("u64", SpecialType.UInt64),
            new SpecialTypeSymbol("f32", SpecialType.Float32),
            new SpecialTypeSymbol("f64", SpecialType.Float64),
        };

        private readonly string _name;
        private readonly SpecialType _specialType;

        private SpecialTypeSymbol(string name, SpecialType specialType)
        {
            if (string.IsNullOrEmpty(name))
                throw new System.ArgumentException();
            if (unchecked((uint)specialType >= SpecialTypeCount))
                throw new System.ArgumentException();

            _name = name;
            _specialType = specialType;
        }

        public override string Name => _name;

        public override SpecialType SpecialType => _specialType;

        internal static NamedTypeSymbol GetSpecialType(SpecialType specialType) => unchecked((uint)specialType < SpecialTypeCount ? s_specialTypes[(int)specialType] : null);
    }
}
