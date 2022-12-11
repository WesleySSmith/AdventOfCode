#pragma warning disable CS8524
#pragma warning disable CS8509
using MoreLinq;

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

//Part1(lines[0]);
Part2(lines[0]);

static void Part1(string line) {
   
   var index = line.ToArray()
   .Window(4)
   .Select((seq, index) => seq.Distinct().Count() == 4 ? index + 4 : (int?)null)
   .First(i => i.HasValue)
   .Value;
   
    Console.Out.WriteLine($"Part 1 Index: {index}");
}

static void Part2(string line) {
   var index = line.ToArray()
   .Window(14)
   .Select((seq, index) => seq.Distinct().Count() == 14 ? index + 14: (int?)null)
   .First(i => i.HasValue)
   .Value;
   
    Console.Out.WriteLine($"Part 2 Index: {index}");
}
