#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Diagnostics;
using MoreLinq;

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

var monkeys = lines
    .GroupAdjacent(l => l.Length > 0)
    .Where(group => group.Key)
    .Select(Monkey.Parse).ToArray();


//Part1(monkeys);
Part2(monkeys);

Console.Out.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");


static void Part1(Monkey[] monkeys) {

    for (int round =0; round < 20; round++) {

        foreach (var monkey in monkeys) {

            foreach (var item in monkey.ItemWorryLevels) {
                monkey.InspectionCount++;
                var worryLevel = item;
                
                worryLevel = monkey.Operation switch {
                    Operation.Add => worryLevel + monkey.Operand,
                    Operation.Multiply => worryLevel * monkey.Operand,
                    Operation.Square => worryLevel * worryLevel
                };
                
                worryLevel /= 3;

                var testResult = worryLevel % monkey.TestDivisibleBy == 0;
                var monkeyThrowTo = monkeys[testResult switch {
                    true => monkey.TrueMonkey,
                    false => monkey.FalseMonkey
                }];
                monkeyThrowTo.ItemWorryLevels.Add(worryLevel);
            }
            monkey.ItemWorryLevels.Clear();
        }
    }

    var top2 = monkeys.Select(m => m.InspectionCount).OrderByDescending(c => c).Take(2);
    var monkeyBusiness = top2.ElementAt(0) * top2.ElementAt(1);

    Console.Out.WriteLine($"Part 1 MonkeyBusiness: {monkeyBusiness}");
}

static void Part2(Monkey[] monkeys) {
    checked {

        var modOperand = monkeys.Select(m => m.TestDivisibleBy).Aggregate(1, (v1, v2) => v1 * v2);
        Console.Out.WriteLine($"modOperand: {modOperand}");
        for (int round =0; round < 10000; round++) {

            foreach (var monkey in monkeys) {

                foreach (var item in monkey.ItemWorryLevels) {
                    monkey.InspectionCount++;
                    var worryLevel = item;
                    
                    worryLevel = monkey.Operation switch {
                        Operation.Add => worryLevel + monkey.Operand,
                        Operation.Multiply => worryLevel * monkey.Operand,
                        Operation.Square => worryLevel * worryLevel
                    };
                    
                    worryLevel %= modOperand;

                    var testResult = worryLevel % monkey.TestDivisibleBy == 0;
                    var monkeyThrowTo = monkeys[testResult switch {
                        true => monkey.TrueMonkey,
                        false => monkey.FalseMonkey
                    }];
                    monkeyThrowTo.ItemWorryLevels.Add(worryLevel);
                }
                monkey.ItemWorryLevels.Clear();
            }
        }
    }
    
    Console.Out.WriteLine(string.Join(",", monkeys.Select(m => m.InspectionCount)));
    var top2 = monkeys.Select(m => m.InspectionCount).OrderByDescending(c => c).Take(2).Select(i => (long)i);
    
    
    var monkeyBusiness = top2.ElementAt(0) * top2.ElementAt(1);

    Console.Out.WriteLine($"Part 2 MonkeyBusiness: {monkeyBusiness}");
}

public enum Operation {
    Add,
    Multiply,
    Square
}

public record Monkey {
    public  List<long> ItemWorryLevels {get; init;}
    public Operation Operation {get; init;}
    public int Operand {get; init;}
    public int TestDivisibleBy {get; init;}
    public int TrueMonkey {get; init;}
    public int FalseMonkey {get; init;}
    public int InspectionCount {get; set;}

    public static Monkey Parse(IEnumerable<string> section) {

        var line = section.ElementAt(1);
        var prefix = "  Starting items: ";
        if (!line.StartsWith(prefix)) {
            throw new Exception("E1");
        }
        var remainder = line[(prefix.Length)..];
        var startingItems = remainder.Split(",").Select(long.Parse).ToList();
        
        line = section.ElementAt(2);
        prefix = "  Operation: new = old ";
        if (!line.StartsWith(prefix)) {
            throw new Exception("E2");
        }
        remainder = line[(prefix.Length)..];
        var op = remainder[0];
        Operation operation;
        int operand;
        if (op == '+') {
            operation = Operation.Add;
            operand = int.Parse(remainder[2..]);
        } else if (op == '*') {
            if (remainder[2..] == "old") {
                operation = Operation.Square;
                operand = 0;
            } else {
                operation = Operation.Multiply;
                operand = int.Parse(remainder[2..]);
            }
        } else {
            throw new Exception("Q1");
        }


        line = section.ElementAt(3);
        prefix = "  Test: divisible by ";
        if (!line.StartsWith(prefix)) {
            throw new Exception("E3");
        }
        remainder = line[(prefix.Length)..];
        var testDivisibleBy = int.Parse(remainder);


        line = section.ElementAt(4);
        prefix = "    If true: throw to monkey ";
        if (!line.StartsWith(prefix)) {
            throw new Exception("E4");
        }
        remainder = line[(prefix.Length)..];
        var trueMonkey = int.Parse(remainder);


        line = section.ElementAt(5);
        prefix = "    If false: throw to monkey ";
        if (!line.StartsWith(prefix)) {
            throw new Exception("E5");
        }
        remainder = line[(prefix.Length)..];
        var falseMonkey = int.Parse(remainder);

        return new Monkey() {
            ItemWorryLevels = startingItems,
            Operation= operation,
            Operand = operand,
            TestDivisibleBy = testDivisibleBy,
            TrueMonkey = trueMonkey,
            FalseMonkey = falseMonkey
        };
    }
}


