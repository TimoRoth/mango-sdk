namespace Mango.Compiler.Syntax
{
    public abstract partial class SyntaxWalker : SyntaxVisitor
    {
        public SyntaxWalker()
        {
        }

        public virtual void VisitList<TNode>(SyntaxList<TNode> list) where TNode : SyntaxNode
        {
            foreach (var item in list)
            {
                VisitListElement(item);
            }
        }

        public virtual void VisitListElement<TNode>(TNode node) where TNode : SyntaxNode
        {
            Visit(node);
        }
    }

    partial class SyntaxWalker
    {
        public override void VisitArgumentInstruction(ArgumentInstructionSyntax node) { }
        public override void VisitArrayType(ArrayTypeSyntax node) { Visit(node.ElementType); }
        public override void VisitBranchInstruction(BranchInstructionSyntax node) { }
        public override void VisitCompilationUnit(CompilationUnitSyntax node) { VisitList(node.Modules); }
        public override void VisitConstantInstruction(ConstantInstructionSyntax node) { Visit(node.ConstantType); }
        public override void VisitFieldDeclaration(FieldDeclarationSyntax node) { Visit(node.FieldType); }
        public override void VisitFieldInstruction(FieldInstructionSyntax node) { Visit(node.FieldType); Visit(node.ContainingType); }
        public override void VisitFunctionBody(FunctionBodySyntax node) { VisitList(node.Locals); VisitList(node.Instructions); }
        public override void VisitFunctionDeclaration(FunctionDeclarationSyntax node) { Visit(node.ReturnType); VisitList(node.Parameters); Visit(node.Body); }
        public override void VisitFunctionInstruction(FunctionInstructionSyntax node) { Visit(node.ReturnType); VisitList(node.ParameterTypes); }
        public override void VisitFunctionType(FunctionTypeSyntax node) { Visit(node.ReturnType); VisitList(node.ParameterTypes); }
        public override void VisitImportDirective(ImportDirectiveSyntax node) { }
        public override void VisitLabeledInstruction(LabeledInstructionSyntax node) { Visit(node.LabeledInstruction); }
        public override void VisitLocalDeclaration(LocalDeclarationSyntax node) { Visit(node.LocalType); }
        public override void VisitLocalInstruction(LocalInstructionSyntax node) { }
        public override void VisitModuleDeclaration(ModuleDeclarationSyntax node) { VisitList(node.Imports); VisitList(node.Members); }
        public override void VisitNoneInstruction(NoneInstructionSyntax node) { }
        public override void VisitParameterDeclaration(ParameterDeclarationSyntax node) { Visit(node.ParameterType); }
        public override void VisitPredefinedType(PredefinedTypeSyntax node) { }
        public override void VisitReferenceType(ReferenceTypeSyntax node) { Visit(node.ReferencedType); }
        public override void VisitSpanType(SpanTypeSyntax node) { Visit(node.ElementType); }
        public override void VisitStructuredType(StructuredTypeSyntax node) { }
        public override void VisitTypeDeclaration(TypeDeclarationSyntax node) { VisitList(node.Fields); }
        public override void VisitTypeInstruction(TypeInstructionSyntax node) { }
    }
}
