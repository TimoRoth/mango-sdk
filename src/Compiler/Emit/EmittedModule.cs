using System;
using System.Collections.Immutable;
using Mango.Compiler.Symbols;
using Mango.Compiler.Verification;

namespace Mango.Compiler.Emit
{
    public sealed class EmittedModule
    {
        private readonly ReadOnlyMemory<int> _functionMap;
        private readonly ReadOnlyMemory<byte> _image;
        private readonly ReadOnlyMemory<int> _instructionMap;
        private readonly ReadOnlyMemory<byte> _name;
        private readonly VerifiedModule _verifiedModule;

        internal EmittedModule(VerifiedModule verifiedModule, ReadOnlyMemory<byte> name, ReadOnlyMemory<byte> image, ReadOnlyMemory<int> functionMap, ReadOnlyMemory<int> instructionMap)
        {
            if (verifiedModule == null)
            {
                throw new ArgumentNullException(nameof(verifiedModule));
            }

            _verifiedModule = verifiedModule;
            _name = name;
            _image = image;
            _functionMap = functionMap;
            _instructionMap = instructionMap;
        }

        public ReadOnlySpan<int> FunctionMap => _functionMap.Span;

        public ImmutableArray<VerifiedFunction> Functions => _verifiedModule.Functions;

        public ReadOnlySpan<byte> Image => _image.Span;

        public ReadOnlySpan<int> InstructionMap => _instructionMap.Span;

        public ReadOnlySpan<byte> Name => _name.Span;

        public ModuleSymbol Symbol => _verifiedModule.Symbol;

        public VerifiedFunction GetFunctionFromOffset(int offset)
        {
            var index = _functionMap.Span.BinarySearch(offset);
            if (index < 0) index = ~index - 1;
            if (index < 0) return null;
            return _verifiedModule.Functions[index];
        }

        public Instruction GetInstructionFromOffset(int offset)
        {
            var index1 = _functionMap.Span.BinarySearch(offset);
            if (index1 < 0) index1 = ~index1 - 1;
            if (index1 < 0) return null;
            var index2 = _instructionMap.Span.BinarySearch(_functionMap.Span[index1]);
            if (index2 < 0) index2 = ~index2;
            var index3 = _instructionMap.Span.BinarySearch(offset);
            if (index3 < 0) return null;
            return _verifiedModule.Functions[index1].Instructions[index3 - index2];
        }
    }
}
