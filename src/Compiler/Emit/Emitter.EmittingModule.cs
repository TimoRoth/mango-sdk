using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Mango.Compiler.Symbols;
using Mango.Compiler.Verification;
using static Interop.Libmango;
using static Interop.Libmango.mango_opcode;
using static Mango.Compiler.Analysis.Verification;
using static Mango.Compiler.Emit.CodeGeneration;

namespace Mango.Compiler.Emit
{
    internal static partial class Emitter
    {
        private sealed class EmittingModule
        {
            private readonly List<int> _functionMap;
            private readonly byte[] _image;
            private readonly List<int> _instructionMap;
            private readonly VerifiedModule _verifiedModule;

            private EmittingModules _emittingModules;
            private mango_feature_flags _features;
            private int _offset;
            private EmittedModule _result;

            public EmittingModule(VerifiedModule verifiedModule)
            {
                if (verifiedModule == null)
                {
                    throw new ArgumentNullException(nameof(verifiedModule));
                }

                _verifiedModule = verifiedModule;

                _functionMap = new List<int>();
                _instructionMap = new List<int>();
                _image = new byte[ushort.MaxValue];
            }

            public void Compile()
            {
                if (_result != null)
                {
                    throw new InvalidOperationException();
                }

                EmitModule();
            }

            public EmittedModule Link(EmittingModules emittingModules)
            {
                if (_result != null)
                {
                    return _result;
                }
                if (_emittingModules != null)
                {
                    throw new Exception(); // circular dependency
                }

                _emittingModules = emittingModules;
                _offset = 0;

                foreach (var import in _verifiedModule.Symbol.Imports)
                {
                    emittingModules.Link(import);
                }

                EmitModule();

                return _result = new EmittedModule(
                    _verifiedModule,
                    new ReadOnlyMemory<byte>(System.Security.Cryptography.SHA256.Create().ComputeHash(_image, 0, _offset), 0, Unsafe.SizeOf<mango_module_name>()),
                    new ReadOnlyMemory<byte>(_image, 0, _offset),
                    new ReadOnlyMemory<int>(_functionMap.ToArray()),
                    new ReadOnlyMemory<int>(_instructionMap.ToArray()));
            }

            internal int GetFunctionOffset(FunctionSymbol function) => _functionMap[function.ContainingModule.Functions.IndexOf(function)];

            internal int GetLabelOffset(VerifiedFunction function, LabelSymbol label) => _instructionMap[function.Labels[label]];

            private void EmitExtern(VerifiedFunction function)
            {
                var returnSlotCount = CalculateReturnsSlotCount(function.Symbol);
                var parametersSlotCount = CalculateParametersSlotCount(function.Symbol);
                var adjustment = parametersSlotCount - returnSlotCount;
                var ordinal = function.Symbol.GetSystemCallOrdinal();

                if (adjustment < sbyte.MinValue || adjustment > sbyte.MaxValue) throw new Exception();
                if (ordinal < ushort.MinValue || ordinal > ushort.MaxValue) throw new Exception();

                if (_emittingModules == null)
                {
                    _functionMap.Add(_offset);
                }

                WriteFunction(default);

                WriteByteCode(new ByteCode(SYSCALL, i8: (sbyte)adjustment, u16: (ushort)ordinal));

                if (function.Symbol.ReturnsVoid)
                {
                    WriteByteCode(new ByteCode(RET));
                }
                else
                {
                    WriteByteCode(new ByteCode(Select(GetIntermediateType(function.Symbol.ReturnType), RET_X32, RET_X64, RET_X32, RET_X64, RET_X32)));
                }
            }

            private void EmitFunction(VerifiedFunction function)
            {
                var parametersSlotCount = CalculateParametersSlotCount(function.Symbol);
                var localsSlotCount = CalculateLocalsSlotCount(function.Symbol);
                var maxStack = CalculateMaxStack(function);

                if (parametersSlotCount < byte.MinValue || parametersSlotCount > byte.MaxValue) throw new Exception();
                if (localsSlotCount < byte.MinValue || localsSlotCount > byte.MaxValue) throw new Exception();
                if (parametersSlotCount + localsSlotCount > byte.MaxValue) throw new Exception();
                if (maxStack < byte.MinValue || maxStack > byte.MaxValue) throw new Exception();

                if (_emittingModules == null)
                {
                    _functionMap.Add(_offset);
                }

                WriteFunction(new mango_func_def
                {
                    arg_count = (byte)parametersSlotCount,
                    loc_count = (byte)localsSlotCount,
                    max_stack = (byte)maxStack,
                });

                var generator = new Generator(function, this, _emittingModules);
                var dispatcher = new Dispatcher(generator);

                foreach (var instruction in function.Instructions)
                {
                    if (_emittingModules == null)
                    {
                        _instructionMap.Add(_offset);
                    }

                    WriteByteCode(dispatcher.Visit(instruction, _offset));
                }
            }

            private void EmitModule()
            {
                var moduleCount = _verifiedModule.Symbol.CreateImportsClosure().Count;
                var importCount = _verifiedModule.Symbol.Imports.Length;
                var entryPointOffset = _verifiedModule.Symbol.EntryPoint != null && _emittingModules != null ? GetFunctionOffset(_verifiedModule.Symbol.EntryPoint) : 0;

                if (moduleCount < byte.MinValue || moduleCount > byte.MaxValue) throw new Exception();
                if (importCount < byte.MinValue || importCount > byte.MaxValue) throw new Exception();
                if (entryPointOffset < ushort.MinValue || entryPointOffset > ushort.MaxValue) throw new Exception();

                WriteModule(new mango_module_def
                {
                    version = MANGO_VERSION_MAJOR,
                    features = (byte)_features,
                    module_count = (byte)moduleCount,
                    import_count = (byte)importCount,
                    entry_point_0 = (byte)(entryPointOffset != 0 ? CALL_S : NOP),
                    entry_point_1 = (byte)(entryPointOffset & 255),
                    entry_point_2 = (byte)(entryPointOffset >> 8),
                    entry_point_3 = (byte)HALT,
                });

                foreach (var import in _verifiedModule.Symbol.Imports)
                {
                    WriteImport(_emittingModules?.Link(import));
                }

                foreach (var function in _verifiedModule.Functions)
                {
                    if (function.Symbol.IsExtern)
                    {
                        EmitExtern(function);
                    }
                    else
                    {
                        EmitFunction(function);
                    }
                }
            }

            private void WriteByteCode(ByteCode byteCode)
            {
                byteCode.CopyTo(new Span<byte>(_image, _offset, byteCode.Length));
                _offset += byteCode.Length;
                _features |= byteCode.Features;
            }

            private void WriteFunction(mango_func_def definition)
            {
                BinaryPrimitives.WriteMachineEndian(new Span<byte>(_image, _offset, Unsafe.SizeOf<mango_func_def>()), ref definition);
                _offset += Unsafe.SizeOf<mango_func_def>();
            }

            private void WriteImport(EmittedModule module)
            {
                module?.Name.CopyTo(new Span<byte>(_image, _offset, Unsafe.SizeOf<mango_module_name>()));
                _offset += Unsafe.SizeOf<mango_module_name>();
            }

            private void WriteModule(mango_module_def definition)
            {
                BinaryPrimitives.WriteMachineEndian(new Span<byte>(_image, _offset, Unsafe.SizeOf<mango_module_def>()), ref definition);
                _offset += Unsafe.SizeOf<mango_module_def>();
            }
        }
    }
}
