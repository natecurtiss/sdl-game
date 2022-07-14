namespace Game;

static class Mappings
{
    static readonly Sprite _money = new("Assets/T_Money.png".Find());
    static readonly Sprite _coal = new("Assets/T_Coal.png".Find());
    static readonly Sprite _banana = new("Assets/T_Banana.png".Find());
    static readonly Sprite[] _items = {_money, _coal, _banana};
    static readonly Random _random = new();
    
    public static bool IsValid(Item item, Direction dir) => item switch
    {
        Money => dir == Down,
        Coal => dir == Right,
        Banana => dir == Left,
        _ => false
    };

    public static Sprite Sprite(Item item) => _items[(int) item];
    public static Item Random() => (Item) _random.Next(0, 3);
}