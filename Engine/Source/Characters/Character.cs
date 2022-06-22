using static System.Numerics.Matrix4x4;

namespace Engine;

public sealed class Character : IDisposable
{
    internal readonly Action<Character>? Start;
    internal readonly Action<Character>? Stop;
    internal readonly Action<Character, float>? Update;

    public string Name { get; set; }
    public string? Tag { get; set; }
    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public Vector2 Scale { get; set; }
    public int SortingOrder { get; set; }
    public Sprite? Sprite { get; set; }
    public AudioSource AudioSource { get; } = new();

    public Bounds Bounds => new(Position, Scale * (Sprite?.Size ?? Vector2.One));
    public Matrix4x4 Model
    {
        get
        {
            var size = new Vector2(Sprite is null ? 1 : Sprite.Size.X, Sprite is null ? 1 : Sprite.Size.Y) / (Window.Size / 2f);
            var scale = CreateScale(Scale.X * size.X, Scale.Y * size.Y, 1);
            var rot = CreateRotationZ(MathF.PI / 180 * Rotation);
            var pos = CreateTranslation(Position.X / (Window.Size.X / 2f), Position.Y / (Window.Size.Y / 2f), 0);
            return scale * rot * pos;
        }
    }

    public Character
    (
        string name,
        string? tag = null,
        Action<Character>? start = null, 
        Action<Character>? stop = null, 
        Action<Character, float>? update = null, 
        string? spriteFile = null,
        string? audioFile = null,
        Vector2? position = null, 
        float? rotation = null, 
        Vector2? scale = null)
    {
        Name = name;
        Tag = tag;
        Start = start;
        Stop = stop;
        Update = update;
        Sprite = spriteFile is null ? null : new(spriteFile);
        AudioSource.File = audioFile;
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
    
    public void Dispose()
    {
        Sprite?.Dispose();
        AudioSource.Dispose();
    }
}