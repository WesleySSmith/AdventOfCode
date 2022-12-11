#pragma warning disable CS8524
#pragma warning disable CS8509
using MoreLinq;

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Part1(lines);
Part2(lines);

static void Part1(IEnumerable<string> lines) {
   
    var sacks = lines.Select(l => (l[0..(l.Length / 2)].ToArray(), l[(l.Length / 2)..].ToArray()));
    var intersect = sacks.Select(pair => pair.Item1.Intersect(pair.Item2).Single());
    var scores = intersect.Select(c => c >= 'a' && c <= 'z' ? c - 'a' + 1 : c - 'A' + 27);
    Console.Out.WriteLine($"Part 1 Total Score: {scores.Sum()}");
}

static void Part2(IEnumerable<string> lines) {
   
    var groups = lines.Chunk(3);
    var badges = groups.Select(g => g.ToArray()).Select(g => g[0].Intersect(g[1]).Intersect(g[2]).Single());
    var scores = badges.Select(c => c >= 'a' && c <= 'z' ? c - 'a' + 1 : c - 'A' + 27);
    Console.Out.WriteLine($"Part 2 Total Score: {scores.Sum()}");
}


