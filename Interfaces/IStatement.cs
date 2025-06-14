namespace PixelWallE.Interfaces;


public interface IStatement
{
    void Accept(IVisitor visitor);
}
