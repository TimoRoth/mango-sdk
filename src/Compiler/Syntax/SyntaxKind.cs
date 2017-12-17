namespace Mango.Compiler.Syntax
{
    public enum SyntaxKind
    {
        None,

        #region ...

        ExclamationToken,
        DoubleQuoteToken,
        HashToken,
        DollarToken,
        PercentToken,
        AmpersandToken,
        SingleQuoteToken,
        OpenParenToken,
        CloseParenToken,
        AsteriskToken,
        PlusToken,
        CommaToken,
        MinusToken,
        DotToken,
        SlashToken,
        ColonToken,
        SemicolonToken,
        LessThanToken,
        EqualsToken,
        GreaterThanToken,
        QuestionToken,
        AtToken,
        OpenBracketToken,
        BackslashToken,
        CloseBracketToken,
        CaretToken,
        UnderscoreToken,
        BacktickToken,
        OpenBraceToken,
        BarToken,
        CloseBraceToken,
        TildeToken,

        BoolKeyword,
        I8Keyword,
        U8Keyword,
        I16Keyword,
        U16Keyword,
        I32Keyword,
        U32Keyword,
        I64Keyword,
        U64Keyword,
        F32Keyword,
        F64Keyword,
        VoidKeyword,

        ModuleKeyword,
        TypeKeyword,
        FieldKeyword,
        DeclareKeyword,
        DefineKeyword,
        LocalKeyword,
        GlobalKeyword,

        LdnullKeyword,
        LdcKeyword,
        LdftnKeyword,
        LdindKeyword,
        LdfldKeyword,
        LdelemKeyword,
        LdlocKeyword,
        LdargKeyword,
        LdfldaKeyword,
        LdelemaKeyword,
        LdlocaKeyword,
        LdargaKeyword,
        StindKeyword,
        StfldKeyword,
        StelemKeyword,
        StlocKeyword,
        StargKeyword,
        SyscallKeyword,
        CallKeyword,
        CalliKeyword,
        RetKeyword,

        BreakKeyword,
        DupKeyword,
        NopKeyword,
        PopKeyword,

        AddKeyword,
        AndKeyword,
        DivKeyword,
        DivUnKeyword,
        MulKeyword,
        OrKeyword,
        RemKeyword,
        RemUnKeyword,
        ShlKeyword,
        ShrKeyword,
        ShrUnKeyword,
        SubKeyword,
        XorKeyword,

        NegKeyword,
        NotKeyword,

        ConvKeyword,
        ConvUnKeyword,

        CeqKeyword,
        CgtKeyword,
        CgtUnKeyword,
        CltKeyword,
        CltUnKeyword,

        BeqKeyword,
        BgeKeyword,
        BgeUnKeyword,
        BgtKeyword,
        BgtUnKeyword,
        BleKeyword,
        BleUnKeyword,
        BltKeyword,
        BltUnKeyword,
        BneUnKeyword,
        BrKeyword,
        BrfalseKeyword,
        BrtrueKeyword,

        BeqSKeyword,
        BgeSKeyword,
        BgeUnSKeyword,
        BgtSKeyword,
        BgtUnSKeyword,
        BleSKeyword,
        BleUnSKeyword,
        BltSKeyword,
        BltUnSKeyword,
        BneUnSKeyword,
        BrSKeyword,
        BrfalseSKeyword,
        BrtrueSKeyword,

        LdobjKeyword,
        StobjKeyword,
        CpobjKeyword,
        InitobjKeyword,
        LdlenKeyword,
        NewarrKeyword,
        NewobjKeyword,



        EndOfFileToken,

        BadToken,

        IdentifierToken,
        StringLiteralToken,
        CharacterLiteralToken,
        I32LiteralToken,







        #endregion




        #region Type Expressions

        ArrayType,
        BoolType,
        Float32Type,
        Float64Type,
        FunctionType,
        Int16Type,
        Int32Type,
        Int64Type,
        Int8Type,
        StructuredType,
        ReferenceType,
        SpanType,
        UInt16Type,
        UInt32Type,
        UInt64Type,
        UInt8Type,
        VoidType,

        #endregion

        #region Declarations

        CompilationUnit,
        ModuleDeclaration,
        TypeDeclaration,
        FieldDeclaration,
        FunctionDeclaration,
        ParameterDeclaration,
        FunctionBody,
        LocalDeclaration,

        #endregion

        #region Instructions

        LabeledInstruction,

        Ldnull,
        Ldc,
        Ldftn,
        Ldind,
        Ldfld,
        Ldelem,
        Ldloc,
        Ldarg,
        Ldflda,
        Ldelema,
        Ldloca,
        Ldarga,
        Stind,
        Stfld,
        Stelem,
        Stloc,
        Starg,
        Syscall,
        Call,
        Calli,
        Ret,

        Break,
        Dup,
        Nop,
        Pop,

        Add,
        And,
        Div,
        Div_Un,
        Mul,
        Or,
        Rem,
        Rem_Un,
        Shl,
        Shr,
        Shr_Un,
        Sub,
        Xor,

        Neg,
        Not,

        Conv,
        Conv_Un,

        Ceq,
        Cgt,
        Cgt_Un,
        Clt,
        Clt_Un,

        Beq,
        Bge,
        Bge_Un,
        Bgt,
        Bgt_Un,
        Ble,
        Ble_Un,
        Blt,
        Blt_Un,
        Bne_Un,
        Br,
        Brfalse,
        Brtrue,

        Beq_S,
        Bge_S,
        Bge_Un_S,
        Bgt_S,
        Bgt_Un_S,
        Ble_S,
        Ble_Un_S,
        Blt_S,
        Blt_Un_S,
        Bne_Un_S,
        Br_S,
        Brfalse_S,
        Brtrue_S,

        Ldobj,
        Stobj,
        Cpobj,
        Initobj,
        Ldlen,
        Newarr,
        Newobj,

        #endregion
    }
}
