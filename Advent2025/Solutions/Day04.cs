using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace Advent2025.Solutions;

public class Day04: IDay
{
    
    [Benchmark, Arguments(false)]
    public int Part1(bool sample = false)
    {
        var (scores, rolls) = BuildMap(sample);
        return rolls.Count(roll => scores.GetValueOrDefault(roll, 0) < 4);
    }
    
    [Benchmark, Arguments(false)]
    public int Part2(bool sample = false)
    {
        var (scores, rolls) = BuildMap(sample);
        
        int total = 0, removed = -1;
        while (removed != 0)
        {
            removed = rolls.RemoveWhere(roll =>
            {
                if (scores.GetValueOrDefault(roll, 0) >= 4) return false;
                
                foreach (var dir in Util.Directions.All8)
                    AddValue(scores, roll + dir, -1);
                return true;

            });
            total += removed;
        }

        return total;
    }

    private static (Dictionary<Complex, int> scores, HashSet<Complex> rolls) BuildMap(bool sample)
    {
        var scores = new Dictionary<Complex, int>();
        var rolls = new HashSet<Complex>();
        
        var pos = new Complex(-1,0);
        foreach (var c in Util.InputChars<Day04>(sample))
        {
            pos += 1;
            switch (c)
            {
                case Util.Newline or Util.EndOfFile:
                    pos = new Complex(-1,pos.Imaginary + 1);
                    continue;
                case '.':
                    continue;
                case '@':
                {
                    rolls.Add(pos);
                    foreach (var dir in Util.Directions.All8)
                        AddValue(scores, pos+dir, 1);

                    break;
                }
            }
        }

        return (scores, rolls);
    }
    
    private static void AddValue<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, TValue value) where TValue : INumber<TValue>
    {
        if (!dictionary.TryAdd(key, value))
            dictionary[key] += value;
    }
}