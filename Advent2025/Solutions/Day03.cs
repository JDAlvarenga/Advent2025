using BenchmarkDotNet.Attributes;

namespace Advent2025.Solutions;

public class Day03: IDay
{
    [Benchmark, Arguments(false)]
    public int Part1(bool sample = false)
    {
        var sum = 0;
        char max10 = '0', max1 = '0';
        
        foreach (var c in Util.InputChars<Day03>(sample))
        {
            // Console.WriteLine($"DEBUG {c} = {(int)c}");
            if (c is Util.Newline or Util.EndOfFile)
            {
                // Console.WriteLine($"DEBUG: added{max10}{max1}");
                sum += 10*(max10-'0') + (max1-'0');
                max10 = '0';
                max1 = '0';
                continue;
            }
            
            if (max1 > max10)
            {
                max10 = max1;
                max1 = '0';
            }
            
            if (c > max1)
            {
                max1 = c;
            }
        }
        return sum;
    }
    
    private static readonly long[] Powers = [100_000_000_000, 10_000_000_000, 1_000_000_000, 100_000_000, 10_000_000, 1_000_000, 100_000, 10_000, 1_000, 100, 10, 1];
    [Benchmark, Arguments(false)]
    public long Part2(bool sample = false)
    {
        var sum = 0L;
        
        var joltage = new char[12];
        Array.Fill(joltage, '0');
        
        foreach (var c in Util.InputChars<Day03>(sample))
        {
            if (c is Util.Newline or Util.EndOfFile)
            {
                for (var i = 0; i < joltage.Length; i++)
                    sum += Powers[i]*(joltage[i]-'0');  
                
                Array.Fill(joltage, '0');
                continue;
            }

            for (var i = 0; i < joltage.Length-1; i++)
            {
                if (joltage[i] >= joltage[i + 1]) continue;
                
                joltage[i] = joltage[i + 1];
                joltage[i + 1] = '0';
            }

            if (joltage[^1] < c)
            {
                joltage[^1] = c;
            }
        }
        return sum;
    }
}