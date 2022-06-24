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

const float scale_factor = 2;
var player = new Character(name: "Player", scale: Vector2.One / scale_factor, position: new(-90, 0), sortingOrder: 3, spriteFile: "Assets/T_Player.png".Find()).AddTo(world).AddTo(renderer);
var pile = new Character(name: "Pile", scale: Vector2.One / scale_factor, position: new(45, -32), spriteFile: "Assets/T_PileOfCoal.png".Find()).AddTo(world).AddTo(renderer);
var trash = new Character(name: "Trash", scale: Vector2.One / scale_factor, position: new(-300, -3), sortingOrder: 5, spriteFile: "Assets/T_Trash.png".Find()).AddTo(world).AddTo(renderer);
var furnace = new Character(name: "Furnace", scale: Vector2.One / scale_factor, position: new(340, -3), sortingOrder: 5, spriteFile: "Assets/T_Furnace.png".Find()).AddTo(world).AddTo(renderer);
var barPos = new Vector2(-250, 250);
var barFillPos = barPos +new Vector2(0.5f, -0f);
var barScale = 0.75f;
var barFillAmt = 1.02f * barScale;
var barFG = new Character(name: "Bar_FG", scale: Vector2.One * barScale, position: barPos, sortingOrder: 99, spriteFile: "Assets/T_Bar_FG.png".Find()).AddTo(world).AddTo(renderer);
var barFill = new Character(name: "Bar_Fill", scale: new Vector2(barFillAmt, barFillAmt), position: barFillPos, sortingOrder: 98, spriteFile: "Assets/T_Bar_Fill.png".Find()).AddTo(world).AddTo(renderer);
var barBG = new Character(name: "Bar_BG", scale: new Vector2(barFillAmt, barFillAmt) * 0.98f, position: barPos, sortingOrder: 97, spriteFile: "Assets/T_Bar_BG.png".Find()).AddTo(world).AddTo(renderer);

Character spawned = null!;
var currentItem = Mappings.Random();
var isSorting = false;
var hasLost = false;
var timeLeft = 5f;
var timer = timeLeft;

new Character
(
    name: "Shovel",
    scale: Vector2.One / scale_factor,
    sortingOrder: 1,
    rotation: -75f,
    position: new(-30, 0),
    spriteFile: "Assets/T_Shovel.png".Find(),
    start: _ => Spawn(),
    update: (me, dt) =>
    {
        if (hasLost)
            return;
        timer -= dt;
        barFill.Scale = new(timer / timeLeft * barFillAmt, barFillAmt);
        barFill.Position = barFillPos - new Vector2(barFill.Sprite!.Size.X * barFillAmt / 2, 0) + timer / timeLeft * new Vector2(barFill.Sprite!.Size.X * barFillAmt / 2, 0);
        if (timer <= 0)
        {
            Lose();
            barFill.RemoveFrom(renderer);
        }
        if (isSorting)
            return;
        var dir = None;
        if (input.GetKeyDown(Key.D) || input.GetKeyDown(Key.RightArrow))
            dir = Right;
        else if (input.GetKeyDown(Key.LeftArrow) || input.GetKeyDown(Key.A))
            dir = Left;
        else if (input.GetKeyDown(Key.S) || input.GetKeyDown(Key.DownArrow))
            dir = Down;
        else if (input.GetKey(Key.D) || input.GetKey(Key.RightArrow))
            dir = Right;
        else if (input.GetKey(Key.LeftArrow) || input.GetKey(Key.A))
            dir = Left;
        else if (input.GetKey(Key.S) || input.GetKey(Key.DownArrow))
            dir = Down;
        if (dir == None)
            return;
        var ease = Ease.QuadInOut;
        isSorting = true;
        tweener.Tween(me, new {X = 30, Y = 30}, 0.2f).Ease(ease);
        tweener.Tween(me, new {Rotation = -110f}, 0.1f).Ease(ease);
        tweener.Timer(0.1f).OnComplete(() => tweener.Tween(me, new {Rotation = -45f}, 0.1f).Ease(ease));
        tweener.Timer(0.2f).OnComplete(() =>
        {
            tweener.Tween(me, new {Rotation = -75f}, 0.1f).Ease(ease);
            tweener.Tween(me, new {X = -30, Y = 0f}, 0.1f).Ease(ease);
        });
        tweener.Timer(0.3f).OnComplete(() => isSorting = false);
        Sort(dir);
    }
).AddTo(world).AddTo(renderer);

void Spawn()
{
    currentItem = Mappings.Random();
    spawned = new Character("Spawned", scale: Vector2.One / scale_factor, position: pile.Position + new Vector2(0, 20), sortingOrder: 0).AddTo(world).AddTo(renderer);
    spawned.Sprite = Mappings.Sprite(currentItem);
    tweener.Tween(spawned, new {Y = pile.Position.Y + 50}, 0.5f).Ease(Ease.QuadInOut);
}

void Sort(Direction dir)
{
    timeLeft *= 0.99f;
    timer = timeLeft;
    spawned.RemoveFrom(renderer);
    spawned.RemoveFrom(world);
    var item = currentItem;
    var sorted = new Character("Sorted", start: me =>
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
        tweener.Tween(me, new {target.X}, duration)
            .Ease(Ease.QuadInOut)
            .OnComplete(() =>
            {
                if (!hasLost)
                {
                    if (Mappings.IsValid(item, dir))
                    {
                        Score();
                    }
                    else
                    {
                        Lose();
                    }
                }
                me.RemoveFrom(renderer);
                me.RemoveFrom(world);
            });
        tweener.Tween(me, new {Y = target.Y + 50}, duration / 2f).Ease(Ease.SineInOut);
        tweener.Timer(0.25f).OnComplete(() => tweener.Tween(me, new {target.Y}, duration / 2f).Ease(Ease.SineInOut));
    }, scale: Vector2.One / scale_factor, position: pile.Position + new Vector2(0, 50)).AddTo(world).AddTo(renderer);
    sorted.Sprite = Mappings.Sprite(item);
    sorted.SortingOrder = dir == Down ? 2 : 4;
    Spawn();
}

void Score()
{
    
}

void Lose()
{
    hasLost = true;
}

window.Run();


