namespace Engine;

public sealed class World : IDisposable
{
    readonly List<Character> _characters = new();

    public World(Window window) => window.OnUpdate += Update;

    internal void Add(Character character)
    {
        _characters.Add(character);
        character.Start?.Invoke(character);
    }
    
    internal void Remove(Character character)
    {
        _characters.Add(character);
        character.Stop?.Invoke(character);
    }
    
    void Update(float dt) => _characters.ToList().ForEach(c => c.Update?.Invoke(c, dt));
    
    public void Dispose() => _characters.ToList().ForEach(c => c.Dispose());
}