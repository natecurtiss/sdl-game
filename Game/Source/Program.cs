var window = new Window("Test");
var renderer = new Renderer(window);
var input = new Input(window);
var world = new World(window);
var tweener = new Tweener();

window.OnStop += () =>
{
    window.Dispose();
    renderer.Dispose();
    world.Dispose();
};
window.OnUpdate += tweener.Update;
renderer.Background = Color.FromArgb(0, 211, 160, 104);

const float scale_factor = 3;
var player = new Character(name: "Player", scale: Vector2.One / scale_factor, position: new(-45, 0), spriteFile: "Assets/T_Player.png".Find()).AddTo(world).AddTo(renderer);
var pile = new Character(name: "Pile", scale: Vector2.One / scale_factor, position: new(30, -21), spriteFile: "Assets/T_PileOfCoal.png".Find()).AddTo(world).AddTo(renderer);
var trash = new Character(name: "Trash", scale: Vector2.One / scale_factor, position: new(-300, -3), sortingOrder: 3, spriteFile: "Assets/T_Trash.png".Find()).AddTo(world).AddTo(renderer);
var furnace = new Character(name: "Furnace", scale: Vector2.One / scale_factor, position: new(330, -3), sortingOrder: 3, spriteFile: "Assets/T_Furnace.png".Find()).AddTo(world).AddTo(renderer);

var currentItem = Mappings.Random();
var isSorting = false;
new Character
(
    name: "Shovel",
    scale: Vector2.One / scale_factor,
    sortingOrder: 1,
    rotation: -75f,
    spriteFile: "Assets/T_Shovel.png".Find(),
    update: (me, _) =>
    {
        if (isSorting)
            return;
        var dir = None;
        if (input.GetKeyDown(Key.D) || input.GetKeyDown(Key.RightArrow))
            dir = Right;
        else if (input.GetKeyDown(Key.LeftArrow) || input.GetKeyDown(Key.A))
            dir = Left;
        else if (input.GetKeyDown(Key.S) || input.GetKeyDown(Key.DownArrow))
            dir = Down;
        if (dir == None)
            return;
        var ease = Ease.QuadInOut;
        isSorting = true;
        tweener.Tween(me, new {X = 30, Y = 30}, 0.4f).Ease(ease);
        tweener.Tween(me, new {Rotation = -110f}, 0.1f).Ease(ease);
        tweener.Timer(0.1f).OnComplete(() => tweener.Tween(me, new {Rotation = -45f}, 0.3f).Ease(ease));
        tweener.Timer(0.4f).OnComplete(() =>
        {
            tweener.Tween(me, new {Rotation = -75f}, 0.3f).Ease(ease);
            tweener.Tween(me, new {X = 0, Y = 0f}, 0.3f).Ease(ease);
        });
        tweener.Timer(0.7f).OnComplete(() => isSorting = false);
        Sort(dir);
    }
).AddTo(world).AddTo(renderer);

void Sort(Direction dir)
{
    var item = new Character("Item Sorting", start: me =>
    {
        var target = dir switch
        {
            Left => trash.Position,
            Right => furnace.Position,
            Down => player.Position,
        };
        var duration = dir switch
        {
            Left => 0.7f,
            Right => 0.6f,
            Down => 0.5f,
        };
        tweener.Tween(me, new {X = target.X}, duration)
            .Ease(Ease.QuadInOut)
            .OnComplete(() =>
            {
                me.RemoveFrom(renderer);
                me.RemoveFrom(world);
            });
        tweener.Tween(me, new {Y = target.Y + 50}, duration / 2f).Ease(Ease.SineInOut);
        tweener.Timer(0.25f).OnComplete(() => tweener.Tween(me, new {Y = target.Y}, duration / 2f).Ease(Ease.SineInOut));
    }, scale: Vector2.One / scale_factor, position: pile.Position + new Vector2(0, 20), sortingOrder: 2).AddTo(world).AddTo(renderer);
    item.Sprite = Mappings.Sprite(currentItem);
    currentItem = Mappings.Random();
}


window.Run();


