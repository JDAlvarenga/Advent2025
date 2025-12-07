using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace Advent2025.Solutions;

public class Day07: IDay
{
    [Benchmark, Arguments(false)]
    public long Part1(bool sample = false)
    {
        var manifold = Util.InputLines<Day07>(sample).ToList();
        var start = new Complex(manifold[0].IndexOf('S'), 0);

        Dictionary<Complex, long> cache = new();
        
        QSplits(manifold, ref cache, start);
        
        return cache.Count;
    }
    
    [Benchmark, Arguments(false)]
    public long Part2(bool sample = false)
    {
        var manifold = Util.InputLines<Day07>(sample).ToList();
        var start = new Complex(manifold[0].IndexOf('S'), 0);

        Dictionary<Complex, long> cache = new();
        
        return QSplits(manifold, ref cache, start);
    }

    private static long QSplits(in List<string> manifold, ref Dictionary<Complex, long> cache, Complex beam)
    {
        while (beam.Imaginary < manifold.Count)
        {
            if (manifold[(int)beam.Imaginary][(int)beam.Real] != '^')
            {
                beam += Util.Directions.Up; // increase in y (line number)
                continue;
            }
            // beam hits splitter...
            if (cache.TryGetValue(beam, out var splits)) return splits;
            
            splits = QSplits(manifold, ref cache, beam+Util.Directions.Left) + QSplits(manifold, ref cache, beam+Util.Directions.Right);
             
            cache.Add(beam, splits);
            return splits;

        }
        return 1;
    }
}