#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Text;
//using MoreLinq;

bool sample = false;
bool debug = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt"); 
if (debug) Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

var points = lines.Select(line => line.Split(',')).Select(a => new Point(int.Parse(a[0]), int.Parse(a[1]), int.Parse(a[2]))).ToArray();

//Part1(points);
Part2(points);


Console.Out.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");


void Part1(Point[] points) {

    HashSet<Point> pointHash = new();

    foreach (var point in points) {
        pointHash.Add(point);
    }

    var count = 0;
    foreach (var point in points) {
        if (!pointHash.Contains(point with {X = point.X + 1})) {
            count++;
        }
        if (!pointHash.Contains(point with {X = point.X - 1})) {
            count++;
        }
        if (!pointHash.Contains(point with {Y = point.Y + 1})) {
            count++;
        }
        if (!pointHash.Contains(point with {Y = point.Y - 1})) {
            count++;
        }
        if (!pointHash.Contains(point with {Z = point.Z + 1})) {
            count++;
        }
        if (!pointHash.Contains(point with {Z = point.Z - 1})) {
            count++;
        }
    }

    Console.WriteLine($"Part 1 count: {count}");

}



void Part2(Point[] rockArray) {

    HashSet<Point> rocks = new();

    foreach (var point in rockArray) {
        rocks.Add(point);
    }

    var xs = rockArray.Select(p => p.X);
    var ys = rockArray.Select(p => p.Y);
    var zs = rockArray.Select(p => p.Z);

    var minX = xs.Min();
    var maxX = xs.Max();
    var minY = ys.Min();
    var maxY = ys.Max();
    var minZ = zs.Min();
    var maxZ = zs.Max();

    HashSet<Point> airs = new();
    for (var x = minX; x <= maxX; x++) {
        for (var y = minY; y <= maxY; y++) {
            for (var z = minZ; z <= maxZ; z++) {
                var point = new Point(x,y,z);
                if (!rocks.Contains(point)) {
                    airs.Add(point);
                }
            }
        }
    }

    HashSet<Point> steams = new();

    for (var x = minX -1 ; x <= maxX + 1; x++) {
        for (var y = minY -1; y <= maxY + 1; y++) {
            for (var z = minZ -1; z <= maxZ + 1; z++) {

                if (
                    (x == minX -1 || x == maxX + 1)
                    || (y == minY -1 || y == maxY + 1)
                    || (z == minZ -1 || z == maxZ + 1)) {
                    var point = new Point(x,y,z);
                    steams.Add(point);
                }
            }
        }
    }

    while(true) {

        List<Point> transfer = new();
        foreach (var point in airs ) {

            if (steams.Contains(point with {X = point.X + 1})
            || steams.Contains(point with {X = point.X - 1})
            || steams.Contains(point with {Y = point.Y + 1})
            || steams.Contains(point with {Y = point.Y - 1})
            || steams.Contains(point with {Z = point.Z + 1})
            || steams.Contains(point with {Z = point.Z - 1})) {
                transfer.Add(point);
            }
        }

        if (transfer.Any()) {
            foreach (var point in transfer) {
                airs.Remove(point);
                steams.Add(point);
            }
        }
        else {
            break;
        }
    }


    var count = 0;

    foreach (var point in rockArray) {
        if (steams.Contains(point with {X = point.X + 1})) {
            count++;
        }
        if (steams.Contains(point with {X = point.X - 1})) {
            count++;
        }
        if (steams.Contains(point with {Y = point.Y + 1})) {
            count++;
        }
        if (steams.Contains(point with {Y = point.Y - 1})) {
            count++;
        }
        if (steams.Contains(point with {Z = point.Z + 1})) {
            count++;
        }
        if (steams.Contains(point with {Z = point.Z - 1})) {
            count++;
        }
    }

    Console.WriteLine($"Part 2 count: {count}");

}

public record Point(int X, int Y, int Z);


