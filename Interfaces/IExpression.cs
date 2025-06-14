namespace PixelWallE.Interfaces;

public interface IExpression
{
    Object Accept(IVisitor visitor);
}
