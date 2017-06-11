#region License

/* Copyright © 2017, Dahomey Technologies and Contributors
 * For conditions of distribution and use, see copyright notice in license.txt file
 */

#endregion

 using System;
using System.Globalization;
using System.Text;

namespace Dahomey.ExpressionEvaluator
{
    internal enum TokenType
    {
        None,
        Identifier,
        Number,
        String,
        False,
        True,
        OpenBracket,
        CloseBracket,
        OpenParenthesis,
        CloseParenthesis,
        Dot,
        Comma,
        Colon,
        Plus,
        Minus,
        Mult,
        Div,
        Mod,
        BitwiseAnd,
        BitwiseOr,
        BitwiseXor,
        BitwiseComplement,
        Interrogation,
        And,
        Or,
        Not,
        Lt,
        Gt,
        Eq,
        Ne,
        Le,
        Ge,
        LeftShift,
        RightShift,
    }

    internal class ExpressionLexer
    {
        private const string NUMBER_CHARS = "0123456789.eE";

        private readonly string expression;
        private int currentPos;
        private char currentChar;
        private string currentToken;
        private TokenType currentTokenType = TokenType.None;
        private readonly StringBuilder sb = new StringBuilder();

        public ExpressionLexer(string expression)
        {
            this.expression = expression;
            Advance();
            NextToken();
        }

        public bool Peek(TokenType tokenType)
        {
            return currentTokenType == tokenType;
        }

        public bool Accept(TokenType tokenType)
        {
            if (currentTokenType == tokenType)
            {
                NextToken();
                return true;
            }

            return false;
        }

        public void Expect(TokenType tokenType)
        {
            if (!Accept(tokenType))
            {
                throw BuildException("expected {0}", tokenType);
            }
        }

        public double Number()
        {
            string token = currentToken;
            Expect(TokenType.Number);
            try
            {
                return double.Parse(token, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw BuildException(ex, "Cannot parse number from '{0}'", token);
            }
        }

        public string String()
        {
            string token = currentToken;
            Expect(TokenType.String);
            return token;
        }

        public string Identifier()
        {
            string token = currentToken;
            Expect(TokenType.Identifier);
            return token;
        }

        private void NextToken()
        {
            if (IsEOF())
            {
                currentToken = null;
                currentTokenType = TokenType.None;
                return;
            }

            SkipWhiteSpace();

            sb.Length = 0;

            if (TryScanNumber())
            {
                return;
            }

            if (TryScanString())
            {
                return;
            }

            if (TryScanName())
            {
                return;
            }

            if (TryScanOperatorOrPunctuation())
            {
                return;
            }

            throw BuildException("Unexpected character {0}", currentChar);
        }

        private bool IsEOF()
        {
            return currentChar == '\0';
        }

        private bool TryScanNumber()
        {
            if (currentChar < '0' || currentChar > '9')
            {
                return false;
            }

            do
            {
                if (currentChar == '\\') // escape char
                {
                    Advance();

                    if (IsEOF())
                    {
                        throw BuildException("Invalid escape sequence");
                    }
                }

                sb.Append(currentChar);
                Advance();
            }
            while (NUMBER_CHARS.IndexOf(currentChar) != -1);

            currentToken = sb.ToString();
            currentTokenType = TokenType.Number;
            return true;
        }

        private bool TryScanString()
        {
            if (currentChar != '"')
            {
                return false;
            }

            Advance();

            do
            {
                sb.Append(currentChar);
                Advance();
            }
            while (!IsEOF() && currentChar != '"');

            if (currentChar != '"')
            {
                throw BuildException("Expected '\"'");
            }
            Advance(); // skip ending "

            currentToken = sb.ToString();
            currentTokenType = TokenType.String;
            return true;
        }

        private bool TryScanName()
        {
            if (!char.IsLetter(currentChar) && currentChar != '_')
            {
                return false;
            }

            do
            {
                sb.Append(currentChar);
                Advance();
            }
            while (char.IsLetterOrDigit(currentChar) || currentChar == '_');

            currentToken = sb.ToString();

            switch(currentToken)
            {
                case "true":
                    currentTokenType = TokenType.True;
                    break;

                case "false":
                    currentTokenType = TokenType.False;
                    break;

                default:
                    currentTokenType = TokenType.Identifier;
                    break;
            }

            return true;
        }

        private bool TryScanOperatorOrPunctuation()
        {
            switch (currentChar)
            {
                case '[':
                    Advance();
                    currentTokenType = TokenType.OpenBracket;
                    break;

                case ']':
                    Advance();
                    currentTokenType = TokenType.CloseBracket;
                    break;

                case '(':
                    Advance();
                    currentTokenType = TokenType.OpenParenthesis;
                    break;

                case ')':
                    Advance();
                    currentTokenType = TokenType.CloseParenthesis;
                    break;

                case '.':
                    Advance();
                    currentTokenType = TokenType.Dot;
                    break;

                case ',':
                    Advance();
                    currentTokenType = TokenType.Comma;
                    break;

                case ':':
                    Advance();
                    currentTokenType = TokenType.Colon;
                    break;

                case '?':
                    Advance();
                    currentTokenType = TokenType.Interrogation;
                    break;

                case '+':
                    Advance();
                    currentTokenType = TokenType.Plus;
                    break;

                case '-':
                    Advance();
                    currentTokenType = TokenType.Minus;
                    break;

                case '*':
                    Advance();
                    currentTokenType = TokenType.Mult;
                    break;

                case '/':
                    Advance();
                    currentTokenType = TokenType.Div;
                    break;

                case '%':
                    Advance();
                    currentTokenType = TokenType.Mod;
                    break;

                case '^':
                    Advance();
                    currentTokenType = TokenType.BitwiseXor;
                    break;

                case '~':
                    Advance();
                    currentTokenType = TokenType.BitwiseComplement;
                    break;

                case '&':
                    Advance();
                    if (currentChar == '&')
                    {
                        Advance();
                        currentTokenType = TokenType.And;

                    }
                    else
                    {
                        currentTokenType = TokenType.BitwiseAnd;
                    }
                    break;

                case '|':
                    Advance();
                    if (currentChar == '|')
                    {
                        Advance();
                        currentTokenType = TokenType.Or;
                    }
                    else
                    {
                        currentTokenType = TokenType.BitwiseOr;
                    }
                    break;

                case '<':
                    Advance();
                    if (currentChar == '<')
                    {
                        Advance();
                        currentTokenType = TokenType.LeftShift;
                    }
                    else if (currentChar == '=')
                    {
                        Advance();
                        currentTokenType = TokenType.Le;
                    }
                    else
                    {
                        currentTokenType = TokenType.Lt;
                    }
                    break;

                case '>':
                    Advance();
                    if (currentChar == '>')
                    {
                        Advance();
                        currentTokenType = TokenType.RightShift;
                    }
                    else if (currentChar == '=')
                    {
                        Advance();
                        currentTokenType = TokenType.Ge;
                    }
                    else
                    {
                        currentTokenType = TokenType.Gt;
                    }
                    break;

                case '!':
                    Advance();
                    if (currentChar == '=')
                    {
                        Advance();
                        currentTokenType = TokenType.Ne;
                    }
                    else
                    {
                        currentTokenType = TokenType.Not;
                    }
                    break;

                case '=':
                    Advance();
                    if (currentChar == '=')
                    {
                        Advance();
                        currentTokenType = TokenType.Eq;
                        break;
                    }
                    throw BuildException("Unexpected character");

                default:
                    return false;
            }

            currentToken = null;
            return true;
        }

        private void SkipWhiteSpace()
        {
            while (!IsEOF())
            {
                if (currentChar == ' ' || currentChar == '\t')
                {
                    Advance();
                }
                else
                {
                    break;
                }
            }
        }

        private void Advance()
        {
            if (currentPos >= expression.Length)
            {
                currentChar = '\0';
                return;
            }

            currentChar = expression[currentPos++];
        }

        public Exception BuildException(string message)
        {
            throw new SyntaxErrorException(string.Format("Syntax error({0}): {1}", currentPos, message));
        }

        public Exception BuildException(string message, params object[] args)
        {
            return BuildException(string.Format(message, args));
        }

        public Exception BuildException(Exception innerException, string message)
        {
            throw new SyntaxErrorException(string.Format("Syntax error({0}): {1}", currentPos, message), innerException);
        }

        public Exception BuildException(Exception innerException, string message, params object[] args)
        {
            return BuildException(innerException, string.Format(message, args));
        }
    }
}
