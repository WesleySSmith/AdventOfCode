#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Text;
//using MoreLinq;

bool sample = true;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

var nodes = lines.Select(Node.Parse).ToArray();

foreach (var node in nodes) {
    node.AdjIndexes = node.Adj.Select(adj => Array.FindIndex(nodes, n => n.Name == adj)).ToList();
}


//Part1(nodes);
Part2(nodes);

Console.Out.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");


#region XXX
/*
void Part1(Node[] nodes) {

    int timeSteps = 30;
    int offset = 100_000;
    Dictionary<string, int> lookup = nodes.Select((n, i) => (n.Name, i)).ToDictionary(t => t.Item1, t => t.Item2);
    var nodesAtTime = nodes.Select(n => Enumerable.Range(0, timeSteps).Select(t => n with {Time = t, TotalFlow = n.Rate * (timeSteps - t)}).ToArray()).ToArray();
  
    var dist = new int[nodesAtTime.Length, nodesAtTime[0].Length];
    var prev = new (int, int)[nodesAtTime.Length, nodesAtTime[0].Length];
    //var target = (array.GetLength(0)-1, array.GetLength(1)-1);

    List<(int, int)> Q = new List<(int, int)>();

    for (int row = 0; row < nodesAtTime.Length; row++) {
        for (int col = 0; col < nodesAtTime[0].Length; col++) {
            dist[row, col] = int.MaxValue;
            prev[row, col] = (-1, -1);
            Q.Add((row, col));
        }
    }

    var firstNodeIndex = lookup["AA"];
    var firstNode = nodesAtTime[firstNodeIndex][0];

    dist[firstNodeIndex, 0] = 0;
    (int,int) target = (-2, -2);
    while (Q.Any()) {
        var u = Q.MinBy(q => dist[q.Item1, q.Item2]);
        Q.Remove(u);

        //Debug(u);

        if (u.Item2 == timeSteps-1) {
            target = u;
            break;
        }

        var row = u.Item1;
        var col = u.Item2;

        var neighbors = new List<(int, int, bool)>();

        var node = nodesAtTime[row][col];

        foreach (var neighbor in node.Adj) {
            neighbors.Add((lookup[neighbor], col+1, false));
        }

        // if not already on, add self as neighbor to turn on
        
        var prevprev = u;
        bool turnedOn = false;
        while (prevprev.Item1 != -1 && prevprev.Item2 != -1) {
            
            var nextprev = prev[prevprev.Item1, prevprev.Item2];
            if (prevprev.Item1 == row && nextprev.Item1 == row) {
                turnedOn = true;
                break;
            }

            prevprev = nextprev;
        }

        if (!turnedOn) {
            neighbors.Add((row,col+1, true));
        }

        var neighborsInQ = neighbors; //.Where(n => Q.Contains(n));
        foreach (var v in neighborsInQ) {
            var cost = (v.Item3 ? (offset - node.TotalFlow) : (0 + offset));
            var alt = dist[u.Item1, u.Item2] + cost;
            if (alt < dist[v.Item1, v.Item2]) {

                if (alt <0) {
                    throw new Exception("too low");
                }
                dist[v.Item1, v.Item2] = alt;
                prev[v.Item1, v.Item2] = u;
            }
        }
    }
    var totalFlow = 0; // dist[target.Item1, target.Item2];

    List<string> debugOut = new List<string>();

    var u2 = target;
    while (u2 != (-1, -1)) {
        var u2temp = prev[u2.Item1,u2.Item2];
        bool addFlow = u2.Item1 == u2temp.Item1;
        if (addFlow) {
            totalFlow += nodesAtTime[u2.Item1][u2.Item2].TotalFlow;
        }
        debugOut.Add($"{nodesAtTime[u2.Item1][u2.Item2].ToString()} // Dist: {dist[u2.Item1, u2.Item2]} // Prev: {prev[u2.Item1, u2.Item2]} // {(addFlow ? "**" : "")}");

        u2 = u2temp;
    }

    debugOut.Reverse();
    Console.WriteLine(string.Join("\n", debugOut));
    Console.WriteLine($"Part 1 Total Flow: {totalFlow}");



    void Debug((int, int) target) {
        var u2 = target;
        while (u2 != (-1, -1)) {
            //Console.WriteLine(nodesAtTime[u2.Item1][u2.Item2].Name + " @ " + nodesAtTime[u2.Item1][u2.Item2].Time);
            u2 = prev[u2.Item1,u2.Item2];
        }
        Console.WriteLine();
    }
}

*/
#endregion

void Part1(Node[] nodes) {

    int timeSteps = 30;
    Dictionary<(int, ulong, int, int Time), int> memo = new();
    int memoHits = 0;


    int Traverse(int[] path, ulong valves, int totalFlow) {

        if (path.Length == timeSteps) {
            return totalFlow;
        }

        var v = path[^1];
        var memoKey = (v, valves, totalFlow, path.Length);
        if (memo.TryGetValue(memoKey, out var answer)) {
            memoHits++;
            if (memoHits % 1_000_000 == 0) {
                Console.WriteLine($"Memo Hits {memoHits}");
            }
            return answer;
        }

        int? lastTimeHere = null;

        for (int i = path.Length - 2; i >= 0; i--) {
            if (path[i] == v) {
                lastTimeHere = i;
                break;
            }
        }

        if (lastTimeHere.HasValue && path.Length - lastTimeHere > 2) {
            var turnedOnSinceLastHere = false;
            for (int i = lastTimeHere.Value + 1; i < path.Length - 1; i++) {
                if (path[i] == path[i-1]) {
                    turnedOnSinceLastHere = true;
                    break;
                }
            }

            if (!turnedOnSinceLastHere) {
                return 0;
            }
        }

        List<int> options = new();

        if ((valves & (1UL << v)) == 0 && nodes[v].Rate > 0) {
            var pathCopy = new int[path.Length + 1];
            Array.Copy(path, pathCopy, path.Length);
            pathCopy[path.Length] = v;

            var valvesCopy = valves | (1UL << v);

            options.Add(Traverse(pathCopy, valvesCopy, totalFlow + (nodes[v].Rate * (timeSteps - path.Length))));
        }

        foreach (var adjIndex in nodes[v].AdjIndexes) {

            var pathCopy = new int[path.Length + 1];
            Array.Copy(path, pathCopy, path.Length);
            pathCopy[path.Length] = adjIndex;

            options.Add(Traverse(pathCopy, valves, totalFlow));
        }

        var best = options.Max();

        memo.TryAdd((v, valves, totalFlow, path.Length), best);
        return best;
    }

    Dictionary<string, int> lookup = nodes.Select((n, i) => (n.Name, i)).ToDictionary(t => t.Item1, t => t.Item2);
    var firstNodeIndex = lookup["AA"];
    var valves = 0UL;

    var best = Traverse(new[] {firstNodeIndex}, valves, 0);

    Console.WriteLine($"Part 1 best: {best}");

}


int[] Concat(int[] a, int i) {
    var path = new int[a.Length + 1];
    Array.Copy(a, path, a.Length);
    path[a.Length] = i;
    return path;
}

bool ValveOpen(ulong valves, int index) {
    return (valves & (1UL << index)) != 0;
}

ulong OpenValve(ulong valves, int index) {
     return valves | (1UL << index);
}

void Part2(Node[] nodes) {

    int timeSteps = 10;
    Dictionary<(int NodeIndex1, int NodeIndex2, ulong Values, int TotalFlow, int Time), int> memo = new();
    int memoHits = 0;


    int Traverse(int[] path1, int[] path2, ulong valves, int totalFlow) {

        if (path1.Length == timeSteps) {
            return totalFlow;
        }

        var v1 = path1[^1];
        var v2 = path2[^1];
        var memoKey = (v1, v2, valves, totalFlow, path1.Length);
        if (memo.TryGetValue(memoKey, out var answer)) {
            memoHits++;
            if (memoHits % 1_000_000 == 0) {
                Console.WriteLine($"Memo Hits {memoHits}");
            }
            return answer;
        }
/*
        int? lastTimeHere = null;

        for (int i = path.Length - 2; i >= 0; i--) {
            if (path[i] == v) {
                lastTimeHere = i;
                break;
            }
        }

        if (lastTimeHere.HasValue && path.Length - lastTimeHere > 2) {
            var turnedOnSinceLastHere = false;
            for (int i = lastTimeHere.Value + 1; i < path.Length - 1; i++) {
                if (path[i] == path[i-1]) {
                    turnedOnSinceLastHere = true;
                    break;
                }
            }

            if (!turnedOnSinceLastHere) {
                return 0;
            }
        }
*/
        List<int> options = new();

        
        if (v1 == v2 && (valves & (1UL << v1)) == 0 && nodes[v1].Rate > 0) {
            // Same room, available valve to optionally turn on
            {
                var pathCopy1 = Concat(path1, v1);
                var valvesCopy = valves | (1UL << v1);

                foreach (var adjIndex in nodes[v1].AdjIndexes) {
                    // 1 is turning on - 2 is moving on
                    var pathCopy2 = Concat(path2, adjIndex);
                    options.Add(Traverse(pathCopy1, pathCopy2, valvesCopy, totalFlow + (nodes[v1].Rate * (timeSteps - path1.Length))));
                }
            }

            for (int i = 0; i < nodes[v1].AdjIndexes.Count; i++) {
                for (int j = i; j < nodes[v2].AdjIndexes.Count; j++) {
                    var adjIndex1 = nodes[v1].AdjIndexes[i];
                    var adjIndex2 = nodes[v2].AdjIndexes[j];

                    var pathCopy1 = Concat(path1, adjIndex1);
                    var pathCopy2 = Concat(path2, adjIndex2);
                    options.Add(Traverse(pathCopy1, pathCopy2, valves, totalFlow));
                }
            }
        } else if (v1 == v2) {
            // Same room, NO available valve to optionally turn on

            for (int i = 0; i < nodes[v1].AdjIndexes.Count; i++) {
                for (int j = i; j < nodes[v2].AdjIndexes.Count; j++) {
                    var adjIndex1 = nodes[v1].AdjIndexes[i];
                    var adjIndex2 = nodes[v2].AdjIndexes[j];

                    var pathCopy1 = Concat(path1, adjIndex1);
                    var pathCopy2 = Concat(path2, adjIndex2);
                    options.Add(Traverse(pathCopy1, pathCopy2, valves, totalFlow));
                }
            }
        } else {
            // different rooms

            if (!ValveOpen(valves, v1) && nodes[v1].Rate > 0 && !ValveOpen(valves, v2) && nodes[v2].Rate > 0 ) {
                // both have valves that can be opened

                {
                    // both turn it on
                    var pathCopy1 = Concat(path1, v1);
                    var pathCopy2 = Concat(path2, v2);
                    var valvesCopy = OpenValve(OpenValve(valves, v1), v2);
                    options.Add(Traverse(pathCopy1, pathCopy2, valvesCopy, totalFlow + (nodes[v1].Rate * (timeSteps - path1.Length)) + (nodes[v2].Rate * (timeSteps - path2.Length))));
                }

                {
                    var pathCopy1 = Concat(path1, v1);
                    var valvesCopy = valves | (1UL << v1);

                    foreach (var adjIndex in nodes[v1].AdjIndexes) {
                        // 1 is turning on - 2 is moving on
                        var pathCopy2 = Concat(path2, adjIndex);
                        options.Add(Traverse(pathCopy1, pathCopy2, valvesCopy, totalFlow + (nodes[v1].Rate * (timeSteps - path1.Length))));
                    }
                }

                {
                    var pathCopy2 = Concat(path2, v2);
                    var valvesCopy = valves | (1UL << v2);

                    foreach (var adjIndex in nodes[v2].AdjIndexes) {
                        // 2 is turning on - 1 is moving on
                        var pathCopy1 = Concat(path1, adjIndex);
                        options.Add(Traverse(pathCopy1, pathCopy2, valvesCopy, totalFlow + (nodes[v1].Rate * (timeSteps - path1.Length))));
                    }
                }

                for (int i = 0; i < nodes[v1].AdjIndexes.Count; i++) {
                    for (int j = 0; j < nodes[v2].AdjIndexes.Count; j++) {
                        var adjIndex1 = nodes[v1].AdjIndexes[i];
                        var adjIndex2 = nodes[v2].AdjIndexes[j];

                        var pathCopy1 = Concat(path1, adjIndex1);
                        var pathCopy2 = Concat(path2, adjIndex2);
                        options.Add(Traverse(pathCopy1, pathCopy2, valves, totalFlow));
                    }
                }



            } else if (!ValveOpen(valves, v1) && nodes[v1].Rate > 0) {
                // 1 can open, 2 must move

                {
                    var pathCopy1 = Concat(path1, v1);
                    var valvesCopy = valves | (1UL << v1);

                    foreach (var adjIndex in nodes[v1].AdjIndexes) {
                        // 1 is turning on - 2 is moving on
                        var pathCopy2 = Concat(path2, adjIndex);
                        options.Add(Traverse(pathCopy1, pathCopy2, valvesCopy, totalFlow + (nodes[v1].Rate * (timeSteps - path1.Length))));
                    }
                }

                for (int i = 0; i < nodes[v1].AdjIndexes.Count; i++) {
                    for (int j = 0; j < nodes[v2].AdjIndexes.Count; j++) {
                        var adjIndex1 = nodes[v1].AdjIndexes[i];
                        var adjIndex2 = nodes[v2].AdjIndexes[j];

                        var pathCopy1 = Concat(path1, adjIndex1);
                        var pathCopy2 = Concat(path2, adjIndex2);
                        options.Add(Traverse(pathCopy1, pathCopy2, valves, totalFlow));
                    }
                }

            } else if (!ValveOpen(valves, v2) && nodes[v2].Rate > 0) {
                // 2 can open, 1 must move

                {
                    var pathCopy2 = Concat(path2, v2);
                    var valvesCopy = valves | (1UL << v2);

                    foreach (var adjIndex in nodes[v2].AdjIndexes) {
                        // 2 is turning on - 1 is moving on
                        var pathCopy1 = Concat(path1, adjIndex);
                        options.Add(Traverse(pathCopy1, pathCopy2, valvesCopy, totalFlow + (nodes[v1].Rate * (timeSteps - path1.Length))));
                    }
                }

                for (int i = 0; i < nodes[v1].AdjIndexes.Count; i++) {
                    for (int j = 0; j < nodes[v2].AdjIndexes.Count; j++) {
                        var adjIndex1 = nodes[v1].AdjIndexes[i];
                        var adjIndex2 = nodes[v2].AdjIndexes[j];

                        var pathCopy1 = Concat(path1, adjIndex1);
                        var pathCopy2 = Concat(path2, adjIndex2);
                        options.Add(Traverse(pathCopy1, pathCopy2, valves, totalFlow));
                    }
                }

            } else {
                // both must move

                for (int i = 0; i < nodes[v1].AdjIndexes.Count; i++) {
                    for (int j = 0; j < nodes[v2].AdjIndexes.Count; j++) {
                        var adjIndex1 = nodes[v1].AdjIndexes[i];
                        var adjIndex2 = nodes[v2].AdjIndexes[j];

                        var pathCopy1 = Concat(path1, adjIndex1);
                        var pathCopy2 = Concat(path2, adjIndex2);
                        options.Add(Traverse(pathCopy1, pathCopy2, valves, totalFlow));
                    }
                }

            }
        }

        var best = options.Max();

        //memo.TryAdd((v1, v2, valves, totalFlow, path1.Length), best);
        return best;
    }

    Dictionary<string, int> lookup = nodes.Select((n, i) => (n.Name, i)).ToDictionary(t => t.Item1, t => t.Item2);
    var firstNodeIndex = lookup["AA"];
    var valves = 0UL;

    var best = Traverse(new[] {firstNodeIndex}, new[] {firstNodeIndex},  valves, 0);

    Console.WriteLine($"Part 2 best: {best}");

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