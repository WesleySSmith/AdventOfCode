#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
//using MoreLinq;

bool sample = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
//Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");


string NORMAL_BG      = Console.IsOutputRedirected ? "" : "\x1b[49m";
string RED_BG        = Console.IsOutputRedirected ? "" : "\x1b[41m";
string GREEN_BG      = Console.IsOutputRedirected ? "" : "\x1b[42m";
string YELLOW_BG      = Console.IsOutputRedirected ? "" : "\x1b[43m";
string X1_BG      = Console.IsOutputRedirected ? "" : "\x1b[44m";
string X2_BG      = Console.IsOutputRedirected ? "" : "\x1b[45m";

Stopwatch sw = Stopwatch.StartNew();

Dictionary<string, long> Possible2Memo = [];

var towels = lines[0].Split(",").Select(s => s.Trim()).ToHashSet();
var designs = lines.Skip(2).ToArray();

//Part1(towels, designs);
Part2(towels, designs);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");



void Part1(HashSet<string> towels, string[] designs)
{
    
    var possible = designs.Count(d => Possible1(towels, d));


    Console.Out.WriteLine($"Part 1: {possible}");
}

bool Possible1(HashSet<string> towels, string pattern) {
    if (towels.Contains(pattern)) {
        return true;
    }

    for (int i = 1; i < pattern.Length; i++) {
        if (towels.Contains(pattern[..i])) {
            if (Possible1(towels, pattern[i..])) {
                return true;
            }
        }
    }
    return false;
}


void Part2(HashSet<string> towels, string[] designs) {

    var ways = designs.Sum(d => {
        Console.WriteLine($"Checking {d}");
        var w = Possible2(towels, d);
        Console.WriteLine(w);
        return w;
        });
    Console.Out.WriteLine($"Part 2: {ways}");
}



long Possible2(HashSet<string> towels, string pattern) {
    if (Possible2Memo.TryGetValue(pattern, out var memoWays)) {
        return memoWays;
    }
    var allWays = 0L;
    for (int i = 1; i < pattern.Length; i++) {
        if (towels.Contains(pattern[..i])) {
            var ways = Possible2(towels, pattern[i..]);
            allWays += ways;
        }
    }
    if (towels.Contains(pattern)) {
        allWays++;
    }
    Possible2Memo.Add(pattern, allWays);
    return allWays;
}

