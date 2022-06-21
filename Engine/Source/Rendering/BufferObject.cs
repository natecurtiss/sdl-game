using Silk.NET.OpenGL;

namespace Engine;

sealed class BufferObject<T> : IDisposable where T : unmanaged
{
    readonly uint _handle;
    readonly BufferTargetARB _bufferType;
    
    public unsafe BufferObject(Span<T> data, BufferTargetARB bufferType)
    {
        _bufferType = bufferType;
        _handle = Window.Graphics.GenBuffer();
        Bind();
        fixed (void* d = data)
            Window.Graphics.BufferData(bufferType, (nuint) (data.Length * sizeof(T)), d, BufferUsageARB.StaticDraw);
    }
    
    public void Bind() => Window.Graphics.BindBuffer(_bufferType, _handle);
    public void Dispose() => Window.Graphics.DeleteBuffer(_handle);
}