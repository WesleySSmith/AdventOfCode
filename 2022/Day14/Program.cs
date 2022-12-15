#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
//using MoreLinq;

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

var rockPaths = lines.Select(l => l.Split(" -> ").Select(Point.Parse).ToArray()).ToArray();

//Part1(rockPaths);
Part2(rockPaths);

Console.Out.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");


static void Part1(Point[][] rockPaths) {

    int minX = rockPaths.SelectMany(path => path.Select(p => p.X)).Min();
    int minY = rockPaths.SelectMany(path => path.Select(p => p.Y)).Min();
    int maxX = rockPaths.SelectMany(path => path.Select(p => p.X)).Max();
    int maxY = rockPaths.SelectMany(path => path.Select(p => p.Y)).Max();

    T[,] cave = new T[1000,1000];

    foreach (var path in rockPaths) {
        var start = path[0];
        foreach (var point in path[1..]) {
            if (start.X == point.X) {
                // vertical
                for (var y = Math.Min(start.Y, point.Y); y <= Math.Max(start.Y, point.Y); y++) {
                    cave[start.X, y] = T.Rock;
                }
            } else {
                // horizontal
                for (var x = Math.Min(start.X, point.X); x <= Math.Max(start.X, point.X); x++) {
                    cave[x, start.Y] = T.Rock;
                }
            }
            start = point;
        }
    }

    int sandCount = 0;
    while (true) {
        sandCount++;
        //Console.WriteLine($"Sand {sandCount} falling");

        Point sand = new Point(500,0);
        while(true) {
            // down
            Point next = sand with {Y = sand.Y + 1};
            if (cave[next.X, next.Y] == T.Air) {
                sand = next;
            } else {
                // down-left
                next = new Point(sand.X - 1, sand.Y + 1);
                if (cave[next.X, next.Y] == T.Air) {
                    sand = next;
                }
                else {
                    //down-right
                    next = new Point(sand.X + 1, sand.Y + 1);
                    if (cave[next.X, next.Y] == T.Air) {
                        sand = next;
                    } else {
                        //stuck
                        cave[sand.X, sand.Y] = T.Sand;
                        break;
                    }
                }
            }
            if (sand.X < minX || sand.Y > maxX) {
                goto done;
            }
        }

    }
done:
   
    Console.WriteLine($"Part 1 Sand Count: {sandCount-1}");
}


static void Part2(Point[][] rockPaths) {

    int minX = rockPaths.SelectMany(path => path.Select(p => p.X)).Min();
    int minY = rockPaths.SelectMany(path => path.Select(p => p.Y)).Min();
    int maxX = rockPaths.SelectMany(path => path.Select(p => p.X)).Max();
    int maxY = rockPaths.SelectMany(path => path.Select(p => p.Y)).Max();

    T[,] cave = new T[1000,1000];

    foreach (var path in rockPaths.Append(new[] {new Point(0, maxY + 2), new Point(999, maxY + 2)})) {
        var start = path[0];
        foreach (var point in path[1..]) {
            if (start.X == point.X) {
                // vertical
                for (var y = Math.Min(start.Y, point.Y); y <= Math.Max(start.Y, point.Y); y++) {
                    cave[start.X, y] = T.Rock;
                }
            } else {
                // horizontal
                for (var x = Math.Min(start.X, point.X); x <= Math.Max(start.X, point.X); x++) {
                    cave[x, start.Y] = T.Rock;
                }
            }
            start = point;
        }
    }

    int sandCount = 0;
    while (true) {
        sandCount++;
        //Console.WriteLine($"Sand {sandCount} falling");

        Point sand = new Point(500,0);
        while(true) {
            // down
            Point next = sand with {Y = sand.Y + 1};
            if (cave[next.X, next.Y] == T.Air) {
                sand = next;
            } else {
                // down-left
                next = new Point(sand.X - 1, sand.Y + 1);
                if (cave[next.X, next.Y] == T.Air) {
                    sand = next;
                }
                else {
                    //down-right
                    next = new Point(sand.X + 1, sand.Y + 1);
                    if (cave[next.X, next.Y] == T.Air) {
                        sand = next;
                    } else {
                        //stuck
                        cave[sand.X, sand.Y] = T.Sand;
                        if (sand == new Point(500,0)) {
                            goto done;
                        }
                        break;
                    }
                }
            }
            
        }

    }
done:
   
    Console.WriteLine($"Part 2 Sand Count: {sandCount}");
}

public record Point(int X, int Y) {
    public static Point Parse(string s) {
        var coords = s.Split(",").Select(int.Parse).ToArray();
        return new Point(coords[0], coords[1]);
    }
}

public enum T {
    Air = 0,
    Rock = 1,
    Sand = 2
}