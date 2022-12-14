#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Diagnostics;
//using MoreLinq;

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

var array = lines.Select(l => l.ToArray()).ToArray();

var twoDArray = new char[array.Length, array[0].Length];
        for (var i = 0; i < array.Length; i++)
        {
            for (var j = 0; j < array[0].Length; j++)
            {
                twoDArray[i,j] = array[i][j];
            }
        }


//Part1(twoDArray);
Part2(twoDArray);

Console.Out.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");


static void Part1(char[,] array) {

    (int, int) start = (-1,-1);
    (int, int) end = (-1,-1);
    for (int row = 0; row < array.GetLength(0); row++) {
        for (int col = 0; col < array.GetLength(1); col++) {
        var cell = array[row,col];
            if (cell == 'S') {
                array[row, col] = 'a';
                start = (row, col);
            } else if (cell == 'E') {
                array[row,col] = 'z';
                end = (row, col);
            }
        }
    }

    var shortest = ShortestPath(array, start, end);
    Console.Out.WriteLine($"Part 1 length: {shortest}");
}


static void Part2(char[,] array) {
    List<(int, int)> starts = new List<(int, int)>();
    (int, int) end = (-1,-1);
    for (int row = 0; row < array.GetLength(0); row++) {
        for (int col = 0; col < array.GetLength(1); col++) {
        var cell = array[row,col];
            if (cell == 'a') {
                starts.Add((row,col));
            }
            if (cell == 'S') {
                array[row, col] = 'a';
                starts.Add((row,col));
            } else if (cell == 'E') {
                array[row,col] = 'z';
                end = (row, col);
            }
        }
    }

    Console.Out.WriteLine($"Part 2 starts to check: {starts.Count()}");

    var goodStarts = starts.Where(start => {
        var row = start.Item1;
        var col = start.Item2;
        return 
           (row != 0 && array[row - 1, col] == 'b')
        || (row != array.GetLength(0) - 1 && array[row + 1, col]  == 'b')
        || (col != 0 && array[row, col - 1 ]== 'b')
        || (col != array.GetLength(1) - 1 && array[row, col + 1] == 'b');
    });
    
    Console.Out.WriteLine($"Part 2 good starts to check: {goodStarts.Count()}");
    var paths = goodStarts.Select((start,i) => {
        var pathLen = ShortestPath(array, start, end);
        Console.WriteLine($"[{i}] Shortest from [{start.Item1},{start.Item2}]: {pathLen}");
        return pathLen;
        });
    
    Console.Out.WriteLine($"Part 2 best path: {paths.Where(l => l > 0).Min()}");
}

static int ShortestPath(char[,] array, (int, int) start, (int, int) end) {
        var dist = new int[array.GetLength(0), array.GetLength(1)];
        var prev = new (int, int)[array.GetLength(0), array.GetLength(1)];
        (int, int) target = (-1, -1);
        List<(int, int)> Q = new List<(int, int)>();

        for (int row = 0; row < array.GetLength(0); row++) {
            for (int col = 0; col < array.GetLength(1); col++) {
                dist[row, col] = int.MaxValue;
                prev[row, col] = (-1, -1);
                Q.Add((row, col));
            }
        }

        dist[start.Item1, start.Item2] = 0;

        while (Q.Any()) {
            var u = Q.MinBy(q => dist[q.Item1, q.Item2]);
            Q.Remove(u);

            var row = u.Item1;
            var col = u.Item2;

            if (u == end) {
                return dist[row,col];
            }

            var currentHeight = array[row, col];
            var neighbors = new List<(int, int)>();
            if (row != 0) {
                // above
                if (array[row - 1, col] <= currentHeight + 1) {
                    neighbors.Add((row - 1, col));
                }
            }
            if (row != array.GetLength(0) - 1) {
                //below
                if (array[row + 1, col] <= currentHeight + 1) {
                    neighbors.Add((row + 1, col));
                }
            }
            if (col != 0) {
                //left
                if (array[row, col - 1 ] <= currentHeight + 1) {
                    neighbors.Add((row, col - 1));
                }
            }
            if (col != array.GetLength(1) - 1) {
                //right
                if (array[row, col + 1] <= currentHeight + 1) {
                    neighbors.Add((row, col +1));
                }
            }
            var neighborsInQ = neighbors.Where(n => Q.Contains(n));
            foreach (var v in neighborsInQ) {
                var alt = dist[u.Item1, u.Item2] + 1;
                if (alt < dist[v.Item1, v.Item2]) {
                    dist[v.Item1, v.Item2] = alt;
                    prev[v.Item1, v.Item2] = u;
                }
            }
        }

        return -1;
    }

public class Node {
    
    public char Height;
    public bool Visited;
}


