using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace Advent2025.Solutions;

public class Day06: IDay
{
    [Benchmark, Arguments(false)]
    public long Part1(bool sample = false)
    {
        List<List<long>> numbers = [];

        using var lines = Util.InputLines<Day06>(sample).GetEnumerator();
        
        // First line
        lines.MoveNext();
        var line = lines.Current.AsSpan();
        
        foreach (var range in line.Split(' '))
        {
            if (range.Start.Value == range.End.Value) continue;
            numbers.Add([long.Parse(line[range])]);
        }
        
        var lastLine = false;
        MemoryExtensions.SpanSplitEnumerator<char> ranges = default;
        while (!lastLine && lines.MoveNext())
        {
            var i = 0;
            line = lines.Current.AsSpan();

            ranges = line.Split(' ');
            while (ranges.MoveNext())
            {
                var range = ranges.Current;
                if (range.Start.Value == range.End.Value) continue;
                if (!long.TryParse(line[range], out var n))
                {
                    lastLine = true;
                    break;
                }
                
                numbers[i++].Add(n);
            }
        }

        List<bool> isAddition = [];
        do
        {
            var range = ranges.Current;
            if (range.Start.Value == range.End.Value) continue;

            isAddition.Add(ranges.Source[range.Start] == '+');
        } while (ranges.MoveNext());

        return isAddition.AsParallel().Zip(numbers).Sum(zip =>
        {
            return zip.First ? zip.Second.Sum() : Enumerable.Aggregate(zip.Second, (a, b) => a * b);
        });
        
    }

    [Benchmark, Arguments(false)]
    public long Part2(bool sample = false)
    {
        var sheet = Util.InputLines<Day06>(sample).ToList();

        var numLen = sheet.Count-1;
        var cols = sheet[0].Length;

        var total = 0L;
        var num = new char[numLen];
        List<long> numCache = [];
        for (var col = cols-1; col >= 0; col--)
        {
            for (var row = 0; row < numLen; row++)
            {
                var c = sheet[row][col];
                
                num[row] = c;
            }
    
            // Fails to parse when empty
            if (int.TryParse(num, out var n))
            {
                numCache.Add(n);
                continue;
            }
            
            // end of numbers -> do operation
            var add = sheet[numLen][col+1] == '+';
            
            total += add ? numCache.Sum() : numCache.Aggregate<long>((a, b) =>a * b);
            numCache.Clear();

        }
        
        // repeat for 'last' operation
        total += sheet[numLen][0] == '+' ? numCache.Sum() : numCache.Aggregate((a, b) =>a * b);
        return total;

    }
}