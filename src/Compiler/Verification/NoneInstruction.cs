namespace Mango.Compiler.Verification
{
    public sealed class NoneInstruction : Instruction
    {
        internal NoneInstruction(InstructionKind kind, ExecutionState state) : base(kind, state)
        {
        }
    }
}
