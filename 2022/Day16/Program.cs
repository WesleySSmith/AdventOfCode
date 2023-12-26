#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Text;
//using MoreLinq;

bool sample = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input-gavin.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

var nodes = lines.Select(Node.Parse).ToArray();

foreach (var node in nodes) {
    node.AdjIndexes = node.Adj.Select(adj => Array.FindIndex(nodes, n => n.Name == adj)).ToList();
}

 var valveIndexes = nodes.Select((n,i) => (n,i)).Where(n => n.n.Rate > 0).Select(n => n.i).ToArray();

    var nV = nodes.Length;

    var A1 = new int[nV, nV];
    for (int i = 0; i < nV; i++) {
        for (int j = 0; j < nV; j++) {
            A1[i,j] = 9999;
        }
    }

    for (int i = 0; i < nV; i++) {
        foreach (var adjIndex in nodes[i].AdjIndexes) {
            A1[i, adjIndex] = 1;
            A1[adjIndex, i] = 1;
        }
    }


    int[,] matrix = A1;

    for (var k = 0; k < nV; k++) {
      for (var i = 0; i < nV; i++) {
        for (var j = 0; j < nV; j++) {
          if (matrix[i,k] + matrix[k,j] < matrix[i,j])
            matrix[i,j] = matrix[i,k] + matrix[k,j];
        }
      }
    }

Dictionary<(int currentValve, int timeLeft, ulong valves), (ulong Valves, int Score, string Log)> memo1 = new();
Dictionary<(int currentValve, int timeLeft, ulong valves), (ulong Valves, int Score, string Log)> memo2 = new();
int memoHits1 = 0;
int memoHits2 = 0;
var firstNodeIndex = Array.FindIndex(nodes, n => n.Name == "AA");

//Part1(nodes);
Part2(nodes);

Console.Out.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");


bool ValveOpen(ulong valves, int index) {
    return (valves & (1UL << index)) != 0;
}

ulong OpenValve(ulong valves, int index) {
     return valves | (1UL << index);
}


(ulong Valves, int Score, string Log) Traverse1(int currentValve, int timeLeft, ulong valves) {

        if (memo1.TryGetValue((currentValve, timeLeft, valves), out var answer)) {
            memoHits1++;
            if (memoHits1 % 1_000_000 == 0) {
                Console.WriteLine($"Memo 1 Hits {memoHits1}");
            }
            return answer;
        }

        var valvesAfterOpening = OpenValve(valves, currentValve);
        List<(ulong, int, string)> options = new();
        foreach(var valve in valveIndexes) {
            if (!ValveOpen(valvesAfterOpening, valve)) {
                int timeToValve = matrix[currentValve, valve];
                if (timeToValve + 1 <= timeLeft) {
                    options.Add(Traverse1(valve, timeLeft - timeToValve - 1, valvesAfterOpening));
                }
            }
        }

        if (options.Count == 0) {
            options.Add((valvesAfterOpening, 0, null /*$"No more @ {timeLeft}" */));
            // No time to visit any more valves
        }

        var best = options.MaxBy(o => o.Item2);
        var waterForThisValve = nodes[currentValve].Rate * timeLeft;
        best.Item2 += waterForThisValve;
        //best.Item3 += $"\n{currentValve} @ {timeLeft}s adding {waterForThisValve}";
        memo1.Add((currentValve, timeLeft, valves), best);
        return best;
    }



    (ulong Valves, int Score, string Log) Traverse2(int currentValve, int timeLeft, ulong valves) {

        if (memo2.TryGetValue((currentValve, timeLeft, valves), out var answer)) {
            memoHits2++;
            if (memoHits2 % 1_000_000 == 0) {
                Console.WriteLine($"Memo 2 Hits {memoHits2}");
            }
            return answer;
        }

        var valvesAfterOpening = OpenValve(valves, currentValve);
        List<(ulong, int, string)> options = new();
        foreach(var valve in valveIndexes) {
            if (!ValveOpen(valvesAfterOpening, valve)) {
                int timeToValve = matrix[currentValve, valve];
                if (timeToValve + 1 <= timeLeft) {
                    options.Add(Traverse2(valve, timeLeft - timeToValve - 1, valvesAfterOpening));
                }
            }
        }
        options.Add(Traverse1(firstNodeIndex, 26, valvesAfterOpening));

        var best = options.MaxBy(o => o.Item2);
        var waterForThisValve = nodes[currentValve].Rate * timeLeft;
        best.Item2 += waterForThisValve;
        //best.Item3 += $"\n{currentValve} @ {timeLeft}s adding {waterForThisValve}";
        memo2.Add((currentValve, timeLeft, valves), best);
        return best;
    }

void Part1(Node[] nodes) {
    
    var answer = Traverse1(firstNodeIndex, 26, 0UL);
    //Console.WriteLine(answer.Log);
    Console.WriteLine($"Part 1a best: {answer.Score}");
}

void Part2(Node[] nodes) {
    var answer = Traverse2(firstNodeIndex, 26, 0UL);
    //Console.WriteLine(answer.Log);
    Console.WriteLine($"Part 2 best: {answer.Score}");
}



record Node {
    public string Name {get; init;}
    public int Rate {get; init;}
    //public List<Node> AdjNodes {get;} = new ();
    public List<string> Adj {get; init;}

    public List<int> AdjIndexes {get; set;}

    public override string ToString()
    {
        return $"Name: ${Name} / Rate: {Rate} / Adj: {string.Join(", ", Adj)}";
    }
    public static Node Parse(string s) {
        var parts = s.Split(" ");

        return new Node {
            Name = parts[1],
            Rate = int.Parse(parts[4][5..^1]),
            Adj = string.Join(" ", parts[9..]).Split(", ").ToList()
        };

    }
}