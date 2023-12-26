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


var map = Part1(lines);
Part2(map);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


Node[,]  Part1(string[] lines)
{
    

    var minRow = 0;
    var minCol = 0;
    var maxRow = lines.Length - 1;
    var maxCol = lines[0].Length - 1;

    Node[,] map = new Node[maxRow + 1, maxCol +1];
    Node start = null;

    for (var row = 0; row <= maxRow; row++) {
        for (var col = 0; col <= maxCol; col++) {
            map[row,col] = new Node();
        }
    }

    for (var row = 0; row <= maxRow; row++) {
        for (var col = 0; col <= maxCol; col++) {

            var c = lines[row][col];
            var node = map[row,col];
            node.C = c;
            node.Row = row;
            node.Col = col;
            
            switch (c) {
                case '|':
                    if (row > minRow) node.N1 = map[row-1, col];
                    if (row < maxRow) node.N2 = map[row+1, col];
                    break;
                case '-':
                    if (col > minCol) node.N1 = map[row, col-1];
                    if (col < maxCol) node.N2 = map[row, col+1];
                    break;
                case 'L':
                    if (row > minRow) node.N1 = map[row-1, col];
                    if (col < maxCol) node.N2 = map[row, col+1];
                    break;
                case 'J':
                    if (row > minRow) node.N1 = map[row-1, col];
                    if (col > minCol) node.N2 = map[row, col-1];
                    break;
                case '7':
                    if (col > minCol) node.N1 = map[row, col-1];
                    if (row < maxRow) node.N2 = map[row+1, col];
                    break;
                case 'F':
                    if (col < maxCol) node.N1 = map[row, col+1];
                    if (row < maxRow) node.N2 = map[row+1, col];
                    break;
                case '.':
                    break;
                case 'S': 
                    start = node;
                    break;
            }

           
        }
    }

    //Figure out S
             
    // Up
    bool n = false, s = false, e = false, w = false;
    if (start.Row > minRow ) {
        var adj = map[start.Row-1, start.Col];
        if (adj.Adjacent(start)) {
            start.AddAdjacent(adj);
            n = true;
        }
    } 

    // Down
    if (start.Row < maxRow) {
        var adj = map[start.Row + 1, start.Col];
        if (adj.Adjacent(start)) {
            start.AddAdjacent(adj);
            s = true;
        }
    }

    // Left 
    if (start.Col > minCol) {
        var adj = map[start.Row, start.Col-1];
        if (adj.Adjacent(start)) {
            start.AddAdjacent(adj);
            w = true;
        }
    } 

    // Right
    if (start.Col < maxCol) {
        var adj = map[start.Row, start.Col + 1];
        if (adj.Adjacent(start)) {
            start.AddAdjacent(adj);
            e = true;
        }
    }

         if (n && s) start.C = '|';
    else if (n && w) start.C = 'J';
    else if (n && e) start.C = 'L';
    else if (w && e) start.C = '-';
    else if (s && w) start.C = '7';
    else if (s && e) start.C = 'F';
    else throw new Exception("invalid start");

    if (start.N1 == null || start.N2 == null) {
        throw new Exception("Not enough adjacent to start");
    }


    var count = 1;
    var from = start;
    from.InPath = true;
    var current = start.N1;  // Arbitrarily choose which way to go
    do {
        var oldCurrent = current;
        current = current.Flow(from);
        from = oldCurrent;
        count++;
    } while (current != start);
    
    Console.Out.WriteLine($"Len is {count}.  Max dist: {count / 2}");

    return map;

}

void Part2(Node[,] map)
{

    var minRow = 0;
    var minCol = 0;
    var maxRow = map.GetLength(0) - 1;
    var maxCol = map.GetLength(1) - 1;

    var numIn = 0;
    for (var row = minRow; row <= maxRow; row++) {
        bool amIn = false;
        for (var col = minCol; col <= maxCol; col++) {
            var node = map[row,col];
            if (node.InPath) {
                if (node.C is '|' or '7' or 'F') {
                    amIn = !amIn;
                }
            }
            else {
                if (amIn) {
                    node.Inside = true;
                    numIn++;
                }
            }
        }
    }

    Console.WriteLine($"Num inside: {numIn}");


    // for (var row = minRow; row <= maxRow; row++) {
    //     for (var col = minCol; col <= maxCol; col++) {
    //         Console.Write(map[row,col].C);
    //     }
    //     Console.WriteLine();
    // }

    // Console.WriteLine();

    // for (var row = minRow; row <= maxRow; row++) {
    //     for (var col = minCol; col <= maxCol; col++) {
    //         Console.Write(map[row,col].InPath ? 'X' : '.');
    //     }
    //     Console.WriteLine();
    // }

    // for (var row = minRow; row <= maxRow; row++) {
    //     for (var col = minCol; col <= maxCol; col++) {
    //         Console.Write(map[row,col].Inside ? 'I' : '.');
    //     }
    //     Console.WriteLine();
    // }

}

class Node {

    public int Row;
    public int Col;
    public char C;
    public Node N1;
    public Node N2;

    public bool Inside;

    public bool InPath;

    public bool Adjacent(Node n) => n == N1 || n == N2;

    public void AddAdjacent(Node n) {
        if (N1 == null) {
            N1 = n;
        } else if (N2 == null){
            N2 = n;
        } else {
            throw new Exception("Too many adjacent");
        }
    }

    public Node Flow(Node from) {

        InPath = true;
        if (from == N1) {
            return N2;
        } 
        if (from == N2) {
            return N1;
        }
        throw new Exception("Invalid from");
    }
}