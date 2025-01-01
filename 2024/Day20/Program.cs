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

string NORMAL_FG      = Console.IsOutputRedirected ? "" : "\x1b[39m";
string BRIGHT_BLUE_FG      = Console.IsOutputRedirected ? "" : "\x1b[94m";
string BRIGHT_MAGENTA_FG      = Console.IsOutputRedirected ? "" : "\x1b[95m";

string RESET_CONSOLE = Console.IsOutputRedirected ? "" : "\x1b[0m";

Stopwatch sw = Stopwatch.StartNew();



var minRow = 0;
var maxRow = lines.Length -1;
var minCol = 0; 
var maxCol = lines[0].Length -1;


var blocks = lines.Select(l => l.Select(c => c).ToArray()).ToArray();
int startRow = -1;
int startCol = -1;
int endRow = -1;
int endCol = -1;
var board = new bool[blocks.Length, blocks[0].Length];
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
        board[row,col] = ch is 'S' or 'E' or '.';
    }
}

RC start = new RC(startRow, startCol);
RC end = new RC(endRow, endCol);


// var test = ReachableFrom(new RC(10, 10), 2);
// test.ForEach(x => {
//     Console.WriteLine(x);
// });
// var testMap = new bool[20, 20];
// for (int row = 0; row < 20; row++) {
//     for (int col = 0; col < 20; col++) {
//         testMap[row, col] = test.Contains(new RC(row, col));
//     }
// }
// PrintMapTest(testMap);


//Part1();
Part2();

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");

void Part1()
{
   var path = ShortestPath1(board, new RC(startRow, startCol), new RC(endRow, endCol));


    //PrintMap1(map, new RC(1,1), new RC(rows,cols));
    //var shortestPath = ShortestPath1(map, new State(1,1), new RC(rows,cols));

    Dictionary<RC, int> pathPos = new();
    for (int i = 0; i < path.Count; i++) {
        var pos = path[i];
        pathPos.Add(pos, i);
    }

    Dictionary<int, int> LengthSavedToCountD = new();
    for (int i = 0; i < path.Count; i++)
    {
        var currentRc = path[i];
        var currentPathPos = pathPos[currentRc];
        RC cheatRc;
        int cheatPos;
        // North cheat
        cheatRc = currentRc with { Row = currentRc.Row - 2 };
        NewMethod(pathPos, LengthSavedToCountD, currentPathPos, currentRc, cheatRc);

        // South cheat
        cheatRc = currentRc with { Row = currentRc.Row + 2 };
        NewMethod(pathPos, LengthSavedToCountD, currentPathPos, currentRc, cheatRc);

        // West cheat
        cheatRc = currentRc with { Col = currentRc.Col - 2 };
        NewMethod(pathPos, LengthSavedToCountD, currentPathPos, currentRc, cheatRc);

        // East cheat
        cheatRc = currentRc with { Col = currentRc.Col + 2 };
        NewMethod(pathPos, LengthSavedToCountD, currentPathPos, currentRc, cheatRc);
    }

    LengthSavedToCountD.OrderBy(kvp => kvp.Key).ForEach(kvp => {
        //Console.WriteLine($"{kvp.Value} cheats that save {kvp.Key}");
    });

    var totalCheats = LengthSavedToCountD.Where(kvp => kvp.Key >= (sample ? 40 : 100)).Sum(kvp => kvp.Value);

    Console.Out.WriteLine($"Part 1: {totalCheats}");

    void NewMethod(Dictionary<RC, int> pathPos, Dictionary<int, int> LengthSavedToCountD, int currentPos, RC currentRc, RC cheatRc)
    {
        int cheatPos;
        if (pathPos.TryGetValue(cheatRc, out cheatPos))
        {
            var lengthSaved = currentPos - cheatPos - 2;
            if (lengthSaved > 1) {
                LengthSavedToCountD[lengthSaved] = LengthSavedToCountD.TryGetValue(lengthSaved, out var numSaves) ? numSaves + 1 : 1;
                //PrintMap1(board, pathPos, start, end, currentRc, cheatRc);
            }
        }

    }
}





List<RC> ShortestPath1(bool[,] board, RC start, RC end) {
    var prev = new Dictionary<RC, RC>();
    var dist = new Dictionary<RC, int>();
    var Q = new PriorityQueue<RC, int>();

    Q.Enqueue(start, 0);
    dist[(start)] = 0;
    prev.Add(start, null);

    while (Q.Count > 0) {
        var u = Q.Dequeue();
        var row = u.Row;
        var col = u.Col;

        if (row == end.Row && col == end.Col) {
            break;
        }

        var neighbors = new List<RC>();

        // North
        if (board[row -1, col]) {
            neighbors.Add(new RC(row - 1, col));
        }

        // South
        if (board[row+1, col]) {
            neighbors.Add(new RC(row + 1, col));
        }

        //West
        if (board[row, col-1]) {
            neighbors.Add(new RC(row, col - 1));
        }
        //East
        if (board[row, col+1]) {
            neighbors.Add(new RC(row, col + 1));
        }
        
        foreach (var v in neighbors) {
            var alt = dist[u] + 1;
            if (alt < (dist.TryGetValue(v, out var d) ? d : int.MaxValue)) {
                dist[v] = alt;
                prev[v] = u; 
                if (Q.UnorderedItems.All(i => i.Element != v)) {
                    Q.Enqueue(v, alt);
                }
            }
        }
    }

    List<RC> path = new();
    var node = end;
    while (node != null) {
        path.Add(node);
        node = prev[node];
    }
    path.Reverse();
    return path;
}

void PrintMap1(bool[,] map, Dictionary<RC, int> pathPos, RC start, RC end, RC cheatStart, RC cheatEnd) {
    var numRows = map.GetLength(0);
    var numCols = map.GetLength(1);

    for (int r = 0; r < numRows; r++)
    {
        Console.WriteLine();
        for (int c = 0; c < numCols; c ++) {
            var legal = map[r, c];
            string ch;
            if (legal) {
                ch = pathPos[new RC(r, c)].ToString().PadLeft(2, '0');
            } else {
                ch = "##";
            }

            bool specialColor = false;
            if (r == cheatStart.Row && c == cheatStart.Col) {
                Console.Write(BRIGHT_BLUE_FG);
                specialColor = true;
            } else if (r == cheatEnd.Row && c == cheatEnd.Col) {
                Console.Write(BRIGHT_MAGENTA_FG);
                specialColor = true;
            }

            if (r == start.Row && c == start.Col) {
                Console.Write(GREEN_BG);
                specialColor = true;
            } else if (r == end.Row && c == end.Col) {
                Console.Write(YELLOW_BG);
                specialColor = true;
            }

            Console.Write(ch);
            if (specialColor) {
                Console.Write(RESET_CONSOLE);
            }
        }
    }
    Console.WriteLine();
}

void PrintMapTest(bool[,] map) {
    var numRows = map.GetLength(0);
    var numCols = map.GetLength(1);

    for (int r = 0; r < numRows; r++)
    {
        Console.WriteLine();
        for (int c = 0; c < numCols; c ++) {
            var legal = map[r, c];
            string ch;
            if (legal) {
                ch = "O";
            } else {
                ch = ".";
            }
            Console.Write(ch);
        }
    }
    Console.WriteLine();
}

void Part2()
{
   var path = ShortestPath1(board, new RC(startRow, startCol), new RC(endRow, endCol));


    //PrintMap1(map, new RC(1,1), new RC(rows,cols));
    //var shortestPath = ShortestPath1(map, new State(1,1), new RC(rows,cols));

    Dictionary<RC, int> pathPos = new();
    for (int i = 0; i < path.Count; i++) {
        var pos = path[i];
        pathPos.Add(pos, i);
    }

    Dictionary<int, int> LengthSavedToCountD = new();
    for (int i = 0; i < path.Count; i++)
    {
        // if (i > 10) {
        //     break;
        // }

        var currentRc = path[i];
        var currentPathPos = pathPos[currentRc];
        int cheatPos;

        foreach (var cheatRc in ReachableFrom(currentRc, 20 /* XXX 20*/)) {
            NewMethod(pathPos, LengthSavedToCountD, currentPathPos, currentRc, cheatRc);
        }
    }

    LengthSavedToCountD.OrderBy(kvp => kvp.Key).ForEach(kvp => {
        Console.WriteLine($"{kvp.Value} cheats that save {kvp.Key}");
    });

    var minCutoff = sample ? 50 : 100;
    var totalCheats = LengthSavedToCountD.Where(kvp => kvp.Key >= minCutoff).Sum(kvp => kvp.Value);

    Console.Out.WriteLine($"Part 2: {totalCheats} > {minCutoff} picoseconds");

    void NewMethod(Dictionary<RC, int> pathPos, Dictionary<int, int> LengthSavedToCountD, int currentPos, RC currentRc, RC cheatRc)
    {
        int cheatPos;
        if (pathPos.TryGetValue(cheatRc, out cheatPos))
        {
            var lengthSaved = currentPos - cheatPos - (Math.Abs(cheatRc.Row -currentRc.Row) + Math.Abs(cheatRc.Col - currentRc.Col));
            if (lengthSaved > 1) {
                LengthSavedToCountD[lengthSaved] = LengthSavedToCountD.TryGetValue(lengthSaved, out var numSaves) ? numSaves + 1 : 1;
                //PrintMap1(board, pathPos, start, end, currentRc, cheatRc);
            }
        }

    }
}

List<RC> ReachableFrom(RC start, int radius) {
    List<RC> result = new(radius * radius / 2);
    for (int r = -radius; r <= radius; r++) {
        for (int c = -radius; c <= radius; c++) {
            var dist = Math.Abs(r) + Math.Abs(c);
            if (dist != 0 && dist <= radius) {
                result.Add(new RC(r + start.Row ,c + start.Col));
            }
        }
    }
    return result;
}

record RC(int Row, int Col);

