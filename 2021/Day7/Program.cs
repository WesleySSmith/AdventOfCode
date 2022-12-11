//using MoreLinq;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Numerics;


string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
//Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");
var sw =Stopwatch.StartNew();

var hpos = lines[0].Split(',').Select(int.Parse).ToArray();
//Part1(hpos);
Part2(hpos);

Console.Out.WriteLine($"Total time {sw.ElapsedMilliseconds}");


static void Part1(IEnumerable<int> hpos) {
    var best = Enumerable.Range(0, hpos.Max())
        .Select(choice => (
            choice: choice,
            fuel: hpos.Select(h => Math.Abs(h - choice)).Sum()))
        .MinBy(x => x.fuel);

    Console.Out.WriteLine($"Best is position {best.choice} with fuel: {best.fuel}");
}

static void Part2(IEnumerable<int> hpos) {
   var best = Enumerable.Range(0, hpos.Max())
        .Select(choice => (
            choice: choice,
            fuel: hpos.Select(h => SumSeries(Math.Abs(h - choice))).Sum()))
        .MinBy(x => x.fuel);

    Console.Out.WriteLine($"Best is position {best.choice} with fuel: {best.fuel}");
}

static int SumSeries(int n) {
    return (n + 1) * n / 2;
}









