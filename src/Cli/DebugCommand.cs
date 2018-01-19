using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Mango.Compiler;
using Mango.Compiler.Symbols;
using Mango.Debugger;

namespace Mango.Cli
{
    internal static class DebugCommand
    {
        internal static void Debug(Compilation compilation, string startupModule = null, int heapSize = 1024, int stackSize = 256)
        {
            var memoryDump1 = (byte[])null;
            var sw = new System.Diagnostics.Stopwatch();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Mango Virtual Machine");
            Console.WriteLine("(c) 2018 Klaus Hartke");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("compiling...");
            var emittedModules = compilation.Build();

        start:
            Console.ForegroundColor = ConsoleColor.DarkGray;
            var process = Process.Create(heapSize: heapSize, stackSize: stackSize);
            {
                var missing = (startupModule == null) ? emittedModules.Modules.Single().Name : emittedModules.GetModuleByName(startupModule).Name;
                while (!missing.IsEmpty)
                {
                    var module = emittedModules.GetModuleByFingerprint(missing);
                    Console.WriteLine("loading {0,-44} [{1}]", module.Symbol.Name, ToHex(module.Name));
                    process.ImportModule(module.Name, module.Image, module);
                    missing = process.GetMissingModule();
                }
            }
            Console.WriteLine("running...");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;

        run_again:
            sw.Restart();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            var result = process.Run();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(sw.Elapsed);
            Console.ForegroundColor = ConsoleColor.Gray;

            if (result == ErrorCode.Success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();
                Console.WriteLine("Done");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else if (result == ErrorCode.SystemCall)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine();
                Console.WriteLine("System Call [0x{0:x}] {0}", process.GetSystemCall());
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else if (result == ErrorCode.Breakpoint)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine();
                Console.WriteLine("Breakpoint hit");
                Console.ForegroundColor = ConsoleColor.Gray;
                Print(process.GetStackTrace());
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine("Error: {0}", result);
                Console.ForegroundColor = ConsoleColor.Gray;
                Print(process.GetStackTrace());
            }

            var snapshot = process.CreateSnapshot();

        prompt_again:
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("(s)tack trace  (d)isassembly  (m)emory                    (r)estart  (c)ontinue");
            Console.WriteLine("(p)arameters  (l)ocals  (e)valuation stack                               (q)uit");
            Console.ForegroundColor = ConsoleColor.Gray;

        read_again:
            switch (Console.ReadKey(true).KeyChar)
            {
            case 's':
                Print(snapshot.StackTrace);
                goto prompt_again;
            case 'd':
                Print(snapshot.CurrentStackFrame);
                goto prompt_again;
            case 'm':
                var memoryDump2 = process.CreateMemoryDump();
                Print(memoryDump1, memoryDump2, process.GetStackSize());
                memoryDump1 = memoryDump2;
                goto prompt_again;
            case 'p':
                Print(snapshot.GetParameters(snapshot.CurrentStackFrame));
                goto prompt_again;
            case 'l':
                Print(snapshot.GetLocals(snapshot.CurrentStackFrame));
                goto prompt_again;
            case 'e':
                Print(snapshot.GetEvaluationStack(snapshot.CurrentStackFrame));
                goto prompt_again;
            case 'r':
                process.Dispose();
                Console.WriteLine();
                goto start;
            case 'c':
                goto run_again;
            case 'q':
                process.Dispose();
                Console.ResetColor();
                return;
            default:
                goto read_again;
            }
        }

        private static void Print(ImmutableArray<StackFrame> stackTrace)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Stack trace:");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;

            foreach (var stackFrame in stackTrace)
            {
                var module = stackFrame.FunctionSymbol?.ContainingModule.Name ?? "#" + stackFrame.ModuleIndex.ToString();
                if (module.Length > 10)
                {
                    module = module.Substring(0, 4) + ".." + module.Substring(module.Length - 4, 4);
                }

                var function = stackFrame.FunctionSymbol?.Name ?? "<unknown>";
                if (function.Length > 30)
                {
                    function = function.Substring(0, 18) + ".." + function.Substring(function.Length - 10, 10);
                }

                Console.WriteLine(" {0} | {1:X2}:{2:X4} | {3,-10} | {4,-30} {5,20}",
                    stackFrame == stackTrace.Last() ? '>' : ' ',
                    stackFrame.ModuleIndex,
                    stackFrame.ByteCodeOffset,
                    module,
                    function,
                    "");
            }

            Console.WriteLine();
        }

        private static void Print(ImmutableArray<int> evaluationStack)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Evaluation stack:");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;

            var i = 0;
            foreach (var item in evaluationStack)
            {
                Console.WriteLine(" {0} | {1:X8} | 0x{2,-10:X8} {2,12}",
                    i == 0 ? '>' : ' ',
                    4 * i,
                    item);
                i++;
            }

            Console.WriteLine();
        }

        private static void Print(ImmutableStack<TypedValue> evaluationStack)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Evaluation stack:");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;

            if (evaluationStack != null)
            {
                var i = 0;
                foreach (var item in evaluationStack)
                {
                    Console.WriteLine(" {0} | {1:X8} | {2} !{3} = {4}",
                        i == 0 ? '>' : ' ',
                        item.MemoryOffset,
                        item.Type.SpecialType.ToString().ToLowerInvariant(),
                        i,
                        item.Value);
                    i++;
                }
            }

            Console.WriteLine();
        }

        private static void Print(StackFrame stackFrame)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Disassembly:");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;

            if (stackFrame == null || stackFrame.VerifiedFunction == null || stackFrame.CurrentInstruction == null)
            {
                Console.WriteLine();
                return;
            }

            var instructions = stackFrame.VerifiedFunction.Instructions;
            var instruction = stackFrame.CurrentInstruction;
            var index = instructions.IndexOf(instruction);

            var lower = index;
            var upper = index;
            while (lower - 1 >= 0 || upper + 1 < instructions.Length)
            {
                if (lower - 1 >= 0) lower--;
                if (upper - lower >= 12) break;
                if (upper + 1 < instructions.Length) upper++;
                if (upper - lower >= 12) break;
            }

            if (lower - 1 >= 0)
            {
                Console.WriteLine("   |         | ...");
            }

            for (var i = lower; i <= upper; i++)
            {
                Console.WriteLine(" {0} | {1:X2}:{2:X4} | {3}",
                    i == index ? '>' : ' ',
                    stackFrame.ModuleIndex,
                    0 /*instructions[i].ByteCodeOffset*/,
                    instructions[i].Kind.ToString().ToLowerInvariant().Replace('_', '.'));
            }

            if (upper + 1 < instructions.Length)
            {
                Console.WriteLine("   |         | ...");
            }

            Console.WriteLine();
        }

        private static void Print(ImmutableDictionary<LocalSymbol, TypedValue> locals)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Local variables:");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;

            if (locals != null)
            {
                foreach (var local in locals.OrderBy(l => l.Key.Name))
                {
                    Console.WriteLine("   | {0:X8} | {1} {2} = {3}",
                        local.Value.MemoryOffset,
                        local.Key.Type.SpecialType.ToString().ToLowerInvariant(),
                        local.Key.Name,
                        local.Value.Value);
                }
            }

            Console.WriteLine();
        }

        private static void Print(ImmutableDictionary<ParameterSymbol, TypedValue> parameters)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Parameters:");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;

            if (parameters != null)
            {
                foreach (var parameter in parameters.OrderBy(p => p.Key.Name))
                {
                    Console.WriteLine("   | {0:X8} | {1} {2} = {3}",
                        parameter.Value.MemoryOffset,
                        parameter.Key.Type.SpecialType.ToString().ToLowerInvariant(),
                        parameter.Key.Name,
                        parameter.Value.Value);
                }
            }

            Console.WriteLine();
        }

        private static void Print(byte[] bytes)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Memory:");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;

            for (var i = 0; i < bytes.Length; i += 16)
            {
                var c = bytes.Length - i;
                Console.Write("{0:X8} ", i);
                for (var j = 0; j < 8; j++)
                    Console.Write(" {0}", j >= c ? "  " : bytes[i + j].ToString("X2"));
                Console.Write(" ");
                for (var j = 8; j < 16; j++)
                    Console.Write(" {0}", j >= c ? "  " : bytes[i + j].ToString("X2"));
                Console.Write("  ");
                for (var j = 0; j < 8; j++)
                    Console.Write("{0}", j >= c ? ' ' : bytes[i + j] < ' ' ? '.' : bytes[i + j] >= 127 ? '.' : (char)bytes[i + j]);
                Console.Write(" ");
                for (var j = 8; j < 16; j++)
                    Console.Write("{0}", j >= c ? ' ' : bytes[i + j] < ' ' ? '.' : bytes[i + j] >= 127 ? '.' : (char)bytes[i + j]);
                Console.WriteLine();
            }
        }

        private static void Print(byte[] before, byte[] after, int stackSize)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Memory:");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;

            if (before == null)
                before = after;
            var bytes = after;

            for (var i = 0; i < bytes.Length; i += 16)
            {
                if (i == 64 || i == 64 + stackSize)
                    Console.WriteLine();

                var c = bytes.Length - i;
                Console.Write("  {0:X8} ", i);
                for (var j = 0; j < 8; j++)
                {
                    if (bytes[i + j] != before[i + j])
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(" {0}", j >= c ? "  " : bytes[i + j].ToString("X2"));
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Console.Write(" ");
                for (var j = 8; j < 16; j++)
                {
                    if (bytes[i + j] != before[i + j])
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(" {0}", j >= c ? "  " : bytes[i + j].ToString("X2"));
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Console.Write("  ");
                for (var j = 0; j < 8; j++)
                {
                    if (bytes[i + j] != before[i + j])
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("{0}", j >= c ? ' ' : bytes[i + j] < ' ' ? '.' : bytes[i + j] >= 127 ? '.' : (char)bytes[i + j]);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Console.Write(" ");
                for (var j = 8; j < 16; j++)
                {
                    if (bytes[i + j] != before[i + j])
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("{0}", j >= c ? ' ' : bytes[i + j] < ' ' ? '.' : bytes[i + j] >= 127 ? '.' : (char)bytes[i + j]);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }

        private static string ToHex(ReadOnlySpan<byte> bytes)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
