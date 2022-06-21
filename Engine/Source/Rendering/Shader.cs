using Silk.NET.OpenGL;

namespace Engine;

sealed class Shader : IDisposable
{
    readonly uint _shader;
    
    public Shader(string vertexSource, string fragmentSource)
    {
        var vertex = Load(ShaderType.VertexShader, vertexSource);
        var fragment = Load(ShaderType.FragmentShader, fragmentSource);
        _shader = Window.Graphics.CreateProgram();
        Window.Graphics.AttachShader(_shader, vertex);
        Window.Graphics.AttachShader(_shader, fragment);
        Window.Graphics.LinkProgram(_shader);
        Window.Graphics.DetachShader(_shader, vertex);
        Window.Graphics.DetachShader(_shader, fragment);
        Window.Graphics.DeleteShader(vertex);
        Window.Graphics.DeleteShader(fragment);
    }
    
    public void Use() => Window.Graphics.UseProgram(_shader);
    
    public unsafe void SetUniform(string name, Matrix4x4 value)
    {
        var location = Window.Graphics.GetUniformLocation(_shader, name);
        Window.Graphics.UniformMatrix4(location, 1, false, (float*) &value);
    }
    
    public void SetUniform(string name, int value)
    {
        var location = Window.Graphics.GetUniformLocation(_shader, name);
        Window.Graphics.Uniform1(location, value);
    }

    uint Load(ShaderType type, string source)
    {
        var loaded = Window.Graphics.CreateShader(type);
        Window.Graphics.ShaderSource(loaded, source);
        Window.Graphics.CompileShader(loaded);
        return loaded;
    }
    
    public void Dispose() => Window.Graphics.DeleteProgram(_shader);
}