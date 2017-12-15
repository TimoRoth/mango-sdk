using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mango.Compiler;
using Mango.Compiler.Syntax;
using Mango.Debugger;

namespace Mango.Cli
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            switch (args.Length > 0 ? args[0] : null)
            {
            case "build":
                Compile(args.Skip(1)).Emit(".");
                return 0;
            case "build-c":
                EmitSingleModuleToC("demo.inc", "demo_name", "demo_code", Compile(args.Skip(1)));
                return 0;
            case "run":
                var error = Run(Compile(args.Skip(1)), heapSize: 512, stackSize: 416);
                Console.WriteLine(error);
                return (int)error;
            default:
                Console.WriteLine("Unknown command");
                return 1;
            }
        }

        private static Compilation Compile(IEnumerable<string> paths)
        {
            return Compilation.Create("App", paths.Select(path => SyntaxTree.ParseText(File.ReadAllText(path), path)));
        }

        private static void EmitBytesToC(StreamWriter writer, string variable, ReadOnlySpan<byte> bytes)
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

        private static void EmitSingleModuleToC(string path, string nameVariable, string codeVariable, Compilation compilation)
        {
            var module = compilation.Build().Single();

            using (var writer = File.CreateText(path))
            {
                EmitBytesToC(writer, nameVariable, module.Name);
                writer.WriteLine();
                EmitBytesToC(writer, codeVariable, module.Image);
            }
        }

        private static ErrorCode Run(Compilation compilation, int heapSize = 128, int stackSize = 32)
        {
            var module = compilation.Build().Single();

            using (var process = Process.Create(heapSize, stackSize))
            {
                process.ImportModule(module.Name, module.Image, module.Symbol);
                return process.Run();
            }
        }
    }
}
