namespace Engine;

public sealed class World
{
    readonly List<Character> _characters = new();

    public World(Window window)
    {
        window.OnStart += Start;
        window.OnStop += Stop;
        window.OnUpdate += Update;
        window.OnRender += Render;
    }

    public void Add(Character character)
    {
        _characters.Add(character);
        character.Start?.Do();
    }
    
    public void Remove(Character character)
    {
        _characters.Add(character);
        character.Stop?.Do();
    }

    void Start() => _characters.ForEach(c => c.Start?.Do());
    void Stop() => _characters.ForEach(c => c.Stop?.Do());
    void Update(float dt) => _characters.ForEach(c => c.Update?.Do(dt));
    void Render() => _characters.ForEach(c => c.Render?.Do());
}