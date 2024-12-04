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


void Part1(IEnumerable<string> lines)
{
    var regex = new Regex(@"mul\(([0-9]{1,3}),([0-9]{1,3})\)");
    var allLines = lines.Aggregate(new StringBuilder(), (sb, line) => sb.Append(line)).ToString();
    var matches = regex.Matches(allLines);
    var products = matches.Select(match => (long)(int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value)));
    var sum = products.Sum();
    Console.Out.WriteLine($"Part 1: {sum}");
}


void Part2(IEnumerable<string> lines)
{
    var regex = new Regex(@"mul\(([0-9]{1,3}),([0-9]{1,3})\)|do\(\)|don't\(\)");
    var allLines = lines.Aggregate(new StringBuilder(), (sb, line) => sb.Append(line)).ToString();
    var matches = regex.Matches(allLines);

    var sum = matches.Aggregate((Sum: 0L , Enabled: true), (agg, match) => {
        var mVal = match.Groups[0].Value;
        if (mVal.StartsWith("mul")) {
            if (agg.Enabled) {
                var product = (long)(int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value));
                return (agg.Sum + product, agg.Enabled);
            } else {
                return agg;
            }
        } else if (mVal.StartsWith("don")) {
            return (agg.Sum, false);
        } else {
            return (agg.Sum, true);
        }
    } ).Sum;

    Console.Out.WriteLine($"Part 2: {sum}");
}