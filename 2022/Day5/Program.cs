#pragma warning disable CS8524
#pragma warning disable CS8509
using MoreLinq;

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

//Part1(lines);
Part2(lines);

static void Part1(IEnumerable<string> linesEnum) {
   
    var lines = linesEnum.ToArray();
    var split = Array.FindIndex(lines, l => l.Length == 0);
    var stack = lines[..(split-1)];
    var counts = lines[split-1];
    var instuctions = lines[(split+1)..];

    var numStacks = counts.Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;
    var stacks = new Stack<char>[numStacks];
    for (int ii = 0; ii < numStacks; ii++) {
        stacks[ii] = new Stack<char>();
    }

    var reversedStack = stack.Reverse();
    foreach (var s in reversedStack) {
        var offset = 0;
        for (int ii = 0; ii < numStacks; ii++, offset += 4 ) {
            var c = s[offset+1];
            if (c != ' ') {
                stacks[ii].Push(c);
            }
        }
    }

    var moves = instuctions.Select(Move.Parse);

    foreach (var move in moves) {
        for (int ii = 0; ii < move.Count; ii++) {
            stacks[move.To-1].Push(stacks[move.From-1].Pop());
        }
    }


    var message = string.Join("", stacks.Select(stack => stack.Peek()));
    

    Console.Out.WriteLine($"Part 1 Message: {message}");
}

static void Part2(IEnumerable<string> linesEnum) {
   
    var lines = linesEnum.ToArray();
    var split = Array.FindIndex(lines, l => l.Length == 0);
    var stack = lines[..(split-1)];
    var counts = lines[split-1];
    var instuctions = lines[(split+1)..];

    var numStacks = counts.Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;
    var stacks = new Stack<char>[numStacks];
    for (int ii = 0; ii < numStacks; ii++) {
        stacks[ii] = new Stack<char>();
    }

    var reversedStack = stack.Reverse();
    foreach (var s in reversedStack) {
        var offset = 0;
        for (int ii = 0; ii < numStacks; ii++, offset += 4 ) {
            var c = s[offset+1];
            if (c != ' ') {
                stacks[ii].Push(c);
            }
        }
    }

    var moves = instuctions.Select(Move.Parse);

    foreach (var move in moves) {
        var tempStack = new Stack<char>();
        for (int ii = 0; ii < move.Count; ii++) {
            tempStack.Push(stacks[move.From-1].Pop());
        }
        for (int ii = 0; ii < move.Count; ii++) {
            stacks[move.To-1].Push(tempStack.Pop());
        }
    }

    var message = string.Join("", stacks.Select(stack => stack.Peek()));
    

    Console.Out.WriteLine($"Part 2 Message: {message}");
}

class Move {
    public int Count;
    public int From;
    public int To;

    public static Move Parse(string s) {
        var parts = s.Split(' ');
        return new Move {
            Count = int.Parse(parts[1]),
            From = int.Parse(parts[3]),
            To = int.Parse(parts[5])
        };
    }
}

