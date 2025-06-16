using System.Diagnostics.CodeAnalysis;

namespace PixelWallE.Global;

public class DynamicValue : IParsable<DynamicValue>
{
    public object? Value { get; set; }
    public Type Type { get; set; }

    public DynamicValue(object? value)
    {
        Value = value;
        Type = value?.GetType() ?? typeof(object);
    }

    public DynamicValue(object? value, Type type)
    {
        Value = value;
        Type = type;
    }

    public DynamicValue(Type type)
    {
        Value = default(Type);
        Type = type;
    }

    public bool ToBoolean()
    {
        if (Value is null)
            return false;
        return (bool)Value;
    }

    public int ToInterger()
    {
        if (Value is null)
            return 0;
        return (int)Value;
    }

    public override string ToString()
    {
        if (Value is null)
            return "";
        return (string)Value;
    }

    public static DynamicValue operator +(DynamicValue a, DynamicValue b) => new((dynamic)a.Value! + (dynamic)b.Value!);
    public static DynamicValue operator -(DynamicValue a, DynamicValue b) => new((dynamic)a.Value! - (dynamic)b.Value!);
    public static DynamicValue operator *(DynamicValue a, DynamicValue b) => new((dynamic)a.Value! * (dynamic)b.Value!);
    public static DynamicValue operator /(DynamicValue a, DynamicValue b) => new((dynamic)a.Value! / (dynamic)b.Value!);
    public static DynamicValue operator %(DynamicValue a, DynamicValue b) => new((dynamic)a.Value! % (dynamic)b.Value!);
    public static DynamicValue operator &(DynamicValue a, DynamicValue b) => new((dynamic)a.Value! & (dynamic)b.Value!);
    public static DynamicValue operator |(DynamicValue a, DynamicValue b) => new((dynamic)a.Value! | (dynamic)b.Value!);
    public static DynamicValue operator ^(DynamicValue a, DynamicValue b)
    {
        if (a.Value is double castedA && b.Value is double castedB)
            return new DynamicValue(Math.Pow(castedA, castedB), a.Type);
        throw new Exception();
    }
    public static DynamicValue operator ==(DynamicValue a, DynamicValue b) => new((dynamic)a.Value! == (dynamic)b.Value!);
    public static DynamicValue operator !=(DynamicValue a, DynamicValue b) => new((dynamic)a.Value! != (dynamic)b.Value!);
    public static DynamicValue operator <(DynamicValue a, DynamicValue b) => new((dynamic)a.Value! < (dynamic)b.Value!);
    public static DynamicValue operator <=(DynamicValue a, DynamicValue b) => new((dynamic)a.Value! <= (dynamic)b.Value!);
    public static DynamicValue operator >(DynamicValue a, DynamicValue b) => new((dynamic)a.Value! > (dynamic)b.Value!);
    public static DynamicValue operator >=(DynamicValue a, DynamicValue b) => new((dynamic)a.Value! >= (dynamic)b.Value!);
    public static DynamicValue operator -(DynamicValue a) => new(-(dynamic)a.Value!);
    public static DynamicValue operator !(DynamicValue a) => new(!(dynamic)a.Value!);

    public override bool Equals(object? obj)
        => ReferenceEquals(obj, this)
        && obj is DynamicValue value
        && Value!.Equals(value.Value);

    public override int GetHashCode() => Value != null ? Value.GetHashCode() : Type.GetHashCode();

    public static DynamicValue Parse(string s, IFormatProvider? provider)
    {
        if (int.TryParse(s, provider, out int num))
            return new DynamicValue(num);
        if (bool.TryParse(s, out bool b))
            return new DynamicValue(b);
        return new DynamicValue(s);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out DynamicValue result)
    {
        if (int.TryParse(s, provider, out int num))
            result = new DynamicValue(num);
        else if (bool.TryParse(s, out bool b))
            result = new DynamicValue(b);
        else
            result = new DynamicValue(s);
        return true;
    }
}