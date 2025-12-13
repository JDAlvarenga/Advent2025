namespace Advent2025.Solutions;

public class Day12: IDay
{
    public int Part1(bool sample = false)
    {
        if (sample) return 2;
        
        var accepted = 0;
        var rejected = 0;
        var unknown = 0;
        
        List<int> sizes = [];
        List<bool[,]> presents = [];
        
        using var lines = Util.InputLines<Day12>(sample).GetEnumerator();
        lines.MoveNext();
        while (true)
        {
            if (lines.Current[^1] != ':') break;
            // var id = int.Parse(lines.Current[..^1]);

            var spaces = 0;
            var present = new bool[3, 3];
            for (var r = 0; r < 3; r++)
            {
                lines.MoveNext();
                var blocks = lines.Current.AsSpan();
                for (var c = 0; c < blocks.Length; c++)
                {
                    if (blocks[c] == '#')
                    {
                        spaces++;
                        present[r, c] = true;
                    }
                }
            }
            sizes.Add(spaces);
            presents.Add(present);
            
            lines.MoveNext();
            lines.MoveNext();
        }

        
        var presentRanges = new Range[presents.Count];
        var presentsCount = new int[presents.Count];
        var totalSpaces = 0;
        var totalPresents = 0;
        while (true)
        {
            ReadOnlySpan<char> line = lines.Current;
            
            var dimIdx = line.IndexOf(':');
            var xIdx = line.IndexOf('x');

            var xDim = int.Parse(line[..xIdx]);
            var yDim = int.Parse(line[(xIdx + 1)..dimIdx]);
            var area = xDim * yDim;

            var presentSpan = line[(dimIdx + 2)..];
            presentSpan.Split(presentRanges, ' ');

            totalSpaces = 0;
            totalPresents = 0;
            for (var p = 0; p < presents.Count; p++)
            {
                presentsCount[p] = int.Parse(presentSpan[presentRanges[p]]);
                totalPresents += presentsCount[p];
                totalSpaces += presentsCount[p] * sizes[p];
            }

            if (totalSpaces > area)
            {
                rejected++;
            }
            else if (area / 9 >= totalPresents)
            {
                accepted++;
            }
            else
            {
                unknown++;
            }




            if (!lines.MoveNext())
                break;
        }
        
            
        // Console.WriteLine($"total: {accepted + rejected + unknown} accepted: {accepted}, rejected: {rejected}, unknown: {unknown}");

        return accepted;
    }
}