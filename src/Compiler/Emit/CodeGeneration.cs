using System;
using System.Collections.Immutable;
using System.Linq;
using Mango.Compiler.Symbols;
using Mango.Compiler.Verification;
using static Interop.Libmango;

namespace Mango.Compiler.Emit
{
    internal static class CodeGeneration
    {
        internal static int CalculateLocalsSlotCount(FunctionSymbol function)
        {
            var slotCount = 0;
            foreach (var local in function.Locals)
            {
                slotCount += (local.Type.TypeLayout.Size + 3) / 4;
            }
            return slotCount;
        }

        internal static int CalculateMaxStack(VerifiedFunction function)
        {
            var maxStack = 0;

            foreach (var instruction in function.Instructions)
            {
                var stack = instruction.Stack;
                var stackSize = 0;
                while (!stack.IsEmpty)
                {
                    stack = stack.Pop(out var type);
                    stackSize += (type.TypeLayout.Size + 3) / 4;
                }
                if (maxStack < stackSize)
                {
                    maxStack = stackSize;
                }
            }

            return maxStack;
        }

        internal static int CalculateParametersSlotCount(FunctionSymbol function)
        {
            var slotCount = 0;
            foreach (var parameter in function.Parameters)
            {
                slotCount += (parameter.Type.TypeLayout.Size + 3) / 4;
            }
            return slotCount;
        }

        internal static int CalculateReturnsSlotCount(FunctionSymbol function)
        {
            var slotCount = 0;
            if (!function.ReturnsVoid)
            {
                slotCount += (function.ReturnType.TypeLayout.Size + 3) / 4;
            }
            return slotCount;
        }

        internal static int CalculateStackSlot(ImmutableStack<TypeSymbol> stack, ParameterSymbol parameter)
        {
            var function = (FunctionSymbol)parameter.ContainingSymbol;
            var slot = 0;
            foreach (var type in stack)
                slot += (type.TypeLayout.Size + 3) / 4;
            foreach (var local in function.Locals)
                slot += (local.Type.TypeLayout.Size + 3) / 4;
            foreach (var p in function.Parameters.Reverse())
                if (p == parameter) return slot; else slot += (p.Type.TypeLayout.Size + 3) / 4;
            throw new Exception();
        }

        internal static int CalculateStackSlot(ImmutableStack<TypeSymbol> stack, LocalSymbol local)
        {
            var function = (FunctionSymbol)local.ContainingSymbol;
            var slot = 0;
            foreach (var type in stack)
                slot += (type.TypeLayout.Size + 3) / 4;
            foreach (var l in function.Locals.Reverse())
                if (l == local) return slot; else slot += (l.Type.TypeLayout.Size + 3) / 4;
            throw new Exception();
        }

        internal static mango_opcode Select(TypeSymbol type, mango_opcode i32)
        {
            return type.SpecialType == SpecialType.Int32 ? i32 :
                   throw new NotSupportedException();
        }

        internal static mango_opcode Select(TypeSymbol type, mango_opcode i32, mango_opcode i64)
        {
            return type.SpecialType == SpecialType.Int32 ? i32 :
                   type.SpecialType == SpecialType.Int64 ? i64 :
                   throw new NotSupportedException();
        }

        internal static mango_opcode Select(TypeSymbol type, mango_opcode i32, mango_opcode i64, mango_opcode reference)
        {
            return type.SpecialType == SpecialType.Int32 ? i32 :
                   type.SpecialType == SpecialType.Int64 ? throw new NotSupportedException() :
                   type.TypeKind == TypeKind.Reference ? reference :
                   throw new NotSupportedException();
        }

        internal static mango_opcode Select(TypeSymbol type, mango_opcode i32, mango_opcode i64, mango_opcode f32, mango_opcode f64)
        {
            return type.SpecialType == SpecialType.Int32 ? i32 :
                   type.SpecialType == SpecialType.Int64 ? i64 :
                   type.SpecialType == SpecialType.Float32 ? f32 :
                   type.SpecialType == SpecialType.Float64 ? f64 :
                   throw new NotSupportedException();
        }

        internal static mango_opcode Select(TypeSymbol type, mango_opcode i32, mango_opcode i64, mango_opcode f32, mango_opcode f64, mango_opcode reference)
        {
            return type.SpecialType == SpecialType.Int32 ? i32 :
                   type.SpecialType == SpecialType.Int64 ? i64 :
                   type.SpecialType == SpecialType.Float32 ? f32 :
                   type.SpecialType == SpecialType.Float64 ? f64 :
                   type.TypeKind == TypeKind.Reference ? reference :
                   throw new NotSupportedException();
        }

        internal static mango_opcode Select(TypeSymbol type, mango_opcode i8, mango_opcode i16, mango_opcode i32, mango_opcode i64, mango_opcode u8, mango_opcode u16, mango_opcode u32, mango_opcode u64, mango_opcode f32, mango_opcode f64, mango_opcode reference)
        {
            return type.SpecialType == SpecialType.Int8 ? i8 :
                   type.SpecialType == SpecialType.Int16 ? i16 :
                   type.SpecialType == SpecialType.Int32 ? i32 :
                   type.SpecialType == SpecialType.Int64 ? i64 :
                   type.SpecialType == SpecialType.UInt8 ? u8 :
                   type.SpecialType == SpecialType.UInt16 ? u16 :
                   type.SpecialType == SpecialType.UInt32 ? u32 :
                   type.SpecialType == SpecialType.UInt64 ? u64 :
                   type.SpecialType == SpecialType.Float32 ? f32 :
                   type.SpecialType == SpecialType.Float64 ? f64 :
                   type.TypeKind == TypeKind.Reference ? reference :
                   throw new NotSupportedException();
        }
    }
}
