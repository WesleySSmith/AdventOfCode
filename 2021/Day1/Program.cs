using MoreLinq;

//string[] lines = File.ReadAllLines("input.txt"); 
string[] lines = File.ReadAllLines("sample.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");
var depths = lines.Select(int.Parse);
Part2(depths);


static void Part1(IEnumerable<int> depths) {
    int last = int.MaxValue;
    int count = 0;
    foreach (var depth in depths) {
        if (depth > last) {
            count++;
        }
        last = depth;
    }

    Console.Out.WriteLine($"Part 1 Count: {count}");
}

static void Part2(IEnumerable<int> depthsEnumerable) {
    var depths = depthsEnumerable.ToArray();
    int last = int.MaxValue;
    int count = 0;
    for (int window = 0; window < depths.Length - 2; window++) {
        var sum = depths[window..(window+3)].Sum();
        if (sum > last) {
            count++;
        }
        last = sum;
    }
    Console.Out.WriteLine($"Part 2 Count: {count}");
}

