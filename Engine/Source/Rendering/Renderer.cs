using System.Drawing;
using Silk.NET.OpenGL;

namespace Engine;

public sealed class Renderer : IDisposable
{
    readonly List<Character> _targets = new();
    readonly float[] _vertices =
    { 
        //X     Y     Z     U   V
        0.5f,  0.5f, 0.0f, 1f, 1f,
        0.5f, -0.5f, 0.0f, 1f, 0f, 
       -0.5f, -0.5f, 0.0f, 0f, 0f,
       -0.5f,  0.5f, 0.0f, 0f, 1f
    };
    readonly uint[] _indices =
    {
        0, 1, 3,
        1, 2, 3
    };

    BufferObject<float> _vbo = null!;
    BufferObject<uint> _ebo = null!;
    VertexArrayObject<float, uint> _vao = null!;
    
    public Color Background { get; set; }

    public Renderer(Window window, Color bg = default)
    {
        window.OnStart += Start;
        window.OnRender += Render;
        Background = bg;
    }

    void Start()
    {
        _vbo = new(_vertices, BufferTargetARB.ArrayBuffer);
        _ebo = new(_indices, BufferTargetARB.ElementArrayBuffer);
        _vao = new(_vbo, _ebo);
        
        _vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5, 0);
        _vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3);
    }

    unsafe void Render()
    {
        Window.Graphics.Clear(ClearBufferMask.ColorBufferBit);
        Window.Graphics.ClearColor(Background);
        _vao.Bind();
        foreach (var target in _targets.OrderBy(t => t.SortingOrder))
        {
            if (target.Sprite is null)
                continue;
            var sprite = target.Sprite;
            if (!sprite.IsInitialized)
                sprite.Init();
            sprite.Shader?.Use();
            sprite.Texture?.Bind();
            sprite.Shader?.SetUniform("uTexture0", 0);
            sprite.Shader?.SetUniform("uModel", target.Model);
            sprite.Shader?.SetUniform("uProjection", Matrix4x4.CreateOrthographic(Window.Size.X, Window.Size.Y, 0.01f, 100f));
            Window.Graphics.DrawElements(PrimitiveType.Triangles, (uint) _indices.Length, DrawElementsType.UnsignedInt, null);
        }
    }

    public void Add(Character character) => _targets.Add(character);
    public void Remove(Character character) => _targets.Remove(character);

    public void Dispose()
    {
        _vbo.Dispose();
        _ebo.Dispose();
        _vao.Dispose();
    }
}