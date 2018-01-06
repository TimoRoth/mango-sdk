using System.Linq;

namespace Mango.Compiler.Syntax
{
    public abstract partial class SyntaxRewriter : SyntaxVisitor<SyntaxNode>
    {
        public SyntaxRewriter()
        {
        }

        public virtual SyntaxList<TNode> VisitList<TNode>(SyntaxList<TNode> list) where TNode : SyntaxNode
        {
            return SyntaxFactory.List(list.Select(VisitListElement).ToArray());
        }

        public virtual TNode VisitListElement<TNode>(TNode node) where TNode : SyntaxNode
        {
            return (TNode)Visit(node);
        }
    }

    partial class SyntaxRewriter
    {
        public override SyntaxNode VisitArgumentInstruction(ArgumentInstructionSyntax node) { return SyntaxFactory.ArgumentInstruction(node.Kind, node.ParameterName); }
        public override SyntaxNode VisitArrayType(ArrayTypeSyntax node) { return SyntaxFactory.ArrayType((TypeSyntax)Visit(node.ElementType), node.Length); }
        public override SyntaxNode VisitBranchInstruction(BranchInstructionSyntax node) { return SyntaxFactory.BranchInstruction(node.Kind, node.LabelName); }
        public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node) { return SyntaxFactory.CompilationUnit(VisitList(node.Modules)); }
        public override SyntaxNode VisitConstantInstruction(ConstantInstructionSyntax node) { return SyntaxFactory.ConstantInstruction(node.Kind, (TypeSyntax)Visit(node.ConstantType), node.ConstantValue); }
        public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node) { return SyntaxFactory.FieldDeclaration((TypeSyntax)Visit(node.FieldType), node.FieldName); }
        public override SyntaxNode VisitFieldInstruction(FieldInstructionSyntax node) { return SyntaxFactory.FieldInstruction(node.Kind, (TypeSyntax)Visit(node.FieldType), (StructuredTypeSyntax)Visit(node.ContainingType), node.FieldName); }
        public override SyntaxNode VisitFunctionBody(FunctionBodySyntax node) { return SyntaxFactory.FunctionBody(VisitList(node.Locals), VisitList(node.Instructions)); }
        public override SyntaxNode VisitFunctionDeclaration(FunctionDeclarationSyntax node) { return SyntaxFactory.FunctionDeclaration((TypeSyntax)Visit(node.ReturnType), node.FunctionName, VisitList(node.Parameters), (FunctionBodySyntax)Visit(node.Body)); }
        public override SyntaxNode VisitFunctionInstruction(FunctionInstructionSyntax node) { return SyntaxFactory.FunctionInstruction(node.Kind, (TypeSyntax)Visit(node.ReturnType), node.ModuleName, node.FunctionName, VisitList(node.ParameterTypes)); }
        public override SyntaxNode VisitFunctionType(FunctionTypeSyntax node) { return SyntaxFactory.FunctionType((TypeSyntax)Visit(node.ReturnType), VisitList(node.ParameterTypes)); }
        public override SyntaxNode VisitImportDirective(ImportDirectiveSyntax node) { return SyntaxFactory.ImportDirective(node.ModuleName); }
        public override SyntaxNode VisitLabeledInstruction(LabeledInstructionSyntax node) { return SyntaxFactory.LabeledInstruction(node.LabelName, (InstructionSyntax)Visit(node.LabeledInstruction)); }
        public override SyntaxNode VisitLocalDeclaration(LocalDeclarationSyntax node) { return SyntaxFactory.LocalDeclaration((TypeSyntax)Visit(node.LocalType), node.LocalName); }
        public override SyntaxNode VisitLocalInstruction(LocalInstructionSyntax node) { return SyntaxFactory.LocalInstruction(node.Kind, node.LocalName); }
        public override SyntaxNode VisitModuleDeclaration(ModuleDeclarationSyntax node) { return SyntaxFactory.ModuleDeclaration(node.ModuleName, VisitList(node.Imports), VisitList(node.Members)); }
        public override SyntaxNode VisitNoneInstruction(NoneInstructionSyntax node) { return SyntaxFactory.NoneInstruction(node.Kind); }
        public override SyntaxNode VisitParameterDeclaration(ParameterDeclarationSyntax node) { return SyntaxFactory.ParameterDeclaration((TypeSyntax)Visit(node.ParameterType), node.ParameterName); }
        public override SyntaxNode VisitPredefinedType(PredefinedTypeSyntax node) { return SyntaxFactory.PredefinedType(node.Kind); }
        public override SyntaxNode VisitReferenceType(ReferenceTypeSyntax node) { return SyntaxFactory.ReferenceType((TypeSyntax)Visit(node.ReferencedType)); }
        public override SyntaxNode VisitSpanType(SpanTypeSyntax node) { return SyntaxFactory.SpanType((TypeSyntax)Visit(node.ElementType)); }
        public override SyntaxNode VisitStructuredType(StructuredTypeSyntax node) { return SyntaxFactory.StructuredType(node.ModuleName, node.TypeName); }
        public override SyntaxNode VisitTypeDeclaration(TypeDeclarationSyntax node) { return SyntaxFactory.TypeDeclaration(node.TypeName, VisitList(node.Fields)); }
        public override SyntaxNode VisitTypeInstruction(TypeInstructionSyntax node) { return SyntaxFactory.TypeInstruction(node.Kind, (TypeSyntax)node.Type); }
    }
}
