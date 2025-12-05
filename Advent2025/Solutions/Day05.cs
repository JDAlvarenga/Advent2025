using BenchmarkDotNet.Attributes;

namespace Advent2025.Solutions;

public class Day05: IDay
{
    [Benchmark, Arguments(false)]
    public int Part1(bool sample = false)
    {
        using var file = Util.GetInputStream<Day05>(sample);
        SortedList<long, long> ranges = new();
        
        // Read ranges
        ReadOnlySpan<char> line;
        while (!(line = file.ReadLine()).IsEmpty)
        {
            var dashIdx = line.IndexOf('-');
            
            var from = long.Parse(line[..dashIdx]);
            var to = long.Parse(line[(dashIdx + 1)..]);
            
            // Sort by end values
            // Avoid duplicate keys
            if (!ranges.TryAdd(to, from))
                ranges[to] = long.Min(ranges[to], from);
        }

        var toValues = ranges.Keys.ToArray();
        var fromValues = ranges.Values.ToArray();

        var count = 0;
        // Check ranges
        while (file.EndOfStream == false)
        {
            var id = long.Parse(file.ReadLine().AsSpan());
            
            // All ranges to the left of index are less than id   
            var idx = Array.BinarySearch(toValues, id);
            if (idx < 0) idx = ~idx;
            if (idx == toValues.Length) continue;
            
            // check only the ranges to the right of index
            while (idx < fromValues.Length)
            {
                if (fromValues[idx++] > id) continue;
                count++;
                break;
            }
        }

        return count;
    }
    
    [Benchmark, Arguments(false)]
    public long Part2(bool sample = false)
    {
        using var file = Util.GetInputStream<Day05>(sample);
        SortedSet<(long from, long to)> ranges = new();
        
        // Read ranges
        ReadOnlySpan<char> line;
        while (!(line = file.ReadLine()).IsEmpty)
        {
            var dashIdx = line.IndexOf('-');
            
            var from = long.Parse(line[..dashIdx]);
            var to = long.Parse(line[(dashIdx + 1)..]);
            
            ranges.Add((from, to));
        }

        var count = 0L; 
        
        // Merge ranges
        var iter = ranges.GetEnumerator();
        (long from, long to) last = (-1, -2);
        while(iter.MoveNext())
        {
            var current = iter.Current;
            if (last.to < current.from) // ranges do not overlap
            {
                count += last.to - last.from + 1;
                last = current;
                continue;
            }
            
            last.to = long.Max(last.to, current.to);
        }
        
        count += last.to - last.from + 1;
        

        return count;
    }
}