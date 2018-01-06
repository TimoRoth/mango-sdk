namespace Mango.Compiler.Syntax
{
    public abstract partial class SyntaxVisitor
    {
        public virtual void DefaultVisit(SyntaxNode node)
        {
        }

        public virtual void Visit(SyntaxNode node)
        {
            if (node != null)
            {
                node.Accept(this);
            }
        }
    }

    public abstract partial class SyntaxVisitor<TResult>
    {
        public virtual TResult DefaultVisit(SyntaxNode node)
        {
            return default;
        }

        public virtual TResult Visit(SyntaxNode node)
        {
            if (node != null)
            {
                return node.Accept(this);
            }

            return default;
        }
    }

    partial class SyntaxVisitor
    {
        public virtual void VisitArgumentInstruction(ArgumentInstructionSyntax node) => DefaultVisit(node);
        public virtual void VisitArrayType(ArrayTypeSyntax node) => DefaultVisit(node);
        public virtual void VisitBranchInstruction(BranchInstructionSyntax node) => DefaultVisit(node);
        public virtual void VisitCompilationUnit(CompilationUnitSyntax node) => DefaultVisit(node);
        public virtual void VisitConstantInstruction(ConstantInstructionSyntax node) => DefaultVisit(node);
        public virtual void VisitFieldDeclaration(FieldDeclarationSyntax node) => DefaultVisit(node);
        public virtual void VisitFieldInstruction(FieldInstructionSyntax node) => DefaultVisit(node);
        public virtual void VisitFunctionBody(FunctionBodySyntax node) => DefaultVisit(node);
        public virtual void VisitFunctionDeclaration(FunctionDeclarationSyntax node) => DefaultVisit(node);
        public virtual void VisitFunctionInstruction(FunctionInstructionSyntax node) => DefaultVisit(node);
        public virtual void VisitFunctionType(FunctionTypeSyntax node) => DefaultVisit(node);
        public virtual void VisitImportDirective(ImportDirectiveSyntax node) => DefaultVisit(node);
        public virtual void VisitLabeledInstruction(LabeledInstructionSyntax node) => DefaultVisit(node);
        public virtual void VisitLocalDeclaration(LocalDeclarationSyntax node) => DefaultVisit(node);
        public virtual void VisitLocalInstruction(LocalInstructionSyntax node) => DefaultVisit(node);
        public virtual void VisitModuleDeclaration(ModuleDeclarationSyntax node) => DefaultVisit(node);
        public virtual void VisitNoneInstruction(NoneInstructionSyntax node) => DefaultVisit(node);
        public virtual void VisitParameterDeclaration(ParameterDeclarationSyntax node) => DefaultVisit(node);
        public virtual void VisitPredefinedType(PredefinedTypeSyntax node) => DefaultVisit(node);
        public virtual void VisitReferenceType(ReferenceTypeSyntax node) => DefaultVisit(node);
        public virtual void VisitSpanType(SpanTypeSyntax node) => DefaultVisit(node);
        public virtual void VisitStructuredType(StructuredTypeSyntax node) => DefaultVisit(node);
        public virtual void VisitTypeDeclaration(TypeDeclarationSyntax node) => DefaultVisit(node);
        public virtual void VisitTypeInstruction(TypeInstructionSyntax node) => DefaultVisit(node);
    }

    partial class SyntaxVisitor<TResult>
    {
        public virtual TResult VisitArgumentInstruction(ArgumentInstructionSyntax node) => DefaultVisit(node);
        public virtual TResult VisitArrayType(ArrayTypeSyntax node) => DefaultVisit(node);
        public virtual TResult VisitBranchInstruction(BranchInstructionSyntax node) => DefaultVisit(node);
        public virtual TResult VisitCompilationUnit(CompilationUnitSyntax node) => DefaultVisit(node);
        public virtual TResult VisitConstantInstruction(ConstantInstructionSyntax node) => DefaultVisit(node);
        public virtual TResult VisitFieldDeclaration(FieldDeclarationSyntax node) => DefaultVisit(node);
        public virtual TResult VisitFieldInstruction(FieldInstructionSyntax node) => DefaultVisit(node);
        public virtual TResult VisitFunctionBody(FunctionBodySyntax node) => DefaultVisit(node);
        public virtual TResult VisitFunctionDeclaration(FunctionDeclarationSyntax node) => DefaultVisit(node);
        public virtual TResult VisitFunctionInstruction(FunctionInstructionSyntax node) => DefaultVisit(node);
        public virtual TResult VisitFunctionType(FunctionTypeSyntax node) => DefaultVisit(node);
        public virtual TResult VisitImportDirective(ImportDirectiveSyntax node) => DefaultVisit(node);
        public virtual TResult VisitLabeledInstruction(LabeledInstructionSyntax node) => DefaultVisit(node);
        public virtual TResult VisitLocalDeclaration(LocalDeclarationSyntax node) => DefaultVisit(node);
        public virtual TResult VisitLocalInstruction(LocalInstructionSyntax node) => DefaultVisit(node);
        public virtual TResult VisitModuleDeclaration(ModuleDeclarationSyntax node) => DefaultVisit(node);
        public virtual TResult VisitNoneInstruction(NoneInstructionSyntax node) => DefaultVisit(node);
        public virtual TResult VisitParameterDeclaration(ParameterDeclarationSyntax node) => DefaultVisit(node);
        public virtual TResult VisitPredefinedType(PredefinedTypeSyntax node) => DefaultVisit(node);
        public virtual TResult VisitReferenceType(ReferenceTypeSyntax node) => DefaultVisit(node);
        public virtual TResult VisitSpanType(SpanTypeSyntax node) => DefaultVisit(node);
        public virtual TResult VisitStructuredType(StructuredTypeSyntax node) => DefaultVisit(node);
        public virtual TResult VisitTypeDeclaration(TypeDeclarationSyntax node) => DefaultVisit(node);
        public virtual TResult VisitTypeInstruction(TypeInstructionSyntax node) => DefaultVisit(node);
    }
}
