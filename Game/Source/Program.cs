var window = new Window("Test");
var renderer = new Renderer(window);
var world = new World(window);
window.OnStop += () =>
{
    window.Dispose();
    renderer.Dispose();
    world.Dispose();
};

var player = new Character(scale: Vector2.One / 5000, sprite: Sprites.PolyMars).AddTo(world).AddTo(renderer);

window.Run();


