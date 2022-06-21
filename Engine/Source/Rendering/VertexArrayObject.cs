using Silk.NET.OpenGL;

namespace Engine;

sealed class VertexArrayObject<TVertex, TIndex> : IDisposable 
    where TVertex : unmanaged 
    where TIndex : unmanaged
{
    readonly uint _handle;

    public VertexArrayObject(BufferObject<TVertex> vbo, BufferObject<TIndex> ebo)
    {
        _handle = Window.Graphics.GenVertexArray();
        Bind();
        vbo.Bind();
        ebo.Bind();
    }
    
    public unsafe void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, uint vertexSize, int offset)
    {
        Window.Graphics.VertexAttribPointer(index, count, type, false, vertexSize * (uint) sizeof(TVertex), (void*) (offset * sizeof(TVertex)));
        Window.Graphics.EnableVertexAttribArray(index);
    }
    
    public void Bind() => Window.Graphics.BindVertexArray(_handle);
    public void Dispose() => Window.Graphics.DeleteVertexArray(_handle);
}