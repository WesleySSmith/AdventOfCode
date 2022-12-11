#pragma warning disable CS8524
#pragma warning disable CS8509
using MoreLinq;

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Part1(lines);
Part2(lines);

static void Part1(IEnumerable<string> lines) {
   
    var pairs = lines.Select(l => l.Split(','));
    var assignments = pairs.Select(pair => pair.Select(Assignment.Parse).ToArray());
    var numOverlaps = assignments.Count(assignment => IsFullyOverlap(assignment[0], assignment[1]));

    Console.Out.WriteLine($"Part 1 Overlaps: {numOverlaps}");
}

static void Part2(IEnumerable<string> lines) {
   
    var pairs = lines.Select(l => l.Split(','));
    var assignments = pairs.Select(pair => pair.Select(Assignment.Parse).ToArray());
    var numOverlaps = assignments.Count(assignment => IsOverlap(assignment[0], assignment[1]));

    Console.Out.WriteLine($"Part 2 Overlaps: {numOverlaps}");
}



static bool IsFullyOverlap(Assignment a1, Assignment a2) {
    return 
       (a1.Min >= a2.Min && a1.Max <= a2.Max) 
    || (a2.Min >= a1.Min && a2.Max <= a1.Max);
}

static bool IsOverlap(Assignment a1, Assignment a2) {
    return ! 
       ((a1.Max < a2.Min) 
    || (a1.Min > a2.Max));
}
class Assignment {
    public int Min;
    public int Max;

    public static Assignment Parse(string assignmentString) {
        var split = assignmentString.Split('-').Select(int.Parse).ToArray();
        return new Assignment {
            Min = split[0],
            Max = split[1]
        };
    }
}

