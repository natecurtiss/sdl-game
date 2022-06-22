namespace Engine;

public static class MathExt
{
    public static Vector2 Normalized(this Vector2 v) => v == Vector2.Zero ? v : Vector2.Normalize(v);
}