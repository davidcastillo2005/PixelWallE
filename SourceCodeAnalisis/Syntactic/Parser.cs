using PixelWallE.Global.AST;
using PixelWallE.Interfaces;
using PixelWallE.SourceCodeAnalisis.Syntactic.Enums;
using PixelWallE.SourceCodeAnalisis.Syntactic.Extensions;

namespace PixelWallE.SourceCodeAnalisis.Syntactic;

public enum OperatorState { Shift, Reduce }

public class Parser
{
    private int tokenIndex;
    private Token[] tokens;
    private readonly Dictionary<TokenType, OperatorState> ShiftOrReduceOperators = new()
    {
        { TokenType.Plus, OperatorState.Shift }, { TokenType.Minus, OperatorState.Reduce },
        { TokenType.Dot, OperatorState.Shift }, { TokenType.Division, OperatorState.Reduce },
        { TokenType.Modulus, OperatorState.Reduce }, { TokenType.Exponentiation, OperatorState.Shift },
        { TokenType.Equal, OperatorState.Reduce }, { TokenType.NotEqual, OperatorState.Reduce },
        { TokenType.Greater, OperatorState.Reduce }, { TokenType.GreaterOrEqual, OperatorState.Reduce },
        { TokenType.Less, OperatorState.Reduce }, { TokenType.LessOrEqual, OperatorState.Reduce },
    };
    private delegate bool TryGetFunc(out IExpression? expre);

    public List<IStatement> Lines { get; private set; } = [];
    public List<Problem> Problems { get; private set; } = [];

    public Parser(Token[] tokens)
    {
        this.tokens = tokens;
    }

    public IStatement Parse()
        => TryGetCodeBlock(out IStatement? expre) ? expre! : throw new Exception();

    private bool TryGetCodeBlock(out IStatement? expre)
    {
        bool ReadLine;
        do
        {
            if (ReadLine = TryGetAssignStatement(out IStatement? line)
                || TryGetGoToStatement(out line)
                || TryGetAction(out line)
                || TryGetLabelStatement(Lines.Count, out line))
            {
                Lines.Add(line!);
                continue;
            }
            ReadLine = TryMatchToken(TokenType.NewLine);
        } while (ReadLine);

        CodeBlock node = new([.. Lines]);
        return GetDefaultExpre(node, out expre);
    }

    private bool TryGetGoToStatement(out IStatement? lineExpre)
    {
        int startIndex = tokenIndex;
        if (TryMatchAllTokens(
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
            if (TryMatchToken(TokenType.LeftCurly)
                && TryParseBooleanExpre(out IExpression cond)
                && TryMatchToken(TokenType.RightCurly))
            {
                @goto = new GoToStatement(targetLabel, cond, GetCoord(startIndex));
            }
            else
            {
                @goto = new GoToStatement(targetLabel, null, GetCoord(startIndex));
            }
            return GetDefaultExpre(@goto, out lineExpre);
        }
        return ResetTokenIndex(startIndex, out lineExpre);
    }

    private Coord GetCoord(int startIndex)
        => new(tokens[startIndex].Coords.Row, tokens[startIndex].Coords.Col,
               tokens[tokenIndex].Coords.Col - tokens[startIndex].Coords.Col);

    private bool TryGetLabelStatement(int lineIndex, out IStatement? lineExpre)
    {
        int startIndex = tokenIndex;
        string value = tokens[tokenIndex].Value;
        if (!TryMatchAllTokens([TokenType.Identifier, TokenType.NewLine]))
            return ResetTokenIndex(startIndex, out lineExpre);
        IStatement label = new LabelStatement(value, GetCoord(startIndex));
        return GetDefaultExpre(label, out lineExpre);
    }

    private bool TryGetAssignStatement(out IStatement? expre)
    {
        int startIndex = tokenIndex;
        if (!(TryMatchAllTokens([TokenType.Identifier, TokenType.Assign])
            && TryParseExpre(out IExpression? value)))
            return ResetTokenIndex(startIndex, out expre);
        var assign = new AssignStatement(tokens[startIndex].Value, value!, GetCoord(startIndex));
        return GetDefaultExpre(assign, out expre);
    }

    private bool TryParseExpre(out IExpression? expre)
    {
        int startIndex = tokenIndex;
        string name = tokens[tokenIndex].Value;
        TryGetFunc[] list = [TryParseBooleanExpre, TryParseArithExpre, TryParseStringExpre];

        foreach (var func in list)
        {
            if (func(out IExpression? value) && TryMatchToken(TokenType.NewLine))
                return GetDefaultExpre(value!, out expre);
            tokenIndex = startIndex;
        }

        return ResetTokenIndex(startIndex, out expre);
    }

    #region Methods

    private bool TryParseMethod(out IExpression[]? parameters)
    {
        int startIndex = tokenIndex;
        if (TryMatchAllTokens([TokenType.Identifier, TokenType.LeftCurly])
            && TryGetParams(out parameters))
        {
            return true;
        }
        return ResetTokenIndex(startIndex, out parameters);
    }

    private bool TryGetAction(out IStatement? statement)
    {
        int startIndex = tokenIndex;
        string identifier = tokens[tokenIndex].Value;
        if (TryParseMethod(out IExpression[]? parameters))
        {
            statement = new Global.AST.Action(identifier, parameters!, GetCoord(startIndex));
            return true;
        }
        return ResetTokenIndex(startIndex, out statement);
    }

    private bool TryGetFunction(out IExpression? expre)
    {
        int startIndex = tokenIndex;
        string identifier = tokens[tokenIndex].Value;
        if (TryParseMethod(out IExpression[]? parameters))
        {
            expre = new Function(identifier, parameters!, GetCoord(startIndex));
            return true;
        }
        return ResetTokenIndex(startIndex, out expre);
    }

    private bool TryGetParams(out IExpression[]? parameters)
    {
        int startIndex = tokenIndex;
        List<TryGetFunc> list = [TryParseBooleanExpre, TryParseArithExpre, TryParseStringExpre];
        List<IExpression> paramsList = [];
        TryGetFirstParam(list, paramsList);
        TryGetInBetweenParams(list, paramsList);
        if (TryMatchToken(TokenType.RightCurly))
        {
            return GetDefaultExpre([.. paramsList], out parameters);
        }
        Problems.Add(new Error(GetCoord(tokenIndex), "')' expected."));
        return ResetTokenIndex(startIndex, out parameters);
    }

    private void TryGetInBetweenParams(List<TryGetFunc> list, List<IExpression> paramsList)
    {
        while (TryMatchToken(TokenType.Comma))
        {
            int paramIndex = tokenIndex;
            foreach (var func in list)
            {
                if (func(out IExpression? expre))
                {
                    paramsList.Add(expre!);
                    break;
                }
                ResetTokenIndex(paramIndex, out expre);
            }
        }
    }

    private void TryGetFirstParam(List<TryGetFunc> list, List<IExpression> paramsList)
    {
        foreach (var func in list)
        {
            if (func(out IExpression? expre))
            {
                paramsList.Add(expre!);
                break;
            }
        }
    }

    #endregion

    #region Arithmetic

    private bool TryParseArithExpre(out IExpression? expression)
        => TryGetAddExpre(out expression);
    private bool TryGetAddExpre(out IExpression? expre)
        => TryShiftBinaryExpre(TryGetProduct, out expre, [TokenType.Plus, TokenType.Minus]);

    private bool TryGetProduct(out IExpression? expre)
        => TryShiftBinaryExpre(TryGetPow, out expre, [TokenType.Dot, TokenType.Division, TokenType.Modulus]);

    private bool TryGetPow(out IExpression? expre)
        => TryShiftBinaryExpre(TryGetNegativeExpre, out expre, [TokenType.Exponentiation]);

    private bool TryGetNegativeExpre(out IExpression? expre)
        => TryShifttUnaryExpre(TryGetNum, out expre, [TokenType.Minus]);

    private bool TryGetNum(out IExpression? expre)
        => TryGetLiteral(LiteralType.Integer, TryParseArithExpre, out expre);

    #endregion

    #region Boolean

    private bool TryParseBooleanExpre(out IExpression b) => TryGetOrExpre(tokens, out b!);

    private bool TryGetOrExpre(Token[] tokens, out IExpression? expre)
        => TryShiftBinaryExpre(TryGetAndExpre, out expre, [TokenType.Or]);

    private bool TryGetAndExpre(out IExpression? expre)
        => TryShiftBinaryExpre(TryGetComparator, out expre, [TokenType.And]);

    private bool TryGetComparator(out IExpression? expre) => TryShiftBinaryExpre(TryMatchComp, out expre,
    [
        TokenType.Equal,
        TokenType.NotEqual,
        TokenType.GreaterOrEqual,
        TokenType.Greater,
        TokenType.Less,
        TokenType.LessOrEqual
    ]);

    private bool TryMatchComp(out IExpression? expre) =>
        TryParseArithExpre(out expre) || TryGetComplement(tokens, out expre)
        || TryParseStringExpre(out expre);

    private bool TryGetComplement(Token[] tokens, out IExpression? expre)
        => TryShifttUnaryExpre(TryGetBool, out expre, [TokenType.Not]);

    private bool TryGetBool(out IExpression? expre) =>
        TryGetLiteral(LiteralType.Boolean, TryParseBooleanExpre, out expre);

    #endregion

    #region Strings

    private bool TryParseStringExpre(out IExpression? expre)
        => TryGetLiteral(LiteralType.String, null, out expre);

    #endregion

    #region Tools

    private bool TryShiftBinaryExpre(TryGetFunc tryGetFunc, out IExpression? expre, TokenType[] tokenTypes)
    {
        int startIndex = tokenIndex;
        if (!tryGetFunc(out IExpression? left))
            return ResetTokenIndex(startIndex, out expre);
        if (!TryReduceBinaryExpre(tryGetFunc, left, out expre, tokenTypes))
            return GetDefaultExpre(left!, out expre);
        return true;
    }

    private bool TryShifttUnaryExpre(TryGetFunc tryGetFunc, out IExpression? expre, TokenType[] tokenTypes)
    {
        int startIndex = tokenIndex;
        if (!TryGetMatchToken(tokenTypes, out TokenType type))
            return tryGetFunc(out expre);

        var varcelona = type.ToUnaryType();

        int count = 0;
        while (TryMatchToken(type))
            count++;

        if (!tryGetFunc(out IExpression? argument))
            return ResetTokenIndex(startIndex, out expre);

        for (; count >= 0; count--)
            argument = new UnaryExpreNode(argument!, varcelona, GetCoord(startIndex + count));
        return GetDefaultExpre(argument!, out expre);
    }

    private bool TryReduceBinaryExpre(TryGetFunc tryGetFunc, IExpression? left, out IExpression? expre, TokenType[] tokenTypes)
    {
        int startIndex = tokenIndex;
        if (!TryGetMatchToken(tokenTypes, out TokenType type))
            return ResetTokenIndex(startIndex, out expre);
        OperatorState operatorState = ShiftOrReduceOperators[type];
        if (operatorState == OperatorState.Shift && TryShiftBinaryExpre(tryGetFunc, out IExpression? right, tokenTypes))
            return GetDefaultExpre(new BinaryExpreNode(left!, right!, type.ToBinaryType(), GetCoord(startIndex)), out expre);
        if (operatorState == OperatorState.Reduce && tryGetFunc(out right))
        {
            IExpression node = new BinaryExpreNode(left!, right!, type.ToBinaryType(), GetCoord(startIndex));
            if (TryReduceBinaryExpre(tryGetFunc, node, out IExpression? result, tokenTypes))
                return GetDefaultExpre(result, out expre);
            return GetDefaultExpre(node, out expre);
        }
        return ResetTokenIndex(startIndex, out expre);
    }

    private bool TryGetLiteral(LiteralType literalType, TryGetFunc? tryGetFunc, out IExpression? expre)
    {
        int startIndex = tokenIndex;
        if (TryMatchToken(literalType)
            && DynamicValue.TryParse(tokens[startIndex].Value, null, out DynamicValue? literal))
            return GetDefaultExpre(new LiteralExpre(literal, GetCoord(startIndex)), out expre);
        else if (TryMatchToken(TokenType.LeftCurly)
            && tryGetFunc is not null && tryGetFunc(out IExpression? result)
            && TryMatchToken(TokenType.RightCurly))
            return GetDefaultExpre(result!, out expre);
        else if (TryGetFunction(out result))
            return GetDefaultExpre(result!, out expre);
        else if (TryMatchToken(TokenType.Identifier))
            return GetDefaultExpre(new VariableExpre(tokens[startIndex].Value, GetCoord(startIndex)), out expre);
        return ResetTokenIndex(startIndex, out expre);
    }

    private bool TryGetMatchToken(TokenType[] types, out TokenType type)
    {
        foreach (TokenType item in types)
        {
            if (!TryMatchToken(item))
                continue;
            type = item;
            return true;
        }
        type = TokenType.Unknown;
        return false;
    }

    private bool TryMatchAllTokens(TokenType[] types)
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

    private bool TryMatchToken<T>(T type)
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