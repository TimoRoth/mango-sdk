using System;
using System.Collections.Generic;

namespace Mango.Compiler.Syntax
{
    public sealed partial class SyntaxTree
    {
        private readonly string _path;
        private readonly SyntaxNode _root;

        private SyntaxTree(SyntaxNode root, string path)
        {
            _path = path;
            _root = root;
        }

        public string FilePath => _path;

        public bool HasCompilationUnitRoot => _root.Kind == SyntaxKind.CompilationUnit;

        public SyntaxNode Root => _root;

        public static SyntaxTree Create(SyntaxNode root, string path = "") => new SyntaxTree(root ?? throw new ArgumentNullException(nameof(root)), path);

        public static SyntaxTree ParseText(string text, string path = "") => new SyntaxTree(new Parser.SimpleParser(text).ParseCompilationUnit(), path);

        public IEnumerable<object> GetDiagnostics() => throw new NotImplementedException();

        public SyntaxTree WithFilePath(string path) => new SyntaxTree(_root, path);

        public SyntaxTree WithRoot(SyntaxNode root) => new SyntaxTree(root ?? throw new ArgumentNullException(nameof(root)), _path);
    }
}
