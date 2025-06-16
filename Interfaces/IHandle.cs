using System.Windows.Media;
using PixelWallE.SourceCodeAnalisis.Semantic.Visitors;

namespace PixelWallE.Interfaces;

public interface IHandle
{
    DynamicValue CallFunction(string Name, DynamicValue[] @params);
    void CallAction(string Name, DynamicValue[] @params);
    bool TryGetErrFunction(string Name, DynamicValue[] @params, out DynamicValue @object);
    bool TryGetErrAction(string Name, DynamicValue[] @params, SemanticErrVisitor errVisitor);
    SolidColorBrush ToBrush(string color);
}