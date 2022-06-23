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
renderer.Background = Color.FromArgb(0, 211, 160, 104);

const float scale_factor = 3;
var player = new Character(name: "Player", scale: Vector2.One / scale_factor, position: new(-45, 0), spriteFile: "Assets/T_Player.png".Find()).AddTo(world).AddTo(renderer);
var pile = new Character(name: "Pile", scale: Vector2.One / scale_factor, position: new(30, -21), spriteFile: "Assets/T_PileOfCoal.png".Find()).AddTo(world).AddTo(renderer);
var trash = new Character(name: "Trash", scale: Vector2.One / scale_factor, position: new(-300, -3), sortingOrder: 3, spriteFile: "Assets/T_Trash.png".Find()).AddTo(world).AddTo(renderer);
var furnace = new Character(name: "Furnace", scale: Vector2.One / scale_factor, position: new(330, -3), sortingOrder: 3, spriteFile: "Assets/T_Furnace.png".Find()).AddTo(world).AddTo(renderer);
var item = new Character(name: "Item", scale: Vector2.One / scale_factor, position: pile.Position + new Vector2(0, 40), sortingOrder: 2).AddTo(world).AddTo(renderer);
var money = new Sprite("Assets/T_Money.png".Find());
var coal = new Sprite("Assets/T_Coal.png".Find());
var banana = new Sprite("Assets/T_Banana.png".Find());

var isItem = false;
var isSwinging = false;
var onSwing = () => { };
new Character
(
    name: "Shovel", 
    scale: Vector2.One / scale_factor,
    sortingOrder: 1, 
    spriteFile: "Assets/T_Shovel.png".Find(),
    update: (me, dt) =>
    {
        if (!isSwinging && !isItem && input.Axis().X != 0 || input.Axis().Y != 0)
        {
            isSwinging = true;
            onSwing();
        }
        else
        {
            me.Position += Vector2.One * 20f * dt;
            me.Rotation += 90 * dt;
        }
    }
).AddTo(world).AddTo(renderer);

window.Run();


