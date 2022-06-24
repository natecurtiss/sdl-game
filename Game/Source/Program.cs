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

var startMenu = new Character("Start Menu", sortingOrder: 100, spriteFile: "Assets/T_StartMenu.png".Find()).AddTo(renderer).AddTo(world);
var loseMenu = new Character("Lose Menu", sortingOrder: 101, position: new(0, 600), spriteFile: "Assets/T_LoseMenu.png".Find()).AddTo(world);

var advanceSound = new Character(name: "Advance Sound", audioFile: "Assets/S_Advance.mp3".Find()).AddTo(world);
var shovelSound = new Character(name: "Shovel Sound", audioFile: "Assets/S_Shovel.mp3".Find()).AddTo(world);
var leftMoveSound = new Character(name: "Valid Move Sound", audioFile: "Assets/S_ValidMove.mp3".Find()).AddTo(world);
var rightMoveSound = new Character(name: "Valid Move Sound", audioFile: "Assets/S_ValidMove.mp3".Find()).AddTo(world);
var downMoveSound = new Character(name: "Valid Move Sound", audioFile: "Assets/S_ValidMove.mp3".Find()).AddTo(world);
var invalidMoveSound = new Character(name: "Invalid Move Sound", audioFile: "Assets/S_InvalidMove.mp3".Find()).AddTo(world);
var timeOutSound = new Character(name: "Time Out Sound", audioFile: "Assets/S_TimeOut.mp3".Find()).AddTo(world);
var trainSound = new Character(name: "Train Sound", audioFile: "Assets/S_Advance.mp3".Find()).AddTo(world);

const float scale_factor = 2;
new Character(name: "Floor", scale: Vector2.One, position: new(0, -215), sortingOrder: -5, spriteFile: "Assets/T_Floor.png".Find()).AddTo(world).AddTo(renderer);
new Character(name: "Windows", scale: Vector2.One *0.8f, position: new(0, 60), sortingOrder: -5, spriteFile: "Assets/T_Windows.png".Find()).AddTo(world).AddTo(renderer);
new Character(name: "Windows", scale: Vector2.One *0.8f, position: new(250, 60), sortingOrder: -5, spriteFile: "Assets/T_Windows.png".Find()).AddTo(world).AddTo(renderer);
new Character(name: "Windows", scale: Vector2.One *0.8f, position: new(-250, 60), sortingOrder: -5, spriteFile: "Assets/T_Windows.png".Find()).AddTo(world).AddTo(renderer);
var player = new Character(name: "Player", scale: Vector2.One / scale_factor, position: new(-90, -30), sortingOrder: 3, spriteFile: "Assets/T_Player.png".Find()).AddTo(world).AddTo(renderer);
var pile = new Character(name: "Pile", scale: Vector2.One / scale_factor, position: new(45, -62), spriteFile: "Assets/T_PileOfCoal.png".Find()).AddTo(world).AddTo(renderer);
var trash = new Character(name: "Trash", scale: Vector2.One / scale_factor, position: new(-300, -33), sortingOrder: 5, spriteFile: "Assets/T_Trash.png".Find()).AddTo(world).AddTo(renderer);
var furnace = new Character(name: "Furnace", scale: Vector2.One / scale_factor, position: new(340, -33), sortingOrder: 5, spriteFile: "Assets/T_Furnace.png".Find()).AddTo(world).AddTo(renderer);
var barPos = new Vector2(-250, 200);
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
var hasStarted = false;
var timeLeft = 4f;
var timer = timeLeft;

new Character
(
    name: "Shovel",
    scale: Vector2.One / scale_factor,
    sortingOrder: 1,
    rotation: -75f,
    position: new(-30, -30),
    spriteFile: "Assets/T_Shovel.png".Find(),
    update: (me, dt) =>
    {
        if (!hasStarted)
        {
            if (input.GetKeyDown(Key.Space))
            {
                advanceSound.AudioSource.Play();
                hasStarted = true;
                tweener.Tween(startMenu, new {Y = 600}, 0.25f).Ease(Ease.QuadInOut).OnComplete(() =>
                {
                    startMenu.RemoveFrom(renderer);
                    startMenu.RemoveFrom(world);
                });
                Spawn();
            }
            return;
        }

        if (hasLost)
        {
            if (input.GetKeyDown(Key.Space))
            {
                advanceSound.AudioSource.Play();
                tweener.Tween(loseMenu, new {Y = 600}, 0.25f).Ease(Ease.QuadInOut).OnComplete(() => loseMenu.RemoveFrom(renderer));
                hasLost = false;
                timeLeft = 4f;
                timer = timeLeft;
                isSorting = false;
                barFill.Scale = new(barFillAmt, barFillAmt);
                currentItem = Mappings.Random();
                spawned.RemoveFrom(renderer);
                spawned.RemoveFrom(world);
                spawned = null!;
                Spawn();
            }
            return;
        }
        timer -= dt;
        barFill.Scale = new(timer / timeLeft * barFillAmt, barFillAmt);
        barFill.Position = barFillPos - new Vector2(barFill.Sprite!.Size.X * barFillAmt / 2, 0) + timer / timeLeft * new Vector2(barFill.Sprite!.Size.X * barFillAmt / 2, 0);
        if (timer <= 0)
        {
            Lose();
            timeOutSound.AudioSource.Play();
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
        shovelSound.AudioSource.Play();
        var ease = Ease.QuadInOut;
        isSorting = true;
        tweener.Tween(me, new {X = 30, Y = 0}, 0.2f).Ease(ease);
        tweener.Tween(me, new {Rotation = -110f}, 0.1f).Ease(ease);
        tweener.Timer(0.1f).OnComplete(() => tweener.Tween(me, new {Rotation = -45f}, 0.1f).Ease(ease));
        tweener.Timer(0.2f).OnComplete(() =>
        {
            tweener.Tween(me, new {Rotation = -75f}, 0.1f).Ease(ease);
            tweener.Tween(me, new {X = -30, Y = -30f}, 0.1f).Ease(ease);
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
    timeLeft *= 0.95f;
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
            Left => 0.5f,
            Right => 0.5f,
            Down => 0.5f,
        };
        var sound = dir switch
        {
            Left => 0.4f,
            Right => 0.3f,
            Down => 0.3f,
        };
        tweener.Tween(me, new {target.X}, duration).Ease(Ease.QuadInOut);
        tweener.Timer(sound).OnComplete(() =>
        {
            if (!hasLost)
            {
                if (Mappings.IsValid(item, dir))
                {
                    Score(dir);
                }
                else
                {
                    invalidMoveSound.AudioSource.Play();
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

void Score(Direction dir)
{
    if (dir == Left)
        leftMoveSound.AudioSource.Play();
    else if (dir == Right)
        rightMoveSound.AudioSource.Play();
    else if (dir == Down)
        downMoveSound.AudioSource.Play();
}

void Lose()
{
    loseMenu.AddTo(renderer);
    tweener.Tween(loseMenu, new {Y = 0}, 0.5f).Ease(Ease.QuadInOut);
    hasLost = true;
}

window.Run();


