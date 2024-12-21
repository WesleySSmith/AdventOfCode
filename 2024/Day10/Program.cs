#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using MoreLinq;
//using MoreLinq;

bool sample = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
//Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

//Part1(lines);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(string[] lines)
{
    var rows = lines.Length;
    var cols = lines[0].Length;

    var map = new char[rows+2,cols+2];
    List<RC> trailheads = new();

    for (int r = 0; r < rows +2; r++) {
        for (int c = 0; c < cols +2; c++) {
            if (r == 0 || c == 0 || r == rows+1 || c == cols+1) {
                map[r,c] = (char)0;
            }
            else {
                var ch = map[r,c] = lines[r-1][c-1];
                if (ch == '0') {
                    trailheads.Add(new RC(r,c));
                }
            }
        }
    } 

    var acc = 0;
    foreach(var trailhead in trailheads) {
        var paths = SolveBase(trailhead, map);
        acc += paths;
    }


    Console.Out.WriteLine($"Part 1: {acc}");
}


void Part2(string[] lines) {
   var rows = lines.Length;
    var cols = lines[0].Length;

    var map = new char[rows+2,cols+2];
    List<RC> trailheads = new();

    for (int r = 0; r < rows +2; r++) {
        for (int c = 0; c < cols +2; c++) {
            if (r == 0 || c == 0 || r == rows+1 || c == cols+1) {
                map[r,c] = (char)0;
            }
            else {
                var ch = map[r,c] = lines[r-1][c-1];
                if (ch == '0') {
                    trailheads.Add(new RC(r,c));
                }
            }
        }
    } 

    var acc = 0;
    foreach(var trailhead in trailheads) {
        var paths = SolveBase2(trailhead, map);
        acc += paths;
    }


    Console.Out.WriteLine($"Part 2: {acc}");
}


int SolveBase(RC trailhead, char[,] map) {
    return Solve(trailhead, map).Distinct().Count();
}

List<RC> Solve(RC current, char[,] map) {
    var elevation = map[current.Row, current.Col];
    if (elevation == '9') {
        return [current];
    }
    var nextElevation = elevation + 1;
    List<RC> trails = [];
    var left = current with {Row = current.Row -1};
    if (map[left.Row,left.Col] == nextElevation) {
        trails.AddRange(Solve(left, map));
    }
    var right = current with {Row = current.Row +1};
    if (map[right.Row,right.Col] == nextElevation) {
        trails.AddRange(Solve(right, map));
    }
    var up = current with {Col = current.Col -1};
    if (map[up.Row,up.Col] == nextElevation) {
        trails.AddRange(Solve(up, map));
    }
    var down = current with {Col = current.Col +1};
    if (map[down.Row,down.Col] == nextElevation) {
        trails.AddRange(Solve(down, map));
    }
    return trails;
}



int SolveBase2(RC trailhead, char[,] map) {
    return Solve(trailhead, map).Count();
}

int Solve2(RC current, char[,] map) {
    var elevation = map[current.Row, current.Col];
    if (elevation == '9') {
        return 1;
    }
    var nextElevation = elevation + 1;
    int trailCount = 0;
    var left = current with {Row = current.Row -1};
    if (map[left.Row,left.Col] == nextElevation) {
        trailCount += Solve2(left, map);
    }
    var right = current with {Row = current.Row +1};
    if (map[right.Row,right.Col] == nextElevation) {
        trailCount += Solve2(right, map);
    }
    var up = current with {Col = current.Col -1};
    if (map[up.Row,up.Col] == nextElevation) {
        trailCount += Solve2(up, map);
    }
    var down = current with {Col = current.Col +1};
    if (map[down.Row,down.Col] == nextElevation) {
        trailCount += Solve2(down, map);
    }
    return trailCount;
}

record RC(int Row, int Col);

