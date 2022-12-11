using MoreLinq;

Instruction SplitLine(string line) {
    var split = line.Split(' ');
    var direction = split[0] switch {
        "forward" => Direction.Forward,
        "down" => Direction.Down,
        "up" => Direction.Up,
        _ => throw new Exception("oops")
        };
    var amount = int.Parse(split[1]);
    return new Instruction {
        Direction = direction,
         Amount = amount
    };
}

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

var instructions = lines.Select(SplitLine);
Part2(instructions);


static void Part1(IEnumerable<Instruction> instructions) {
    long hPos = 0;
    long vPos = 0;
    foreach (var instruction in instructions) {
        switch (instruction.Direction) {
            case Direction.Forward:
                hPos += instruction.Amount;
                break;
            case Direction.Down:
                vPos += instruction.Amount;
                break;
            case Direction.Up:
                vPos -= instruction.Amount;
                break;    
        }
    }

    Console.Out.WriteLine($"Part 1 h: {hPos}, v: {vPos}: answer: {hPos * vPos}");
}

static void Part2(IEnumerable<Instruction> instructions) {
    int aim = 0;
    int hPos = 0;
    int vPos = 0;
    foreach (var instruction in instructions) {
        switch (instruction.Direction) {
            case Direction.Forward:
                hPos += instruction.Amount;
                vPos += aim * instruction.Amount;
                break;
            case Direction.Down:
                aim += instruction.Amount;
                break;
            case Direction.Up:
                aim -= instruction.Amount;
                break;    
        }
    }

    Console.Out.WriteLine($"Part 1 h: {hPos}, v: {vPos}: answer: {hPos * vPos}");
}

enum Direction {
    Forward,
    Down,
    Up
}

class Instruction {
    public Direction Direction;
    public int Amount;
}



