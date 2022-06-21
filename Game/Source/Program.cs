var window = new Window("Test");
var renderer = new Renderer(window);
var input = new Input(window);
var world = new World(window);
window.OnStop += () =>
{
    window.Dispose();
    renderer.Dispose();
    world.Dispose();
};

new Character("Player", scale: Vector2.One / 5000, sprite: Sprites.PolyMars, update: (me, dt) =>
{
    if (input.GetKeyDown(Key.A))
        Console.WriteLine("A");
}).AddTo(world).AddTo(renderer);

window.Run();


