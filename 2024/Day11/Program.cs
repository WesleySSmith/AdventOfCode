#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using MoreLinq;
//using MoreLinq;

bool sample = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
//Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

//Part1(lines);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


// 1234


void Part1(string[] lines)
{
    var stones = lines[0].Split(' ').Select(long.Parse).ToList();

    for (int l = 0; l < 25; l++) {
        Console.Out.WriteLine(l);
        var len = stones.Count;
        for (int i = 0; i < len; i++) {

            var stone = stones[i];
            if (stone == 0) {
                stones[i] = 1;
            } else {
                int numDigits = (int)Math.Floor(Math.Log10(stone) + 1);
                if (numDigits % 2 == 0) {
                    var leftHalf = (long)(stone / Math.Pow(10, numDigits / 2));
                    var rightHalf = (long)(stone - leftHalf * Math.Pow(10, numDigits / 2));
                    stones[i] = leftHalf;
                    stones.Add(rightHalf);
                } else {
                    stones[i] = stone * 2024;
                }
            }
        }
    }


    Console.Out.WriteLine($"Part 1: {stones.Count()}");
}


void Part2(string[] lines) {
    var stones = lines[0].Split(' ').Select(long.Parse).ToList();
    var stonesD = stones.ToDictionary(s => s, s => 1L);

    
    for (int l = 0; l < 75; l++) {
        var stonesD2 = new Dictionary<long, long>(stones.Count * 2);
        //Console.Out.WriteLine(l);
        foreach (var stoneKV in stonesD) {
            var stone = stoneKV.Key;
            var count = stoneKV.Value;
            if (stone == 0) {
                stonesD2[1] = count + (stonesD2.TryGetValue(1, out var existing) ? existing : 0L);
                //CollectionsMarshal.GetValueRefOrAddDefault(stonesD2, 1, out _) += count;
            } else {
                int numDigits = (int)Math.Floor(Math.Log10(stone) + 1);
                if (numDigits % 2 == 0) {
                    var leftHalf = (long)(stone / Math.Pow(10, numDigits / 2));
                    var rightHalf = (long)(stone - leftHalf * Math.Pow(10, numDigits / 2));

                    stonesD2[leftHalf] = count + (stonesD2.TryGetValue(leftHalf, out var existing) ? existing : 0L);
                    stonesD2[rightHalf] = count + (stonesD2.TryGetValue(rightHalf, out var existing2) ? existing2 : 0L);
                    //CollectionsMarshal.GetValueRefOrAddDefault(stonesD2, leftHalf, out _) += count;
                    //CollectionsMarshal.GetValueRefOrAddDefault(stonesD2, rightHalf, out _) += count;
                } else {
                    var newVal = stone * 2024;
                    stonesD2[newVal] = count + (stonesD2.TryGetValue(newVal, out var existing) ? existing : 0L);
                    //CollectionsMarshal.GetValueRefOrAddDefault(stonesD2, newVal, out _) += count;
                }
            }
        }
        stonesD = stonesD2;
    }

    var totalStones = stonesD.Sum(x => x.Value);
    Console.Out.WriteLine($"Part 2: {totalStones}");
}

