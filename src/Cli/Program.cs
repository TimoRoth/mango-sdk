using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mango.Compiler;
using Mango.Compiler.Emit;
using Mango.Compiler.Syntax;

namespace Mango.Cli
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            switch (args.FirstOrDefault())
            {
            case "build-c":
                EmitSingleModuleToC("demo.inc", "demo_name", "demo_code", Compile(args.Skip(1)).Build());
                return 0;
            case "debug":
                DebugCommand.Debug(Compile(args.Skip(1)).Build());
                return 0;
            default:
                Console.WriteLine("Unknown command");
                return 1;
            }
        }

        private static Compilation Compile(IEnumerable<string> paths)
        {
            return Compilation.Create("App", paths.Select(path => SyntaxTree.ParseText(File.ReadAllText(path), path)));
        }

        private static void EmitBytesToC(TextWriter writer, string variable, ReadOnlySpan<byte> bytes)
        {
            writer.WriteLine("static const uint8_t {0}[{1}] = {{", variable, bytes.Length);
            for (var i = 0; i < bytes.Length; i++)
            {
                if ((i & 7) == 0) writer.Write("   ");
                writer.Write(" 0x");
                writer.Write(bytes[i].ToString("X2"));
                writer.Write(",");
                if ((i & 7) == 7 || i == bytes.Length - 1) writer.WriteLine();
            }
            writer.WriteLine("};");
        }

        private static void EmitSingleModuleToC(string path, string nameVariable, string codeVariable, EmittedModules emittedModules)
        {
            var module = emittedModules.Modules.Single();

            using (var writer = File.CreateText(path))
            {
                EmitBytesToC(writer, nameVariable, module.Name);
                writer.WriteLine();
                EmitBytesToC(writer, codeVariable, module.Image);
            }
        }
    }
}
