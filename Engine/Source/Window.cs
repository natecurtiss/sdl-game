using Silk.NET.Input;
using Silk.NET.Windowing;

namespace Engine;

public sealed class Window : IDisposable
{
    public event Action OnStart = null!;
    public event Action OnStop = null!;
    public event Action<float> OnUpdate = null!;
    public event Action OnRender = null!;
    public event Action<Key> OnKeyPress = null!;
    public event Action<Key> OnKeyRelease = null!;
    
    readonly IWindow _native;

    public Window(string title)
    {
        var options = WindowOptions.Default;
        {
            options.Size = new(800);
            options.Title = title;
            options.WindowBorder = WindowBorder.Fixed;
        }
        _native = Silk.NET.Windowing.Window.Create(options);
        _native.Load += () =>
        {
            _native.Center();
            var input = _native.CreateInput().Keyboards[0];
            input.KeyDown += (_, key, _) => OnKeyPress((Key) (int) key);
            input.KeyUp += (_, key, _) => OnKeyRelease((Key) (int) key);
            OnStart();
        };
        _native.Closing += OnStop;
        _native.Update += dt => OnUpdate((float) dt);
        _native.Render += _ => OnRender();
    }
    
    public void Dispose() => _native.Dispose();
    public void Open() => _native.Run();
    public void Close() => _native.Close();
}