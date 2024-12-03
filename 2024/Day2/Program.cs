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
    var count = 0;
    foreach (var line in lines) {
        var report = line.Split(' ').Select(int.Parse).ToList();

        var diffs = report.Pairwise((a,b) => b-a);
        if (diffs.All(diff => diff >= 1 && diff <= 3) || diffs.All(diff => diff <= -1 && diff >= -3)) {
            count++;
        }
        
    }
    
    Console.Out.WriteLine($"Part 1: {count}");
}

void Part2(IEnumerable<string> lines)
{
   var count = 0;
    foreach (var line in lines)
    {
        var report = line.Split(' ').Select(int.Parse).ToList();

        (var increasingMistakes, var decreasingMistakes) = ScoreReport(report);

        if (increasingMistakes == 0 || decreasingMistakes == 0)
        {
            count++;
        }
        else if (increasingMistakes <= 2 || decreasingMistakes <= 2) {
            for (int removeIndex = 0; removeIndex < report.Count; removeIndex++)
            {
                var listWithOneOmitted = report.Take(removeIndex).Concat(report.Skip(removeIndex+1)).ToList();
                (increasingMistakes, decreasingMistakes) = ScoreReport(listWithOneOmitted);
                if (increasingMistakes == 0 || decreasingMistakes == 0)
                {
                    count++;
                    break;
                }
            }
        } else {
            //Console.Out.WriteLine($"{increasingMistakes} / {decreasingMistakes}");
        }

    }
    Console.Out.WriteLine($"Part 2: {count}");

    static (int, int) ScoreReport(List<int> report)
    {
        var diffs = report.Pairwise((a, b) => b - a);

        var increasing = diffs.Count(diff => diff >= 1 && diff <= 3);
        var decreasing = diffs.Count(diff => diff <= -1 && diff >= -3);
        var increasingMistakes = diffs.Count() - increasing;
        var decreasingMistakes = diffs.Count() - decreasing;
        return (increasingMistakes, decreasingMistakes);
    }
}