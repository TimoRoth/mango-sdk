using Mango.Compiler.Syntax;

namespace Mango.Compiler.Analysis
{
    public interface IInterpreter<T>
    {
        T Add_Div_DivUn_Mul_Rem_RemUn_Sub(NoneInstructionSyntax instruction, T value1, T value2);
        T And_Or_Xor(NoneInstructionSyntax instruction, T value1, T value2);
        void Beq_BneUn(BranchInstructionSyntax instruction, T value1, T value2);
        void Bge_BgeUn_Bgt_BgtUn_Ble_BleUn_Blt_BltUn(BranchInstructionSyntax instruction, T value1, T value2);
        void Br(BranchInstructionSyntax instruction);
        void Break(NoneInstructionSyntax instruction);
        void Brfalse(BranchInstructionSyntax instruction, T value);
        void Brtrue(BranchInstructionSyntax instruction, T value);
        T Call_Syscall(FunctionInstructionSyntax instruction, T[] arguments);
        void CallVoid_SyscallVoid(FunctionInstructionSyntax instruction, T[] arguments);
        T Calli(TypeInstructionSyntax instruction, T[] arguments, T function);
        void CalliVoid(TypeInstructionSyntax instruction, T[] arguments, T function);
        T Ceq(NoneInstructionSyntax instruction, T value1, T value2);
        T Cgt_CgtUn_Clt_CltUn(NoneInstructionSyntax instruction, T value1, T value2);
        T Conv_ConvUn(TypeInstructionSyntax instruction, T value);
        void Cpobj(TypeInstructionSyntax instruction, T destination, T source);
        void Dup(NoneInstructionSyntax instruction, T value);
        void Initobj(TypeInstructionSyntax instruction, T destination);
        T Ldarg(ArgumentInstructionSyntax instruction);
        T Ldarga(ArgumentInstructionSyntax instruction);
        T Ldc(ConstantInstructionSyntax instruction);
        T Ldelem(TypeInstructionSyntax instruction, T array, T index);
        T Ldelema(TypeInstructionSyntax instruction, T array, T index);
        T Ldfld(FieldInstructionSyntax instruction, T obj);
        T Ldflda(FieldInstructionSyntax instruction, T obj);
        T Ldftn(FunctionInstructionSyntax instruction);
        T Ldind(TypeInstructionSyntax instruction, T address);
        T Ldlen(NoneInstructionSyntax instruction, T array);
        T Ldloc(LocalInstructionSyntax instruction);
        T Ldloca(LocalInstructionSyntax instruction);
        T Ldnull(NoneInstructionSyntax instruction);
        T Ldobj(TypeInstructionSyntax instruction, T source);
        T Neg(NoneInstructionSyntax instruction, T value);
        T Newarr(TypeInstructionSyntax instruction, T length);
        T Newobj(FunctionInstructionSyntax instruction, T[] arguments);
        void Nop(NoneInstructionSyntax instruction);
        T Not(NoneInstructionSyntax instruction, T value);
        T Phi(T value1, T value2);
        void Pop(NoneInstructionSyntax instruction, T value);
        void Ret(NoneInstructionSyntax instruction);
        void Ret(NoneInstructionSyntax instruction, T value);
        T Shl_Shr_ShrUn(NoneInstructionSyntax instruction, T value, T amount);
        void Starg(ArgumentInstructionSyntax instruction, T value);
        void Stelem(TypeInstructionSyntax instruction, T array, T index, T value);
        void Stfld(FieldInstructionSyntax instruction, T obj, T value);
        void Stind(TypeInstructionSyntax instruction, T address, T value);
        void Stloc(LocalInstructionSyntax instruction, T value);
        void Stobj(TypeInstructionSyntax instruction, T destination, T source);
    }
}
