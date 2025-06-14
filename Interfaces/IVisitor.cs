using PixelWallE.SourceCodeAnalisis.Syntactic.Enums;

namespace PixelWallE.Interfaces;

public interface IVisitor
{
    void Visit(IStatement statement, Coord coord);
    void AssignVisit(string identifier, Object value, Coord coord);
    void LabelVisit(string identifier, Coord coord);
    void GotoVisit(string targetLabel, Object? condition, Coord coord);
    void ActionVisit(string identifier, Object[] arguments, Coord coord);
    void CodeBlockVisit(IStatement[] lines);
    Object FunctionVisit(string identifier, Object[] arguments, Coord coord);
    Object VariableVisit(string identifier, Coord coord);
    Object BinaryVisit(Object left, BinaryOperationType op, Object right, Coord coord);
    Object UnaryVisit(Object argument, UnaryOperationType op, Coord coord);
    Object LiteralVisit(Object value, Coord coord);
    Object[] ParametersVisit(IExpression[] expressions);
}

