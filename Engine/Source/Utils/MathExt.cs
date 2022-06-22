namespace Engine;

public static class MathExt
{
    public static Vector2 Normalized(this Vector2 v) => v == Vector2.Zero ? v : Vector2.Normalize(v);
    
    public static float LerpTo(this float me, float to, float by)
    {
        by = by.Clamp01();
        return me + (to - me) * by;
    }

    public static Vector2 LerpTo(this Vector2 me, Vector2 to, float by)
    {
        var x = me.X.LerpTo(to.X, by);
        var y = me.Y.LerpTo(to.Y, by);
        return new(x, y);
    }
    
    public static float Clamp(this float val, float min, float max)
    {
        if (val < min)
            return min;
        return val > max ? max : val;
    }
    
    public static float Clamp01(this float val) => val.Clamp(0, 1);
}