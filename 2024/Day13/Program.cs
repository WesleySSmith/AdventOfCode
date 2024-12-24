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
    var games = lines.Batch(4).Select(b => {

        var r = new Regex(@"X\+([0-9]+), Y\+([0-9]+)");
        var line1 = b.ElementAt(0);
        var matches = r.Match(line1);
        var x = int.Parse(matches.Groups[1].Value);
        var y = int.Parse(matches.Groups[2].Value);
        var mA = new Machine1(x, y);

        var line2 = b.ElementAt(1);
        matches = r.Match(line2);
        x = int.Parse(matches.Groups[1].Value);
        y = int.Parse(matches.Groups[2].Value);
        var mB = new Machine1(x, y);

        r = new Regex(@"X=([0-9]+), Y=([0-9]+)");
        var line3 = b.ElementAt(2);
        matches = r.Match(line3);
        x = int.Parse(matches.Groups[1].Value);
        y = int.Parse(matches.Groups[2].Value);

        return new Game1(mA, mB, x, y);
    });

    var acc = 0;
    foreach (var game in games) {
        var b = ((game.PrizeY * game.A.DeltaX) - (game.PrizeX * game.A.DeltaY)) / ((game.A.DeltaX * game.B.DeltaY) - (game.A.DeltaY * game.B.DeltaX));
        var a = (game.PrizeX - (game.B.DeltaX * b)) / game.A.DeltaX;

        Console.Out.WriteLine($"A: {a}, B: {b}");
        if ((a - Math.Floor(a)) < 0.001 && a <= 100 && (b - Math.Floor(b) < 0.001) && b <= 100) {
            var cost = (int)a*3 + (int)b*1;
            acc += cost;

            
        }
    }


    Console.Out.WriteLine($"Part 1: {acc}");
}


void Part2(string[] lines) {
    var games = lines.Batch(4).Select(b => {

        var r = new Regex(@"X\+([0-9]+), Y\+([0-9]+)");
        var line1 = b.ElementAt(0);
        var matches = r.Match(line1);
        var x = int.Parse(matches.Groups[1].Value);
        var y = int.Parse(matches.Groups[2].Value);
        var mA = new Machine2(x, y);

        var line2 = b.ElementAt(1);
        matches = r.Match(line2);
        x = int.Parse(matches.Groups[1].Value);
        y = int.Parse(matches.Groups[2].Value);
        var mB = new Machine2(x, y);

        r = new Regex(@"X=([0-9]+), Y=([0-9]+)");
        var line3 = b.ElementAt(2);
        matches = r.Match(line3);
        x = int.Parse(matches.Groups[1].Value);
        y = int.Parse(matches.Groups[2].Value);

        return new Game2(mA, mB, x + 10000000000000, y+10000000000000);
    });

    var acc = 0L;
    foreach (var game in games) {

        var b1 = ((game.PrizeY * game.A.DeltaX) - (game.PrizeX * game.A.DeltaY));
        var b2 = ((game.A.DeltaX * game.B.DeltaY) - (game.A.DeltaY * game.B.DeltaX));
        if (b1 % b2 != 0) {
            continue;
        }
        var b = b1 / b2;
        var a = (game.PrizeX - (game.B.DeltaX * b)) / game.A.DeltaX;

        Console.Out.WriteLine($"A: {a}, B: {b}");
        var cost = a*3 + b*1;
        acc += cost;
    }


    Console.Out.WriteLine($"Part 2: {acc}");
   
}

record Machine1(int DeltaX, int DeltaY);
record Game1(Machine1 A, Machine1 B, float PrizeX, float PrizeY);

record Machine2(long DeltaX, long DeltaY);
record Game2(Machine2 A, Machine2 B, long PrizeX, long PrizeY);
