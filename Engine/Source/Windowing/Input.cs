using static Engine.Key;

namespace Engine;

public sealed class Input
{
    readonly Dictionary<Key, KeyDown> _keys = new();
    readonly IEnumerable<Key> _all = Enum.GetValues<Key>();

    public Input(Window window)
    {
        foreach (var key in _all)
            if (key != Any && key != Unknown)
                _keys.Add(key, KeyDown.Up);
        window.OnKeyPress += Press;
        window.OnKeyRelease += Release;
        window.OnLateUpdate += Update;
    }

    public Vector2 Axis()
    {
        var x = 0f;
        var y = 0f;
        if (GetKey(D) || GetKey(RightArrow))
            x = 1f;
        else if (GetKey(A) || GetKey(LeftArrow))
            x = -1f;
        if (GetKey(W) || GetKey(UpArrow))
            y = 1f;
        else if (GetKey(S) || GetKey(DownArrow))
            y = -1f;
        return new(x, y);
    }
    
    public bool GetKey(Key key)
    {
        if (key == Unknown) return false;
        if (key == Any)
        {
            var keys = _keys.Values;
            if (keys.Any(state => state is KeyDown.Down or KeyDown.Pressed))
                return true;
        }
        else
        {
            return _keys[key] == KeyDown.Down || GetKeyDown(key);
        }
        return false;
    }
    
    public bool GetKeyUp(Key key)
    {
        if (key == Unknown) return false;
        if (key == Any)
        {
            var keys = _keys.Values;
            if (keys.Any(state => state == KeyDown.Released))
                return true;
        }
        else
        {
            return _keys[key] == KeyDown.Released;
        }
        return false;
    }
    
    public bool GetKeyDown(Key key)
    {
        if (key == Unknown) return false;
        if (key == Any)
        {
            var keys = _keys.Values;
            if (keys.Any(state => state == KeyDown.Pressed))
                return true;
        }
        else
        {
            return _keys[key] == KeyDown.Pressed;
        }
        return false;
    }

    void Press(Key key)
    {
        if (key == Unknown) return;
        if (_keys[key] != KeyDown.Pressed && _keys[key] != KeyDown.Down) 
            _keys[key] = KeyDown.Pressed;
    }

    void Release(Key key)
    {
        if (key == Unknown) return;
        if (_keys[key] != KeyDown.Released && _keys[key] != KeyDown.Up) 
            _keys[key] = KeyDown.Released;
    }
    
    void Update(float dt)
    {
        foreach (var (key, state) in _keys)
        {
            _keys[key] = state switch
            {
                KeyDown.Pressed => KeyDown.Down,
                KeyDown.Released => KeyDown.Up,
                _ => _keys[key]
            };
        }
    }
}