using MoreLinq;

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

var elves = lines
    .GroupAdjacent(l => l.Length > 0)
    .Where(group => group.Key)
    .Select(s => s.Select(s => int.Parse(s)));

//Part1(elves);
Part2(elves);


static void Part1(IEnumerable<IEnumerable<int>> elves) {
    var max = elves.Select(elf => elf.Sum()).Max();

    Console.Out.WriteLine($"Part 1 Max: {max}");
}

static void Part2(IEnumerable<IEnumerable<int>> elves) {
    var top3Sum = elves.Select(elf => elf.Sum()).OrderByDescending(elfTotal => elfTotal).Take(3).Sum();

    Console.Out.WriteLine($"Part 2 Sum: {top3Sum}");
}

