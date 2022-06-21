namespace Engine;

public sealed class Character
{
    public Start? Start { get; set; }
    public Stop? Stop { get; set; }
    public Update? Update { get; set; }
    public Render? Render { get; set; }
    
}