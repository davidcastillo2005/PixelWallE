using PixelWallE.Global.AST;
using PixelWallE.Interfaces;
using PixelWallE.SourceCodeAnalisis.Syntactic.Enums;

namespace PixelWallE.SourceCodeAnalisis.Semantic.Visitors;

public class SemanticVisitor(Context context) : IVisitor
{
    private readonly Context Context = context;

    public List<Problem> SemanticProblems { get; set; } = [];

    #region Visit
    public void Visit(IStatement statement, Coord coord) => statement.Accept(this);

    public DynamicValue VariableVisit(string identifier, Coord coord)
    {
        if (Context.Variables.TryGetValue(identifier, out DynamicValue? @return))
        {
            return CheckDynamicValue(coord, @return);
        }
        SemanticProblems.Add(new Error(coord, $"'{identifier}' not declared."));
        return CheckDynamicValue(coord, @return);
    }

    public void ActionVisit(string identifier, DynamicValue[] arguments, Coord coord)
    {
        Context.Handler.TryGetErrAction(identifier, arguments, this, coord);
    }

    public DynamicValue FunctionVisit(string identifier, DynamicValue[] arguments, Coord coord)
    {
        if (Context.Handler.TryGetErrFunction(identifier, arguments, this, coord, out DynamicValue @return))
        {
            return CheckDynamicValue(coord, @return);
        }
        return @return;
    }

    public DynamicValue[] ParametersVisit(IExpression[] parameters)
    {
        List<DynamicValue> results = [];
        foreach (var item in parameters)
        {
            results.Add(item.Accept(this));
        }
        return [.. results];
    }

    public void AssignVisit(string identifier, DynamicValue value, Coord coord)
    {
        Context.Variables[identifier] = CheckDynamicValue(coord, value);
    }

    public DynamicValue LiteralVisit(DynamicValue value, Coord coord)
        => CheckDynamicValue(coord, value);

    public DynamicValue UnaryVisit(DynamicValue argument, UnaryOperationType op, Coord coord)
    {
        if (argument.Value is int && op == UnaryOperationType.Not || argument.Value is bool && op == UnaryOperationType.Negative)
        {
            SemanticProblems.Add(new Error(coord, $"Unsupported {op} for {argument.Type}."));
        }
        switch (op)
        {
            case UnaryOperationType.Not:
                return CheckDynamicValue(coord, new DynamicValue(typeof(bool)));
            case UnaryOperationType.Negative:
                return CheckDynamicValue(coord, new DynamicValue(typeof(int)));
            default:
                SemanticProblems.Add(new Error(coord, $"Unsupported {op}"));
                return CheckDynamicValue(coord, null);
        }
    }

    public DynamicValue BinaryVisit(DynamicValue left, BinaryOperationType op, DynamicValue right, Coord coord)
    {
        if (left.Type != right.Type)
        {
            SemanticProblems.Add(new Error(coord, $"Unsupported {op} for {left.Type} and {right.Type}"));
        }
        else if (right.Value is int rInt && (op == BinaryOperationType.Divide || op == BinaryOperationType.Modulus) && rInt == 0)
        {
            SemanticProblems.Add(new Error(coord, "Division by zero is not supported"));
        }
        switch (op)
        {
            case BinaryOperationType.Add:
            case BinaryOperationType.Subtract:
            case BinaryOperationType.Multiply:
            case BinaryOperationType.Divide:
            case BinaryOperationType.Power:
            case BinaryOperationType.Modulus:
                return CheckDynamicValue(coord, new DynamicValue(typeof(int)));
            case BinaryOperationType.Or:
            case BinaryOperationType.And:
            case BinaryOperationType.LessOrEqualThan:
            case BinaryOperationType.LessThan:
            case BinaryOperationType.GreaterOrEqualThan:
            case BinaryOperationType.GreaterThan:
            case BinaryOperationType.Equal:
            case BinaryOperationType.NotEqual:
                return CheckDynamicValue(coord, new DynamicValue(typeof(bool)));
            default:
                SemanticProblems.Add(new Error(coord, $"Unsupported {op}"));
                return CheckDynamicValue(coord, null);
        }
    }

    public void LabelVisit(string identifier, Coord coord)
    {
        if (Context.Labels.ContainsKey(identifier))
            SemanticProblems.Add(new Error(coord, $"{identifier} already declared."));
        else
        {
            Context.Labels[identifier] = coord.Row - 1;
        }
    }

    public void GotoVisit(string targetLabel, DynamicValue? condition, Coord coord)
    {
        if (!Context.Labels.ContainsKey(targetLabel))
        {
            SemanticProblems.Add(new Error(coord, $"{targetLabel} not declared."));
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

    #endregion

    #region LabelSearch

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

    #endregion

    public DynamicValue CheckDynamicValue(Coord coord, DynamicValue? value)
    {
        if (value is null)
        {
            SemanticProblems.Add(new Error(coord, "Null value"));
            return new DynamicValue(null!);
        }
        var type = value.Type;
        if (type != typeof(int) && type != typeof(bool) && type != typeof(string))
        {
            SemanticProblems.Add(new Error(coord, "Unsupported type"));
        }
        return value;
    }
}