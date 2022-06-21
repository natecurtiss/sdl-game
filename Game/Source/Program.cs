var window = new Window("Test");
var renderer = new Renderer(window);
var world = new World(window);
window.OnStop += () =>
{
    window.Dispose();
    renderer.Dispose();
    world.Dispose();
};

var player = new Character(sprite: Sprites.PolyMars).AddTo(world).AddTo(renderer);
player.Scale = Vector2.One / 100;

window.Run();


