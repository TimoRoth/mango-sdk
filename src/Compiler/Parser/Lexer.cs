using System;
using System.Runtime.InteropServices;
using Mango.Compiler.Diagnostics;
using Mango.Compiler.Syntax;

namespace Mango.Compiler.Parser
{
    partial class SimpleParser
    {
        private readonly char[] _text;

        public SimpleParser(string text)
        {
            _text = new char[text.Length + 1];
            text.CopyTo(0, _text, 0, text.Length);
        }

        #region Diagnostics

        internal void AddError(ErrorCode errorCode)
        {
            var err = string.Format("[{0}..{1}) {2}:{3} {4}", _tokenInfo.Start, _tokenInfo.Length, _tokenInfo.Line + 1, _tokenInfo.Column + 1, errorCode);
            throw new Exception(err);
        }

        #endregion

        #region Lexer

        internal TokenInfo _tokenInfo;

        private int _column;
        private int _line;
        private int _pos;

        public void Lex()
        {
            ScanSyntaxTrivia(isTrailing: false);

            var tokenInfo = default(TokenInfo);
            tokenInfo.Start = _pos;
            tokenInfo.Line = _line;
            tokenInfo.Column = _column;
            ScanSyntaxToken(ref tokenInfo);
            tokenInfo.Length = _pos - tokenInfo.Start;
            _tokenInfo = tokenInfo;

            ScanSyntaxTrivia(isTrailing: true);
        }

        private bool ScanIdentifier(ref TokenInfo info)
        {
            var start = _pos;

            for (; ; )
            {
                switch (_text[_pos])
                {
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                case '.':
                case '_':
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                    _pos++;
                    _column++;
                    break;

                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    if (_pos == start)
                    {
                        return false;
                    }
                    goto case 'A';

                case '$':
                case '%':
                case '@':
                    if (_pos != start)
                    {
                        return false;
                    }
                    goto case 'A';

                default:
                    if (_pos == start)
                    {
                        return false;
                    }
                    else
                    {
                        info.Identifier = new string(_text, start, _pos - start);
                        return true;
                    }
                }
            }
        }

        private bool ScanIdentifierOrKeyword(ref TokenInfo info)
        {
            if (ScanIdentifier(ref info))
            {
                switch (info.Identifier)
                {
                case "bool": info.Kind = SyntaxKind.BoolKeyword; break;
                case "i8": info.Kind = SyntaxKind.I8Keyword; break;
                case "i16": info.Kind = SyntaxKind.I16Keyword; break;
                case "i32": info.Kind = SyntaxKind.I32Keyword; break;
                case "i64": info.Kind = SyntaxKind.I64Keyword; break;
                case "u8": info.Kind = SyntaxKind.U8Keyword; break;
                case "u16": info.Kind = SyntaxKind.U16Keyword; break;
                case "u32": info.Kind = SyntaxKind.U32Keyword; break;
                case "u64": info.Kind = SyntaxKind.U64Keyword; break;
                case "f32": info.Kind = SyntaxKind.F32Keyword; break;
                case "f64": info.Kind = SyntaxKind.F64Keyword; break;
                case "void": info.Kind = SyntaxKind.VoidKeyword; break;

                case "module": info.Kind = SyntaxKind.ModuleKeyword; break;
                case "import": info.Kind = SyntaxKind.ImportKeyword; break;
                case "type": info.Kind = SyntaxKind.TypeKeyword; break;
                case "field": info.Kind = SyntaxKind.FieldKeyword; break;
                case "declare": info.Kind = SyntaxKind.DeclareKeyword; break;
                case "define": info.Kind = SyntaxKind.DefineKeyword; break;
                case "local": info.Kind = SyntaxKind.LocalKeyword; break;
                case "global": info.Kind = SyntaxKind.GlobalKeyword; break;

                case "add": info.Kind = SyntaxKind.AddKeyword; break;
                case "and": info.Kind = SyntaxKind.AndKeyword; break;
                case "beq": info.Kind = SyntaxKind.BeqKeyword; break;
                case "beq.s": info.Kind = SyntaxKind.BeqSKeyword; break;
                case "bge": info.Kind = SyntaxKind.BgeKeyword; break;
                case "bge.s": info.Kind = SyntaxKind.BgeSKeyword; break;
                case "bge.un": info.Kind = SyntaxKind.BgeUnKeyword; break;
                case "bge.un.s": info.Kind = SyntaxKind.BgeUnSKeyword; break;
                case "bgt": info.Kind = SyntaxKind.BgtKeyword; break;
                case "bgt.s": info.Kind = SyntaxKind.BgtSKeyword; break;
                case "bgt.un": info.Kind = SyntaxKind.BgtUnKeyword; break;
                case "bgt.un.s": info.Kind = SyntaxKind.BgtUnSKeyword; break;
                case "ble": info.Kind = SyntaxKind.BleKeyword; break;
                case "ble.s": info.Kind = SyntaxKind.BleSKeyword; break;
                case "ble.un": info.Kind = SyntaxKind.BleUnKeyword; break;
                case "ble.un.s": info.Kind = SyntaxKind.BleUnSKeyword; break;
                case "blt": info.Kind = SyntaxKind.BltKeyword; break;
                case "blt.s": info.Kind = SyntaxKind.BltSKeyword; break;
                case "blt.un": info.Kind = SyntaxKind.BltUnKeyword; break;
                case "blt.un.s": info.Kind = SyntaxKind.BltUnSKeyword; break;
                case "bne.un": info.Kind = SyntaxKind.BneUnKeyword; break;
                case "bne.un.s": info.Kind = SyntaxKind.BneUnSKeyword; break;
                case "break": info.Kind = SyntaxKind.BreakKeyword; break;
                case "brfalse": info.Kind = SyntaxKind.BrfalseKeyword; break;
                case "brfalse.s": info.Kind = SyntaxKind.BrfalseSKeyword; break;
                case "br": info.Kind = SyntaxKind.BrKeyword; break;
                case "br.s": info.Kind = SyntaxKind.BrSKeyword; break;
                case "brtrue": info.Kind = SyntaxKind.BrtrueKeyword; break;
                case "brtrue.s": info.Kind = SyntaxKind.BrtrueSKeyword; break;
                case "calli": info.Kind = SyntaxKind.CalliKeyword; break;
                case "call": info.Kind = SyntaxKind.CallKeyword; break;
                case "ceq": info.Kind = SyntaxKind.CeqKeyword; break;
                case "cgt": info.Kind = SyntaxKind.CgtKeyword; break;
                case "cgt.un": info.Kind = SyntaxKind.CgtUnKeyword; break;
                case "clt": info.Kind = SyntaxKind.CltKeyword; break;
                case "clt.un": info.Kind = SyntaxKind.CltUnKeyword; break;
                case "conv": info.Kind = SyntaxKind.ConvKeyword; break;
                case "conv.un": info.Kind = SyntaxKind.ConvUnKeyword; break;
                case "cpobj": info.Kind = SyntaxKind.CpobjKeyword; break;
                case "div": info.Kind = SyntaxKind.DivKeyword; break;
                case "div.un": info.Kind = SyntaxKind.DivUnKeyword; break;
                case "dup": info.Kind = SyntaxKind.DupKeyword; break;
                case "initobj": info.Kind = SyntaxKind.InitobjKeyword; break;
                case "ldarga": info.Kind = SyntaxKind.LdargaKeyword; break;
                case "ldarg": info.Kind = SyntaxKind.LdargKeyword; break;
                case "ldc": info.Kind = SyntaxKind.LdcKeyword; break;
                case "ldelema": info.Kind = SyntaxKind.LdelemaKeyword; break;
                case "ldelem": info.Kind = SyntaxKind.LdelemKeyword; break;
                case "ldflda": info.Kind = SyntaxKind.LdfldaKeyword; break;
                case "ldfld": info.Kind = SyntaxKind.LdfldKeyword; break;
                case "ldftn": info.Kind = SyntaxKind.LdftnKeyword; break;
                case "ldind": info.Kind = SyntaxKind.LdindKeyword; break;
                case "ldlen": info.Kind = SyntaxKind.LdlenKeyword; break;
                case "ldloca": info.Kind = SyntaxKind.LdlocaKeyword; break;
                case "ldloc": info.Kind = SyntaxKind.LdlocKeyword; break;
                case "ldnull": info.Kind = SyntaxKind.LdnullKeyword; break;
                case "ldobj": info.Kind = SyntaxKind.LdobjKeyword; break;
                case "mul": info.Kind = SyntaxKind.MulKeyword; break;
                case "neg": info.Kind = SyntaxKind.NegKeyword; break;
                case "newarr": info.Kind = SyntaxKind.NewarrKeyword; break;
                case "newobj": info.Kind = SyntaxKind.NewobjKeyword; break;
                case "nop": info.Kind = SyntaxKind.NopKeyword; break;
                case "not": info.Kind = SyntaxKind.NotKeyword; break;
                case "or": info.Kind = SyntaxKind.OrKeyword; break;
                case "pop": info.Kind = SyntaxKind.PopKeyword; break;
                case "rem": info.Kind = SyntaxKind.RemKeyword; break;
                case "rem.un": info.Kind = SyntaxKind.RemUnKeyword; break;
                case "ret": info.Kind = SyntaxKind.RetKeyword; break;
                case "shl": info.Kind = SyntaxKind.ShlKeyword; break;
                case "shr": info.Kind = SyntaxKind.ShrKeyword; break;
                case "shr.un": info.Kind = SyntaxKind.ShrUnKeyword; break;
                case "starg": info.Kind = SyntaxKind.StargKeyword; break;
                case "stelem": info.Kind = SyntaxKind.StelemKeyword; break;
                case "stfld": info.Kind = SyntaxKind.StfldKeyword; break;
                case "stind": info.Kind = SyntaxKind.StindKeyword; break;
                case "stloc": info.Kind = SyntaxKind.StlocKeyword; break;
                case "stobj": info.Kind = SyntaxKind.StobjKeyword; break;
                case "sub": info.Kind = SyntaxKind.SubKeyword; break;
                case "syscall": info.Kind = SyntaxKind.SyscallKeyword; break;
                case "xor": info.Kind = SyntaxKind.XorKeyword; break;

                default: info.Kind = SyntaxKind.IdentifierToken; break;
                }

                return true;
            }
            else
            {
                info.Kind = SyntaxKind.BadToken;
                return false;
            }
        }

        private bool ScanNumericLiteral(ref TokenInfo info)
        {
            var start = _pos;

            for (; ; )
            {
                var ch = _text[_pos];

                if (ch >= '0' && ch <= '9')
                {
                    _pos++;
                    _column++;
                }
                else
                {
                    break;
                }
            }

            if (_pos == start)
            {
                return false;
            }
            else
            {
                info.Kind = SyntaxKind.I32LiteralToken;
                info.I32Value = int.Parse(new string(_text, start, _pos - start));
                return true;
            }
        }

        private void ScanStringLiteral(ref TokenInfo info)
        {
            var start = _pos;
            var quote = _text[start];

            if (quote == '"' || quote == '\'')
            {
                _pos++;
                _column++;

                for (; ; )
                {
                    var ch = _text[_pos];
                    if (ch == quote)
                    {
                        _pos++;
                        _column++;
                        break;
                    }
                    else if (ch == '\r' || ch == '\n' || ch == -1)
                    {
                        AddError(ErrorCode.ERR_NewlineInConst);
                        break;
                    }
                    else
                    {
                        _pos++;
                        _column++;
                    }
                }

                if (quote == '\'')
                {
                    info.Kind = SyntaxKind.CharacterLiteralToken;

                    if ((_pos - 1) - (start + 1) != 1)
                    {
                        AddError(_pos - start - 2 != 0 ? ErrorCode.ERR_TooManyCharsInConst : ErrorCode.ERR_EmptyCharConst);
                    }

                    if ((_pos - 1) - (start + 1) != 0)
                    {
                        info.CharValue = _text[start + 1];
                    }
                }
                else
                {
                    info.Kind = SyntaxKind.StringLiteralToken;

                    if ((_pos - 1) - (start + 1) != 0)
                    {
                        info.StringValue = new string(_text, start + 1, _pos - 2);
                    }
                    else
                    {
                        info.StringValue = string.Empty;
                    }
                }
            }
            else
            {
                info.Kind = SyntaxKind.BadToken;
            }
        }

        private void ScanSyntaxToken(ref TokenInfo info)
        {
            switch (_text[_pos])
            {
            case 'a':
            case 'b':
            case 'c':
            case 'd':
            case 'e':
            case 'f':
            case 'g':
            case 'h':
            case 'i':
            case 'j':
            case 'k':
            case 'l':
            case 'm':
            case 'n':
            case 'o':
            case 'p':
            case 'q':
            case 'r':
            case 's':
            case 't':
            case 'u':
            case 'v':
            case 'w':
            case 'x':
            case 'y':
            case 'z':
            case 'A':
            case 'B':
            case 'C':
            case 'D':
            case 'E':
            case 'F':
            case 'G':
            case 'H':
            case 'I':
            case 'J':
            case 'K':
            case 'L':
            case 'M':
            case 'N':
            case 'O':
            case 'P':
            case 'Q':
            case 'R':
            case 'S':
            case 'T':
            case 'U':
            case 'V':
            case 'W':
            case 'X':
            case 'Y':
            case 'Z':
            case '$':
            case '%':
            case '.':
            case '@':
            case '_':
                ScanIdentifierOrKeyword(ref info);
                break;

            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
                ScanNumericLiteral(ref info);
                break;

            case '"':
            case '\'':
                ScanStringLiteral(ref info);
                break;

            case '!':
            case '#':
            case '&':
            case '(':
            case ')':
            case '*':
            case '+':
            case ',':
            case '-':
            case '/':
                info.Kind = SyntaxKind.ExclamationToken + (_text[_pos] - '!');
                _pos++;
                _column++;
                break;

            case ':':
            case ';':
            case '<':
            case '=':
            case '>':
            case '?':
                info.Kind = SyntaxKind.ColonToken + (_text[_pos] - ':');
                _pos++;
                _column++;
                break;

            case '[':
            case '\\':
            case ']':
            case '^':
            case '`':
                info.Kind = SyntaxKind.OpenBracketToken + (_text[_pos] - '[');
                _pos++;
                _column++;
                break;

            case '{':
            case '|':
            case '}':
            case '~':
                info.Kind = SyntaxKind.OpenBraceToken + (_text[_pos] - '{');
                _pos++;
                _column++;
                break;

            case '\0' when _pos + 1 == _text.Length:
                info.Kind = SyntaxKind.EndOfFileToken;
                break;

            default:
                _pos++;
                _column++;
                info.Kind = SyntaxKind.BadToken;
                AddError(ErrorCode.ERR_UnexpectedCharacter);
                break;
            }
        }

        private void ScanSyntaxTrivia(bool isTrailing)
        {
            for (; ; )
            {
                var ch = _text[_pos];
                if (ch == ' ')
                {
                    _pos++;
                    _column++;
                    for (; ; )
                    {
                        ch = _text[_pos];
                        if (ch != ' ')
                        {
                            break;
                        }
                        else
                        {
                            _pos++;
                            _column++;
                        }
                    }
                }
                else if (ch < ' ')
                {
                    if (ch == '\n')
                    {
                        _pos++;
                        _line++;
                        _column = 0;
                        if (isTrailing)
                        {
                            break;
                        }
                    }
                    else if (ch == '\r')
                    {
                        _pos++;
                        if (_text[_pos] == '\n')
                        {
                            _pos++;
                        }
                        _line++;
                        _column = 0;
                        if (isTrailing)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else if (ch == '/')
                {
                    if (_text[_pos + 1] == '/')
                    {
                        _pos += 2;
                        _column += 2;
                        for (; ; )
                        {
                            ch = _text[_pos];
                            if (ch == '\n' || ch == '\r' || ch == '\0' && _pos + 1 == _text.Length)
                            {
                                break;
                            }
                            else
                            {
                                _pos++;
                                _column++;
                            }
                        }
                    }
                    else if (_text[_pos + 1] == '*')
                    {
                        _pos += 2;
                        _column += 2;
                        for (; ; )
                        {
                            ch = _text[_pos];
                            if (ch == '\0' && _pos + 1 == _text.Length)
                            {
                                AddError(ErrorCode.ERR_OpenEndedComment);
                                break;
                            }
                            else if (ch == '*' && _text[_pos + 1] == '/')
                            {
                                _pos += 2;
                                _column += 2;
                                break;
                            }
                            else if (ch == '\n')
                            {
                                _pos++;
                                _line++;
                                _column = 0;
                            }
                            else if (ch == '\r')
                            {
                                _pos++;
                                if (ch == '\n')
                                {
                                    _pos++;
                                }
                                _line++;
                                _column = 0;
                            }
                            else
                            {
                                _pos++;
                                _column++;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        [StructLayout(LayoutKind.Auto)]
        internal struct TokenInfo
        {
            internal SyntaxKind Kind;
            internal int Start;
            internal int Length;
            internal int Column;
            internal int Line;
            internal string Identifier;
            internal string StringValue;
            internal char CharValue;
            internal int I32Value;
            internal long I64Value;
            internal float F32Value;
            internal double F64Value;
        }

        #endregion
    }
}
