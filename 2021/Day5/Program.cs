using MoreLinq;
using System.Diagnostics;
using System.Text;
using System.Text.Json;


string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");
var sw =Stopwatch.StartNew();

var lineSegments = new (Point, Point)[lines.Length];
int ii = 0;
foreach (var l in lines) {
    var points = l.Split(" -> ");
    var p1 = points[0].Split(",");
    var p2 = points[1].Split(",");

    lineSegments[ii].Item1.X = int.Parse(p1[0]);
    lineSegments[ii].Item1.Y = int.Parse(p1[1]);
    lineSegments[ii].Item2.X = int.Parse(p2[0]);
    lineSegments[ii].Item2.Y = int.Parse(p2[1]);
    ii++;
}

// var lineSegments = lines
//     .Select(l => {
//         var points = l.Split(" -> ")
//         .Select(p => {
//             var coords = p.Split(",");
//             return new Point {X = int.Parse(coords[0]), Y = int.Parse(coords[1])};
//         }).ToArray();
//         return (points[0], points[1]);
//     }).ToArray();

Console.Out.WriteLine($"After parsing: {sw.ElapsedMilliseconds}");

//Part1(lineSegments);
Part2(lineSegments, sw);


static void Part1(IEnumerable<(Point, Point)> lineSegments) {
   
   var maxRange = lineSegments.Aggregate((0, 0), ((int, int) acc, (Point, Point) pts) => 
        (Math.Max(acc.Item1, Math.Max(pts.Item1.X, pts.Item2.X)),
        Math.Max(acc.Item2, Math.Max(pts.Item1.Y, pts.Item2.Y))
        ));

    var seaFloor = new int[maxRange.Item1 + 1, maxRange.Item2 + 1];

    var horiz = lineSegments.Where(line => line.Item1.X == line.Item2.X);
    var vert =  lineSegments.Where(line => line.Item1.Y == line.Item2.Y);

    foreach (var line in horiz) {
        var startY = Math.Min(line.Item1.Y, line.Item2.Y);
        var endY = Math.Max(line.Item1.Y, line.Item2.Y);
        for (int ii = startY; ii <= endY; ii++) {
            seaFloor[line.Item1.X, ii]++;
        }
    }

    foreach (var line in vert) {
        var startX = Math.Min(line.Item1.X, line.Item2.X);
        var endX = Math.Max(line.Item1.X, line.Item2.X);
        for (int ii = startX; ii <= endX; ii++) {
            seaFloor[ii, line.Item1.Y]++;
        }
    }

    var overlaps = seaFloor.Cast<int>().Count(val => val > 1);

    Console.Out.WriteLine($"Overlaps: {overlaps}");
}

static void Part2(IList<(Point, Point)> lineSegments, Stopwatch sw) {

   var maxRange = lineSegments.Aggregate((0, 0), ((int, int) acc, (Point, Point) pts) => 
        (Math.Max(acc.Item1, Math.Max(pts.Item1.X, pts.Item2.X)),
        Math.Max(acc.Item2, Math.Max(pts.Item1.Y, pts.Item2.Y))
        ));

    Console.Out.WriteLine($"After maxRange: {sw.ElapsedMilliseconds}");

    var seaFloor = new int[maxRange.Item1 + 1, maxRange.Item2 + 1];
    Console.Out.WriteLine($"After seaFloor: {sw.ElapsedMilliseconds}");

    var horiz = lineSegments.Where(line => line.Item1.X == line.Item2.X);
    var vert =  lineSegments.Where(line => line.Item1.Y == line.Item2.Y);
    var diag =  lineSegments.Where(line => line.Item1.Y != line.Item2.Y && line.Item1.X != line.Item2.X);
    Console.Out.WriteLine($"After segmentation: {sw.ElapsedMilliseconds}");


    foreach (var line in horiz) {
        var startY = Math.Min(line.Item1.Y, line.Item2.Y);
        var endY = Math.Max(line.Item1.Y, line.Item2.Y);
        for (int ii = startY; ii <= endY; ii++) {
            seaFloor[line.Item1.X, ii]++;
        }
    }
    Console.Out.WriteLine($"After horiz: {sw.ElapsedMilliseconds}");

    foreach (var line in vert) {
        var startX = Math.Min(line.Item1.X, line.Item2.X);
        var endX = Math.Max(line.Item1.X, line.Item2.X);
        for (int ii = startX; ii <= endX; ii++) {
            seaFloor[ii, line.Item1.Y]++;
        }
    }
    Console.Out.WriteLine($"After vert: {sw.ElapsedMilliseconds}");

    foreach (var line in diag) {
        var startPos = line.Item1.X < line.Item2.X ? line.Item1 : line.Item2;
        var endPos = startPos.X == line.Item1.X ? line.Item2 : line.Item1;
        var isYIncreasing = endPos.Y > startPos.Y;

        for (int ii = startPos.X; ii <= endPos.X; ii++) {
            seaFloor[ii, startPos.Y + (ii - startPos.X)*(isYIncreasing ? 1 : -1)]++;
        }
    }
    Console.Out.WriteLine($"After diag: {sw.ElapsedMilliseconds}");

    //var overlaps = seaFloor.Cast<int>().Count(val => val > 1);

    // var overlaps = 0;
    // for (int ii = 0; ii < maxRange.Item1; ii++) {
    //     for (int jj = 0; jj < maxRange.Item2; jj++) {
    //         if (seaFloor[ii,jj] > 1) {
    //             overlaps++;
    //         }
    //     }
    // }

    var overlaps = 0;
    foreach (var point in seaFloor) {
        if (point > 1) {
            overlaps++;
        }
    }

    Console.Out.WriteLine($"After count: {sw.ElapsedMilliseconds}");
    Console.Out.WriteLine($"Overlaps: {overlaps}");
}


struct Point {
    public int X;
    public int Y;
}







