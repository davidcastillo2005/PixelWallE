namespace PixelWallE.SourceCodeAnalisis.Lexical
{
    public class Lexer
    {
        private int sourceIndex = 0;
        private int currentRow = 1;
        private int currentColumn = 1;
        private List<Token> tokens = [];
        public List<Problem> Problems = [];
        private delegate bool IsIntOrId(string source, int startIndex);

        private readonly Dictionary<string, TokenType> keyword = new Dictionary<string, TokenType>
        {
        {"GoTo", TokenType.GoTo},
        {"false", TokenType.Boolean},
        {"true", TokenType.Boolean},
        };

        public Token[] Scan(string source)
        {
            bool ReadChar;
            do
            {
                if (!(ReadChar = ReadWhiteSpace(source))
                    && (ReadChar = TryGetNewLineToken(source, out Token? token)
                    || TryGetStrToken(source, out token)
                    || TryGetIntToken(source, out token)
                    || TryGetSymToken(source, out token)
                    || TryGetIdentifier(source, out token)))
                {
                    tokens.Add(token!);
                    currentColumn += token!.Value.Length;
                }
                else if (!ReadChar && sourceIndex < source.Length)
                {
                    InvalidChar(source);
                    ReadChar = true;
                    sourceIndex++;
                    currentColumn++;
                }
            } while (ReadChar);

            tokens.Add(new Token(TokenType.EOF, "$", currentRow, currentColumn));
            return [.. tokens];
        }

        private void InvalidChar(string source)
            => Problems.Add(new Warning(new Coord(currentRow, currentColumn, 1),
                                           $"Invalid character '{source[sourceIndex]}'."));

        private bool TryGetNewLineToken(string input, out Token? token)
        {
            int startIndex = sourceIndex;
            if (TryMatchPattern(input, "\n") || TryMatchPattern(input, "\r\n"))
            {
                token = new Token(TokenType.NewLine, "\n", currentRow, currentColumn);
                currentRow++;
                currentColumn = 0;
                return true;
            }
            return ResetSourceIndex(startIndex, out token);
        }

        private bool ReadWhiteSpace(string input)
        {
            var space = false;
            while (TryMatchPattern(input, " "))
            {
                currentColumn++;
                space = true;
            }
            return space;
        }

        #region Puntuactions

        private bool TryGetSymToken(string source, out Token? token)
        {
            int startIndex = sourceIndex;
            if (sourceIndex >= source.Length)
            {
                token = null;
                return false;
            }
            switch (source[sourceIndex++])
            {
                case '+':
                    return GetDefaultToken(TokenType.Plus, "+", out token);
                case '-':
                    return GetDefaultToken(TokenType.Minus, "-", out token);
                case '*':
                    if (source[sourceIndex] != '*')
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
                    if (source[sourceIndex] == '=')
                    {
                        sourceIndex++;
                        return GetDefaultToken(TokenType.LessOrEqual, "<=", out token);
                    }
                    else if (source[sourceIndex] == '-')
                    {
                        sourceIndex++;
                        return GetDefaultToken(TokenType.Assign, "<-", out token);
                    }
                    else
                    {
                        return GetDefaultToken(TokenType.Less, "<", out token);
                    }

                case '>':
                    if (source[sourceIndex] == '=')
                    {
                        sourceIndex++;
                        return GetDefaultToken(TokenType.GreaterOrEqual, ">=", out token);
                    }
                    else
                    {
                        return GetDefaultToken(TokenType.Greater, ">", out token);
                    }
                case '=' when source[sourceIndex++] == '=':
                    return GetDefaultToken(TokenType.Equal, "==", out token);
                case ',':
                    return GetDefaultToken(TokenType.Comma, ",", out token);
            }
            return ResetSourceIndex(startIndex, out token);
        }

        #endregion

        #region Interger

        private bool TryGetIntToken(string input, out Token? token)
        {
            if (TryGetIntOrIdOrKey(input, IsInterger, out string? value))
            {
                return GetDefaultToken(TokenType.Interger, value!, out token);
            }
            return ResetSourceIndex(sourceIndex, out token);
        }

        private bool IsInterger(string input, int startIndex)
            => sourceIndex < input.Length && char.IsDigit(input[sourceIndex]);

        #endregion

        #region Identifier

        private bool TryGetIdentifier(string input, out Token? token)
        {
            if (TryGetIntOrIdOrKey(input, IsIdentifier, out string? value))
            {
                var isKeyword = keyword.TryGetValue(value!, out TokenType keywordType);
                return isKeyword ? GetDefaultToken(keywordType, value!, out token) : GetDefaultToken(TokenType.Identifier, value!, out token);
            }
            return ResetSourceIndex(sourceIndex, out token);
        }

        private bool IsIdentifier(string input, int startIndex)
            => sourceIndex < input.Length && !((char.IsDigit(input[sourceIndex]) || input[sourceIndex] == '-') && startIndex == sourceIndex) && (input[sourceIndex] == '-' || char.IsLetterOrDigit(input[sourceIndex]));

        #endregion

        #region String

        private bool TryGetStrToken(string input, out Token? token)
        {
            int startIndex = sourceIndex;
            if (TryMatchPattern(input, "\"") && TryMatchPattern(input, "\""))
            {
                return GetDefaultToken(TokenType.String, "", out token);
            }
            ResetSourceIndex(startIndex, out token);
            if (TryMatchPattern(input, "\"") && TryGetStrValue(input, out string? value) && TryMatchPattern(input, "\""))
            {
                return GetDefaultToken(TokenType.String, value!, out token);
            }
            token = null;
            return ResetSourceIndex(startIndex, out token);
        }

        private bool TryGetStrValue(string input, out string? tokenValue)
        {
            int startIndex = sourceIndex;
            string temp = "";
            while (input[sourceIndex] != '\"')
            {
                temp += input[sourceIndex++];
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

        private bool TryGetIntOrIdOrKey(string input, IsIntOrId isIntOrId, out string? tokenValue)
        {
            int startIndex = sourceIndex;
            string temp = "";

            while (isIntOrId(input, startIndex))
            {
                temp += input[sourceIndex++];
            }

            if (temp != "")
            {
                tokenValue = temp;
                return true;
            }
            return ResetSourceIndex(startIndex, out tokenValue);
        }

        private bool TryMatchPattern(string input, string pattern)
        {
            int startIndex = sourceIndex;

            for (int i = 0; i < pattern.Length; i++)
            {
                if (i + startIndex > input.Length - 1 || pattern[i] != input[i + startIndex])
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