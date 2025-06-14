using System.Diagnostics.CodeAnalysis;

namespace PixelWallE.Global;

public class Object : IParsable<Object>
{
    public object? Value { get; set; }
    public Type Type { get; set; }

    public Object(object? value)
    {
        Value = value;
        Type = value?.GetType() ?? typeof(object);
    }

    public Object(object? value, Type type)
    {
        Value = value;
        Type = type;
    }

    public Object(Type type)
    {
        Value = default(Type);
        Type = type;
    }

    public bool ToBoolean()
    {
        if (Value is not null && Value is bool bValue)
            return bValue;
        throw new Exception();
    }

    public int ToInterger()
    {
        if (Value is not null && Value is int iValue)
            return iValue;
        throw new Exception();
    }

    public override string ToString()
    {
        if (Value is not null && Value is string sValue)
            return sValue;
        throw new Exception();
    }

    public static Object operator +(Object a, Object b) => new((dynamic)a.Value! + (dynamic)b.Value!);
    public static Object operator -(Object a, Object b) => new((dynamic)a.Value! - (dynamic)b.Value!);
    public static Object operator *(Object a, Object b) => new((dynamic)a.Value! * (dynamic)b.Value!);
    public static Object operator /(Object a, Object b) => new((dynamic)a.Value! / (dynamic)b.Value!);
    public static Object operator %(Object a, Object b) => new((dynamic)a.Value! % (dynamic)b.Value!);
    public static Object operator &(Object a, Object b) => new((dynamic)a.Value! & (dynamic)b.Value!);
    public static Object operator |(Object a, Object b) => new((dynamic)a.Value! | (dynamic)b.Value!);
    public static Object operator ^(Object a, Object b)
    {
        if (a.Value is double castedA && b.Value is double castedB)
            return new Object(Math.Pow(castedA, castedB), a.Type);
        throw new Exception();
    }
    public static Object operator ==(Object a, Object b) => new((dynamic)a.Value! == (dynamic)b.Value!);
    public static Object operator !=(Object a, Object b) => new((dynamic)a.Value! != (dynamic)b.Value!);
    public static Object operator <(Object a, Object b) => new((dynamic)a.Value! < (dynamic)b.Value!);
    public static Object operator <=(Object a, Object b) => new((dynamic)a.Value! <= (dynamic)b.Value!);
    public static Object operator >(Object a, Object b) => new((dynamic)a.Value! > (dynamic)b.Value!);
    public static Object operator >=(Object a, Object b) => new((dynamic)a.Value! >= (dynamic)b.Value!);
    public static Object operator -(Object a) => new(-(dynamic)a.Value!);
    public static Object operator !(Object a) => new(!(dynamic)a.Value!);

    public override bool Equals(object? obj)
        => ReferenceEquals(obj, this)
        && obj is Object value
        && Value!.Equals(value.Value);

    public override int GetHashCode() => Value != null ? Value.GetHashCode() : Type.GetHashCode();

    public static Object Parse(string s, IFormatProvider? provider)
    {
        if (int.TryParse(s, provider, out int num))
            return new Object(num);
        if (bool.TryParse(s, out bool b))
            return new Object(b);
        return new Object(s);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Object result)
    {
        if (int.TryParse(s, provider, out int num))
            result = new Object(num);
        else if (bool.TryParse(s, out bool b))
            result = new Object(b);
        else
            result = new Object(s);
        return true;
    }
}