using PixelWallE.SourceCodeAnalisis.Syntactic.Enums;

namespace PixelWallE.Interfaces;

public interface IVisitor
{
    void Visit(IStatement statement, Coord coord);
    void AssignVisit(string identifier, DynamicValue value, Coord coord);
    void LabelVisit(string identifier, Coord coord);
    void GotoVisit(string targetLabel, DynamicValue? condition, Coord coord);
    void ActionVisit(string identifier, DynamicValue[] arguments, Coord coord);
    void CodeBlockVisit(IStatement[] lines);
    DynamicValue FunctionVisit(string identifier, DynamicValue[] arguments, Coord coord);
    DynamicValue VariableVisit(string identifier, Coord coord);
    DynamicValue BinaryVisit(DynamicValue left, BinaryOperationType op, DynamicValue right, Coord coord);
    DynamicValue UnaryVisit(DynamicValue argument, UnaryOperationType op, Coord coord);
    DynamicValue LiteralVisit(DynamicValue value, Coord coord);
    DynamicValue[] ParametersVisit(IExpression[] expressions);
}

