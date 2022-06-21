using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Engine;

public sealed class Window : IDisposable
{
    public event Action? OnStart;
    public event Action? OnStop;
    public event Action<float>? OnUpdate;
    public event Action? OnRender;
    public event Action<Key>? OnKeyPress;
    public event Action<Key>? OnKeyRelease;
    
    readonly IWindow _native;

    internal static GL Graphics { get; private set; } = null!;

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
            Graphics = _native.CreateOpenGL();
            var input = _native.CreateInput().Keyboards[0];
            input.KeyDown += (_, key, _) => OnKeyPress?.Invoke((Key) (int) key);
            input.KeyUp += (_, key, _) => OnKeyRelease?.Invoke((Key) (int) key);
            OnStart?.Invoke();
        };
        _native.Closing += OnStop;
        _native.Update += dt => OnUpdate?.Invoke((float) dt);
        _native.Render += _ => OnRender?.Invoke();
    }
    
    public void Run() => _native.Run();
    public void Dispose() => _native.Dispose();
}