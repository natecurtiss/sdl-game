using static System.AppDomain;
using static System.IO.Path;

namespace Engine;

public static class PathExt
{
    public static string Find(this string path)
    {
        var dir = CurrentDomain.BaseDirectory;            
        var file = Combine(dir, path);
        var full = GetFullPath(file);
        Console.WriteLine(File.Exists(full) ? $"{full} loaded!" : $"{full} NOT LOADED!");
        return full;
    }
}