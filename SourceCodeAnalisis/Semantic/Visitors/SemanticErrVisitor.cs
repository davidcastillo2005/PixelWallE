using PixelWallE.Global;
using PixelWallE.Global.AST;
using PixelWallE.Interfaces;
using PixelWallE.SourceCodeAnalisis.Semantic;
using PixelWallE.SourceCodeAnalisis.Syntactic.Enums;

namespace PixelWallE.SourceCodeAnalisis.Semantic.Visitors;

public class SemanticErrVisitor(Context context) : IVisitor
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
    private readonly Context Context = context;
    public List<Error> Exceptions { get; set; } = [];

    public void Visit(IStatement statement, Coord coord) => statement.Accept(this);

    public Object VariableVisit(string identifier, Coord coord)
    {
        if (Context.Variables.TryGetValue(identifier, out Object? value))
        {
            return GetResult(coord, value);
        }
        AddException(coord, $"{identifier} not declared.");
        return GetResult(coord, value);
    }

    public void ActionVisit(string identifier, Object[] arguments, Coord coord)
    {
        if (Context.Handler.TryGetErrAction(identifier, arguments, this))
        {
            AddException(coord, "Wall-E is already spawned.");
        }
    }

    public Object FunctionVisit(string identifier, Object[] arguments, Coord coord)
    {
        if (Context.Handler.TryGetErrFunction(identifier, arguments, out Object result))
        {
            //TODO Error: Method 'identifier' not declared.
            throw new Exception();
        }
        return result;
    }

    public Object[] ParametersVisit(IExpression[] parameters)
    {
        List<Object> results = [];
        foreach (var item in parameters)
        {
            results.Add(item.Accept(this));
        }
        return [.. results];
    }

    public void AssignVisit(string identifier, Object value, Coord coord)
    {
        if (!Context.Variables.ContainsKey(identifier))
        {
            //TODO Error: 
        }
        else
        {
            Context.Variables[identifier] = GetResult(coord, value);
        }
    }

    public Object LiteralVisit(Object value, Coord coord)
        => GetResult(coord, value);

    public Object UnaryVisit(Object argument, UnaryOperationType op, Coord coord)
    {
        if (argument.Value is int && op == UnaryOperationType.Not)
        {
            //TODO Error: Unsupported {op} for {argument.Type}.
        }
        else if (argument.Value is bool && op == UnaryOperationType.Negative)
        {
            //TODO Error: Unsupported {op} for {argument.Type}.
        }
        return GetResult(coord, argument);
    }

    public Object BinaryVisit(Object left, BinaryOperationType op, Object right, Coord coord)
    {
        if (left.Type != right.Type)
        {
            AddException(coord, $"Unsupported {op} for {left.Type} and {right.Type}");
        }
        else if (right.Value is int rInt && (op == BinaryOperationType.Divide || op == BinaryOperationType.Modulus) && rInt == 0)
        {
            AddException(coord, "Division by zero is not supported");
        }
        switch (op)
        {
            case BinaryOperationType.Add:
            case BinaryOperationType.Subtract:
            case BinaryOperationType.Multiply:
            case BinaryOperationType.Divide:
            case BinaryOperationType.Power:
            case BinaryOperationType.Modulus:
                return GetResult(coord, new Object(typeof(int)));
            case BinaryOperationType.Or:
            case BinaryOperationType.And:
            case BinaryOperationType.LessOrEqualThan:
            case BinaryOperationType.LessThan:
            case BinaryOperationType.GreaterOrEqualThan:
            case BinaryOperationType.GreaterThan:
            case BinaryOperationType.Equal:
            case BinaryOperationType.NotEqual:
                return GetResult(coord, new Object(typeof(bool)));
            default:
                throw new Exception();
        }
    }

    public void LabelVisit(string identifier, Coord coord)
    {
        if (Context.Labels.ContainsKey(identifier))
            AddException(coord, $"{identifier} already declared.");
    }

    public void GotoVisit(string targetLabel, Object? condition, Coord coord)
    {
        if (!Context.Labels.ContainsKey(targetLabel))
        {
            AddException(coord, $"{targetLabel} not declared.");
        }
    }

    public void CodeBlockVisit(IStatement[] lines)
    {
        SearchLabel(lines);
        foreach (IStatement? item in lines)
        {
            if (item is not LabelStatement)
                item.Accept(this);
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

    #region Tools

    private Object GetResult(Coord coord, Object? value)
    {
        if (value is null)
        {
            AddException(coord, "Null value");
            return new Object(value);
        }
        var type = value.Type;
        if (type != typeof(int) && type != typeof(bool) && type != typeof(string))
        {
            AddException(coord, "Unsupported value");
        }
        return new Object(type);
    }

    public void AddException(Coord coord, string message)
    {
        Exceptions.Add(new Error(coord, message));
    }

    #endregion
}