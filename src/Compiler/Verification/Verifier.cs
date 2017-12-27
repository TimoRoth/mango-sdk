using System;
using System.Collections.Immutable;
using System.Linq;
using Mango.Compiler.Analysis;
using Mango.Compiler.Symbols;
using Mango.Compiler.Syntax;

namespace Mango.Compiler.Verification
{
    internal static partial class Verifier
    {
        public static VerifiedFunction VerifyFunction(FunctionDeclarationSyntax functionDeclaration, SemanticModel semanticModel, ref int sum)
        {
            if (functionDeclaration == null)
            {
                throw new ArgumentNullException(nameof(functionDeclaration));
            }
            if (semanticModel == null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            var binder = semanticModel.Compilation.Binder;
            var symbol = binder.BindFunction(functionDeclaration) ?? throw new Exception();

            if (symbol.IsExtern)
            {
                return new VerifiedFunction(symbol, default, default);
            }
            else
            {
                var body = functionDeclaration.Body.Instructions;
                var state = semanticModel.Verify(functionDeclaration).State;

                var converter = new Converter(binder, symbol);
                var dispatcher = new Dispatcher(converter);

                var instructions = ImmutableArray.CreateBuilder<Instruction>(body.Count);
                var labels = ImmutableDictionary.CreateBuilder<LabelSymbol, int>();

                for (var i = 0; i < body.Count; i++)
                {
                    var instruction = body[i];

                    while (instruction is LabeledInstructionSyntax labeledInstruction)
                    {
                        labels.Add(binder.BindLabel(labeledInstruction) ?? throw new Exception(), sum + i);
                        instruction = labeledInstruction.LabeledInstruction;
                    }

                    instructions.Add(dispatcher.Visit(instruction, new ExecutionState(default, default, state[i].Stack)));
                }
                sum += body.Count;

                return new VerifiedFunction(symbol, instructions.MoveToImmutable(), labels.ToImmutable());
            }
        }

        public static VerifiedModule VerifyModule(ModuleDeclarationSyntax moduleDeclaration, SemanticModel semanticModel)
        {
            if (moduleDeclaration == null)
            {
                throw new ArgumentNullException(nameof(moduleDeclaration));
            }
            if (semanticModel == null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            var binder = semanticModel.Compilation.Binder;
            var symbol = binder.BindModule(moduleDeclaration) ?? throw new Exception();

            var functions = ImmutableArray.CreateBuilder<VerifiedFunction>();
            var sum = 0;

            foreach (var functionDeclaration in moduleDeclaration.Members.OfType<FunctionDeclarationSyntax>())
            {
                functions.Add(VerifyFunction(functionDeclaration, semanticModel, ref sum));
            }

            return new VerifiedModule(symbol, functions.ToImmutable());
        }
    }
}
