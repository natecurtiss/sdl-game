using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;
using PixelFormat = Silk.NET.OpenGL.PixelFormat;

namespace Engine;

sealed class Texture : IDisposable
{
    public readonly Vector2 Size;
    readonly uint _handle;
    
    public unsafe Texture(string path)
    {
        using var image = new Bitmap(path);
        image.RotateFlip(RotateFlipType.RotateNoneFlipY);

        _handle = Window.Graphics.GenTexture();
        Bind();

        Size = new(image.Width, image.Height);
        var rect = new Rectangle(0, 0, image.Width, image.Height);
        var data = image.LockBits(rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        var length = data.Stride * data.Height;
        var bytes = new byte[length];
        
        Marshal.Copy(data.Scan0, bytes, 0, length);
        image.UnlockBits(data);

        fixed (void* pixels = &bytes[0])
        {
            Window.Graphics.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint) image.Width, (uint) image.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, pixels);
            Window.Graphics.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) GLEnum.Repeat);
            Window.Graphics.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) GLEnum.Repeat);
            Window.Graphics.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.Linear);
            Window.Graphics.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.Linear);
            Window.Graphics.GenerateMipmap(TextureTarget.Texture2D);
        }
    }
    
    public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
    {
        Window.Graphics.ActiveTexture(textureSlot);
        Window.Graphics.BindTexture(TextureTarget.Texture2D, _handle);
    }
    public void Dispose() => Window.Graphics.DeleteTexture(_handle);
}