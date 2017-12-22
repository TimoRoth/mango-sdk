namespace Mango.Compiler.Syntax
{
    public static class SyntaxFactory
    {
        public static ArrayTypeSyntax ArrayType(TypeSyntax elementType, int length) { var syntax = new ArrayTypeSyntax(elementType, length); syntax.ElementType.Parent = syntax; return syntax; }
        public static FunctionTypeSyntax FunctionType(TypeSyntax returnType, SyntaxList<TypeSyntax> parameterTypes) { var syntax = new FunctionTypeSyntax(returnType, parameterTypes); syntax.ReturnType.Parent = syntax; foreach (var parameterType in syntax.ParameterTypes) parameterType.Parent = syntax; return syntax; }
        public static StructuredTypeSyntax StructuredType(string moduleName, string typeName) { var syntax = new StructuredTypeSyntax(moduleName, typeName); return syntax; }
        public static PredefinedTypeSyntax PredefinedType(SyntaxKind kind) { var syntax = new PredefinedTypeSyntax(kind); return syntax; }
        public static ReferenceTypeSyntax ReferenceType(TypeSyntax referencedType) { var syntax = new ReferenceTypeSyntax(referencedType); syntax.ReferencedType.Parent = syntax; return syntax; }
        public static SpanTypeSyntax SpanType(TypeSyntax elementType) { var syntax = new SpanTypeSyntax(elementType); syntax.ElementType.Parent = syntax; return syntax; }

        public static CompilationUnitSyntax CompilationUnit(SyntaxList<ModuleDeclarationSyntax> modules) { var syntax = new CompilationUnitSyntax(modules); foreach (var module in syntax.Modules) module.Parent = syntax; return syntax; }

        public static ModuleDeclarationSyntax ModuleDeclaration(string moduleName, SyntaxList<ImportDirectiveSyntax> imports, SyntaxList<ModuleMemberSyntax> members) { var syntax = new ModuleDeclarationSyntax(moduleName, imports, members); foreach (var import in imports) import.Parent = syntax; foreach (var member in syntax.Members) member.Parent = syntax; return syntax; }
        public static ImportDirectiveSyntax ImportDirective(string moduleName) { var syntax = new ImportDirectiveSyntax(moduleName); return syntax; }

        public static TypeDeclarationSyntax TypeDeclaration(string typeName, SyntaxList<FieldDeclarationSyntax> fields) { var syntax = new TypeDeclarationSyntax(typeName, fields); foreach (var field in syntax.Fields) field.Parent = syntax; return syntax; }
        public static FieldDeclarationSyntax FieldDeclaration(TypeSyntax fieldType, string fieldName) { var syntax = new FieldDeclarationSyntax(fieldType, fieldName); syntax.FieldType.Parent = syntax; return syntax; }

        public static FunctionDeclarationSyntax FunctionDeclaration(TypeSyntax returnType, string functionName, SyntaxList<ParameterDeclarationSyntax> parameters, int systemCallOrdinal) { var syntax = new FunctionDeclarationSyntax(returnType, functionName, parameters, systemCallOrdinal, null); syntax.ReturnType.Parent = syntax; foreach (var parameter in syntax.Parameters) parameter.Parent = syntax; return syntax; }
        public static FunctionDeclarationSyntax FunctionDeclaration(TypeSyntax returnType, string functionName, SyntaxList<ParameterDeclarationSyntax> parameters, FunctionBodySyntax body) { var syntax = new FunctionDeclarationSyntax(returnType, functionName, parameters, null, body); syntax.ReturnType.Parent = syntax; foreach (var parameter in syntax.Parameters) parameter.Parent = syntax; syntax.Body.Parent = syntax; return syntax; }
        public static ParameterDeclarationSyntax ParameterDeclaration(TypeSyntax parameterType, string parameterName) { var syntax = new ParameterDeclarationSyntax(parameterType, parameterName); syntax.ParameterType.Parent = syntax; return syntax; }
        public static FunctionBodySyntax FunctionBody(SyntaxList<LocalDeclarationSyntax> locals, SyntaxList<InstructionSyntax> instructions) { var syntax = new FunctionBodySyntax(locals, instructions); foreach (var local in syntax.Locals) local.Parent = syntax; foreach (var instruction in syntax.Instructions) instruction.Parent = syntax; return syntax; }
        public static LocalDeclarationSyntax LocalDeclaration(TypeSyntax localType, string localName) { var syntax = new LocalDeclarationSyntax(localType, localName); syntax.LocalType.Parent = syntax; return syntax; }

        public static ArgumentInstructionSyntax ArgumentInstruction(SyntaxKind kind, string parameterName) { var syntax = new ArgumentInstructionSyntax(kind, parameterName); return syntax; }
        public static BranchInstructionSyntax BranchInstruction(SyntaxKind kind, string labelName) { var syntax = new BranchInstructionSyntax(kind, labelName); return syntax; }
        public static ConstantInstructionSyntax ConstantInstruction(SyntaxKind kind, TypeSyntax constantType, int constantValue) { var syntax = new ConstantInstructionSyntax(kind, constantType, constantValue); syntax.ConstantType.Parent = syntax; return syntax; }
        public static FieldInstructionSyntax FieldInstruction(SyntaxKind kind, TypeSyntax fieldType, StructuredTypeSyntax containingType, string fieldName) { var syntax = new FieldInstructionSyntax(kind, fieldType, containingType, fieldName); syntax.FieldType.Parent = syntax; syntax.ContainingType.Parent = syntax; return syntax; }
        public static FunctionInstructionSyntax FunctionInstruction(SyntaxKind kind, TypeSyntax returnType, string moduleName, string functionName, SyntaxList<TypeSyntax> parameterTypes) { var syntax = new FunctionInstructionSyntax(kind, returnType, moduleName, functionName, parameterTypes); syntax.ReturnType.Parent = syntax; foreach (var parameterType in syntax.ParameterTypes) parameterType.Parent = syntax; return syntax; }
        public static LabeledInstructionSyntax LabeledInstruction(string labelName, InstructionSyntax labeledInstruction) { var syntax = new LabeledInstructionSyntax(labelName, labeledInstruction); syntax.LabeledInstruction.Parent = syntax; return syntax; }
        public static LocalInstructionSyntax LocalInstruction(SyntaxKind kind, string localName) { var syntax = new LocalInstructionSyntax(kind, localName); return syntax; }
        public static NoneInstructionSyntax NoneInstruction(SyntaxKind kind) { var syntax = new NoneInstructionSyntax(kind); return syntax; }
        public static TypeInstructionSyntax TypeInstruction(SyntaxKind kind, TypeSyntax type) { var syntax = new TypeInstructionSyntax(kind, type); syntax.Type.Parent = syntax; return syntax; }

        public static SyntaxList<TNode> List<TNode>(params TNode[] nodes) where TNode : SyntaxNode => new SyntaxList<TNode>(nodes);

        public static TypeSyntax ParseType(string text) => new Parser.SimpleParser(text).ParseTypeExpression();
    }
}
