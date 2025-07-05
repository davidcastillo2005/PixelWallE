namespace PixelWallE.Interfaces;

public interface IExpression
{
    DynamicValue Accept(IVisitor visitor);
}
