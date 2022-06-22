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

new Character
(
    name: "Player",
    scale: Vector2.One / 5,
    spriteFile: "Assets/T_PolyMars.jpg".Find(),
    audioFile: "Assets/S_PolyMars.mp3".Find(),
    update: (me, dt) =>
    {
        var axis = input.Axis().Normalized();
        if (me.Bounds.IsAbove(window.Bounds) && axis.Y > 0)
            axis.Y = 0;
        else if (me.Bounds.IsBelow(window.Bounds) && axis.Y < 0)
            axis.Y = 0;
        if (me.Bounds.IsRightOf(window.Bounds) && axis.X > 0)
            axis.X = 0;
        else if (me.Bounds.IsLeftOf(window.Bounds) && axis.X < 0)
            axis.X = 0;
        me.Position += axis * dt * 450;
        if (input.GetKeyDown(Key.Space))
            me.AudioSource.Play();
    }
).AddTo(world).AddTo(renderer);

window.Run();


