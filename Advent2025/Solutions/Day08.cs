using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace Advent2025.Solutions;

public class Day08: IDay
{
    [Benchmark, Arguments(false)]
    public int Part1(bool sample = false)
    {
        List<Vector3> boxes = new();
        
        Span<Range> ranges = new Range[3];
        foreach (ReadOnlySpan<char> line in Util.InputLines<Day08>(sample))
        {
            line.Split(ranges, ',');
            var box = new Vector3(
                x: float.Parse(line[ranges[0]]),
                y: float.Parse(line[ranges[1]]),
                z: float.Parse(line[ranges[2]])
            );
            
            boxes.Add(box);
        }
        
        // Get distances
        PriorityQueue<(int box1, int box2), float> distances = new();
        
        for(var i = 0; i < boxes.Count; i++)
        {
            for (var j = i + 1; j < boxes.Count; j++)
            {
                var l = (boxes[i] - boxes[j]).Length();
                distances.Enqueue((i, j), l);
            }
        }

        
        
        // make connections
        Dictionary<int, int> circuits = new();
        Dictionary<int, int> boxCircuit = new();


        int currentConn = 0;
        var limit = sample ? 10 : 1000;
        while (++currentConn < limit && distances.TryDequeue(out var conn, out _))
        {
            var (box1, box2) = conn;
    
            var inC1 = boxCircuit.TryGetValue(box1, out var c1);
            var inC2 = boxCircuit.TryGetValue(box2, out var c2);
    
            
            if (inC1)
            {
                if (inC2)
                {
                    // Both boxes are part of the same circuit
                    if (circuits[c1] == circuits[c2])
                    {
                        continue;
                    }
                    
                    // Boxes are part of different circuits, mark circuit2 as circuit1
                    var previous = circuits[c2];
                    for (var cidx = 0; cidx < circuits.Count; cidx++)
                    {
                        if (circuits[cidx] == previous)
                            circuits[cidx] = circuits[c1];
                    }
                    // circuits[c2] = circuits[c1];
                }
                else
                {
                    // Add box2 to same circuit as box1
                    boxCircuit.Add(box2, c1);
                }
            }
            else if (inC2)
            {
                // Add box1 to same circuit as box2
                boxCircuit.Add(box1, c2);
                
            }
            else
            {
                // Neither of the boxes are part of a circuit
                var id = circuits.Count;
                circuits.Add(id, id);
                boxCircuit.Add(box1, id);
                boxCircuit.Add(box2, id);
            }
        }
        
        
        // count boxes per circuit
        Dictionary<int, int> circuitCount = new();
        foreach (var c in boxCircuit.Values)
        {
            if (!circuitCount.TryAdd(circuits[c], 1))
                circuitCount[circuits[c]]++;
        }
        var sizes = circuitCount.Values.ToList();
        sizes.Sort();
    
        var last = sizes.Count - 1;
        return sizes[last] * sizes[last-1] * sizes[last - 2];
    }
    
    
    
    
    
    
    [Benchmark, Arguments(false)]
    public long Part2(bool sample = false)
    {
        List<Vector3> boxes = new();
        
        Span<Range> ranges = new Range[3];
        foreach (ReadOnlySpan<char> line in Util.InputLines<Day08>(sample))
        {
            line.Split(ranges, ',');
            var box = new Vector3(
                x: float.Parse(line[ranges[0]]),
                y: float.Parse(line[ranges[1]]),
                z: float.Parse(line[ranges[2]])
            );
            
            boxes.Add(box);
        }
        
        // Get distances
        PriorityQueue<(int box1, int box2), float> distances = new();
        
        for(var i = 0; i < boxes.Count; i++)
        {
            for (var j = i + 1; j < boxes.Count; j++)
            {
                var l = (boxes[i] - boxes[j]).Length();
                distances.Enqueue((i, j), l);
            }
        }
        
        
        
        // make connections
        Dictionary<int, int> circuits = new();
        Dictionary<int, int> boxCircuit = new();

        var lastConn = 0L;

        
        while (distances.TryDequeue(out var conn, out _))
        {
            var (box1, box2) = conn;

            var inC1 = boxCircuit.TryGetValue(box1, out var c1);
            var inC2 = boxCircuit.TryGetValue(box2, out var c2);

            
            if (inC1)
            {
                if (inC2)
                {
                    // Both boxes are part of the same circuit
                    if (circuits[c1] == circuits[c2])
                    {
                        continue;
                    }
                    
                    // Boxes are part of different circuits, mark circuit2 as circuit1

                    int previousC;
                    int newC;
                    if (circuits[c1] < circuits[c2])
                    {
                        newC = circuits[c1];
                        previousC = circuits[c2];
                    }
                    else
                    {
                        newC = circuits[c2];
                        previousC = circuits[c1];
                    }
                    
                    
                    for (var cidx = 0; cidx < circuits.Count; cidx++)
                    {
                        if (circuits[cidx] == previousC)
                            circuits[cidx] = newC;
                    }
                    
                    // Is single circuit
                    if (newC == 0  && boxCircuit.Count == boxes.Count && circuits.Values.All(x => x == 0))
                    {
                        lastConn = (long)boxes[box1].X * (long)boxes[box2].X;
                        break;
                    }
                }
                else
                {
                    // Add box2 to same circuit as box1
                    boxCircuit.Add(box2, c1);
                    // Is single circuit
                    if (circuits[c1] == 0  && boxCircuit.Count == boxes.Count && circuits.Values.All(x => x == 0))
                    {
                        lastConn = (long)boxes[box1].X * (long)boxes[box2].X;
                        break;
                    }
                }
            }
            else if (inC2)
            {
                // Add box1 to same circuit as box2
                boxCircuit.Add(box1, c2);
                
                // Is single circuit
                if (circuits[c2] == 0  && boxCircuit.Count == boxes.Count && circuits.Values.All(x => x == 0))
                {
                    lastConn = (long)boxes[box1].X * (long)boxes[box2].X;
                    break;
                }
                
            }
            else
            {
                // Neither of the boxes are part of a circuit
                var id = circuits.Count;
                circuits.Add(id, id);
                boxCircuit.Add(box1, id);
                boxCircuit.Add(box2, id);
            }
        }


        return lastConn;
    }
}