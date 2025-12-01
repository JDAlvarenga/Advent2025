using BenchmarkDotNet.Attributes;

namespace Advent2025.Solutions;


public class Day01: IDay
{
    [Benchmark, Arguments(false)]
    public int Part1(bool sample = false)
    {
        var position = 50;
        var count = 0;
        foreach (var line in Util.InputLines<Day01>(sample))
        {
            var rotation = int.Parse(line[1..]) * (line[0] == 'R' ? 1 : -1) % 100;
            position = (position + 100 + rotation) % 100;
            if (position == 0) count++;
        }

        return count;
    }
    
    [Benchmark, Arguments(false)]
    public int Part2(bool sample = false)
    {
        var position = 50;
        var count = 0;
        foreach (var line in Util.InputLines<Day01>(sample))
        {
            var rotation = int.Parse(line[1..]) * (line[0] == 'R' ? 1 : -1);
            count += int.Abs(rotation / 100);
            
            rotation %= 100;

            var next = position + rotation;
            if (position is not 0 && next <= 0 || next >= 100) count++;
            position = (next + 100) % 100;
        }

        return count;
    }
}