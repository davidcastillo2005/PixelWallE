using PixelWallE.Interfaces;

namespace PixelWallE.Data.AST;

public class CodeBlock(IStatement[] lines) : IStatement
{
    public IStatement[] Lines { get; protected set; } = lines;

    public void Accept(IVisitor visitor) => visitor.CodeBlockVisit(Lines);
}