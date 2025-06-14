namespace PixelWallE.Global;

public class Token(TokenType type, string value, int row, int column)
{
    public TokenType Type { get; } = type;
    public string Value { get; set; } = value;
    public Coord Coords { get; set; } = new Coord(row, column, value.Length);

    public override string ToString()
    {
        return $"{Coords} : {Type} : {Value}";
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
