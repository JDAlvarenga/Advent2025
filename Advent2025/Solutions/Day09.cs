using System.Collections.Concurrent;
using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace Advent2025.Solutions;

public class Day09: IDay
{
    [Benchmark, Arguments(false)]
    public long Part1(bool sample = false)
    {
        
        var minX = int.MaxValue;
        var minY = int.MaxValue;
        var maxX = int.MinValue;
        var maxY = int.MinValue;
        

        List<Complex> points = [];
        
        Span<Range> ranges= new Range[2];
        foreach (ReadOnlySpan<char> line in Util.InputLines<Day09>(sample))
        {
            line.Split(ranges, ',');
            var x = int.Parse(line[ranges[0]]);
            var y = int.Parse(line[ranges[1]]);

            maxX = int.Max(maxX, x);
            maxY = int.Max(maxY, y);
            minX = int.Min(minX, x);
            minY = int.Min(minY, y);

            points.Add(new Complex(x, y));
        }
        
        const int halfNumSlices = 2;
        const int numSlices = halfNumSlices * 2;
        const double angleStep = Math.Tau / numSlices;
        
        var slices = new ConcurrentBag<Complex>[numSlices];
        
        for(var i = 0; i < numSlices; i++) slices[i] = [];
        
        Complex center = new(((double)maxX - minX)/2, ((double)maxY - minY)/2);

        // Save each point in one of numSlices around the center of all points (angle) 
        points.AsParallel().ForAll(p =>
        {
            var phase =  (p - center).Phase;
            if (phase < 0) phase += Math.Tau;

            var idx = (int)(phase / angleStep);
            slices[idx].Add(p);
        });
        
        // Compare each point in a slice to all points in its oposite slice
        
        var maxAreas = new long [halfNumSlices];

        Parallel.For(0, numSlices / 2 - 1, i =>
        {
            foreach (var p1 in slices[i])
            {
                foreach (var p2 in slices[(i + halfNumSlices) % numSlices])
                {
                    var a = p1 - p2;
                    var area = (long)(a.Real+1) * (long)(a.Imaginary+1);

                    maxAreas[i] = long.Max(maxAreas[i], area);
                }
            }
        });

        return maxAreas.Max();
    }

    
    
    
    
    [Benchmark, Arguments(false)]
    public long Part2(bool sample = false)
    {
        List<Complex> points = [];
        SortedSet<int> xCoords = [];
        SortedSet<int> yCoords = [];
        
        // Parse points
        Span<Range> ranges= new Range[2];
        foreach (ReadOnlySpan<char> line in Util.InputLines<Day09>(sample))
        {
            line.Split(ranges, ',');
            var x = int.Parse(line[ranges[0]]);
            var y = int.Parse(line[ranges[1]]);

            xCoords.Add(x);
            yCoords.Add(y);
            
            points.Add(new Complex(x, y));
        }
        
        // Used to convert to compressed coords
        var xMapping = xCoords.Index().ToDictionary(
            keySelector: tuple => tuple.Item,
            elementSelector: tuple => tuple.Index);
        var yMapping = yCoords.Index().ToDictionary(
            keySelector: tuple => tuple.Item,
            elementSelector: tuple => tuple.Index);
        
        // Used to convert back to original space at end
        var toOriginalX = xCoords.ToList();
        var toOriginalY = yCoords.ToList();
        
        
        // map to new compressed space
        var cPoints = points.Select(p => new Complex(xMapping[(int)p.Real], yMapping[(int)p.Imaginary])).ToList();

        
        // build map
        var map = new char[xMapping.Count, yMapping.Count];
        {
            var pos = cPoints[0];
            for (var p = 1; p < cPoints.Count; p++)
            {
                var dir = (cPoints[p] - pos) switch
                {
                    { Real: > 0 } => Util.Directions.Right,
                    { Real: < 0 } => Util.Directions.Left,
                    { Imaginary: > 0 } => Util.Directions.Up,
                    { Imaginary: < 0 } => Util.Directions.Down,
                    _ => throw new ArgumentOutOfRangeException()
                };

                map[(int)pos.Real, (int)pos.Imaginary] = '#';
                
                while ((pos+=dir) != cPoints[p])
                {
                    map[(int)pos.Real, (int)pos.Imaginary] = 'X';
                }
            }
            var dr = (cPoints[0] - pos) switch
            {
                { Real: > 0 } => Util.Directions.Right,
                { Real: < 0 } => Util.Directions.Left,
                { Imaginary: > 0 } => Util.Directions.Up,
                { Imaginary: < 0 } => Util.Directions.Down,
                _ => throw new ArgumentOutOfRangeException()
            };
            map[(int)pos.Real, (int)pos.Imaginary] = '#';
            while ((pos+=dr) != cPoints[0])
            {
                map[(int)pos.Real, (int)pos.Imaginary] = 'X';
            }
        }
        
        
        // Find a tile that is inside by looking for a wall 'X' (not corner '#') with a non 'wall' character '\0' immediately after.
        Complex start = default;
        for (var j = 0; j < map.GetLength(1); j++)
        {
            for (var i = 0; i < map.GetLength(0); i++)
            {
                if (map[i, j] == '#') break;
                if (map[i, j] == '\0') continue;

                if (map[i, j] == 'X')
                {
                    if (map[++i, j] == '\0')
                    {
                        start = new Complex(i, j);
                        break;
                    }
                }

            }
            if (start != default) break;
            
            // retry from other side
            for (var i = map.GetLength(0)-1; i >= 0; i--)
            {
                if (map[i, j] == '#') break;
                if (map[i, j] == '\0') continue;

                if (map[i, j] == 'X')
                {
                    if (map[--i, j] == '\0')
                    {
                        start = new Complex(i, j);
                        break;
                    }
                }

            }
            if (start != default) break;
        }

        if (start == default) throw new Exception("Could not determine a starting 'inside' point for flood fill");
        
        
        // Fill inside tiles
        FloodFill(ref map, start, '\0', 'x');
        
        
        // // print map
        // for (var j = map.GetLength(1)-1; j >= 0; j--)
        // {
        //     for (var i = 0; i < map.GetLength(0); i++)
        //     {
        //         Console.Write(map[i, j] == '\0' ? '.' : map[i, j]);
        //     }
        //     Console.WriteLine();
        // }
        
   
        
        // find the max valid area for a point
        
        var maxAreas = new long [cPoints.Count - 1];
        // for (var p1 = 0; p1 < cPoints.Count-1; p1++)
        Parallel.For(0, cPoints.Count - 1, p1 => 
        {
            for (var p2 = p1 + 1; p2 < cPoints.Count; p2++)
            {
                var pos = cPoints[p1];
                // find corners points of the possible area.
                var diff = cPoints[p2] - pos;
                var xDiff = new Complex(diff.Real.CompareTo(0), 0);
                var yDiff = new Complex(0, diff.Imaginary.CompareTo(0));

                var secondPoint = new Complex(cPoints[p2].Real, cPoints[p1].Imaginary);
                var forthPoint = new Complex(cPoints[p1].Real, cPoints[p2].Imaginary);

                // traverse the edges of the area. Discard it if 'outside' character '\0' is found.
                while (pos != secondPoint)
                {
                    pos += xDiff;
                    if (map[(int)pos.Real, (int)pos.Imaginary] == '\0') goto InvalidArea;
                }

                while (pos != cPoints[p2])
                {
                    pos += yDiff;
                    if (map[(int)pos.Real, (int)pos.Imaginary] == '\0') goto InvalidArea;
                }

                while (pos != forthPoint)
                {
                    pos -= xDiff;
                    if (map[(int)pos.Real, (int)pos.Imaginary] == '\0') goto InvalidArea;
                }

                while (pos != cPoints[p1])
                {
                    pos -= yDiff;
                    if (map[(int)pos.Real, (int)pos.Imaginary] == '\0') goto InvalidArea;
                }

                var xLen = long.Abs(toOriginalX[(int)cPoints[p2].Real] - toOriginalX[(int)cPoints[p1].Real]) + 1;
                var yLen = long.Abs(toOriginalY[(int)cPoints[p2].Imaginary] -
                                    toOriginalY[(int)cPoints[p1].Imaginary]) + 1;
                var a = xLen * yLen;
                maxAreas[p1] = long.Max(maxAreas[p1], a);

                InvalidArea: ;
            }

        }
        );
        
        return maxAreas.Max();
    }

    private static void FloodFill(ref char[,] map, Complex start, char from, char to)
    {
        if (map[(int)start.Real, (int)start.Imaginary] == to) return;
        
        Queue<Complex> queue = [];
        
        queue.Enqueue(start);
        map[(int)start.Real, (int)start.Imaginary] = to;

        var n = map.GetLength(0);
        var m = map.GetLength(1);


        while (queue.TryDequeue(out var pos))
        {
            foreach (var d in Util.Directions.All4)
            {
                var next = pos + d;
                var x = (int)next.Real;
                var y = (int)next.Imaginary;
                if (x < 0 || x >= n || y < 0 || y >= m || map[x, y] != from) continue;
                map[x, y] = to;
                queue.Enqueue(next);
            }
        }

    }
}