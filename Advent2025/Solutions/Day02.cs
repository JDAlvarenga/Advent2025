using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;

namespace Advent2025.Solutions;

public class Day02: IDay
{
    private static readonly long[] Powers = [1, 10, 100, 1_000, 10_000, 100_000, 1_000_000, 10_000_000, 100_000_000, 1_000_000_000];
    
    [Benchmark, Arguments(false)]
    public long Part1_Gen(bool sample = false)
    {
        using var file = Util.GetInputStream<Day02>(sample);
        var input = file.ReadToEnd().AsSpan();

        var limits = input.SplitAny(",-");

        var sum = 0L;

        while (limits.MoveNext())
        {
            var startR = limits.Current;
            var start = long.Parse(limits.Source[startR]);
            var len = startR.End.Value - startR.Start.Value;
            
            var halfLen = (len+1) / 2;
            
            // take first half of the number
            var num = len % 2 == 0
                ? long.Parse(limits.Source[startR.Start..(startR.Start.Value + halfLen)])
                : Powers[halfLen-1];
            
            
            limits.MoveNext();
            var endR = limits.Current;
            var end = long.Parse(limits.Source[endR]);
            
            // duplicate the first half of the number
            var next = num*Powers[halfLen] + num;
            
            // Console.WriteLine($"DEBUG: {limits.Source[startR]} - {limits.Source[endR]}");
            // add if in range, stop after end
            while (next <= end)
            {
                // Console.WriteLine(next);
                if (next >= start)
                    sum += next;
                num++;
                if (num == Powers[halfLen]) halfLen++;
                next = num*Powers[halfLen] + num;
                
            }


        }

        return sum;
    }
    
    [Benchmark, Arguments(false)]
    public long Part2_Parallel(bool sample = false)
    {
        using var file = Util.GetInputStream<Day02>(sample);
        var input = file.ReadToEnd().AsSpan();

        var limits = input.SplitAny(",-");
        
        var ids = new ConcurrentBag<long>();
        
        while (limits.MoveNext())
        {
            var startR = limits.Current;
            var start = long.Parse(limits.Source[startR]);

            limits.MoveNext();
            var endR = limits.Current;
            var end = long.Parse(limits.Source[endR]);


            //skip single digits
            if (start < 10)
                start = 11;
            
            // for(var n = start; n <= end; n++)
            Parallel.For(start, end + 1, n =>
            {
                var str = n.ToString().AsSpan();
                
                for (var l = 1; l <= (str.Length + 1) / 2; l++)
                {
                    if (str.Length % l != 0) continue;
                    
                    var isValid = true;
                    
                    for (var i = 1; i < str.Length / l; i++)
                        if (!str[..l].SequenceEqual(str[(i * l)..((i + 1) * l)]))
                        {
                            isValid = false;
                            break;
                        }

                    if (!isValid) continue;
                    
                    ids.Add(n);
                    break;
                }
                
            }
            );
        }

        return ids.Sum();
    }
    
    [Benchmark, Arguments(false)]
    public long Part2(bool sample = false)
    {
        using var file = Util.GetInputStream<Day02>(sample);
        var input = file.ReadToEnd().AsSpan();

        var limits = input.SplitAny(",-");
        
        var ids = new ConcurrentBag<long>();
        
        while (limits.MoveNext())
        {
            var startR = limits.Current;
            var start = long.Parse(limits.Source[startR]);

            limits.MoveNext();
            var endR = limits.Current;
            var end = long.Parse(limits.Source[endR]);


            //skip single digits
            if (start < 10)
                start = 11;
            
            for(var n = start; n <= end; n++)
            // Parallel.For(start, end + 1, n =>
            {
                var str = n.ToString().AsSpan();
                
                for (var l = 1; l <= (str.Length + 1) / 2; l++)
                {
                    if (str.Length % l != 0) continue;
                    
                    var isValid = true;
                    
                    for (var i = 1; i < str.Length / l; i++)
                        if (!str[..l].SequenceEqual(str[(i * l)..((i + 1) * l)]))
                        {
                            isValid = false;
                            break;
                        }

                    if (!isValid) continue;
                    
                    ids.Add(n);
                    break;
                }
                
            }
            // );
        }

        return ids.Sum();
    }
        
    
    // Generate invalid ids and check if they are in range
    [Benchmark, Arguments(false)]
    public long Part2_Gen(bool sample = false)
    {
        using var file = Util.GetInputStream<Day02>(sample);
        var input = file.ReadToEnd().AsSpan();
    
        var limits = input.SplitAny(",-");
    
        var ids = new HashSet<long>();
    
        while (limits.MoveNext())
        {
            var source = limits.Source;
            
            var startSpan = source[limits.Current];
            var start = long.Parse(startSpan);
            var lenS = startSpan.Length;
            
            limits.MoveNext();
            var endSpan = source[limits.Current];
            var end = long.Parse(endSpan);
            var lenE = endSpan.Length;

            if (start < 10)
            {
                startSpan = "11".AsSpan();
                start = 11L;
                lenS = 2;
            }

            var sliced = false;
            if (lenS != lenE)
            {
                end = Powers[lenS] - 1;
                sliced = true;
            }
            
            SecondSlice:
            
            
            // Generate value by copying the first l digits
            for (var l = 1; l <= (lenS+1)/2; l++)
            {
                // But only if it's possible to have the same number of digits as the limit
                if (lenS%l != 0) continue;
                
                
                // use the first l digits from start of range to generate ids
                var num = long.Parse(startSpan[..l]);
                
    
                
                // build first invalid id
                var next = num;
                for (var r = l; r < lenS; r+=l)
                {
                    next += num * Powers[r];
                }
                
                //Console.WriteLine($"DEBUG: {startSpan} - {endSpan} ({l})");
                
                
                while (next <= end)
                {
                    if (next >= start)
                    {
                        // Console.WriteLine(next);
                        ids.Add(next);
                    }
                    // else Console.WriteLine($"DISCARDED: {next}");

                    ++num;
                    if (num == Powers[l]) break;
                    
                    // build next invalid id
                    next = num;
                    for (var r = l; r < lenS; r+=l)
                    {
                        next += num * Powers[r];
                    }
                    
                }
    
            }

            if (sliced)
            {
                start = Powers[lenS];
                startSpan = start.ToString().AsSpan();
                lenS = startSpan.Length;

                end = long.Parse(endSpan);
                
                sliced = false;
                goto SecondSlice;
            }
    
        }
    
        return ids.Sum();
    }
    
}