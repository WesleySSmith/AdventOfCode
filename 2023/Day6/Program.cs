#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using MoreLinq;


bool sample = false;


string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();


//Part1(lines);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(string[] lines)
{

    var nums = Enumerable.Range(0, 2).Select(lineNum => lines[lineNum][11..].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList());

    var races = nums.ElementAt(0).Zip(nums.ElementAt(1)).Select((t) => (Time: t.First, MaxDistance: t.Second)).ToList();

    int score = 1;
    foreach (var race in races) {
        int waysToWin = 0;
        for (int holdTime = 0; holdTime <= race.Time; holdTime++) {
            var distance = (race.Time - holdTime) * holdTime;
            if (distance > race.MaxDistance) {
                waysToWin++;
            }
        }
        score *= waysToWin;
    }

    Console.Out.WriteLine($"Score is {score}");

}

void Part2(string[] lines)
{
     var nums = Enumerable.Range(0, 2).Select(lineNum => lines[lineNum][11..].Replace(" ", "").Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList());

    var races = nums.ElementAt(0).Zip(nums.ElementAt(1)).Select((t) => (Time: t.First, MaxDistance: t.Second)).ToList();

    long score = 1;
    foreach (var race in races) {
        int waysToWin = 0;
        for (int holdTime = 0; holdTime <= race.Time; holdTime++) {
            var distance = (race.Time - holdTime) * holdTime;
            if (distance > race.MaxDistance) {
                waysToWin++;
            }
        }
        score *= waysToWin;
    }

    Console.Out.WriteLine($"Score is {score}");
}

