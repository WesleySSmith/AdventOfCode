#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Diagnostics;
using MoreLinq;

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

var moves = lines.Select(l => l == "noop" 
? new Step {Instruction = Instruction.Noop} 
: new Step {Instruction = Instruction.AddX, Argument = int.Parse(l[4..])});

Part1(moves);
//Part2(moves);

Console.Out.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");

static void Part1(IEnumerable<Step> steps) {

    List<int> log = new List<int>();
    int Clock = 1;
    int X = 1; 
    
    foreach(var step in steps) {
        log.Add(X);
        Clock += 1;
        if (step.Instruction == Instruction.AddX) 
        {
            log.Add(X);
            X += step.Argument;
            Clock += 1;
        }
    }

    
    //Console.Out.WriteLine(string.Join("\n", log.Select((l,i) => ((l.Clock + 20) % 40 == 0 ? "* " : "  ") + $"{l.Clock}:  {l.X}")));

    var sumSignalStrength = log.Select((l,i) => (l,i)).Where(l => (l.i + 1 + 20) % 40 == 0).Sum(l => l.l * (l.i + 1));

    Console.Out.WriteLine($"Part 1 Sum: {sumSignalStrength}");
}


static void Part2(IEnumerable<Step> steps) {

    List<int> log = new List<int>();
    int Clock = 1;
    int X = 1; 
    
    foreach(var step in steps) {
        log.Add(X);
        Clock += 1;
        if (step.Instruction == Instruction.AddX) 
        {
            log.Add(X);
            X += step.Argument;
            Clock += 1;
        }
    }

    var crt = string.Join("\n", 
        log.Batch(40)
        .Select(line => 
            new String(
                line.Select((log, i) =>
                    (i >= log -1 && i <= log + 1) ? '#' : ' ')
                .ToArray()
            )
        )
    );

    Console.Out.WriteLine($"Part 2 CRT: \n{crt}");
}

public enum Instruction {
    Noop,
    AddX
}

public record Step {
    public Instruction Instruction {get; init;}
    public int Argument {get; init;}
}


