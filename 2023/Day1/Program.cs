#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using MoreLinq;


bool sample = false;

Dictionary<string, int> Nums = new Dictionary<string, int> {
    {"0", 0},
    {"1", 1},
    {"2", 2},
    {"3", 3},
    {"4", 4},
    {"5", 5},
    {"6", 6},
    {"7", 7},
    {"8", 8},
    {"9", 9},
    {"one", 1},
    {"two", 2},
    {"three", 3},
    {"four", 4},
    {"five", 5},
    {"six", 6},
    {"seven", 7},
    {"eight", 8},
    {"nine", 9},
    };


string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();
var linesArray = lines.Select(l => l.ToArray());

//Part1(linesArray);
Part2(linesArray);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(IEnumerable<char[]> lines)
{
    var sum = lines.Select(line =>
    {
        var digits = line.Where(ch => ch >= '0' && ch <= '9').ToList();
        return (digits[0] - '0') * 10 + digits[^1] - '0';
    }).Sum();
    Console.Out.WriteLine($"Part 1: {sum}");
}

void Part2(IEnumerable<char[]> lines)
{
    var sum = lines.Select(line =>
    {
        var digits = LineToInts(line);
        return digits[0] * 10 + digits[^1];
    }).Sum();
    Console.Out.WriteLine($"Part 2: {sum}");
}


List<int> LineToInts(char[] line)
{
    List<int> result = new();

    var slice = new ReadOnlySpan<char>(line);
    while (slice.Length > 0)
    {
        foreach (var m in Nums)
        {
            if (slice.StartsWith(m.Key))
            {
                result.Add(m.Value);
                break;
            }
        }
        slice = slice[1..];
    }
    return result;
}
