#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
//using MoreLinq;
using MoreLinq;

using Quad = (int,int,int,int);

bool sample = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt").ToArray();
//Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");


string NORMAL_BG      = Console.IsOutputRedirected ? "" : "\x1b[49m";
string RED_BG        = Console.IsOutputRedirected ? "" : "\x1b[41m";
string GREEN_BG      = Console.IsOutputRedirected ? "" : "\x1b[42m";
string YELLOW_BG      = Console.IsOutputRedirected ? "" : "\x1b[43m";

string NORMAL_FG      = Console.IsOutputRedirected ? "" : "\x1b[39m";
string BRIGHT_BLUE_FG      = Console.IsOutputRedirected ? "" : "\x1b[94m";
string BRIGHT_MAGENTA_FG      = Console.IsOutputRedirected ? "" : "\x1b[95m";

string RESET_CONSOLE = Console.IsOutputRedirected ? "" : "\x1b[0m";

Stopwatch sw = Stopwatch.StartNew();


var input = lines.Select(int.Parse).ToArray();

//sw.Restart();
//Part1(input);
//Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");
sw.Restart();
Part2(input);
Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");

void Part1(int[] input)
{
    var acc = 0L;
    foreach (long num in input) {
        
        var n = num;
        for (int i = 0;i<2000;i++) {
            n = (n * 64) ^ n;
            n = n % 16777216;

            n = (n / 32) ^ n;
            n = n % 16777216;

            n = (n * 2048) ^ n;
            n = n % 16777216;

            //Console.Out.WriteLine($"loop {i}: {n}");
        }

        acc += n;
    }
    Console.Out.WriteLine($"Part 1: {acc}");
}

void Part2(int[] input) {

    var iterations = 2000;

    List<Dictionary<Quad, int>> bestPrices = new(iterations);
    foreach (long num in input) {
        //Console.Out.WriteLine("--- " + num);
        int[] lastDigits = new int[iterations+1];
        lastDigits[0] = (int)(num % 10);
        var n = num;
        for (int i = 0;i<iterations-1;i++) {
            n = (n * 64) ^ n;
            n = n % 16777216;

            n = (n / 32) ^ n;
            n = n % 16777216;

            n = (n * 2048) ^ n;
            n = n % 16777216;
            lastDigits[i+1] = (int)(n % 10);
        }
        var offsets = lastDigits.Pairwise((a,b) => b-a).ToArray();

        List<(Quad Seq, int Price)> sequences = new();
        for (int i = 3; i < iterations; i++) {
            var seq = (offsets[i-3], offsets[i-2], offsets[i-1], offsets[i]);
            var price = lastDigits[i+1];
            sequences.Add((seq,price));
        }

        var sequenceDictionary = sequences.GroupBy(x => x.Seq, x => x.Price).ToDictionary(x => x.Key, x => x.First());
        bestPrices.Add(sequenceDictionary);
    }

    Dictionary<Quad, int> overallBest = new();
    foreach (var bestForBuyer in bestPrices) {
        //Console.Out.WriteLine(bestForBuyer.TryGetValue((-2,1,-1,3), out var v) ? v.ToString() : "xxx");
        foreach (var kvp in bestForBuyer) {
            var current = overallBest.TryGetValue(kvp.Key, out var c) ? c : 0;
            current += kvp.Value;
            overallBest[kvp.Key] = current;
        }
    }

    //Console.Out.WriteLine($"Overall {overallBest[(-2,1,-1,3)]}");
    var mostBananas = overallBest.Values.Max();


    Console.Out.WriteLine($"Part 2: {mostBananas}");
}

