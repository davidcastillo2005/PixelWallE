namespace PixelWallE.Data;

public class Problem(Coord coord, string message) : Exception(message)
{
    public int Row { get; private set; } = coord.Row;
    public int Column { get; private set; } = coord.Col;
    public int Length { get; private set; } = coord.Length;

    public virtual string PrintMessage()
    {
        return $"<{Row}, {Column}> {base.Message}";
    }
}

public class Warning(Coord coord, string message) : Problem(coord, message)
{
    public override string PrintMessage()
    {
        return $"WARNING: <{Row}, {Column}> {base.Message}";
    }
}

public class Error(Coord coord, string message) : Problem(coord, message)
{
    public override string PrintMessage()
    {
        return $"ERROR: <{Row}, {Column}> {base.Message}";
    }
}