namespace Mango.Compiler.Diagnostics
{
    internal enum ErrorCode
    {
        None,

        ERR_UnexpectedCharacter,
        ERR_NewlineInConst,
        ERR_TooManyCharsInConst,
        ERR_EmptyCharConst,
        ERR_OpenEndedComment,

        ERR_SyntaxError,
        ERR_EOFExpected,
        ERR_IdentifierExpected,
        ERR_I32LiteralExpected,
        ERR_ModuleExpected,
        ERR_InstructionExpected,
    }
}
