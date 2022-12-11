using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MoreLinq;

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 

var earliestTimestamp = int.Parse(lines[0]);


// Part 1
//var buses = lines[1].Split(",").Where(s => s != "x").Select(int.Parse);
//var answer = buses.Select(b => (bus: b, afterEarliest: b - earliestTimestamp % b)).MinBy(t => t.afterEarliest).First();
//Console.Out.WriteLine($"Have to wait {answer.afterEarliest} for bus {answer.bus}: {answer.afterEarliest * answer.bus}");

// Part 2
var busAndOffset = 
    lines[1].Split(",")
    .Select( b => b == "x" ? -1 : int.Parse(b))
    .Scan((offset: -1, bus: 0), (a, b) => (a.Item1+1, b))
    .Where(d => d.bus != -1);

Console.Out.WriteLine(busAndOffset.ToDelimitedString(","));

var N = busAndOffset.Aggregate((long)1, (a, bo) => a *= bo.bus);
Console.Out.WriteLine($"N: {N}");

var congruent = busAndOffset.Select(
    bo => (
        bi: bo.bus - bo.offset,
        Ni: N / bo.bus,
        xi: Enumerable.Range(1, int.MaxValue).First(guess => (guess * N / bo.bus) % bo.bus == 1)
        )
    )
    
    
    .Aggregate(0L, (a,data) => a + (data.bi * data.Ni * data.xi));

Console.Out.WriteLine($"x: {congruent} aka {congruent % N} (mod {N})");



