using Mango.Compiler.Diagnostics;
using Mango.Compiler.Syntax;

namespace Mango.Compiler.Parser
{
    partial class SimpleParser
    {
        #region Parser

        internal SyntaxKind CurrentToken
        {
            get
            {
                if (_tokenInfo.Kind == SyntaxKind.None)
                {
                    Lex();
                }

                return _tokenInfo.Kind;
            }
        }

        internal SyntaxKind EatToken()
        {
            var ct = CurrentToken;
            MoveToNextToken();
            return ct;
        }

        internal SyntaxKind EatToken(SyntaxKind kind)
        {
            var ct = CurrentToken;

            if (ct == kind)
            {
                MoveToNextToken();
                return ct;
            }

            return CreateMissingToken(kind, ct, reportError: true);
        }

        private SyntaxKind CreateMissingToken(SyntaxKind expected, SyntaxKind actual, bool reportError)
        {
            var token = actual;

            if (reportError)
            {
                AddError(ErrorCode.ERR_SyntaxError);
            }

            return token;
        }

        private void MoveToNextToken()
        {
            _tokenInfo.Kind = SyntaxKind.None;
        }

        #endregion
    }
}
