#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using MoreLinq;

bool sample = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
//Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();


string NORMAL_BG      = Console.IsOutputRedirected ? "" : "\x1b[49m";
string RED_BG        = Console.IsOutputRedirected ? "" : "\x1b[41m";
string GREEN_BG      = Console.IsOutputRedirected ? "" : "\x1b[42m";
string YELLOW_BG      = Console.IsOutputRedirected ? "" : "\x1b[43m";
string X1_BG      = Console.IsOutputRedirected ? "" : "\x1b[44m";
string X2_BG      = Console.IsOutputRedirected ? "" : "\x1b[45m";


var minRow = 0;
var maxRow = lines.Length -1;
var minCol = 0; 
var maxCol = lines[0].Length -1;


var blocks = lines.Select(l => l.Select(c => c).ToArray()).ToArray();
int startRow = -1;
int startCol = -1;
int endRow = -1;
int endCol = -1;
var board = new char[blocks.Length, blocks[0].Length];
for (int row = minRow; row <= maxRow; row++) {
    for (int col = minCol; col <= maxCol; col++) {
        var ch = blocks[row][col];
        if (ch == 'S') {
            startRow = row;
            startCol = col;
        } else if (ch == 'E') {
            endRow = row;
            endCol = col;
        }
        board[row,col] = ch;
    }
}


//Part1();
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");



void Part1()
{
   var shortPath = ShortestPath1(board, new State(new RC(startRow, startCol), Dir.E, 0), (endRow, endCol));
    

    Console.Out.WriteLine($"Part 1: {shortPath}");



}



int ShortestPath1(char[,] board, State start, (int, int) end) {
    var dist = new Dictionary<State, int>();
    var Q = new PriorityQueue<State, int>();

    Q.Enqueue(start, 0);
    dist[(start)] = 0;

    while (Q.Count > 0) {
        var u = Q.Dequeue();
        //if (debug) {
        //    Console.WriteLine($"Considering {u}");
        //}
        
        var row = u.Pos.Row;
        var col = u.Pos.Col;
        var dir = u.Dir;

        if (row == end.Item1 && col == end.Item2) {
            return dist[u];
        }

        var neighbors = new List<State>();

        // North
        if (dir != Dir.S && board[row-1, col] is '.' or 'E') {
            neighbors.Add(new State(new RC(row - 1, col), Dir.N, dir == Dir.N ? 0 : 1000));
        }

        // South
        if (dir != Dir.N && board[row+1, col] is '.' or 'E') {
            neighbors.Add(new State(new RC(row + 1, col), Dir.S, dir == Dir.S ? 0 : 1000));
        }

        // West
        if (dir != Dir.E && board[row, col-1] is '.' or 'E') {
            neighbors.Add(new State(new RC(row, col - 1), Dir.W, dir == Dir.W ? 0 : 1000));
        }

        // East
        if (dir != Dir.W && board[row, col+1] is '.' or 'E') {
            neighbors.Add(new State(new RC(row, col + 1), Dir.E, dir == Dir.E ? 0 : 1000));
        }
        
        foreach (var v in neighbors) {
            var alt = dist[u] + v.TurnCost + 1;
            if (alt < (dist.TryGetValue(v, out var d) ? d : int.MaxValue)) {
                dist[v] = alt;
                if (Q.UnorderedItems.All(i => i.Element != v)) {
                    Q.Enqueue(v, alt);
                }
            }
        }
    }

    return -1;
}

void Part2(string[] lines) {
    var shortPath = ShortestPath2(board, new State(new RC(startRow, startCol), Dir.E, 0), new RC(endRow, endCol));

    Console.Out.WriteLine($"Part 2: {shortPath}");

}


int ShortestPath2(char[,] board, State start, RC end) {

    var prev = new Dictionary<State, List<State>>();
    var dist = new Dictionary<State, int>();
    var Q = new PriorityQueue<State, int>();

    Q.Enqueue(start, 0);
    dist[(start)] = 0;

    while (Q.Count > 0) {
        var u = Q.Dequeue();
        //if (debug) {
        //    Console.WriteLine($"Considering {u}");
        //}
        
        var row = u.Pos.Row;
        var col = u.Pos.Col;
        var dir = u.Dir;

        if (row == end.Row && col == end.Col) {
            // No neighbors from the end node.
            continue;
        }


        var neighbors = new List<State>();

        // North
        var rc = u.Pos with {Row = u.Pos.Row -1};
        if (dir != Dir.S && board[rc.Row, rc.Col] is '.' or 'E') {
            neighbors.Add(new State(rc, Dir.N, dir == Dir.N ? 0 : 1000));
        }

        // South
        rc = u.Pos with {Row = u.Pos.Row +1};
        if (dir != Dir.N && board[rc.Row, rc.Col] is '.' or 'E') {
            neighbors.Add(new State(rc, Dir.S, dir == Dir.S ? 0 : 1000));
        }

        // West
        rc = u.Pos with {Col = u.Pos.Col -1};
        if (dir != Dir.E && board[rc.Row, rc.Col] is '.' or 'E') {
            neighbors.Add(new State(rc, Dir.W, dir == Dir.W ? 0 : 1000));
        }

        // East
        rc = u.Pos with {Col = u.Pos.Col +1};
        if (dir != Dir.W && board[rc.Row, rc.Col] is '.' or 'E') {
            neighbors.Add(new State(rc, Dir.E, dir == Dir.E ? 0 : 1000));
        }
        
        foreach (var v in neighbors) {
            var alt = dist[u] + v.TurnCost + 1;
            if (alt <= (dist.TryGetValue(v, out var d) ? d : int.MaxValue)) {
                dist[v] = alt;
                if (!prev.TryGetValue(v, out var prevList)) {
                    prevList = [];
                    prev[v] = prevList;
                }
                prevList.Add(u);
                if (Q.UnorderedItems.All(i => i.Element != v)) {
                    Q.Enqueue(v, alt);
                }
            }
        }
    }

    var best = dist
        .Where(s => s.Key.Pos == end)
        .OrderBy(s => s.Value)
        .First();
    Console.Out.WriteLine($"dist[end]: {best.Key}");

    HashSet<RC> hashmap = [end];

    Stack<(State State, List<RC> Path)> stack = new();

    stack.Push((best.Key, new List<RC>()));

    while (stack.Count > 0) {
        var n = stack.Pop();
        if (n.State.Pos == start.Pos) {
            foreach (var n2 in n.Path) {
                hashmap.Add(n2);
            }
        } else {
            foreach(var prevN in prev[n.State]) {
                stack.Push((prevN, [prevN.Pos, .. n.Path]));
            }
        }
    }  

    return hashmap.Count;
}

record State {

    public State(RC pos, Dir dir, int turnCost)
    {
        Pos = pos;
        Dir = dir;
        TurnCost = turnCost;
    }
    public RC Pos;
    public Dir Dir;
    public int TurnCost;
}
enum Dir: byte {
    N,
    S,
    E,
    W,
    U
}

record RC (int Row, int Col);
 

