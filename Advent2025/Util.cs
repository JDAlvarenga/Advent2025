namespace Advent2025;

public static class Util
{
    public static StreamReader GetInputStream<T>(bool sample = false)
    {
        var path = Path.Combine(
            "input",
            sample ? "samples" : string.Empty,
            $"{typeof(T).Name.ToLower()}"
        );
        return new StreamReader(path);
    }

    public static IEnumerable<string> InputLines<T>(bool sample = false)
    {
        using var file = GetInputStream<T>(sample);
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
}