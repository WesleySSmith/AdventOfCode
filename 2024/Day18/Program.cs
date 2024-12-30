#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using MoreLinq;
//using MoreLinq;

bool sample = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
//Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");


string NORMAL_BG      = Console.IsOutputRedirected ? "" : "\x1b[49m";
string RED_BG        = Console.IsOutputRedirected ? "" : "\x1b[41m";
string GREEN_BG      = Console.IsOutputRedirected ? "" : "\x1b[42m";
string YELLOW_BG      = Console.IsOutputRedirected ? "" : "\x1b[43m";
string X1_BG      = Console.IsOutputRedirected ? "" : "\x1b[44m";
string X2_BG      = Console.IsOutputRedirected ? "" : "\x1b[45m";

Stopwatch sw = Stopwatch.StartNew();

//Part1(lines);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");



void Part1(string[] lines)
{
    var rows = sample ? 7 : 71;
    var cols = rows;

    var map = new bool[rows+2,cols+2];

    for (int r = 0; r < rows +2; r++) {
        for (int c = 0; c < cols +2; c++) {
            if (r == 0 || c == 0 || r == rows+1 || c == cols+1) {
                map[r,c] = true;
            }
        }
    } 

    foreach (var line in lines.Take(sample ? 12 : 1024)) {
        var splots = line.Split(',');
        var row = int.Parse(splots[0]);
        var col = int.Parse(splots[1]);
        map[row+1, col+1] = true;
    }

    PrintMap1(map, new RC(1,1), new RC(rows,cols));
    var shortestPath = ShortestPath1(map, new State(1,1), new RC(rows,cols));


    Console.Out.WriteLine($"Part 1: {shortestPath}");
}



int ShortestPath1(bool[,] board, State start, RC end) {
    var dist = new Dictionary<State, int>();
    var Q = new PriorityQueue<State, int>();

    Q.Enqueue(start, 0);
    dist[(start)] = 0;

    while (Q.Count > 0) {
        var u = Q.Dequeue();
        var row = u.Row;
        var col = u.Col;

        if (row == end.Row && col == end.Col) {
            return dist[u];
        }

        var neighbors = new List<State>();

        // North
        if (!board[row -1, col]) {
            neighbors.Add(new State(row - 1, col));
        }

        // South
        if (!board[row+1, col]) {
            neighbors.Add(new State(row + 1, col));
        }

        //West
        if (!board[row, col-1]) {
            neighbors.Add(new State(row, col - 1));
        }
        //East
        if (!board[row, col+1]) {
            neighbors.Add(new State(row, col + 1));
        }
        
        foreach (var v in neighbors) {
            var alt = dist[u] + 1;
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

void PrintMap1(bool[,] map, RC start, RC end) {
    var numRows = map.GetLength(0);
    var numCols = map.GetLength(1);

    for (int r = 0; r < numRows; r++)
    {
        Console.WriteLine();
        for (int c = 0; c < numCols; c ++) {
            var ch = map[r, c];
            if (r == start.Row && c == start.Col) {
                Console.Write(GREEN_BG + (ch ? "#" : ".") + NORMAL_BG);
            } else if (r == end.Row && c == end.Col) {
                Console.Write(YELLOW_BG + (ch ? "#" : ".") + NORMAL_BG);
            }
            else if (ch) {
                Console.Write(RED_BG + "#" + NORMAL_BG);
            } else {
                Console.Write('.');
            }
        }
    }
    Console.WriteLine();
}

void Part2(string[] lines) {

     var rows = sample ? 7 : 71;
    var cols = rows;

    var map = new bool[rows+2,cols+2];

    for (int r = 0; r < rows +2; r++) {
        for (int c = 0; c < cols +2; c++) {
            if (r == 0 || c == 0 || r == rows+1 || c == cols+1) {
                map[r,c] = true;
            }
        }
    } 

    foreach (var line in lines.Take(sample ? 12 : 1024)) {
        var splots = line.Split(',');
        var row = int.Parse(splots[0]);
        var col = int.Parse(splots[1]);
        map[row+1, col+1] = true;
    }

    //PrintMap1(map, new RC(1,1), new RC(rows,cols));
    var shortestPath = ShortestPath1(map, new State(1,1), new RC(rows,cols));

    foreach (var line in lines.Skip(sample ? 12 : 1024)) {
        var splots = line.Split(',');
        var row = int.Parse(splots[0]);
        var col = int.Parse(splots[1]);
        map[row+1, col+1] = true;

        shortestPath = ShortestPath1(map, new State(1,1), new RC(rows,cols));

        if (shortestPath == -1) {
            Console.Out.WriteLine($"Part 2: {row},{col}");
            Environment.Exit(0);
        }
    }
    Console.Out.WriteLine($"Part 2: NO SOLUTION");
}

record RC(int Row, int Col);

record State {

    public State(int row, int col)
    {
        Row = row;
        Col = col;
    }
    public int Row;
    public int Col;
}

