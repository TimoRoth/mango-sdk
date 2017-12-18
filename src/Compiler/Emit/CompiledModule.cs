using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using Mango.Compiler.Analysis;
using Mango.Compiler.Symbols;
using Mango.Compiler.Syntax;
using static Interop.Libmango;
using static Interop.Libmango.mango_func_attributes;
using static Interop.Libmango.mango_module_attributes;
using static Interop.Libmango.mango_opcode;

namespace Mango.Compiler.Emit
{
    public sealed class CompiledModule
    {
        private readonly ImmutableDictionary<FunctionSymbol, int> _functions;
        private readonly byte[] _image;
        private readonly ImmutableArray<int> _instructions;
        private readonly ImmutableDictionary<LabelSymbol, int> _labels;
        private readonly ModuleDeclarationSyntax _moduleDeclaration;
        private readonly ImmutableArray<ModuleSymbol> _references;
        private readonly SemanticModel _semanticModel;

        private byte[] _fingerprint;
        private bool _linking;

        private CompiledModule(SemanticModel semanticModel, ModuleDeclarationSyntax moduleDeclaration, ImmutableArray<ModuleSymbol> references, ImmutableDictionary<FunctionSymbol, int> functions, ImmutableDictionary<LabelSymbol, int> labels, ImmutableArray<int> instructions, byte[] image)
        {
            _semanticModel = semanticModel;
            _moduleDeclaration = moduleDeclaration;
            _references = references;
            _functions = functions;
            _labels = labels;
            _instructions = instructions;
            _image = image;
        }

        public ReadOnlySpan<byte> Image => _image;

        public ReadOnlySpan<byte> Name => _fingerprint ?? throw new InvalidOperationException();

        public ModuleSymbol Symbol => _semanticModel.GetDeclaredSymbol(_moduleDeclaration);

        public void Emit(string path) => System.IO.File.WriteAllBytes(System.IO.Path.Combine(path, System.IO.Path.ChangeExtension(ToHex(_fingerprint), "module")), _image);

        public string GetName() => ToHex(_fingerprint ?? throw new InvalidOperationException());

        internal static CompiledModule Create(ModuleDeclarationSyntax moduleDeclaration, SemanticModel semanticModel)
        {
            var binder = semanticModel.Compilation.Binder;
            var module = binder.BindModule(moduleDeclaration) ?? throw new Exception();
            var references = ImmutableArray.CreateBuilder<ModuleSymbol>();
            var functions = ImmutableDictionary.CreateBuilder<FunctionSymbol, int>();
            var labels = ImmutableDictionary.CreateBuilder<LabelSymbol, int>();
            var instructions = ImmutableArray.CreateBuilder<int>();
            var image = new byte[ushort.MaxValue];
            var entryPointOffset = 0;
            var offset = 0;
            var features = (mango_feature_flags)0;

            foreach (var functionDeclaration in moduleDeclaration.Members.OfType<FunctionDeclarationSyntax>())
            {
                if (functionDeclaration.Body == null)
                    continue;

                var instructions1 = functionDeclaration.Body.Instructions;

                for (var i = 0; i < instructions1.Count; i++)
                {
                    var instruction = instructions1[i];

                    while (instruction is LabeledInstructionSyntax labeledInstruction)
                    {
                        instruction = labeledInstruction.LabeledInstruction;
                    }

                    if (instruction is FunctionInstructionSyntax functionInstruction)
                    {
                        var function = binder.BindFunction(functionInstruction) ?? throw new Exception();
                        if (!function.IsExtern)
                        {
                            var reference = function.ContainingModule;
                            if (reference != module && !references.Contains(reference))
                            {
                                references.Add(reference);
                            }
                        }
                    }
                }
            }

            if (references.Count < byte.MinValue || references.Count > byte.MaxValue) throw new Exception();

            if (references.Count != 0) throw new NotImplementedException(); // TODO: modules closure

            Unsafe.WriteUnaligned(ref image[offset], new mango_module_def
            {
                magic = MANGO_IMAGE_MAGIC,
                attributes = (byte)MANGO_MD_EXECUTABLE,
                import_count = (byte)references.Count,
                initializer_0 = (byte)NOP,
                initializer_1 = (byte)NOP,
                initializer_2 = (byte)NOP,
                initializer_3 = (byte)HALT,
            });

            offset += Unsafe.SizeOf<mango_module_def>() + Unsafe.SizeOf<mango_module_name>() * references.Count;

            foreach (var functionDeclaration in moduleDeclaration.Members.OfType<FunctionDeclarationSyntax>())
            {
                var func = binder.BindFunction(functionDeclaration) ?? throw new Exception();

                if (string.Equals(func.Name, "main", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(func.Name, "@main", StringComparison.OrdinalIgnoreCase))
                {
                    if (func.IsExtern) throw new Exception();
                    if (func.Parameters.Length != 0) throw new Exception();
                    if (!func.ReturnsVoid) throw new Exception();
                    entryPointOffset = offset;
                }

                if (func.IsExtern)
                    continue;
                functions.Add(func, offset);

                var generator = new CodeGenerator(semanticModel.Compilation.Binder, functionDeclaration);
                var dispatcher = new CodeDispatcher(generator);
                var state = semanticModel.Verify(functionDeclaration).State;
                var instructions1 = functionDeclaration.Body.Instructions;

                var returnSlotCount = 0;
                var parametersSlotCount = 0;
                var localsSlotCount = 0;
                var maxStack = 0;

                if (!func.ReturnsVoid)
                    returnSlotCount += (func.ReturnType.TypeLayout.Size + 3) / 4;
                foreach (var parameter in func.Parameters)
                    parametersSlotCount += (parameter.Type.TypeLayout.Size + 3) / 4;
                foreach (var local in func.Locals)
                    localsSlotCount += (local.Type.TypeLayout.Size + 3) / 4;

                foreach (var item in state)
                {
                    var stackSize = 0;
                    var stack = item.Stack;
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

                if (parametersSlotCount < byte.MinValue || parametersSlotCount > byte.MaxValue) throw new Exception();
                if (localsSlotCount < byte.MinValue || localsSlotCount > byte.MaxValue) throw new Exception();
                if (parametersSlotCount + localsSlotCount > byte.MaxValue) throw new Exception();
                if (maxStack < byte.MinValue || maxStack > byte.MaxValue) throw new Exception();

                Unsafe.WriteUnaligned(ref image[offset], new mango_func_def
                {
                    attributes = (byte)(localsSlotCount != 0 ? MANGO_FD_INIT_LOCALS : MANGO_FD_NONE),
                    max_stack = (byte)maxStack,
                    arg_count = (byte)parametersSlotCount,
                    loc_count = (byte)localsSlotCount,
                });

                offset += Unsafe.SizeOf<mango_func_def>();

                for (var i = 0; i < instructions1.Count; i++)
                {
                    instructions.Add(offset);

                    var stack = state[i].Stack;
                    var instruction = instructions1[i];

                    while (instruction is LabeledInstructionSyntax labeledInstruction)
                    {
                        labels.Add(binder.BindLabel(labeledInstruction) ?? throw new Exception(), offset);
                        instruction = labeledInstruction.LabeledInstruction;
                    }

                    var byteCode = dispatcher.Visit(offset, instruction, stack);
                    byteCode.CopyTo(image, offset);
                    offset += byteCode.Length;
                    features |= byteCode.Features;
                }
            }

            if (entryPointOffset < ushort.MinValue || entryPointOffset > ushort.MaxValue) throw new Exception();

            Unsafe.WriteUnaligned(ref image[offset], new mango_app_info
            {
                features = unchecked((byte)writer.Features),
                module_count = 1, // TODO: modules closure
                entry_point_0 = (byte)(entryPointOffset != 0 ? CALL_S : NOP),
                entry_point_1 = (byte)(entryPointOffset & 255),
                entry_point_2 = (byte)(entryPointOffset >> 8),
                entry_point_3 = (byte)HALT,
            });

            offset += Unsafe.SizeOf<mango_app_info>();

            Array.Resize(ref image, offset);

            return new CompiledModule(semanticModel, moduleDeclaration, references.ToImmutable(), functions.ToImmutable(), labels.ToImmutable(), instructions.ToImmutable(), image);
        }

        internal int GetFunctionOffset(FunctionSymbol function) => _functions[function];

        internal int GetLabelOffset(LabelSymbol label) => _labels[label];

        internal int GetReferencedModuleIndex(ModuleSymbol module) => _references.IndexOf(module);

        internal void Link(CompiledModules compiledModules)
        {
            if (_fingerprint != null) return;
            if (_linking) throw new Exception(); // circular dependency
            _linking = true;

            for (var i = 0; i < _references.Length; i++)
            {
                var reference = compiledModules.GetCompiledModuleFromSymbol(_references[i]);
                reference.Link(compiledModules);
                reference._fingerprint.CopyTo(_image, 8 + 12 * i);
            }

            var index = 0;

            foreach (var functionDeclaration in _moduleDeclaration.Members.OfType<FunctionDeclarationSyntax>())
            {
                if (functionDeclaration.Body == null)
                    continue;

                var generator = new CodeGenerator(_semanticModel.Compilation.Binder, functionDeclaration, compiledModules);
                var dispatcher = new CodeDispatcher(generator);
                var state = _semanticModel.Verify(functionDeclaration).State;
                var instructions1 = functionDeclaration.Body.Instructions;

                for (var i = 0; i < instructions1.Count; i++)
                {
                    var stack = state[i].Stack;
                    var instructionSyntax = instructions1[i];

                    while (instructionSyntax is LabeledInstructionSyntax labeledInstructionSyntax)
                    {
                        instructionSyntax = labeledInstructionSyntax.LabeledInstruction;
                    }

                    if (instructionSyntax is FunctionInstructionSyntax || instructionSyntax is BranchInstructionSyntax)
                    {
                        dispatcher.Visit(_instructions[index], instructionSyntax, stack).CopyTo(_image, _instructions[index]);
                    }

                    index++;
                }
            }

            _fingerprint = System.Security.Cryptography.SHA256.Create().ComputeHash(_image);
            Array.Resize(ref _fingerprint, 12);
            _linking = false;
        }

        private static string ToHex(byte[] array)
        {
            var buffer = new char[array.Length * 2];
            for (var i = 0; i < array.Length; i++)
            {
                buffer[2 * i + 0] = "0123456789abcdef"[array[i] >> 4];
                buffer[2 * i + 1] = "0123456789abcdef"[array[i] & 15];
            }
            return new string(buffer);
        }
    }
}
