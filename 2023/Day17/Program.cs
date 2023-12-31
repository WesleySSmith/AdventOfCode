#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Data;
using System.Diagnostics;
using MoreLinq;


bool sample = false;


string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

var minRow = 0;
var maxRow = lines.Length -1;
var minCol = 0;
var maxCol = lines[0].Length -1;


 var blocks = lines.Select(l => l.Select(c => (byte)(c - '0')).ToArray()).ToArray();
    
var board = new byte[blocks.Length, blocks[0].Length];
for (int row = minRow; row <= maxRow; row++) {
    for (int col = minCol; col <= maxCol; col++) {
        board[row,col] = blocks[row][col];
    }
}

//Part1();
Part2();

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1()
{
   

    var shortPath = ShortestPath1(board, new State(minRow,minCol, Dir.U, 0), (maxRow, maxCol));
    
    Console.Out.WriteLine($"Path is {shortPath}.");


}

void Part2()
{

    var shortPath = ShortestPath2(board, new State(minRow,minCol, Dir.U, 0), (maxRow, maxCol));
    
    Console.Out.WriteLine($"Path is {shortPath}.");
}

int ShortestPath1(byte[,] board, State start, (int, int) end) {
    var dist = new Dictionary<State, int>();
    var Q = new PriorityQueue<State, int>();

    Q.Enqueue(start, 0);
    dist[(start)] = 0;

    while (Q.Count > 0) {
        var u = Q.Dequeue();
        //if (debug) {
        //    Console.WriteLine($"Considering {u}");
        //}
        
        var row = u.Row;
        var col = u.Col;
        var dir = u.Dir;
        var times = u.Times;

        if (row == end.Item1 && col == end.Item2) {
            return dist[u];
        }

        var neighbors = new List<State>();

        // North
        if (row != minRow && dir != Dir.S && (dir != Dir.N || times < 2)) {
            neighbors.Add(new State(row - 1, col, Dir.N, dir == Dir.N ? times + 1: 0));
        }

        // South
        if (row != maxRow && dir != Dir.N && (dir != Dir.S || times < 2)) {
            neighbors.Add(new State(row + 1, col, Dir.S, dir == Dir.S ? times + 1 : 0));
        }

        //West
        if (col != minCol && dir != Dir.E && (dir != Dir.W || times < 2)) {
            neighbors.Add(new State(row, col - 1, Dir.W, dir == Dir.W ? times + 1 : 0));
        }
        //East
        if (col != maxCol && dir != Dir.W && (dir != Dir.E || times < 2)) {
            neighbors.Add(new State(row, col + 1, Dir.E, dir == Dir.E ? times + 1 : 0));
        }
        
        foreach (var v in neighbors) {
            var alt = dist[u] + board[v.Row,v.Col];
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

int ShortestPath2(byte[,] board, State start, (int, int) end) {
    var dist = new Dictionary<State, int>();
    var Q = new PriorityQueue<State, int>();

    Q.Enqueue(start, 0);
    dist[(start)] = 0;

    while (Q.Count > 0) {
        var u = Q.Dequeue();
        //if (debug) {
        //    Console.WriteLine($"Considering {u}");
        //}
        
        var row = u.Row;
        var col = u.Col;
        var dir = u.Dir;
        var times = u.Times;

        if (row == end.Item1 && col == end.Item2) {
            return dist[u];
        }

        var neighbors = new List<State>();

        // North
        if (row != minRow && dir != Dir.S && (dir != Dir.N || times < 9) && (dir == Dir.U || dir == Dir.N || times > 2)) {
            neighbors.Add(new State(row - 1, col, Dir.N, dir == Dir.N ? times + 1: 0));
        }

        // South
        if (row != maxRow && dir != Dir.N && (dir != Dir.S || times < 9) && (dir == Dir.U || dir == Dir.S || times > 2)) {
            neighbors.Add(new State(row + 1, col, Dir.S, dir == Dir.S ? times + 1 : 0));
        }

        //West
        if (col != minCol && dir != Dir.E && (dir != Dir.W || times < 9) && (dir == Dir.U || dir == Dir.W || times > 2)) {
            neighbors.Add(new State(row, col - 1, Dir.W, dir == Dir.W ? times + 1 : 0));
        }
        //East
        if (col != maxCol && dir != Dir.W && (dir != Dir.E || times < 9) && (dir == Dir.U || dir == Dir.E || times > 2)) {
            neighbors.Add(new State(row, col + 1, Dir.E, dir == Dir.E ? times + 1 : 0));
        }
        
        foreach (var v in neighbors) {
            var alt = dist[u] + board[v.Row,v.Col];
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

record State {

    public State(int row, int col, Dir dir, int times)
    {
        Row = row;
        Col = col;
        Dir = dir;
        Times = (byte)times;
    }
    public int Row;
    public int Col;
    public Dir Dir;
    public byte Times;
}
enum Dir: byte {
    N,
    S,
    E,
    W,
    U
}
 