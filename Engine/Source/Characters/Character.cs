using static System.Numerics.Matrix4x4;

namespace Engine;

public sealed class Character : IDisposable
{
    internal readonly Action? Start;
    internal readonly Action? Stop;
    internal readonly Action<float>? Update;

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

    public Character(Action? start = null, Action? stop = null, Action<float>? update = null, Sprite? sprite = null)
    {
        Start = start;
        Stop = stop;
        Update = update;
        Sprite = sprite;
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