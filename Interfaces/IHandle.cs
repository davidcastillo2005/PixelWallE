using System.Windows.Media;
using PixelWallE.SourceCodeAnalisis.Semantic.Visitors;

namespace PixelWallE.Interfaces;

public interface IHandle
{
    Object CallFunction(string Name, Object[] @params);
    void CallAction(string Name, Object[] @params);
    bool TryGetErrFunction(string Name, Object[] @params, out Object @object);
    bool TryGetErrAction(string Name, Object[] @params, SemanticErrVisitor errVisitor);
    SolidColorBrush ToBrush(string color);
}