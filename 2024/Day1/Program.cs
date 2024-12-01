#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MoreLinq;


bool sample = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

//Part1(lines);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(IEnumerable<string> lines)
{
    var lists = lines.Select(l => {
        var d = l.Split("   ");
        return (int.Parse(d[0]), int.Parse(d[1]));
    })
    .Aggregate((new List<int>(), new List<int>()), (acc, element) => {
        acc.Item1.Add(element.Item1);
        acc.Item2.Add(element.Item2);
        return acc;
    })
    ;

    var l1 = lists.Item1;
    var l2 = lists.Item2;

    l1.Sort();
    l2.Sort();

var sum = 0;
    for (int ii = 0; ii < l1.Count; ii++) {
        sum += Math.Abs(l1[ii] - l2[ii]);
    }
    
    Console.Out.WriteLine($"Part 1: {sum}");
}

void Part2(IEnumerable<string> lines)
{
    var lists = lines.Select(l => {
        var d = l.Split("   ");
        return (int.Parse(d[0]), int.Parse(d[1]));
    })
    .Aggregate((new List<int>(), new List<int>()), (acc, element) => {
        acc.Item1.Add(element.Item1);
        acc.Item2.Add(element.Item2);
        return acc;
    })
    ;

    var l1 = lists.Item1;
    var l2 = lists.Item2;

    var frequency = new Dictionary<int, int>();
    for (int ii = 0; ii < l2.Count; ii++) {
        CollectionsMarshal.GetValueRefOrAddDefault(frequency, l2[ii], out _)++;
    }

    var sum = 0L;
    for (int ii = 0; ii < l1.Count; ii++) {
        sum += l1[ii] * (frequency.TryGetValue(l1[ii], out var val) ? val : 0);
    }
    
    Console.Out.WriteLine($"Part 2: {sum}");
}