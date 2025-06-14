namespace PixelWallE.Global;

public class Error(Coord coord, string message) : Exception(message)
{
    public int Row { get; private set; } = coord.Row;
    public int Column { get; private set; } = coord.Col;
    public int Length { get; private set; } = coord.Length;

    public string PrintMessage()
    {
        return $"<{Row}, {Column}> {Message}";
    }
}