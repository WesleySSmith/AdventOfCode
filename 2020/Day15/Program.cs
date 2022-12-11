using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MoreLinq;
using System.Diagnostics;

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 

var startingNums = lines[0].Split(',').Select(int.Parse).ToArray();
var sw = new Stopwatch();
sw.Start();
var lookup = new Dictionary<int, int> ();

for (int ii = 0; ii < startingNums.Length - 1; ii++) {
    lookup[startingNums[ii]] = ii;
    //Console.Out.WriteLine($"[{ii}]: {startingNums[ii]}");
}

int prev = startingNums.Last();
for (int ii = startingNums.Length - 1; ii < 30000000 -1 ; ii++) {
    /* if (ii % 100000 == 0) {
        Console.Out.WriteLine($"[{ii}]: {prev}");
    } */
    int newNum;
    var found = lookup.TryGetValue(prev, out var lastIndex);
    if (!found) {
        newNum = 0;
    } else {
        newNum = ii - lastIndex;
    }
    lookup[prev] = ii;
    prev = newNum;
    
}

Console.Out.WriteLine($"$$$$ {prev} $$$$ in {sw.ElapsedMilliseconds}ms");


