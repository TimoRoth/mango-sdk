namespace Mango.Compiler.Syntax
{
    public abstract partial class TypeSyntax : SyntaxNode { private protected TypeSyntax(SyntaxKind kind) : base(kind) { } }
    public sealed partial class ArrayTypeSyntax : TypeSyntax { public TypeSyntax ElementType { get; } public int Length { get; } internal ArrayTypeSyntax(TypeSyntax elementType, int length) : base(SyntaxKind.ArrayType) { ElementType = elementType; Length = length; } }
    public sealed partial class FunctionTypeSyntax : TypeSyntax { public TypeSyntax ReturnType { get; } public SyntaxList<TypeSyntax> ParameterTypes { get; } internal FunctionTypeSyntax(TypeSyntax returnType, SyntaxList<TypeSyntax> parameterTypes) : base(SyntaxKind.FunctionType) { ReturnType = returnType; ParameterTypes = parameterTypes; } }
    public sealed partial class StructuredTypeSyntax : TypeSyntax { public string ModuleName { get; } public string TypeName { get; } internal StructuredTypeSyntax(string moduleName, string typeName) : base(SyntaxKind.StructuredType) { ModuleName = moduleName; TypeName = typeName; } }
    public sealed partial class PredefinedTypeSyntax : TypeSyntax { internal PredefinedTypeSyntax(SyntaxKind kind) : base(kind) { } }
    public sealed partial class ReferenceTypeSyntax : TypeSyntax { public TypeSyntax ReferencedType { get; } internal ReferenceTypeSyntax(TypeSyntax referencedType) : base(SyntaxKind.ReferenceType) { ReferencedType = referencedType; } }
    public sealed partial class SpanTypeSyntax : TypeSyntax { public TypeSyntax ElementType { get; } internal SpanTypeSyntax(TypeSyntax elementType) : base(SyntaxKind.SpanType) { ElementType = elementType; } }

    public sealed partial class CompilationUnitSyntax : TypeSyntax { public SyntaxList<ModuleDeclarationSyntax> Modules { get; } internal CompilationUnitSyntax(SyntaxList<ModuleDeclarationSyntax> modules) : base(SyntaxKind.CompilationUnit) { Modules = modules; } }

    public sealed partial class ModuleDeclarationSyntax : SyntaxNode { public string ModuleName { get; } public SyntaxList<ModuleMemberSyntax> Members { get; } internal ModuleDeclarationSyntax(string moduleName, SyntaxList<ModuleMemberSyntax> members) : base(SyntaxKind.ModuleDeclaration) { ModuleName = moduleName; Members = members; } }
    public abstract partial class ModuleMemberSyntax : SyntaxNode { private protected ModuleMemberSyntax(SyntaxKind kind) : base(kind) { } }

    public sealed partial class TypeDeclarationSyntax : ModuleMemberSyntax { public string TypeName { get; } public SyntaxList<FieldDeclarationSyntax> Fields { get; } internal TypeDeclarationSyntax(string typeName, SyntaxList<FieldDeclarationSyntax> fields) : base(SyntaxKind.TypeDeclaration) { TypeName = typeName; Fields = fields; } }
    public sealed partial class FieldDeclarationSyntax : SyntaxNode { public TypeSyntax FieldType { get; } public string FieldName { get; } internal FieldDeclarationSyntax(TypeSyntax fieldType, string fieldName) : base(SyntaxKind.FieldDeclaration) { FieldType = fieldType; FieldName = fieldName; } }

    public sealed partial class FunctionDeclarationSyntax : ModuleMemberSyntax { public TypeSyntax ReturnType { get; } public string FunctionName { get; } public SyntaxList<ParameterDeclarationSyntax> Parameters { get; } public int? SystemCallOrdinal { get; } public FunctionBodySyntax Body { get; } internal FunctionDeclarationSyntax(TypeSyntax returnType, string functionName, SyntaxList<ParameterDeclarationSyntax> parameters, int? systemCallOrdinal, FunctionBodySyntax body) : base(SyntaxKind.FunctionDeclaration) { ReturnType = returnType; FunctionName = functionName; Parameters = parameters; SystemCallOrdinal = systemCallOrdinal; Body = body; } }
    public sealed partial class ParameterDeclarationSyntax : SyntaxNode { public TypeSyntax ParameterType { get; } public string ParameterName { get; } internal ParameterDeclarationSyntax(TypeSyntax parameterType, string parameterName) : base(SyntaxKind.ParameterDeclaration) { ParameterType = parameterType; ParameterName = parameterName; } }
    public sealed partial class FunctionBodySyntax : SyntaxNode { public SyntaxList<LocalDeclarationSyntax> Locals { get; } public SyntaxList<InstructionSyntax> Instructions { get; } internal FunctionBodySyntax(SyntaxList<LocalDeclarationSyntax> locals, SyntaxList<InstructionSyntax> instructions) : base(SyntaxKind.FunctionBody) { Locals = locals; Instructions = instructions; } }
    public sealed partial class LocalDeclarationSyntax : SyntaxNode { public TypeSyntax LocalType { get; } public string LocalName { get; } internal LocalDeclarationSyntax(TypeSyntax localType, string localName) : base(SyntaxKind.LocalDeclaration) { LocalType = localType; LocalName = localName; } }

    public abstract partial class InstructionSyntax : SyntaxNode { private protected InstructionSyntax(SyntaxKind kind) : base(kind) { } }
    public sealed partial class ArgumentInstructionSyntax : InstructionSyntax { public string ParameterName { get; } internal ArgumentInstructionSyntax(SyntaxKind kind, string parameterName) : base(kind) { ParameterName = parameterName; } }
    public sealed partial class BranchInstructionSyntax : InstructionSyntax { public string LabelName { get; } internal BranchInstructionSyntax(SyntaxKind kind, string labelName) : base(kind) { LabelName = labelName; } }
    public sealed partial class ConstantInstructionSyntax : InstructionSyntax { public TypeSyntax ConstantType { get; } public int ConstantValue { get; } internal ConstantInstructionSyntax(SyntaxKind kind, TypeSyntax constantType, int constantValue) : base(kind) { ConstantType = constantType; ConstantValue = constantValue; } }
    public sealed partial class FieldInstructionSyntax : InstructionSyntax { public TypeSyntax FieldType { get; } public StructuredTypeSyntax ContainingType { get; } public string FieldName { get; } internal FieldInstructionSyntax(SyntaxKind kind, TypeSyntax fieldType, StructuredTypeSyntax containingType, string fieldName) : base(kind) { FieldType = fieldType; ContainingType = containingType; FieldName = fieldName; } }
    public sealed partial class FunctionInstructionSyntax : InstructionSyntax { public TypeSyntax ReturnType { get; } public string ModuleName { get; } public string FunctionName { get; } public SyntaxList<TypeSyntax> ParameterTypes { get; } internal FunctionInstructionSyntax(SyntaxKind kind, TypeSyntax returnType, string moduleName, string functionName, SyntaxList<TypeSyntax> parameterTypes) : base(kind) { ReturnType = returnType; ModuleName = moduleName; FunctionName = functionName; ParameterTypes = parameterTypes; } }
    public sealed partial class LabeledInstructionSyntax : InstructionSyntax { public string LabelName { get; } public InstructionSyntax LabeledInstruction { get; } internal LabeledInstructionSyntax(string labelName, InstructionSyntax labeledInstruction) : base(SyntaxKind.LabeledInstruction) { LabelName = labelName; LabeledInstruction = labeledInstruction; } }
    public sealed partial class LocalInstructionSyntax : InstructionSyntax { public string LocalName { get; } internal LocalInstructionSyntax(SyntaxKind kind, string localName) : base(kind) { LocalName = localName; } }
    public sealed partial class NoneInstructionSyntax : InstructionSyntax { internal NoneInstructionSyntax(SyntaxKind kind) : base(kind) { } }
    public sealed partial class TypeInstructionSyntax : InstructionSyntax { public TypeSyntax Type { get; } internal TypeInstructionSyntax(SyntaxKind kind, TypeSyntax type) : base(kind) { Type = type; } }
}