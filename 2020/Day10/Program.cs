using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

var numsX = lines.Select(int.Parse).OrderBy(n => n).Prepend(0);
var nums = numsX.Append(numsX.Last() + 3).ToArray();
var differenceCounts = new int[4];
var groupsOfOnes = new List<int>();
var oneCounter = 0;
for (int ii = 1; ii < nums.Length; ii++) {
    var difference = nums[ii] - nums[ii-1];
    differenceCounts[difference]++;
    if (difference == 1) {
        oneCounter++;
    } else  {
        if (oneCounter > 1) {
            groupsOfOnes.Add(oneCounter);
        }
        oneCounter = 0;
    }
}
Console.Out.WriteLine($"difference counts: {string.Join(", ", Enumerable.Range(1, 3).Select(n => $"[{n}]: {differenceCounts[n]}"))}");
Console.Out.WriteLine($"Answer 1: {differenceCounts[1] * differenceCounts[3]}");

Console.Out.WriteLine(string.Join(',', groupsOfOnes));
var combinations = groupsOfOnes.Select(o => (long)(o switch { 1 => 1, 2 => 2, 3 => 4, 4 => 7, 5 => 13, _ => 0 })).Aggregate((a,b) => a * b);
Console.Out.WriteLine($"Answer 2: {combinations}");

