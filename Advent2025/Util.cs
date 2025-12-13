using System.Numerics;

namespace Advent2025;

public static class Util
{
    public static StreamReader GetInputStream<T>(bool sample = false, bool? part1 = null)
    {
        var part = part1 switch
        {
            null => null,
            true => "_1",
            false => "_2"
        };
        var path = Path.Combine(
            "input",
            sample ? "samples" : string.Empty,
            $"{typeof(T).Name.ToLower()}{part}"
        );
        return new StreamReader(path);
    }

    public static IEnumerable<string> InputLines<T>(bool sample = false, bool? part1 = null)
    {
        using var file = GetInputStream<T>(sample, part1);
        while (!file.EndOfStream)
        {
            yield return file.ReadLine()!;
        }
    }
    
    public const char Newline = '\n';
    public const char EndOfFile = '\0';
    public static IEnumerable<char> InputChars<T>(bool sample)
    {
        using var file = Util.GetInputStream<T>(sample);
        
        
        while (!file.EndOfStream)
        {
            var ch = (char)file.Read();
            if (ch is '\r' or '\n')
            {
                if (file.Peek() is '\n' or '\r') file.Read();
                
                yield return Newline;
            }
            else
                yield return ch;
        }
        yield return EndOfFile;
    }
    
    public static class Directions
    {
        public static readonly Complex Up     = new Complex(0, 1);
        public static readonly Complex Right  = new Complex(1, 0);
        public static readonly Complex Down   = new Complex(0, -1);
        public static readonly Complex Left   = new Complex(-1, 0);
        public static readonly Complex UpRight    = Up + Right;
        public static readonly Complex DownRight  = Down + Right;
        public static readonly Complex DownLeft   = Down + Left;
        public static readonly Complex UpLeft     = Up + Left;
        
        public static readonly Complex[] All8 = [Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft]; 
        public static readonly Complex[] All4 = [Up, Right, Down, Left];
    }
}