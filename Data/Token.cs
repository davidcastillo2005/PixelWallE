namespace PixelWallE.Data;

public class Token
{
    public TokenType Type { get; }
    public string Value { get; set; }
    public Coord Coords { get; set; }

    public Token(TokenType type, string value, int row, int column)
    {
        Type = type;
        Value = value;
        Coords = new Coord(row, column, GetLength());
    }

    public override string ToString()
    {
        return $"{Coords} : {Type} : {Value}";
    }

    private int GetLength()
    {
        int length = 0 ;
        if (Type == TokenType.String)
        {
            length += 2;
        }
        return Value.Length + length;
    }
}

public enum TokenType
{
    Identifier, Keyword, Plus, Minus, Dot, Division,
    Exponentiation, Modulus, String,
    LeftCurly, RightCurly, LeftBracket, RightBracket,
    LessOrEqual, GreaterOrEqual, Less, Greater, Equal, Interger, Boolean, Assign,
    Comma, Color, NewLine, NotEqual, And, Or, Not, EOF, Unknown, GoTo, False, True
}

public record struct Coord(int Row, int Col, int Length);
