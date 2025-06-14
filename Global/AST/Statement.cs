using PixelWallE.Interfaces;

namespace PixelWallE.Global.AST;

public class AssignStatement(string identifier, IExpression value, Coord coord) : IStatement
{
    public string Identifier { get; } = identifier;
    public IExpression Value { get; } = value;
    public Coord Coord { get; set; } = coord;

    public void Accept(IVisitor visitor)
        => visitor.AssignVisit(Identifier, Value.Accept(visitor), Coord);
}

public class LabelStatement(string identifier, Coord coord) : IStatement
{
    public string Identifier { get; set; } = identifier;
    public Coord Coord { get; } = coord;

    public void Accept(IVisitor visitor)
        => visitor.LabelVisit(Identifier, Coord);
}

public class GoToStatement(string targetLabel, IExpression? condition, Coord coord) : IStatement
{
    public string TargetLabel { get; } = targetLabel;
    public IExpression? Condition { get; } = condition;
    public Coord Coord { get; set; } = coord;

    public void Accept(IVisitor visitor)
        => visitor.GotoVisit(TargetLabel, Condition?.Accept(visitor), Coord);
}