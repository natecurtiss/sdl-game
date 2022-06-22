using NAudio.Wave;

namespace Engine;

public sealed class AudioSource : IDisposable
{
    readonly WaveOutEvent _out = new();

    public bool ShouldLoop { get; set; }
    
    public string? File
    {
        get => _file;
        set
        {
            _file = value;
            if (value is not null)
            {
                using var reader = new AudioFileReader(value);
                _out.Init(reader);
            }
        }
    }
    string? _file;

    internal AudioSource()
    {
        _out.PlaybackStopped += (_, _) =>
        {
            File = _file;
            if (ShouldLoop)
                Play();
        };
    }

    public void Play()
    {
        if (_file is not null) 
            _out.Play();
    }

    public void Dispose() => _out.Dispose();
}