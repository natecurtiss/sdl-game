namespace Engine;

public sealed class Sprite : IDisposable
{
    internal bool IsInitialized { get; private set; }
    internal Vector2 Size => Texture?.Size ?? Vector2.One;
    internal Shader? Shader { get; private set; }
    internal Texture? Texture { get; private set; }
    readonly string _path;
    
    readonly string _vertex = @"
#version 330 core
layout (location = 0) in vec3 vPos;
layout (location = 1) in vec2 vUv;

uniform mat4 uModel;
uniform mat4 uProjection;

out vec2 fUv;

void main()
{
    gl_Position = uProjection * uModel * vec4(vPos, 1.0);
    fUv = vUv;
}";
    
    readonly string _fragment = @"
#version 330 core
in vec2 fUv;

uniform sampler2D uTexture0;

out vec4 FragColor;

void main()
{
    if (texture(uTexture0, fUv).a != 1.0f)
    {
        discard;
    }
    FragColor = texture(uTexture0, fUv);
}";
    
    public Sprite(string path) => _path = path;

    internal void Init()
    {
        Shader = new(_vertex, _fragment);
        Texture = new(_path);
        IsInitialized = true;
    }
    
    public void Dispose()
    {
        Shader?.Dispose();
        Texture?.Dispose();
    }
}