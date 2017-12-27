using System;
using Mango.Compiler.Verification;

namespace Mango.Compiler.Emit
{
    partial class Emitter
    {
        private readonly struct Dispatcher
        {
            private readonly Generator _generator;

            public Dispatcher(Generator generator)
            {
                _generator = generator;
            }

            public ByteCode Visit(Instruction instruction, int offset)
            {
                if (instruction is TypedInstruction typedInstruction)
                {
                    return _generator.VisitTyped(typedInstruction);
                }
                if (instruction is NoneInstruction noneInstruction)
                {
                    return _generator.VisitNone(noneInstruction);
                }
                if (instruction is ArgumentInstruction argumentInstruction)
                {
                    return _generator.VisitArgument(argumentInstruction);
                }
                if (instruction is LocalInstruction localInstruction)
                {
                    return _generator.VisitLocal(localInstruction);
                }
                if (instruction is FunctionInstruction functionInstruction)
                {
                    return _generator.VisitFunction(functionInstruction);
                }
                if (instruction is FieldInstruction fieldInstruction)
                {
                    return _generator.VisitField(fieldInstruction);
                }
                if (instruction is ConditionalBranchInstruction conditionalBranchInstruction)
                {
                    return _generator.VisitConditionalBranch(conditionalBranchInstruction, offset);
                }
                if (instruction is UnconditionalBranchInstruction unconditionalBranchInstruction)
                {
                    return _generator.VisitUnconditionalBranch(unconditionalBranchInstruction, offset);
                }
                if (instruction is ConstantInstruction constantInstruction)
                {
                    return _generator.VisitConstant(constantInstruction);
                }
                if (instruction is ConversionInstruction conversionInstruction)
                {
                    return _generator.VisitConversion(conversionInstruction);
                }
                throw new Exception();
            }
        }
    }
}
