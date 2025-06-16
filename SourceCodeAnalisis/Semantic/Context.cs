using PixelWallE.View;

namespace PixelWallE.SourceCodeAnalisis.Semantic;

public class Context
{
    public bool IsJumping { get; set; } = false;
    public string? TargetLabel { get; set; } = null;
    public Handler Handler { get; }
    public Context(Handler handler)
    {
        Variables = [];
        Labels = [];
        Handler = handler;
    }
    public Dictionary<string, DynamicValue> Variables { get; set; }
    public Dictionary<string, int> Labels { get; set; }

    public void Jump(string targetLabel)
    {
        IsJumping = true;
        TargetLabel = targetLabel;
    }

    public void EndJump()
    {
        IsJumping = false;
        TargetLabel = null;
    }
}