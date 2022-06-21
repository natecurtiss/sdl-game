namespace Engine;

public sealed class World : IDisposable
{
    readonly List<Character> _characters = new();

    public World(Window window) => window.OnUpdate += Update;

    internal void Add(Character character)
    {
        _characters.Add(character);
        character.Start?.Invoke();
    }
    
    internal void Remove(Character character)
    {
        _characters.Add(character);
        character.Stop?.Invoke();
    }
    
    void Update(float dt) => _characters.ForEach(c => c.Update?.Invoke(dt));
    
    public void Dispose() => _characters.ToList().ForEach(c => c.Dispose());
}