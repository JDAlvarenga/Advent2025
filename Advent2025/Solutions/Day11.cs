using BenchmarkDotNet.Attributes;

namespace Advent2025.Solutions;

public class Day11: IDay
{
    [Benchmark, Arguments(false)]
    public long Part1(bool sample = false)
    {
        var connections = GetConnections(sample, true);
        if (!connections.ContainsKey("out")) connections.Add("out", []);
    
        Dictionary<string, long> cache = new() { {"out", 1} };
        
        return CountPaths("you", connections, ref cache);
    }
    
    
    [Benchmark, Arguments(false)]
    public long Part2(bool sample = false)
    {
        var connections = GetConnections(sample, false);
        if (!connections.ContainsKey("out")) connections.Add("out", []);
        
        Dictionary<string, long> fftCache = new() { {"fft", 1} };
        Dictionary<string, long> dacCache = new() { {"dac", 1} };
        Dictionary<string, long> outCache = new() { {"out", 1} };
        
        var dacFft = CountPaths("dac", connections, ref fftCache);
        if (dacFft == 0)
        {
            var svrFft = CountPaths("svr", connections, ref fftCache);
            var fftDac = CountPaths("fft", connections, ref dacCache);
            var dacOut = CountPaths("dac", connections, ref outCache);
            
            return svrFft * fftDac * dacOut;
            
        }
        
        
        var svrDac = CountPaths("svr", connections, ref dacCache);
        var fftOut = CountPaths("fft", connections, ref outCache);
        
        return svrDac * dacFft * fftOut;
        
    }

    private static Dictionary<string, List<string>> GetConnections(bool sample, bool part1)
    {
        Dictionary<string, List<string>> connections = [];
        foreach (ReadOnlySpan<char> line in Util.InputLines<Day11>(sample, sample ? part1 : null))
        {
            var id = line[..3].ToString();
            var outputSpan = line[5..];

            List<string> outputs = new();
            foreach (var o in outputSpan.Split(' '))
            {
                outputs.Add(outputSpan[o].ToString());
            }
            connections.Add(id, outputs);
        }

        return connections;
    }

    private static long CountPaths(in string id, in Dictionary<string, List<string>> connections, ref Dictionary<string, long> cache)
    {
        if (cache.TryGetValue(id, out var count)) return count;
        

        count = 0;
        foreach (var c in connections[id])
        {
            count += CountPaths(c, connections, ref cache);
        }
        
        cache.Add(id, count);
        return count;
    }
}