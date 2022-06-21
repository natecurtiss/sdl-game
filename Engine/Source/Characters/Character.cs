using static System.Numerics.Matrix4x4;

namespace Engine;

public sealed class Character : IDisposable
{
    internal readonly Action<Character>? Start;
    internal readonly Action<Character>? Stop;
    internal readonly Action<Character, float>? Update;

    public Vector2 Position;
    public float Rotation;
    public Vector2 Scale = Vector2.One;
    public int SortingOrder;
    public Sprite? Sprite;

    public Matrix4x4 Model
    {
        get
        {
            var size = new Vector2(Sprite is null ? 1 : Sprite.Size.X, Sprite is null ? 1 : Sprite.Size.Y);
            var scale = CreateScale(Scale.X * size.X, Scale.Y * size.Y, 1);
            var rot = CreateRotationZ(MathF.PI / 180 * Rotation);
            var pos = CreateTranslation(Position.X, Position.Y, 0);
            return scale * rot * pos;
        }
    }

    public Character
    (
        Action<Character>? start = null, 
        Action<Character>? stop = null, 
        Action<Character, float>? update = null, 
        Sprite? sprite = null, 
        Vector2? position = null, 
        float? rotation = null, 
        Vector2? scale = null)
    {
        Start = start;
        Stop = stop;
        Update = update;
        Sprite = sprite;
        Position = position ?? Vector2.Zero;
        Rotation = rotation ?? 0f;
        Scale = scale ?? Vector2.One;
    }

    public Character AddTo(World world)
    {
        world.Add(this);
        return this; 
    }
    
    public Character AddTo(Renderer renderer)
    {
        renderer.Add(this);
        return this; 
    }
    
    public void RemoveFrom(World world) => world.Remove(this);
    public void RemoveFrom(Renderer renderer) => renderer.Remove(this);
    
    public void Dispose() => Sprite?.Dispose();
}