using BenchmarkDotNet.Attributes;

namespace Advent2025.Solutions;

public class Day10: IDay
{
    [Benchmark, Arguments(false)]
    public int Part1(bool sample = false)
    {
        var total = 0;
        foreach (ReadOnlySpan<char> line in Util.InputLines<Day10>(sample))
        {
            // Parse input
            var target = 0;
            List<int> buttons = [];
            
            var ranges = line.Split(' ');
            foreach (var range in ranges)
            {
                switch (line[range.Start])
                {
                    case '(':
                        var button = 0;
                        var buttonSpan = line[(range.Start.Value + 1)..(range.End.Value - 1)];
                        foreach (var nRange in buttonSpan.Split(','))
                        {
                            var n = int.Parse(buttonSpan[nRange]);
                            button |= (1 << n);
                        }
                        buttons.Add(button);
                        break;
                    
                    case '[':
                        var lights = line[(range.Start.Value + 1)..(range.End.Value - 1)];
                        for (var i = 0; i < lights.Length; i++)
                        {
                            if (lights[i] == '#')
                                target |= 1 << i;
                        }
                        break;
                    
                    case '{':
                        continue;
                        
                }
            }
            
            // BFS try all combinations
            total += int.PopCount(MinPresses(target, buttons));
        }

        return total;
    }

    private int MinPresses(int target, List<int> buttons)
    {
        if (target == 0) return 0;
        
        Queue<(int state, int pressed)> queue = new();
        queue.Enqueue((0, 0));

        while (queue.TryDequeue(out var tuple))
        {
            var (state, pressed) = tuple;

            for (var i = 0; i < buttons.Count; i++)
            {
                if ((pressed & 1 << i) != 0) continue;
                var nextPressed = pressed | (1 << i);
                var nextState = state ^ buttons[i];
                
                if (nextState == target) return nextPressed;
                
                queue.Enqueue((nextState, nextPressed));
            }
        }
        
        throw new Exception($"Failed to find a solution for target {target}");
    }

    
    [Benchmark, Arguments(false)]
    public long Part2(bool sample = false)
    {
        var total = 0L;
        foreach (ReadOnlySpan<char> line in Util.InputLines<Day10>(sample))
        {
            // Parse input
            List<int> jolts = [];
            List<int> buttons = [];
            
            var ranges = line.Split(' ');
            foreach (var range in ranges)
            {
                switch (line[range.Start])
                {
                    case '(':
                        var button = 0;
                        var buttonSpan = line[(range.Start.Value + 1)..(range.End.Value - 1)];
                        foreach (var nRange in buttonSpan.Split(','))
                        {
                            var n = int.Parse(buttonSpan[nRange]);
                            button |= (1 << n);
                        }
                        buttons.Add(button);
                        break;
                    
                    case '{':
                        var joltSpan = line[(range.Start.Value + 1)..(range.End.Value - 1)];
                        foreach (var jRange in joltSpan.Split(','))
                        {
                            jolts.Add(int.Parse(joltSpan[jRange]));
                        }
                        break;
                    
                    case '[':
                        continue;
                        
                }
            }
            
            var min = ReduceJolts(jolts, buttons, new Dictionary<int, long>(), new Dictionary<int, List<int>>());
            if (min == long.MaxValue) 
                Console.WriteLine($"Warning: Failed to find a solution for test case \n {line} \n Skipping...");
            else
                total += min;
            
        }
        
        return total;
    }

    private IEnumerable<int> SetBits(int number, int bitCount)
    {
        for (var i = 0; i < bitCount; i++)
        {
            if ((number & 1 << i) != 0) yield return i;
        }
    }

    private int GetTarget(in List<int> jolts)
    {
        var target = 0;
        for (var i = 0; i < jolts.Count; i++)
        {
            if (jolts[i] % 2 == 1)
                target |= 1 << i;
        }

        return target;
    }
    
    private IEnumerable<int> PossiblePresses(int target, List<int> buttons)
    {
        if (target == 0) goto End;
        
        Queue<(int state, int pressed)> queue = new();
        queue.Enqueue((0, 0));

        while (queue.TryDequeue(out var tuple))
        {
            var (state, pressed) = tuple;

            var s = buttons.Count-1;
            for (; s > 0; s--)
            {
                if ((pressed & 1 << (s)) != 0) break;
            }
            
            for (var i = s; i < buttons.Count; i++)
            {
                if ((pressed & 1 << i) != 0) continue;
                var nextPressed = pressed | (1 << i);
                var nextState = state ^ buttons[i];
                
                if (nextState == target) yield return nextPressed;
                else queue.Enqueue((nextState, nextPressed));
            }
        }
        
        // throw new Exception($"Failed to find a solution for target {target}");
        End: ;
    }
    
    private int HashList (in List<int> list) => list.Aggregate(HashCode.Combine);

    private long ReduceJolts(List<int> jolts, List<int> buttons, Dictionary<int, long> joltCache, Dictionary<int, List<int>> targetsCache)
    {
        var hash = HashList(jolts);
        if (joltCache.TryGetValue(hash, out var min)) return min;
        
        var pressMult = 1;
        while (jolts.All(j => j % 2 == 0))
        {
            for (var j = 0; j < jolts.Count; j++)
                jolts[j] /= 2;
            pressMult *= 2; // double the presses for this step
        }
        
        var target = GetTarget(jolts);
        if (!targetsCache.TryGetValue(target, out var presses))
        {
            presses = PossiblePresses(target, buttons).ToList();
            targetsCache.Add(target, presses);
        }

        List<long> possibleCounts = []; 
        foreach (var pressed in presses)
        {
            var joltsCopy = jolts.ToList();
            var pressCount = 0;
            
            // UPDATE JOLTS COPY
            // For every button pressed
            foreach (var button in SetBits(pressed, buttons.Count))
            {
                pressCount++;
                // Decrease the corresponding jolt values 
                foreach (var jolt in SetBits(buttons[button], joltsCopy.Count))
                {
                    joltsCopy[jolt]--;
                }
            }

            if (joltsCopy.Any(j => j < 0))
            {
                continue;
            }
            
            var inner = 0L;
            if (joltsCopy.Any(j => j != 0))
                inner = ReduceJolts(joltsCopy, buttons, joltCache, targetsCache);
            
            if (inner == long.MaxValue) continue;
            
            possibleCounts.Add(pressMult * (inner + pressCount));
        }

        min = possibleCounts.Any() ? possibleCounts.Min() : long.MaxValue;
        
        joltCache.Add(hash, min);

        return min;
    }
}