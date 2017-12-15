using Mango.Compiler.Symbols;
using Mango.Compiler.Syntax;

namespace Mango.Compiler.Analysis
{
    public sealed class SemanticModel
    {
        private readonly Compilation _compilation;

        internal SemanticModel(Compilation compilation)
        {
            _compilation = compilation;
        }

        public Compilation Compilation => _compilation;

        public FieldSymbol GetDeclaredSymbol(FieldDeclarationSyntax declarationSyntax) => _compilation.Binder.BindField(declarationSyntax);
        public FunctionSymbol GetDeclaredSymbol(FunctionDeclarationSyntax declarationSyntax) => _compilation.Binder.BindFunction(declarationSyntax);
        public LocalSymbol GetDeclaredSymbol(LocalDeclarationSyntax declarationSyntax) => _compilation.Binder.BindLocal(declarationSyntax);
        public ModuleSymbol GetDeclaredSymbol(ModuleDeclarationSyntax declarationSyntax) => _compilation.Binder.BindModule(declarationSyntax);
        public ParameterSymbol GetDeclaredSymbol(ParameterDeclarationSyntax declarationSyntax) => _compilation.Binder.BindParameter(declarationSyntax);
        public StructuredTypeSymbol GetDeclaredSymbol(TypeDeclarationSyntax declarationSyntax) => _compilation.Binder.BindType(declarationSyntax);

        public DataFlowAnalysis<TypeSymbol> Verify(FunctionDeclarationSyntax functionDeclaration)
        {
            var verifier = new Verifier(functionDeclaration, _compilation);
            var analyzer = new InterpreterDataFlowAnalyzer<TypeSymbol>(functionDeclaration, verifier);
            return analyzer.Analyze();
        }
    }
}
