using PixelWallE.Global;
using PixelWallE.Interfaces;
using PixelWallE.SourceCodeAnalisis.Syntactic.Enums;
using PixelWallE.SourceCodeAnalisis.Syntactic.Extensions;
using PixelWallE.Global.AST;

namespace PixelWallE.SourceCodeAnalisis.Syntactic;

public enum OperatorState { Shift, Reduce }

public class Parser
{
    private int tokenIndex;
    private readonly Dictionary<TokenType, OperatorState> ShiftOrReduceOperators = new()
    {
        { TokenType.Plus, OperatorState.Shift }, { TokenType.Minus, OperatorState.Reduce },
        { TokenType.Dot, OperatorState.Shift }, { TokenType.Division, OperatorState.Reduce },
        { TokenType.Modulus, OperatorState.Reduce }, { TokenType.Exponentiation, OperatorState.Shift },
        { TokenType.Equal, OperatorState.Reduce }, { TokenType.NotEqual, OperatorState.Reduce },
        { TokenType.Greater, OperatorState.Reduce }, { TokenType.GreaterOrEqual, OperatorState.Reduce },
        { TokenType.Less, OperatorState.Reduce }, { TokenType.LessOrEqual, OperatorState.Reduce },
    };

    delegate bool TryGetFunc(Token[] tokens, out IExpression? expre);
    public List<IStatement> Lines { get; private set; } = [];

    public IStatement Parse(Token[] tokens) => TryGetCodeBlock(tokens, out IStatement? expre) ? expre! : throw new Exception();

    private bool TryGetCodeBlock(Token[] tokens, out IStatement? expre)
    {
        bool ReadLine;
        do
        {
            if (ReadLine = TryGetAssignStatement(tokens, out IStatement? line)
                || TryGetGoToStatement(tokens, out line)
                || TryGetAction(tokens, out line)
                || TryGetLabelStatement(Lines.Count, tokens, out line))
            {
                Lines.Add(line!);
                continue;
            }
            ReadLine = TryMatchToken(tokens, TokenType.NewLine);
        } while (ReadLine);

        CodeBlock node = new([.. Lines]);
        return GetDefaultExpre(node, out expre);
    }

    private bool TryGetGoToStatement(Token[] tokens, out IStatement? lineExpre)
    {
        int startIndex = tokenIndex;
        if (TryMatchAllTokens(tokens,
            [
                TokenType.GoTo,
                TokenType.LeftBracket,
                TokenType.Identifier,
                TokenType.RightBracket
            ]))
        {
            var valueIndex = tokenIndex - 2;
            string targetLabel = tokens[valueIndex].Value;
            IStatement @goto;
            if (TryMatchToken(tokens, TokenType.LeftCurly)
                && TryParseBooleanExpre(tokens, out IExpression cond)
                && TryMatchToken(tokens, TokenType.RightCurly))
            {
                @goto = new GoToStatement(targetLabel, cond, GetCoord(tokens, startIndex));
            }
            else
            {
                @goto = new GoToStatement(targetLabel, null, GetCoord(tokens, startIndex));
            }
            return GetDefaultExpre(@goto, out lineExpre);
        }
        return ResetTokenIndex(startIndex, out lineExpre);
    }

    private Coord GetCoord(Token[] tokens, int startIndex)
        => new(tokens[startIndex].Coords.Row, tokens[startIndex].Coords.Col,
               tokens[tokenIndex].Coords.Col - tokens[startIndex].Coords.Col);

    private bool TryGetLabelStatement(int lineIndex, Token[] tokens, out IStatement? lineExpre)
    {
        int startIndex = tokenIndex;
        string value = tokens[tokenIndex].Value;
        if (!TryMatchAllTokens(tokens, [TokenType.Identifier, TokenType.NewLine]))
            return ResetTokenIndex(startIndex, out lineExpre);
        IStatement label = new LabelStatement(value, GetCoord(tokens, startIndex));
        return GetDefaultExpre(label, out lineExpre);
    }

    private bool TryGetAssignStatement(Token[] tokens, out IStatement? expre)
    {
        int startIndex = tokenIndex;
        if (!(TryMatchAllTokens(tokens, [TokenType.Identifier, TokenType.Assign])
            && TryParseExpre(tokens, out IExpression? value)))
            return ResetTokenIndex(startIndex, out expre);
        var assign = new AssignStatement(tokens[startIndex].Value, value!, GetCoord(tokens, startIndex));
        return GetDefaultExpre(assign, out expre);
    }

    private bool TryParseExpre(Token[] tokens, out IExpression? expre)
    {
        int startIndex = tokenIndex;
        string name = tokens[tokenIndex].Value;
        TryGetFunc[] list = [TryParseBooleanExpre, TryParseArithExpre, TryParseStringExpre];

        foreach (var func in list)
        {
            if (func(tokens, out IExpression? value) && TryMatchToken(tokens, TokenType.NewLine))
                return GetDefaultExpre(value!, out expre);
            tokenIndex = startIndex;
        }

        return ResetTokenIndex(startIndex, out expre);
    }

    #region Methods

    private bool TryParseMethod(Token[] tokens, out IExpression[]? parameters)
    {
        int startIndex = tokenIndex;
        if (TryMatchAllTokens(tokens, [TokenType.Identifier, TokenType.LeftCurly]))
        {
            if (!TryGetParams(tokens, out parameters))
            {
                parameters = [];
            }
            return true;
        }
        return ResetTokenIndex(startIndex, out parameters);
    }

    private bool TryGetAction(Token[] tokens, out IStatement? statement)
    {
        int startIndex = tokenIndex;
        string identifier = tokens[tokenIndex].Value;
        if (TryParseMethod(tokens, out IExpression[]? parameters))
        {
            statement = new Global.AST.Action(identifier, parameters!, GetCoord(tokens, startIndex));
            return true;
        }
        return ResetTokenIndex(startIndex, out statement);
    }

    private bool TryGetFunction(Token[] tokens, out IExpression? expre)
    {
        int startIndex = tokenIndex;
        string identifier = tokens[tokenIndex].Value;
        if (TryParseMethod(tokens, out IExpression[]? parameters))
        {
            expre = new Function(identifier, parameters!, GetCoord(tokens, startIndex));
            return true;
        }
        return ResetTokenIndex(startIndex, out expre);
    }

    private bool TryGetParams(Token[] tokens, out IExpression[]? parameters)
    {
        int startIndex = tokenIndex;
        List<TryGetFunc> list = [TryParseBooleanExpre, TryParseArithExpre, TryParseStringExpre];
        List<IExpression> paramList = [];
        while (!TryMatchToken(tokens, TokenType.RightCurly))
        {
            int paramIndex = tokenIndex;
            foreach (var func in list)
            {
                if (paramList.Count == 0 && func(tokens, out IExpression? value))
                {
                    paramList.Add(value!);
                    break;
                }
                else if (TryMatchToken(tokens, TokenType.Comma) && func(tokens, out value))
                {
                    paramList.Add(value!);
                    break;
                }
                tokenIndex = paramIndex;
            }
            if (paramIndex == tokenIndex)
            {
                return ResetTokenIndex(startIndex, out parameters);
            }
        }
        ;

        return GetDefaultExpre([.. paramList], out parameters);
    }

    #endregion

    #region Arithmetic

    private bool TryParseArithExpre(Token[] tokens, out IExpression? expression)
        => TryGetAddExpre(tokens, out expression);
    private bool TryGetAddExpre(Token[] tokens, out IExpression? expre)
        => TryShiftBinaryExpre(tokens, TryGetProduct, out expre, [TokenType.Plus, TokenType.Minus]);

    private bool TryGetProduct(Token[] tokens, out IExpression? expre)
        => TryShiftBinaryExpre(tokens, TryGetPow, out expre, [TokenType.Dot, TokenType.Division, TokenType.Modulus]);

    private bool TryGetPow(Token[] tokens, out IExpression? expre)
        => TryShiftBinaryExpre(tokens, TryGetNegativeExpre, out expre, [TokenType.Exponentiation]);

    private bool TryGetNegativeExpre(Token[] tokens, out IExpression? expre)
        => TryShifttUnaryExpre(tokens, TryGetNum, out expre, [TokenType.Minus]);

    //private bool TryGetNegativeExpre(Token[] tokens, out IExpression? expre)
    //{
    //    int startIndex = tokenIndex;
    //    if (TryMatchToken(tokens, TokenType.Minus)
    //        && TryParseArithExpre(tokens, out IExpression? argument))
    //    {
    //        var node = new UnaryExpreNode(argument!, UnaryOperationType.Negative, GetCoord(tokens, startIndex));
    //        expre = node;
    //        return true;
    //    }
    //    expre = null;
    //    return false;
    //}

    private bool TryGetNum(Token[] tokens, out IExpression? expre)
        => TryGetLiteral(tokens, LiteralType.Integer, TryParseArithExpre, out expre);

    #endregion

    #region Boolean

    private bool TryParseBooleanExpre(Token[] tokens, out IExpression b) => TryGetOrExpre(tokens, out b!);

    private bool TryGetOrExpre(Token[] tokens, out IExpression? expre)
        => TryShiftBinaryExpre(tokens, TryGetAndExpre, out expre, [TokenType.Or]);

    private bool TryGetAndExpre(Token[] tokens, out IExpression? expre)
        => TryShiftBinaryExpre(tokens, TryGetComparator, out expre, [TokenType.And]);

    private bool TryGetComparator(Token[] tokens, out IExpression? expre) => TryShiftBinaryExpre(tokens, TryMatchComp, out expre,
    [
        TokenType.Equal,
        TokenType.NotEqual,
        TokenType.GreaterOrEqual,
        TokenType.Greater,
        TokenType.Less,
        TokenType.LessOrEqual
    ]);

    private bool TryMatchComp(Token[] tokens, out IExpression? expre) =>
        TryParseArithExpre(tokens, out expre) || TryGetComplement(tokens, out expre)
        || TryParseStringExpre(tokens, out expre);

    private bool TryGetComplement(Token[] tokens, out IExpression? expre)
        => TryShifttUnaryExpre(tokens, TryGetBool, out expre, [TokenType.Not]);

    private bool TryGetBool(Token[] tokens, out IExpression? expre) =>
        TryGetLiteral(tokens, LiteralType.Boolean, TryParseBooleanExpre, out expre);

    #endregion

    #region Strings

    private bool TryParseStringExpre(Token[] tokens, out IExpression? expre)
        => TryGetLiteral(tokens, LiteralType.String, null, out expre);

    #endregion

    #region Tools

    private bool TryShiftBinaryExpre(Token[] tokens, TryGetFunc tryGetFunc, out IExpression? expre, TokenType[] tokenTypes)
    {
        int startIndex = tokenIndex;
        if (!tryGetFunc(tokens, out IExpression? left))
            return ResetTokenIndex(startIndex, out expre);
        if (!TryReduceBinaryExpre(tokens, tryGetFunc, left, out expre, tokenTypes))
            return GetDefaultExpre(left!, out expre);
        return true;
    }

    private bool TryShifttUnaryExpre(Token[] tokens, TryGetFunc tryGetFunc, out IExpression? expre, TokenType[] tokenTypes)
    {
        int startIndex = tokenIndex;
        if (!TryGetMatchToken(tokens, tokenTypes, out TokenType type))
            return tryGetFunc(tokens, out expre);

        var varcelona = type.ToUnaryType();

        int count = 0;
        while (TryMatchToken(tokens, type))
            count++;

        if (!tryGetFunc(tokens, out IExpression? argument))
            return ResetTokenIndex(startIndex, out expre);

        for (; count >= 0; count--)
            argument = new UnaryExpreNode(argument!, varcelona, GetCoord(tokens, startIndex + count));
        return GetDefaultExpre(argument!, out expre);
    }

    private bool TryReduceBinaryExpre(Token[] tokens, TryGetFunc tryGetFunc, IExpression? left, out IExpression? expre, TokenType[] tokenTypes)
    {
        int startIndex = tokenIndex;
        if (!TryGetMatchToken(tokens, tokenTypes, out TokenType type))
            return ResetTokenIndex(startIndex, out expre);
        OperatorState operatorState = ShiftOrReduceOperators[type];
        if (operatorState == OperatorState.Shift && TryShiftBinaryExpre(tokens, tryGetFunc, out IExpression? right, tokenTypes))
            return GetDefaultExpre(new BinaryExpreNode(left!, right!, type.ToBinaryType(), GetCoord(tokens, startIndex)), out expre);
        if (operatorState == OperatorState.Reduce && tryGetFunc(tokens, out right))
        {
            IExpression node = new BinaryExpreNode(left!, right!, type.ToBinaryType(), GetCoord(tokens, startIndex));
            if (TryReduceBinaryExpre(tokens, tryGetFunc, node, out IExpression? result, tokenTypes))
                return GetDefaultExpre(result, out expre);
            return GetDefaultExpre(node, out expre);
        }
        return ResetTokenIndex(startIndex, out expre);
    }

    private bool TryGetLiteral(Token[] tokens, LiteralType literalType, TryGetFunc? tryGetFunc, out IExpression? expre)
    {
        int startIndex = tokenIndex;
        if (TryMatchToken(tokens, literalType)
            && DynamicValue.TryParse(tokens[startIndex].Value, null, out DynamicValue? literal))
            return GetDefaultExpre(new LiteralExpre(literal, GetCoord(tokens, startIndex)), out expre);
        else if (TryMatchToken(tokens, TokenType.LeftCurly)
            && tryGetFunc is not null && tryGetFunc(tokens, out IExpression? result)
            && TryMatchToken(tokens, TokenType.RightCurly))
            return GetDefaultExpre(result!, out expre);
        else if (TryGetFunction(tokens, out result))
            return GetDefaultExpre(result!, out expre);
        else if (TryMatchToken(tokens, TokenType.Identifier))
            return GetDefaultExpre(new VariableExpre(tokens[startIndex].Value, GetCoord(tokens, startIndex)), out expre);
        return ResetTokenIndex(startIndex, out expre);
    }

    private bool TryGetMatchToken(Token[] tokens, TokenType[] types, out TokenType type)
    {
        foreach (TokenType item in types)
        {
            if (!TryMatchToken(tokens, item))
                continue;
            type = item;
            return true;
        }
        type = TokenType.Unknown;
        return false;
    }

    private bool TryMatchAllTokens(Token[] tokens, TokenType[] types)
    {
        int startIndex = tokenIndex;
        foreach (var item in types)
        {
            if (tokens[tokenIndex].Type != item)
            {
                tokenIndex = startIndex;
                return false;
            }
            tokenIndex++;
        }
        return true;
    }

    private bool TryMatchToken<T>(Token[] tokens, T type)
    {
        switch (type)
        {
            case TokenType token:
                if (tokens[tokenIndex].Type != token)
                    return false;
                break;
            case BinaryOperationType binOp:
                if (tokens[tokenIndex].Type != binOp.ToTokenType())
                    return false;
                break;
            case UnaryOperationType unaOp:
                if (tokens[tokenIndex].Type != unaOp.ToTokenType())
                    return false;
                break;
            case LiteralType lit:
                if (tokens[tokenIndex].Type != lit.ToTokenType())
                    return false;
                break;
            default:
                throw new Exception();
        }
        tokenIndex++;
        return true;
    }

    private bool ResetTokenIndex<T>(int index, out T? type)
    {
        tokenIndex = index;
        type = default;
        return false;
    }

    private static bool GetDefaultExpre<T>(T value, out T expre)
    {
        expre = value;
        return true;
    }

    #endregion
}