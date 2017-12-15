using System;
using System.Collections.Immutable;
using System.Linq;
using Mango.Compiler.Symbols;

namespace Mango.Compiler.Analysis
{
    internal static class Verification
    {
        internal static TypeSymbol GetIntermediateType(TypeSymbol t)
        {
            var v = GetVerificationType(t);

            return v.SpecialType == SpecialType.Int8 ? SpecialTypeSymbol.GetSpecialType(SpecialType.Int32) :
                   v.SpecialType == SpecialType.Int16 ? SpecialTypeSymbol.GetSpecialType(SpecialType.Int32) :
                   v.SpecialType == SpecialType.Int32 ? SpecialTypeSymbol.GetSpecialType(SpecialType.Int32) :
                   v;
        }

        internal static TypeSymbol GetVerificationType(TypeSymbol t)
        {
            return t.SpecialType == SpecialType.Void ? throw new Exception() :
                   t.SpecialType == SpecialType.Bool ? SpecialTypeSymbol.GetSpecialType(SpecialType.Int8) :
                   t.SpecialType == SpecialType.Int8 ? SpecialTypeSymbol.GetSpecialType(SpecialType.Int8) :
                   t.SpecialType == SpecialType.UInt8 ? SpecialTypeSymbol.GetSpecialType(SpecialType.Int8) :
                   t.SpecialType == SpecialType.Int16 ? SpecialTypeSymbol.GetSpecialType(SpecialType.Int16) :
                   t.SpecialType == SpecialType.UInt16 ? SpecialTypeSymbol.GetSpecialType(SpecialType.Int16) :
                   t.SpecialType == SpecialType.Int32 ? SpecialTypeSymbol.GetSpecialType(SpecialType.Int32) :
                   t.SpecialType == SpecialType.UInt32 ? SpecialTypeSymbol.GetSpecialType(SpecialType.Int32) :
                   t.SpecialType == SpecialType.Int64 ? SpecialTypeSymbol.GetSpecialType(SpecialType.Int64) :
                   t.SpecialType == SpecialType.UInt64 ? SpecialTypeSymbol.GetSpecialType(SpecialType.Int64) :
                   t.SpecialType == SpecialType.Float32 ? SpecialTypeSymbol.GetSpecialType(SpecialType.Float32) :
                   t.SpecialType == SpecialType.Float64 ? SpecialTypeSymbol.GetSpecialType(SpecialType.Float64) :
                   t is ReferenceTypeSymbol reference ? new ReferenceTypeSymbol(GetVerificationType(reference.ReferencedType)) :
                   t is ArrayTypeSymbol array ? new ArrayTypeSymbol(GetVerificationType(array.ElementType), array.Length) :
                   t is SpanTypeSymbol span ? new SpanTypeSymbol(GetVerificationType(span.ElementType)) :
                   t is FunctionTypeSymbol function ? new FunctionTypeSymbol(GetVerificationType(function.ReturnType), function.ParameterTypes.Select(GetVerificationType).ToImmutableArray()) :
                   t is StructuredTypeSymbol ? t :
                   throw new Exception();
        }

        internal static TypeSymbol Merge(TypeSymbol s, TypeSymbol t)
        {
            return s.SpecialType == SpecialType.Void || t.SpecialType == SpecialType.Void ? throw new Exception() :
                   s == t ? s :
                   s.SpecialType == SpecialType.Null && t.TypeKind == TypeKind.Reference ? t :
                   s.TypeKind == TypeKind.Reference && t.SpecialType == SpecialType.Null ? s :
                   throw new Exception();
        }

        internal static bool VerifierAssignableTo(TypeSymbol s, TypeSymbol t)
        {
            var v = GetVerificationType(t);

            return s == v ||
                   s.SpecialType == SpecialType.Null && v.TypeKind == TypeKind.Reference ||
                   s.SpecialType == SpecialType.Int32 && v.SpecialType == SpecialType.Int8 ||
                   s.SpecialType == SpecialType.Int32 && v.SpecialType == SpecialType.Int16 ||
                   s.SpecialType == SpecialType.Int32 && v.SpecialType == SpecialType.Int32;
        }
    }
}
