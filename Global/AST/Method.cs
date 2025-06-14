using PixelWallE.Interfaces;

namespace PixelWallE.Global.AST;

public abstract class Method(string identifier, IExpression[] arguments, Coord coord)
{
    public string Identifier { get; set; } = identifier;
    public IExpression[] Arguments { get; set; } = arguments;
    public Coord Coord = coord;
}

public class Function(string identifier, IExpression[] arguments, Coord coord) : Method(identifier, arguments, coord), IExpression
{
    public Object Accept(IVisitor visitor)
    {
        return visitor.FunctionVisit(Identifier, visitor.ParametersVisit(Arguments), Coord);
    }
}

public class Action(string identifier, IExpression[] arguments, Coord coord) : Method(identifier, arguments, coord), IStatement
{
    public void Accept(IVisitor visitor)
        => visitor.ActionVisit(Identifier,
                               visitor.ParametersVisit(Arguments),
                               Coord);
}