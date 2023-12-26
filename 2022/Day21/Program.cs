#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Text;
//using MoreLinq;

bool sample = false;
bool debug = true;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input-gavin.txt"); 
if (debug) Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");


Stopwatch sw;

var monkeys = lines.Select(Monkey.Parse).ToList();

sw = Stopwatch.StartNew();
//Part1(monkeys);
Part2(monkeys);

Console.Out.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");

void Part1(IList<Monkey> monkeys) {
   
    long DFS(Monkey monkey) {
        if (monkey.Value.HasValue) {
            return monkey.Value.Value;
        }
        long l = DFS(monkey.Left);
        long r = DFS(monkey.Right);
        return monkey.Operator switch {
            '+' => l + r,
            '-' => l - r,
            '*' => l * r,
            '/' => l / r
        };
    }

    var monkeyDict = monkeys.ToDictionary(m => m.Name, m => m);
    Monkey root = null;

    foreach(var monkey in monkeys) {
        if (monkey.LeftMonkeyName != null) {
            monkey.Left = monkeyDict[monkey.LeftMonkeyName];
        }
        if (monkey.RightMonkeyName != null) {
            monkey.Right = monkeyDict[monkey.RightMonkeyName];
        }
        if (monkey.Name == "root") {
            root = monkey;
        }
    }

    var rootNum = DFS(root);


    Console.WriteLine($"Part 1 Sum: {rootNum}");
}


void Part2(IList<Monkey> monkeys) {
   
    long DFS(Monkey monkey) {
        if (monkey.Value.HasValue) {
            monkey.ComputedValue = monkey.Value.Value;
            return monkey.Value.Value;
        }
        long l = DFS(monkey.Left);
        long r = DFS(monkey.Right);
        monkey.ComputedValue = monkey.Operator switch {
            '+' => l + r,
            '-' => l - r,
            '*' => l * r,
            '/' => l / r
        };
        return monkey.ComputedValue;
    }

    bool? FindHuman(Monkey monkey, Monkey human) {

        if (monkey.Value.HasValue) {
            return null;
        }

        if (monkey.Left == human) {
            monkey.HumanOnRight = false;
        } else if (monkey.Right == human) {
            monkey.HumanOnRight = true;
        } else if (FindHuman(monkey.Left, human).HasValue) {
            monkey.HumanOnRight = false;
        } else if (FindHuman(monkey.Right, human).HasValue) {
            monkey.HumanOnRight = true;
        }
        return monkey.HumanOnRight;
    }

    long FindHumanValue(Monkey monkey, Monkey human, long goal) {

        if (monkey == human) {
            return goal;
        }

        if (monkey.HumanOnRight.Value) {
            // Human on Right
            var l = monkey.Left.ComputedValue;
            var nextGoal = monkey.Operator switch {
            '+' => goal - l,
            '-' => l - goal,
            '*' => goal / l,
            '/' => l / goal
            };
            return FindHumanValue(monkey.Right, human, nextGoal);
        } else {
            // Human on Left
            var r = monkey.Right.ComputedValue;
            var nextGoal = monkey.Operator switch {
            '+' => goal - r,
            '-' => goal + r,
            '*' => goal / r,
            '/' => goal * r
            };
            return FindHumanValue(monkey.Left, human, nextGoal);
        }

       
    }


    var monkeyDict = monkeys.ToDictionary(m => m.Name, m => m);
    Monkey root = null;
    Monkey human = null;

    foreach(var monkey in monkeys) {
        if (monkey.LeftMonkeyName != null) {
            monkey.Left = monkeyDict[monkey.LeftMonkeyName];
        }
        if (monkey.RightMonkeyName != null) {
            monkey.Right = monkeyDict[monkey.RightMonkeyName];
        }
        if (monkey.Name == "root") {
            root = monkey;
        }
        if (monkey.Name == "humn") {
            human = monkey;
        }
    }

    var humanOnRight = FindHuman(root, human);
    Console.WriteLine($"Human on right: {humanOnRight}");
    var ignore = DFS(root);
    var goal = humanOnRight.Value ? root.Left.ComputedValue : root.Right.ComputedValue;
    var humanRoot = humanOnRight.Value? root.Right: root.Left;

    Console.WriteLine($"goal: {goal}");
    Console.WriteLine($"humanRoot: {humanRoot.Name}");

    var humanValue = FindHumanValue(humanRoot, human, goal);
    Console.WriteLine($"Part 2 Human says: {humanValue}");
}

record Monkey() {

    public string Name {get; init;}
    public char Operator {get; init;}
    public string LeftMonkeyName { get; init;}
    public string RightMonkeyName { get; init;}
    public int? Value { get; init;}
    public long ComputedValue {get; set;}
    public Monkey Left {get; set;}
    public Monkey Right {get; set;}
    public bool? HumanOnRight {get; set;}

    public static Monkey Parse(string s) {
        var parts = s.Split(": ");
        var parts2 = parts[1].Split(" ");
        if (parts2.Length == 1) {
            return new Monkey {Name = parts[0], Value = int.Parse(parts2[0])};
        }
        return new Monkey {
                Name = parts[0],
                LeftMonkeyName = parts2[0],
                RightMonkeyName = parts2[2],
                Operator = parts2[1][0]};
    }

}





