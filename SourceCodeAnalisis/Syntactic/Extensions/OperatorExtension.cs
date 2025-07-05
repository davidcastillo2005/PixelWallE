using PixelWallE.SourceCodeAnalisis.Syntactic.Enums;

namespace PixelWallE.SourceCodeAnalisis.Syntactic.Extensions;

public static class OperatorExtension
{
    public static BinaryOperationType ToBinaryType(this TokenType type) => type switch
    
    {
        TokenType.Plus => BinaryOperationType.Add,
        TokenType.Minus => BinaryOperationType.Subtract,
        TokenType.Dot => BinaryOperationType.Multiply,
        TokenType.Division => BinaryOperationType.Divide,
        TokenType.Exponentiation => BinaryOperationType.Power,
        TokenType.Modulus => BinaryOperationType.Modulus,
        TokenType.LessOrEqual => BinaryOperationType.LessOrEqualThan,
        TokenType.GreaterOrEqual => BinaryOperationType.GreaterOrEqualThan,
        TokenType.Less => BinaryOperationType.LessThan,
        TokenType.Greater => BinaryOperationType.GreaterThan,
        TokenType.Equal => BinaryOperationType.Equal,
        TokenType.NotEqual => BinaryOperationType.NotEqual,
        TokenType.And => BinaryOperationType.And,
        TokenType.Or => BinaryOperationType.Or,
        _ => throw new Exception(),
    };

    public static TokenType ToTokenType(this BinaryOperationType op) => op switch
    {
        BinaryOperationType.Add => TokenType.Plus,
        BinaryOperationType.Subtract => TokenType.Minus,
        BinaryOperationType.Multiply => TokenType.Dot,
        BinaryOperationType.Divide => TokenType.Division,
        BinaryOperationType.Modulus => TokenType.Modulus,
        BinaryOperationType.Power => TokenType.Exponentiation,
        BinaryOperationType.LessOrEqualThan => TokenType.LessOrEqual,
        BinaryOperationType.LessThan => TokenType.Less,
        BinaryOperationType.GreaterOrEqualThan => TokenType.GreaterOrEqual,
        BinaryOperationType.GreaterThan => TokenType.Greater,
        BinaryOperationType.Equal => TokenType.Equal,
        BinaryOperationType.NotEqual => TokenType.NotEqual,
        BinaryOperationType.And => TokenType.And,
        BinaryOperationType.Or => TokenType.Or,
        _ => throw new Exception(),
    };

    public static TokenType ToTokenType(this UnaryOperationType op) => op switch
    {
        UnaryOperationType.Negative => TokenType.Minus,
        UnaryOperationType.Not => TokenType.Not,
        _ => throw new Exception(),
    };

    public static TokenType ToTokenType(this LiteralType literal) => literal switch
    {
        LiteralType.Boolean => TokenType.Boolean,
        LiteralType.String => TokenType.String,
        LiteralType.Integer => TokenType.Interger,
        _ => throw new Exception(),
    };

    public static UnaryOperationType ToUnaryType(this TokenType type) => type switch
    {
        TokenType.Minus => UnaryOperationType.Negative,
        TokenType.Not => UnaryOperationType.Not,
        _ => throw new Exception(),
    };
}