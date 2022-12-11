using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MoreLinq;

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 


/* 
var mask0 = 0L;
var mask1 = 0L;
var memory = new Dictionary<long, long>();
foreach (var line in lines) {
    var parts = line.Split(" = ");
    if (parts[0] == "mask") {
        ParseMask(parts[1]);
        Console.Out.WriteLine($"Mask:\t{parts[1]}\n 0-\t{ToBinary(mask0)}\n 1-\t{ToBinary(mask1)}");
    } else {
        var address = long.Parse(parts[0][4..^1]);
        var value = long.Parse(parts[1]);
        var modified = ApplyMask(value);
        memory[address] = modified;
        Console.Out.WriteLine($"Set {address} to {ToBinary(value)} => {ToBinary(modified)}");
    }
}

var memorySum = memory.Values.Aggregate(0L, (a,b) => a + b);
Console.Out.WriteLine($"Memory sum of {memory.Values.Count} addresses: {memorySum}");

long ApplyMask(long address) => (address | mask1) & mask0;

void ParseMask(string mask) {
    mask0 = 0L; // 0 if '0', else 1
    mask1 = 0L; // 1 if '1', else 0
    foreach (var c in mask) {
        mask0 = mask0 << 1;
        mask1 = mask1 << 1;

        if (c == '0') {
            mask0 += 0;
            mask1 += 0;
        } else if (c == '1') {
            mask0 += 1;
            mask1 += 1;
        } else {
            mask0 += 1;
            mask1 += 0;
        }
    }
} */


string mask = "";
var memory = new Dictionary<string, long>();
foreach (var line in lines) {
    var parts = line.Split(" = ");
    if (parts[0] == "mask") {
        mask = parts[1];
        Console.Out.WriteLine($"Mask:\t{mask}");
    } else {
        var address = long.Parse(parts[0][4..^1]);
        var value = long.Parse(parts[1]);
        var modifiedList = ApplyMask(address);
        foreach (var modified in modifiedList) {
            memory[modified] = value;
            Console.Out.WriteLine($"Set {modified} to {value} ({ToBinary(value)}) ");
        }
    }
}

var memorySum = memory.Values.Aggregate(0L, (a,b) => a + b);
Console.Out.WriteLine($"Memory sum of {memory.Values.Count} addresses: {memorySum}");

IEnumerable<string> ApplyMask(long value) {
    var partialMask = String.Concat(mask.EquiZip(ToBinary(value), (a, b) => a switch {'0' => b, '1' => '1', 'X' => 'X', _ => '!'}));
    var numberOfX = partialMask.Count(c => c == 'X');
    return Enumerable.Range(0, (int)Math.Pow(2, numberOfX))
        .Select(v => Convert.ToString(v, 2)
        .PadLeft(numberOfX, '0'))
        .Select(m =>
            String.Concat(
                partialMask
                .Scan((index: -1, c: '*'), (a, b) => (a.index + (b == 'X' ? 1 : 0), b))
                .Skip(1)
                .Select(v => v.c == 'X' ? m[v.index] : v.c)
        ));
}

string ToBinary(long n) => Convert.ToString(n, 2).PadLeft(36, '0');