using PixelWallE.Global;
using PixelWallE.Global.AST;
using PixelWallE.Interfaces;
using PixelWallE.SourceCodeAnalisis.Semantic;
using PixelWallE.SourceCodeAnalisis.Syntactic.Enums;

namespace PixelWallE.SourceCodeAnalisis.Semantic.Visitors;

public class InterpreterVisitor(Context context) : IVisitor
{
    /*
    1- Variable
    2- Action
    3- Function
    3- Parameters
    4- Assign
    5- Literal
    6- Unary
    7- Binary
    8- Label
    9- Goto
    10- CodeBlock
    */
    public Context Context { get; set; } = context;
    public void ActionVisit(string identifier, DynamicValue[] arguments, Coord coord)
    {
        Context.Handler.CallAction(identifier, arguments, coord);
    }

    public void GotoVisit(string targetLabel, DynamicValue? condition, Coord coord)
    {
        if (condition is not null)
        {
            bool cond = condition.ToBoolean();
            if (!cond)
                return;
        }
        Context.Jump(targetLabel);
    }

    public void AssignVisit(string identifier, DynamicValue value, Coord coord) => Context.Variables[identifier] = value;

    public void LabelVisit(string identifier, Coord coord) => Context.Labels[identifier] = coord.Row - 1;

    public DynamicValue BinaryVisit(DynamicValue left, BinaryOperationType op, DynamicValue right, Coord coord) => op switch
    {
        BinaryOperationType.Add => left! + right!,
        BinaryOperationType.Subtract => left! - right!,
        BinaryOperationType.Multiply => left! * right!,
        BinaryOperationType.Divide => left! / right!,
        BinaryOperationType.Power => left! ^ right!,
        BinaryOperationType.Modulus => left! % right!,
        BinaryOperationType.LessOrEqualThan => left! <= right!,
        BinaryOperationType.GreaterOrEqualThan => left! >= right!,
        BinaryOperationType.LessThan => left! < right!,
        BinaryOperationType.GreaterThan => left! > right!,
        BinaryOperationType.Equal => left! == right!,
        BinaryOperationType.NotEqual => left! != right!,
        BinaryOperationType.And => left! & right!,
        BinaryOperationType.Or => left! | right!,
        _ => throw new NotImplementedException(),
    };

    public DynamicValue[] ParametersVisit(IExpression[] expressions)
    {
        List<DynamicValue> results = [];
        foreach (var item in expressions)
        {
            results.Add(item.Accept(this));
        }
        return [.. results];
    }

    public DynamicValue FunctionVisit(string identifier, DynamicValue[] arguments, Coord coord)
    {
        return Context.Handler.CallFunction(identifier, arguments);
    }

    public DynamicValue LiteralVisit(DynamicValue value, Coord coord)
    {
        return value;
    }

    public DynamicValue UnaryVisit(DynamicValue argument, UnaryOperationType op, Coord coord) => op switch
    {
        UnaryOperationType.Not => !argument,
        UnaryOperationType.Negative => -argument,
        _ => throw new Exception(),
    };

    public DynamicValue VariableVisit(string identifier, Coord coord)
    {
        Context.Variables.TryGetValue(identifier, out DynamicValue? result);
        if (result is not null)
            return result;
        else
            throw new NotImplementedException();
    }

    public void Visit(IStatement statement, Coord coord) => statement.Accept(this);

    public void CodeBlockVisit(IStatement[] lines)
    {
        SearchLabel(lines);
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i].Accept(this);
            if (Context.IsJumping)
            {
                i = Context.Labels[Context.TargetLabel!];
                Context.EndJump();
            }
        }
    }

    public void SearchLabel(IStatement[] lines)
    {
        foreach (var item in lines)
        {
            if (item is LabelStatement label)
            {
                label.Accept(this);
            }
        }
    }
}