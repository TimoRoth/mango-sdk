using System;
using System.Collections.Generic;
using Mango.Compiler.Diagnostics;
using Mango.Compiler.Syntax;

namespace Mango.Compiler.Parser
{
    partial class SimpleParser
    {
        private string ParseIdentifierToken()
        {
            if (CurrentToken == SyntaxKind.IdentifierToken)
            {
                var identifier = _tokenInfo.Identifier;
                EatToken();
                return identifier;
            }
            else
            {
                AddError(ErrorCode.ERR_IdentifierExpected);
                return null;
            }
        }

        private int ParseI32LiteralToken()
        {
            if (CurrentToken == SyntaxKind.I32LiteralToken)
            {
                var literal = _tokenInfo.I32Value;
                EatToken();
                return literal;
            }
            else
            {
                AddError(ErrorCode.ERR_I32LiteralExpected);
                return 0;
            }
        }

        private string ParseOptionalModuleIdentifier()
        {
            var name = (string)null;

            if (CurrentToken == SyntaxKind.OpenBracketToken)
            {
                EatToken();
                name = ParseIdentifierToken();
                EatToken(SyntaxKind.CloseBracketToken);
            }

            return name;
        }

        public TypeSyntax ParseTypeExpression()
        {
            var moduleName = (string)null;
            var typeName = (string)null;

            TypeSyntax t;

            switch (CurrentToken)
            {
            case SyntaxKind.BoolKeyword:
                EatToken();
                t = SyntaxFactory.PredefinedType(SyntaxKind.BoolType);
                break;
            case SyntaxKind.I8Keyword:
                EatToken();
                t = SyntaxFactory.PredefinedType(SyntaxKind.Int8Type);
                break;
            case SyntaxKind.U8Keyword:
                EatToken();
                t = SyntaxFactory.PredefinedType(SyntaxKind.UInt8Type);
                break;
            case SyntaxKind.I16Keyword:
                EatToken();
                t = SyntaxFactory.PredefinedType(SyntaxKind.Int16Type);
                break;
            case SyntaxKind.U16Keyword:
                EatToken();
                t = SyntaxFactory.PredefinedType(SyntaxKind.UInt16Type);
                break;
            case SyntaxKind.I32Keyword:
                EatToken();
                t = SyntaxFactory.PredefinedType(SyntaxKind.Int32Type);
                break;
            case SyntaxKind.U32Keyword:
                EatToken();
                t = SyntaxFactory.PredefinedType(SyntaxKind.UInt32Type);
                break;
            case SyntaxKind.I64Keyword:
                EatToken();
                t = SyntaxFactory.PredefinedType(SyntaxKind.Int64Type);
                break;
            case SyntaxKind.U64Keyword:
                EatToken();
                t = SyntaxFactory.PredefinedType(SyntaxKind.UInt64Type);
                break;
            case SyntaxKind.F32Keyword:
                EatToken();
                t = SyntaxFactory.PredefinedType(SyntaxKind.Float32Type);
                break;
            case SyntaxKind.F64Keyword:
                EatToken();
                t = SyntaxFactory.PredefinedType(SyntaxKind.Float64Type);
                break;
            case SyntaxKind.VoidKeyword:
                EatToken();
                t = SyntaxFactory.PredefinedType(SyntaxKind.VoidType);
                break;
            case SyntaxKind.OpenBracketToken:
                EatToken();
                moduleName = ParseIdentifierToken();
                EatToken(SyntaxKind.CloseBracketToken);
                goto case SyntaxKind.IdentifierToken;
            case SyntaxKind.IdentifierToken:
                typeName = ParseIdentifierToken();
                t = SyntaxFactory.StructuredType(moduleName, typeName);
                break;
            default:
                throw new System.Exception();
            }

            while (true)
            {
                switch (CurrentToken)
                {
                case SyntaxKind.AmpersandToken:
                    EatToken();
                    t = SyntaxFactory.ReferenceType(t);
                    break;
                case SyntaxKind.OpenBracketToken:
                    EatToken();
                    if (CurrentToken == SyntaxKind.I32LiteralToken)
                    {
                        t = SyntaxFactory.ArrayType(t, ParseI32LiteralToken());
                    }
                    else
                    {
                        t = SyntaxFactory.SpanType(t);
                    }
                    EatToken(SyntaxKind.CloseBracketToken);
                    break;
                case SyntaxKind.OpenParenToken:
                    EatToken();
                    var ps = new List<TypeSyntax>();
                    while (CurrentToken != SyntaxKind.CloseParenToken)
                    {
                        ps.Add(ParseTypeExpression());
                        if (CurrentToken != SyntaxKind.CommaToken)
                            break;
                        EatToken();
                    }
                    EatToken(SyntaxKind.CloseParenToken);
                    t = SyntaxFactory.FunctionType(t, SyntaxFactory.List(ps.ToArray()));
                    break;
                default:
                    return t;
                }
            }
        }

        public CompilationUnitSyntax ParseCompilationUnit()
        {
            var modules = new List<ModuleDeclarationSyntax>();
            var reportUnexpectedToken = true;

            while (true)
            {
                switch (CurrentToken)
                {
                case SyntaxKind.EndOfFileToken:
                    EatToken();
                    return SyntaxFactory.CompilationUnit(SyntaxFactory.List(modules.ToArray()));

                case SyntaxKind.ModuleKeyword:
                    modules.Add(ParseModuleDeclaration());
                    reportUnexpectedToken = true;
                    break;

                default:
                    var ct = EatToken();
                    if (reportUnexpectedToken)
                    {
                        AddError(ErrorCode.ERR_EOFExpected);
                        reportUnexpectedToken = false;
                    }
                    break;
                }
            }
        }

        private ModuleDeclarationSyntax ParseModuleDeclaration()
        {
            EatToken(SyntaxKind.ModuleKeyword);
            var name = ParseIdentifierToken();
            EatToken(SyntaxKind.OpenBraceToken);
            var members = new List<ModuleMemberSyntax>();
            var reportUnexpectedToken = true;

            while (true)
            {
                switch (CurrentToken)
                {
                case SyntaxKind.CloseBraceToken:
                    EatToken();
                    return SyntaxFactory.ModuleDeclaration(name, SyntaxFactory.List(members.ToArray()));

                case SyntaxKind.TypeKeyword:
                    members.Add(ParseTypeDeclaration());
                    break;

                case SyntaxKind.DeclareKeyword:
                case SyntaxKind.DefineKeyword:
                    members.Add(ParseFunctionDeclaration());
                    break;

                default:
                    var ct = EatToken();
                    if (reportUnexpectedToken)
                    {
                        AddError(ErrorCode.ERR_ModuleExpected);
                        reportUnexpectedToken = false;
                    }
                    break;
                }
            }
        }

        private TypeDeclarationSyntax ParseTypeDeclaration()
        {
            EatToken(SyntaxKind.TypeKeyword);
            var name = ParseIdentifierToken();
            EatToken(SyntaxKind.OpenBraceToken);
            var fields = new List<FieldDeclarationSyntax>();
            var reportUnexpectedToken = true;

            while (true)
            {
                switch (CurrentToken)
                {
                case SyntaxKind.CloseBraceToken:
                    EatToken();
                    return SyntaxFactory.TypeDeclaration(name, SyntaxFactory.List(fields.ToArray()));

                case SyntaxKind.FieldKeyword:
                    fields.Add(ParseFieldDeclaration());
                    break;

                default:
                    var ct = EatToken();
                    if (reportUnexpectedToken)
                    {
                        AddError(ErrorCode.ERR_ModuleExpected);
                        reportUnexpectedToken = false;
                    }
                    break;
                }
            }
        }

        private FieldDeclarationSyntax ParseFieldDeclaration()
        {
            EatToken(SyntaxKind.FieldKeyword);

            var type = ParseTypeExpression();
            var name = ParseIdentifierToken();

            return SyntaxFactory.FieldDeclaration(type, name);
        }

        private FunctionDeclarationSyntax ParseFunctionDeclaration()
        {
            bool isDefinition;

            switch (CurrentToken)
            {
            case SyntaxKind.DeclareKeyword:
                EatToken();
                isDefinition = false;
                break;

            case SyntaxKind.DefineKeyword:
                EatToken();
                isDefinition = true;
                break;

            default:
                throw new Exception();
            }

            var returnType = ParseTypeExpression();
            var name = ParseIdentifierToken();
            var parameters = ParseParameterList();

            if (isDefinition)
            {
                return SyntaxFactory.FunctionDeclaration(returnType, name, parameters, ParseFunctionBody());
            }
            else
            {
                return SyntaxFactory.FunctionDeclaration(returnType, name, parameters, ParseI32LiteralToken());
            }
        }

        private SyntaxList<TypeSyntax> ParseTypeList()
        {
            var types = new List<TypeSyntax>();

            EatToken(SyntaxKind.OpenParenToken);

            while (CurrentToken != SyntaxKind.CloseParenToken)
            {
                types.Add(ParseTypeExpression());

                if (CurrentToken != SyntaxKind.CommaToken)
                {
                    break;
                }

                EatToken();
            }

            EatToken(SyntaxKind.CloseParenToken);
            return SyntaxFactory.List(types.ToArray());
        }

        private SyntaxList<ParameterDeclarationSyntax> ParseParameterList()
        {
            var parameters = new List<ParameterDeclarationSyntax>();

            EatToken(SyntaxKind.OpenParenToken);

            while (CurrentToken != SyntaxKind.CloseParenToken)
            {
                var type = ParseTypeExpression();
                var name = ParseIdentifierToken();

                parameters.Add(SyntaxFactory.ParameterDeclaration(type, name));

                if (CurrentToken != SyntaxKind.CommaToken)
                {
                    break;
                }

                EatToken();
            }

            EatToken(SyntaxKind.CloseParenToken);
            return SyntaxFactory.List(parameters.ToArray());
        }

        private SyntaxList<LocalDeclarationSyntax> ParseLocalsList()
        {
            var locals = new List<LocalDeclarationSyntax>();

            while (CurrentToken == SyntaxKind.LocalKeyword)
            {
                EatToken();

                var type = ParseTypeExpression();
                var name = ParseIdentifierToken();

                locals.Add(SyntaxFactory.LocalDeclaration(type, name));
            }

            return SyntaxFactory.List(locals.ToArray());
        }

        private FunctionBodySyntax ParseFunctionBody()
        {
            EatToken(SyntaxKind.OpenBraceToken);
            var locals = ParseLocalsList();
            var instructions = new List<InstructionSyntax>();
            var reportUnexpectedToken = true;

            while (true)
            {
                switch (CurrentToken)
                {
                case SyntaxKind.CloseBraceToken:
                    EatToken();
                    return SyntaxFactory.FunctionBody(locals, SyntaxFactory.List(instructions.ToArray()));

                case SyntaxKind.IdentifierToken:
                case SyntaxKind.LdnullKeyword:
                case SyntaxKind.LdcKeyword:
                case SyntaxKind.LdftnKeyword:
                case SyntaxKind.LdindKeyword:
                case SyntaxKind.LdfldKeyword:
                case SyntaxKind.LdelemKeyword:
                case SyntaxKind.LdlocKeyword:
                case SyntaxKind.LdargKeyword:
                case SyntaxKind.LdfldaKeyword:
                case SyntaxKind.LdelemaKeyword:
                case SyntaxKind.LdlocaKeyword:
                case SyntaxKind.LdargaKeyword:
                case SyntaxKind.StindKeyword:
                case SyntaxKind.StfldKeyword:
                case SyntaxKind.StelemKeyword:
                case SyntaxKind.StlocKeyword:
                case SyntaxKind.StargKeyword:
                case SyntaxKind.SyscallKeyword:
                case SyntaxKind.CallKeyword:
                case SyntaxKind.CalliKeyword:
                case SyntaxKind.RetKeyword:
                case SyntaxKind.BreakKeyword:
                case SyntaxKind.DupKeyword:
                case SyntaxKind.NopKeyword:
                case SyntaxKind.PopKeyword:
                case SyntaxKind.AddKeyword:
                case SyntaxKind.AndKeyword:
                case SyntaxKind.DivKeyword:
                case SyntaxKind.DivUnKeyword:
                case SyntaxKind.MulKeyword:
                case SyntaxKind.OrKeyword:
                case SyntaxKind.RemKeyword:
                case SyntaxKind.RemUnKeyword:
                case SyntaxKind.ShlKeyword:
                case SyntaxKind.ShrKeyword:
                case SyntaxKind.ShrUnKeyword:
                case SyntaxKind.SubKeyword:
                case SyntaxKind.XorKeyword:
                case SyntaxKind.NegKeyword:
                case SyntaxKind.NotKeyword:
                case SyntaxKind.ConvKeyword:
                case SyntaxKind.ConvUnKeyword:
                case SyntaxKind.CeqKeyword:
                case SyntaxKind.CgtKeyword:
                case SyntaxKind.CgtUnKeyword:
                case SyntaxKind.CltKeyword:
                case SyntaxKind.CltUnKeyword:
                case SyntaxKind.BeqKeyword:
                case SyntaxKind.BgeKeyword:
                case SyntaxKind.BgeUnKeyword:
                case SyntaxKind.BgtKeyword:
                case SyntaxKind.BgtUnKeyword:
                case SyntaxKind.BleKeyword:
                case SyntaxKind.BleUnKeyword:
                case SyntaxKind.BltKeyword:
                case SyntaxKind.BltUnKeyword:
                case SyntaxKind.BneUnKeyword:
                case SyntaxKind.BrKeyword:
                case SyntaxKind.BrfalseKeyword:
                case SyntaxKind.BrtrueKeyword:
                case SyntaxKind.BeqSKeyword:
                case SyntaxKind.BgeSKeyword:
                case SyntaxKind.BgeUnSKeyword:
                case SyntaxKind.BgtSKeyword:
                case SyntaxKind.BgtUnSKeyword:
                case SyntaxKind.BleSKeyword:
                case SyntaxKind.BleUnSKeyword:
                case SyntaxKind.BltSKeyword:
                case SyntaxKind.BltUnSKeyword:
                case SyntaxKind.BneUnSKeyword:
                case SyntaxKind.BrSKeyword:
                case SyntaxKind.BrfalseSKeyword:
                case SyntaxKind.BrtrueSKeyword:
                case SyntaxKind.LdobjKeyword:
                case SyntaxKind.StobjKeyword:
                case SyntaxKind.CpobjKeyword:
                case SyntaxKind.InitobjKeyword:
                case SyntaxKind.LdlenKeyword:
                case SyntaxKind.NewarrKeyword:
                case SyntaxKind.NewobjKeyword:
                    instructions.Add(ParseInstruction());
                    break;

                default:
                    var ct = EatToken();
                    if (reportUnexpectedToken)
                    {
                        AddError(ErrorCode.ERR_InstructionExpected);
                        reportUnexpectedToken = false;
                    }
                    break;
                }
            }
        }

        private InstructionSyntax ParseInstruction()
        {
            switch (CurrentToken)
            {
            case SyntaxKind.IdentifierToken:
                var label = ParseIdentifierToken();
                EatToken(SyntaxKind.ColonToken);
                return SyntaxFactory.LabeledInstruction(label, ParseInstruction());

            case SyntaxKind.LdnullKeyword:
            case SyntaxKind.RetKeyword:
            case SyntaxKind.BreakKeyword:
            case SyntaxKind.DupKeyword:
            case SyntaxKind.NopKeyword:
            case SyntaxKind.PopKeyword:
            case SyntaxKind.AddKeyword:
            case SyntaxKind.AndKeyword:
            case SyntaxKind.DivKeyword:
            case SyntaxKind.DivUnKeyword:
            case SyntaxKind.MulKeyword:
            case SyntaxKind.OrKeyword:
            case SyntaxKind.RemKeyword:
            case SyntaxKind.RemUnKeyword:
            case SyntaxKind.ShlKeyword:
            case SyntaxKind.ShrKeyword:
            case SyntaxKind.ShrUnKeyword:
            case SyntaxKind.SubKeyword:
            case SyntaxKind.XorKeyword:
            case SyntaxKind.NegKeyword:
            case SyntaxKind.NotKeyword:
            case SyntaxKind.CeqKeyword:
            case SyntaxKind.CgtKeyword:
            case SyntaxKind.CgtUnKeyword:
            case SyntaxKind.CltKeyword:
            case SyntaxKind.CltUnKeyword:
            case SyntaxKind.LdlenKeyword:
                return ParseNoneInstruction();

            case SyntaxKind.LdcKeyword:
                return ParseConstantInstruction();

            case SyntaxKind.LdftnKeyword:
            case SyntaxKind.CallKeyword:
            case SyntaxKind.NewobjKeyword:
            case SyntaxKind.SyscallKeyword:
                return ParseFunctionInstruction();

            case SyntaxKind.LdindKeyword:
            case SyntaxKind.LdelemKeyword:
            case SyntaxKind.LdelemaKeyword:
            case SyntaxKind.StindKeyword:
            case SyntaxKind.StelemKeyword:
            case SyntaxKind.CalliKeyword:
            case SyntaxKind.ConvKeyword:
            case SyntaxKind.ConvUnKeyword:
            case SyntaxKind.LdobjKeyword:
            case SyntaxKind.StobjKeyword:
            case SyntaxKind.CpobjKeyword:
            case SyntaxKind.InitobjKeyword:
            case SyntaxKind.NewarrKeyword:
                return ParseTypeInstruction();

            case SyntaxKind.LdfldKeyword:
            case SyntaxKind.LdfldaKeyword:
            case SyntaxKind.StfldKeyword:
                return ParseFieldInstruction();

            case SyntaxKind.LdargKeyword:
            case SyntaxKind.LdargaKeyword:
            case SyntaxKind.StargKeyword:
                return ParseArgumentInstruction();

            case SyntaxKind.LdlocKeyword:
            case SyntaxKind.LdlocaKeyword:
            case SyntaxKind.StlocKeyword:
                return ParseLocalInstruction();

            case SyntaxKind.BeqKeyword:
            case SyntaxKind.BgeKeyword:
            case SyntaxKind.BgeUnKeyword:
            case SyntaxKind.BgtKeyword:
            case SyntaxKind.BgtUnKeyword:
            case SyntaxKind.BleKeyword:
            case SyntaxKind.BleUnKeyword:
            case SyntaxKind.BltKeyword:
            case SyntaxKind.BltUnKeyword:
            case SyntaxKind.BneUnKeyword:
            case SyntaxKind.BrKeyword:
            case SyntaxKind.BrfalseKeyword:
            case SyntaxKind.BrtrueKeyword:
            case SyntaxKind.BeqSKeyword:
            case SyntaxKind.BgeSKeyword:
            case SyntaxKind.BgeUnSKeyword:
            case SyntaxKind.BgtSKeyword:
            case SyntaxKind.BgtUnSKeyword:
            case SyntaxKind.BleSKeyword:
            case SyntaxKind.BleUnSKeyword:
            case SyntaxKind.BltSKeyword:
            case SyntaxKind.BltUnSKeyword:
            case SyntaxKind.BneUnSKeyword:
            case SyntaxKind.BrSKeyword:
            case SyntaxKind.BrfalseSKeyword:
            case SyntaxKind.BrtrueSKeyword:
                return ParseBranchInstruction();

            default:
                throw new Exception();
            }
        }

        private NoneInstructionSyntax ParseNoneInstruction()
        {
            SyntaxKind kind;

            switch (CurrentToken)
            {
            case SyntaxKind.LdnullKeyword: kind = SyntaxKind.Ldnull; break;
            case SyntaxKind.RetKeyword: kind = SyntaxKind.Ret; break;
            case SyntaxKind.BreakKeyword: kind = SyntaxKind.Break; break;
            case SyntaxKind.DupKeyword: kind = SyntaxKind.Dup; break;
            case SyntaxKind.NopKeyword: kind = SyntaxKind.Nop; break;
            case SyntaxKind.PopKeyword: kind = SyntaxKind.Pop; break;
            case SyntaxKind.AddKeyword: kind = SyntaxKind.Add; break;
            case SyntaxKind.AndKeyword: kind = SyntaxKind.And; break;
            case SyntaxKind.DivKeyword: kind = SyntaxKind.Div; break;
            case SyntaxKind.DivUnKeyword: kind = SyntaxKind.Div_Un; break;
            case SyntaxKind.MulKeyword: kind = SyntaxKind.Mul; break;
            case SyntaxKind.OrKeyword: kind = SyntaxKind.Or; break;
            case SyntaxKind.RemKeyword: kind = SyntaxKind.Rem; break;
            case SyntaxKind.RemUnKeyword: kind = SyntaxKind.Rem_Un; break;
            case SyntaxKind.ShlKeyword: kind = SyntaxKind.Shl; break;
            case SyntaxKind.ShrKeyword: kind = SyntaxKind.Shr; break;
            case SyntaxKind.ShrUnKeyword: kind = SyntaxKind.Shr_Un; break;
            case SyntaxKind.SubKeyword: kind = SyntaxKind.Sub; break;
            case SyntaxKind.XorKeyword: kind = SyntaxKind.Xor; break;
            case SyntaxKind.NegKeyword: kind = SyntaxKind.Neg; break;
            case SyntaxKind.NotKeyword: kind = SyntaxKind.Not; break;
            case SyntaxKind.CeqKeyword: kind = SyntaxKind.Ceq; break;
            case SyntaxKind.CgtKeyword: kind = SyntaxKind.Cgt; break;
            case SyntaxKind.CgtUnKeyword: kind = SyntaxKind.Cgt_Un; break;
            case SyntaxKind.CltKeyword: kind = SyntaxKind.Clt; break;
            case SyntaxKind.CltUnKeyword: kind = SyntaxKind.Clt_Un; break;
            case SyntaxKind.LdlenKeyword: kind = SyntaxKind.Ldlen; break;
            default: throw new Exception();
            }

            EatToken();

            return SyntaxFactory.NoneInstruction(kind);
        }

        private ConstantInstructionSyntax ParseConstantInstruction()
        {
            SyntaxKind kind;

            switch (CurrentToken)
            {
            case SyntaxKind.LdcKeyword: kind = SyntaxKind.Ldc; break;
            default: throw new Exception();
            }

            EatToken();
            var type = ParseTypeExpression();
            var value = ParseI32LiteralToken();

            return SyntaxFactory.ConstantInstruction(kind, type, value);
        }

        private FunctionInstructionSyntax ParseFunctionInstruction()
        {
            SyntaxKind kind;

            switch (CurrentToken)
            {
            case SyntaxKind.LdftnKeyword: kind = SyntaxKind.Ldftn; break;
            case SyntaxKind.CallKeyword: kind = SyntaxKind.Call; break;
            case SyntaxKind.NewobjKeyword: kind = SyntaxKind.Newobj; break;
            case SyntaxKind.SyscallKeyword: kind = SyntaxKind.Syscall; break;
            default: throw new Exception();
            }

            EatToken();
            var returnType = ParseTypeExpression();
            var moduleName = ParseOptionalModuleIdentifier();
            var functionName = ParseIdentifierToken();
            var parameterTypes = ParseTypeList();

            return SyntaxFactory.FunctionInstruction(kind, returnType, moduleName, functionName, parameterTypes);
        }

        private TypeInstructionSyntax ParseTypeInstruction()
        {
            SyntaxKind kind;

            switch (CurrentToken)
            {
            case SyntaxKind.LdindKeyword: kind = SyntaxKind.Ldind; break;
            case SyntaxKind.LdelemKeyword: kind = SyntaxKind.Ldelem; break;
            case SyntaxKind.LdelemaKeyword: kind = SyntaxKind.Ldelema; break;
            case SyntaxKind.StindKeyword: kind = SyntaxKind.Stind; break;
            case SyntaxKind.StelemKeyword: kind = SyntaxKind.Stelem; break;
            case SyntaxKind.CalliKeyword: kind = SyntaxKind.Calli; break;
            case SyntaxKind.ConvKeyword: kind = SyntaxKind.Conv; break;
            case SyntaxKind.ConvUnKeyword: kind = SyntaxKind.Conv_Un; break;
            case SyntaxKind.LdobjKeyword: kind = SyntaxKind.Ldobj; break;
            case SyntaxKind.StobjKeyword: kind = SyntaxKind.Stobj; break;
            case SyntaxKind.CpobjKeyword: kind = SyntaxKind.Cpobj; break;
            case SyntaxKind.InitobjKeyword: kind = SyntaxKind.Initobj; break;
            case SyntaxKind.NewarrKeyword: kind = SyntaxKind.Newarr; break;
            default: throw new Exception();
            }

            EatToken();
            var type = ParseTypeExpression();

            return SyntaxFactory.TypeInstruction(kind, type);
        }

        private FieldInstructionSyntax ParseFieldInstruction()
        {
            SyntaxKind kind;

            switch (CurrentToken)
            {
            case SyntaxKind.LdfldKeyword: kind = SyntaxKind.Ldfld; break;
            case SyntaxKind.LdfldaKeyword: kind = SyntaxKind.Ldflda; break;
            case SyntaxKind.StfldKeyword: kind = SyntaxKind.Stfld; break;
            default: throw new Exception();
            }

            EatToken();
            var fieldType = ParseTypeExpression();
            var containingModuleName = ParseOptionalModuleIdentifier();
            var containingTypeName = ParseIdentifierToken();
            var containingType = SyntaxFactory.StructuredType(containingModuleName, containingTypeName);
            EatToken(SyntaxKind.SlashToken);
            var fieldName = ParseIdentifierToken();

            return SyntaxFactory.FieldInstruction(kind, fieldType, containingType, fieldName);
        }

        private ArgumentInstructionSyntax ParseArgumentInstruction()
        {
            SyntaxKind kind;

            switch (CurrentToken)
            {
            case SyntaxKind.LdargKeyword: kind = SyntaxKind.Ldarg; break;
            case SyntaxKind.LdargaKeyword: kind = SyntaxKind.Ldarga; break;
            case SyntaxKind.StargKeyword: kind = SyntaxKind.Starg; break;
            default: throw new Exception();
            }

            EatToken();
            var name = ParseIdentifierToken();

            return SyntaxFactory.ArgumentInstruction(kind, name);
        }

        private LocalInstructionSyntax ParseLocalInstruction()
        {
            SyntaxKind kind;

            switch (CurrentToken)
            {
            case SyntaxKind.LdlocKeyword: kind = SyntaxKind.Ldloc; break;
            case SyntaxKind.LdlocaKeyword: kind = SyntaxKind.Ldloca; break;
            case SyntaxKind.StlocKeyword: kind = SyntaxKind.Stloc; break;
            default: throw new Exception();
            }

            EatToken();
            var name = ParseIdentifierToken();

            return SyntaxFactory.LocalInstruction(kind, name);
        }

        private BranchInstructionSyntax ParseBranchInstruction()
        {
            SyntaxKind kind;

            switch (CurrentToken)
            {
            case SyntaxKind.BeqKeyword: kind = SyntaxKind.Beq; break;
            case SyntaxKind.BgeKeyword: kind = SyntaxKind.Bge; break;
            case SyntaxKind.BgeUnKeyword: kind = SyntaxKind.Bge_Un; break;
            case SyntaxKind.BgtKeyword: kind = SyntaxKind.Bgt; break;
            case SyntaxKind.BgtUnKeyword: kind = SyntaxKind.Bgt_Un; break;
            case SyntaxKind.BleKeyword: kind = SyntaxKind.Ble; break;
            case SyntaxKind.BleUnKeyword: kind = SyntaxKind.Ble_Un; break;
            case SyntaxKind.BltKeyword: kind = SyntaxKind.Blt; break;
            case SyntaxKind.BltUnKeyword: kind = SyntaxKind.Blt_Un; break;
            case SyntaxKind.BneUnKeyword: kind = SyntaxKind.Bne_Un; break;
            case SyntaxKind.BrKeyword: kind = SyntaxKind.Br; break;
            case SyntaxKind.BrfalseKeyword: kind = SyntaxKind.Brfalse; break;
            case SyntaxKind.BrtrueKeyword: kind = SyntaxKind.Brtrue; break;
            case SyntaxKind.BeqSKeyword: kind = SyntaxKind.Beq_S; break;
            case SyntaxKind.BgeSKeyword: kind = SyntaxKind.Bge_S; break;
            case SyntaxKind.BgeUnSKeyword: kind = SyntaxKind.Bge_Un_S; break;
            case SyntaxKind.BgtSKeyword: kind = SyntaxKind.Bgt_S; break;
            case SyntaxKind.BgtUnSKeyword: kind = SyntaxKind.Bgt_Un_S; break;
            case SyntaxKind.BleSKeyword: kind = SyntaxKind.Ble_S; break;
            case SyntaxKind.BleUnSKeyword: kind = SyntaxKind.Ble_Un_S; break;
            case SyntaxKind.BltSKeyword: kind = SyntaxKind.Blt_S; break;
            case SyntaxKind.BltUnSKeyword: kind = SyntaxKind.Blt_Un_S; break;
            case SyntaxKind.BneUnSKeyword: kind = SyntaxKind.Bne_Un_S; break;
            case SyntaxKind.BrSKeyword: kind = SyntaxKind.Br_S; break;
            case SyntaxKind.BrfalseSKeyword: kind = SyntaxKind.Brfalse_S; break;
            case SyntaxKind.BrtrueSKeyword: kind = SyntaxKind.Brtrue_S; break;
            default: throw new Exception();
            }

            EatToken();
            var label = ParseIdentifierToken();

            return SyntaxFactory.BranchInstruction(kind, label);
        }
    }
}
