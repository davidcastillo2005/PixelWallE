namespace PixelWallE.SourceCodeAnalisis.Lexical
{
    public class Lexer(string sourceCode)
    {
        private int sourceIndex = 0;
        private int currentRow = 1;
        private int currentColumn = 1;
        private readonly List<Token> tokens = [];
        public List<Problem> Problems = [];
        private delegate bool IsIntOrId(int startIndex);
        private readonly string sourceCode = sourceCode;
        private readonly Dictionary<string, TokenType> keyword = new Dictionary<string, TokenType>
        {
            {"Goto", TokenType.GoTo},
            {"goto", TokenType.GoTo},
            {"GoTo", TokenType.GoTo},
            {"false", TokenType.Boolean},
            {"true", TokenType.Boolean},
        };

        public Token[] Scan()
        {
            bool ReadChar;
            do
            {
                if (!(ReadChar = ReadWhiteSpace())
                    && (ReadChar = TryGetNewLineToken(out Token? token)
                    || TryGetStrToken(out token)
                    || TryGetIntToken(out token)
                    || TryGetSymToken(out token)
                    || TryGetIdentifier(out token)))
                {
                    tokens.Add(token!);
                    currentColumn += token!.Value.Length;
                }
                else if (!ReadChar && sourceIndex < sourceCode.Length)
                {
                    InvalidChar();
                    ReadChar = true;
                    sourceIndex++;
                    currentColumn++;
                }
            } while (ReadChar);

            tokens.Add(new Token(TokenType.EOF, "$", currentRow, currentColumn));
            return [.. tokens];
        }

        private void InvalidChar()
            => Problems.Add(new Warning(new Coord(currentRow, currentColumn, 1),
                                           $"Invalid character '{sourceCode[sourceIndex]}'."));

        private bool TryGetNewLineToken(out Token? token)
        {
            int startIndex = sourceIndex;
            if (TryMatchPattern("\n") || TryMatchPattern("\r\n"))
            {
                token = new Token(TokenType.NewLine, "\n", currentRow, currentColumn);
                currentRow++;
                currentColumn = 0;
                return true;
            }
            return ResetSourceIndex(startIndex, out token);
        }

        private bool ReadWhiteSpace()
        {
            var space = false;
            while (TryMatchPattern(" "))
            {
                currentColumn++;
                space = true;
            }
            return space;
        }

        #region Puntuactions

        private bool TryGetSymToken(out Token? token)
        {
            int startIndex = sourceIndex;
            if (sourceIndex >= sourceCode.Length)
            {
                token = null;
                return false;
            }
            switch (sourceCode[sourceIndex++])
            {
                case '+':
                    return GetDefaultToken(TokenType.Plus, "+", out token);
                case '-':
                    return GetDefaultToken(TokenType.Minus, "-", out token);
                case '*':
                    if (sourceCode[sourceIndex] != '*')
                    {
                        return GetDefaultToken(TokenType.Dot, "*", out token);
                    }
                    else
                    {
                        sourceIndex++;
                        return GetDefaultToken(TokenType.Exponentiation, "**", out token);
                    }

                case '/':
                    return GetDefaultToken(TokenType.Division, "/", out token);
                case '%':
                    return GetDefaultToken(TokenType.Modulus, "%", out token);
                case '(':
                    return GetDefaultToken(TokenType.LeftCurly, "(", out token);
                case ')':
                    return GetDefaultToken(TokenType.RightCurly, ")", out token);
                case '[':
                    return GetDefaultToken(TokenType.LeftBracket, "[", out token);
                case ']':
                    return GetDefaultToken(TokenType.RightBracket, "]", out token);
                case '<':
                    if (sourceCode[sourceIndex] == '=')
                    {
                        sourceIndex++;
                        return GetDefaultToken(TokenType.LessOrEqual, "<=", out token);
                    }
                    else if (sourceCode[sourceIndex] == '-')
                    {
                        sourceIndex++;
                        return GetDefaultToken(TokenType.Assign, "<-", out token);
                    }
                    else
                    {
                        return GetDefaultToken(TokenType.Less, "<", out token);
                    }

                case '>':
                    if (sourceCode[sourceIndex] == '=')
                    {
                        sourceIndex++;
                        return GetDefaultToken(TokenType.GreaterOrEqual, ">=", out token);
                    }
                    else
                    {
                        return GetDefaultToken(TokenType.Greater, ">", out token);
                    }
                case '=' when sourceCode[sourceIndex++] == '=':
                    return GetDefaultToken(TokenType.Equal, "==", out token);
                case ',':
                    return GetDefaultToken(TokenType.Comma, ",", out token);
            }
            return ResetSourceIndex(startIndex, out token);
        }

        #endregion

        #region Interger

        private bool TryGetIntToken(out Token? token)
        {
            if (TryGetIntOrIdOrKey(IsInterger, out string? value))
            {
                return GetDefaultToken(TokenType.Interger, value!, out token);
            }
            return ResetSourceIndex(sourceIndex, out token);
        }

        private bool IsInterger(int startIndex)
            => sourceIndex < sourceCode.Length && char.IsDigit(sourceCode[sourceIndex]);

        #endregion

        #region Identifier

        private bool TryGetIdentifier(out Token? token)
        {
            if (TryGetIntOrIdOrKey(IsIdentifier, out string? value))
            {
                var isKeyword = keyword.TryGetValue(value!, out TokenType keywordType);
                return isKeyword ? GetDefaultToken(keywordType, value!, out token) : GetDefaultToken(TokenType.Identifier, value!, out token);
            }
            return ResetSourceIndex(sourceIndex, out token);
        }

        private bool IsIdentifier(int startIndex)
            => sourceIndex < sourceCode.Length
                && !((char.IsDigit(sourceCode[sourceIndex])
                    || sourceCode[sourceIndex] == '-'
                    || sourceCode[sourceIndex] == '_') && startIndex == sourceIndex)
                && (sourceCode[sourceIndex] == '-'
                    || sourceCode[sourceIndex] == '_'
                    || char.IsLetterOrDigit(sourceCode[sourceIndex]));

        #endregion

        #region String

        private bool TryGetStrToken(out Token? token)
        {
            int startIndex = sourceIndex;
            if (TryMatchPattern("\"") && TryMatchPattern("\""))
            {
                return GetDefaultToken(TokenType.String, "", out token);
            }
            ResetSourceIndex(startIndex, out token);
            if (TryMatchPattern("\"") && TryGetStrValue(out string? value) && TryMatchPattern("\""))
            {
                return GetDefaultToken(TokenType.String, value!, out token);
            }
            token = null;
            return ResetSourceIndex(startIndex, out token);
        }

        private bool TryGetStrValue(out string? tokenValue)
        {
            int startIndex = sourceIndex;
            string temp = "";
            while (sourceCode[sourceIndex] != '\"')
            {
                temp += sourceCode[sourceIndex++];
            }

            if (temp != "")
            {
                tokenValue = temp;
                return true;
            }
            return ResetSourceIndex(startIndex, out tokenValue);
        }

        #endregion

        #region Tools

        private bool TryGetIntOrIdOrKey(IsIntOrId isIntOrId, out string? tokenValue)
        {
            int startIndex = sourceIndex;
            string temp = "";

            while (isIntOrId(startIndex))
            {
                temp += sourceCode[sourceIndex++];
            }

            if (temp != "")
            {
                tokenValue = temp;
                return true;
            }
            return ResetSourceIndex(startIndex, out tokenValue);
        }

        private bool TryMatchPattern(string pattern)
        {
            int startIndex = sourceIndex;

            for (int i = 0; i < pattern.Length; i++)
            {
                if (i + startIndex > sourceCode.Length - 1 || pattern[i] != sourceCode[i + startIndex])
                {
                    sourceIndex = startIndex;
                    return false;
                }
                sourceIndex++;
            }
            return true;
        }

        private bool ResetSourceIndex<T>(int startIndex, out T? token)
        {
            sourceIndex = startIndex;
            token = default;
            return false;
        }

        private bool GetDefaultToken<T>(T value, out T? output)
        {
            output = value;
            return true;
        }

        private bool GetDefaultToken(TokenType tokentype, string value, out Token output)
        {
            output = new Token(tokentype, value, currentRow, currentColumn);
            return true;
        }

        #endregion
    }
}