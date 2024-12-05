#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using MoreLinq;


bool sample = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
//Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

//Part1(lines);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(string[] lines)
{
    var maxR = lines.Length;
    var maxC = lines[0].Count();
    var a = new char[maxR, maxC];
    for(int r = 0; r < maxR; r++ ) {
        for (int c = 0; c < maxC; c++){
            a[r, c] = lines[r][c];
        }
   }

    var list = new List<string>();

    // Horizontal
    for (int r = 0; r < maxR; r++) {
        for (int c = 0; c < maxC - 3; c++) {
            list.Add(new string([a[r,c], a[r,c+1],a[r,c+2], a[r,c+3]]));
        }
    }

    // Vertical
    for (int r = 0; r < maxR - 3; r++) {
        for (int c = 0; c < maxC; c++) {
            list.Add(new string([a[r,c], a[r+1,c],a[r+2,c], a[r+3,c]]));
        }
    }
    // Down and Right
    for (int r = 0; r < maxR - 3; r++) {
        for (int c = 0; c < maxC - 3; c++) {
            list.Add(new string([a[r,c], a[r+1,c+1],a[r+2,c+2], a[r+3,c+3]]));
        }
    }

    // Down and Left
    for (int r = 0; r < maxR - 3; r++) {
        for (int c = 3; c < maxC; c++) {
            list.Add(new string([a[r,c], a[r+1,c-1],a[r+2,c-2], a[r+3,c-3]]));
        }
    }

    var count = list.Count(l => l == "XMAS" || l == "SAMX");
    Console.Out.WriteLine($"Part 1: {count}");
}


void Part2(string[] lines)
{
    var maxR = lines.Length;
    var maxC = lines[0].Count();
    var a = new char[maxR, maxC];
    for(int r = 0; r < maxR; r++ ) {
        for (int c = 0; c < maxC; c++){
            a[r, c] = lines[r][c];
        }
   }

    var list = new List<string>();

    // Down and Right, then Down and Left
    for (int r = 0; r < maxR - 2; r++) {
        for (int c = 0; c < maxC - 2; c++) {
            list.Add(new string([a[r,c], a[r+1,c+1],a[r+2,c+2],  a[r,c+2], a[r+1,c+1], a[r+2,c]]));
        }
    }

    var count = list.Count(l => l == "MASMAS" || l == "MASSAM" || l == "SAMMAS" | l == "SAMSAM");
    Console.Out.WriteLine($"Part 2: {count}");
}