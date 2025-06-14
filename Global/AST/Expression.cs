using PixelWallE.Interfaces;
using PixelWallE.SourceCodeAnalisis.Syntactic.Enums;

namespace PixelWallE.Global.AST;

public class BinaryExpreNode(IExpression left, IExpression right, BinaryOperationType opType, Coord coord) : IExpression
{
    public IExpression LeftArg { get; set; } = left;
    public IExpression RightArg { get; set; } = right;
    public BinaryOperationType OperatorType { get; set; } = opType;
    public Coord Coord { get; set; } = coord;

    public Object Accept(IVisitor visitor)
        => visitor.BinaryVisit(LeftArg.Accept(visitor), OperatorType, RightArg.Accept(visitor), Coord);
}
public class UnaryExpreNode : IExpression
{
    public IExpression Argument { get; set; }
    public UnaryOperationType OperatorType { get; set; }
    public Coord Coord { get; set; }

    public UnaryExpreNode(IExpression argument, UnaryOperationType opType, Coord coord)
    {
        Argument = argument;
        OperatorType = opType;
        Coord = coord;
    }

    public Object Accept(IVisitor visitor)
        => visitor.UnaryVisit(Argument.Accept(visitor), OperatorType, Coord);
}

public class VariableExpre(string identifier, Coord coord) : IExpression
{
    public string Identifier { get; set; } = identifier;
    public Coord Coord { get; set; } = coord;

    public Object Accept(IVisitor visitor) => visitor.VariableVisit(Identifier, Coord);
}

public class LiteralExpre(Object value, Coord coord) : IExpression
{
    public Object Value { get; set; } = value;
    public Coord Coord { get; set; } = coord;
    public Object Accept(IVisitor visitor) => visitor.LiteralVisit(Value, Coord);
}