#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
//using MoreLinq;

bool sample = false;
bool debug = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt"); 
if (debug) Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

char[][] board = lines.Select(line => line.ToArray()).ToArray();
Stopwatch sw;

sw = Stopwatch.StartNew();
//Part1(board);
Part2(board);

Console.Out.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");

void Part1(char[][] initialBoard) {
   
    var timeBoard = GetTimeBoard(initialBoard);
   
    int shortPath = ShortestPath(timeBoard, (0, 0, 1), (-1, initialBoard.Count()-1, initialBoard[0].Count() -2));
    
    Console.WriteLine($"Part 1 Steps: {shortPath}");
}

void Part2(char[][] initialBoard) {
   
    var timeBoard = GetTimeBoard(initialBoard);
   
    int shortPath1 = ShortestPath(timeBoard, (0, 0, 1), (-1, initialBoard.Count()-1, initialBoard[0].Count() -2));
    int shortPath2 = ShortestPath(timeBoard, (shortPath1, initialBoard.Count()-1, initialBoard[0].Count() -2), (-1, 0, 1));
    int shortPath3 = ShortestPath(timeBoard, (shortPath1+shortPath2, 0, 1), (-1, initialBoard.Count()-1, initialBoard[0].Count() -2));

    Console.WriteLine($"Part 2 Steps: {shortPath1} + {shortPath2} + {shortPath3} = {shortPath1+shortPath2+shortPath3}");
}

bool[,,] GetTimeBoard(char[][] initialBoard) {
    var initialRows = initialBoard.Count();
    var initialCols = initialBoard[0].Count();
    var rows = initialRows - 2;
    var cols = initialCols - 2;
    var timeSteps = sample ? 12 : 600;
    var board = new char[rows, cols];
    
    for (int row = 0; row < rows; row++) {
        for (int col = 0; col < cols; col++) {
            board[row, col] = initialBoard[row+1][col+1];
        }
    }

    var timeBoard = new bool[timeSteps, initialRows, initialCols];

    for (int time = 0; time < timeSteps; time++) {

        // frame
        for (int row = 0; row < initialRows; row++) {
            timeBoard[time, row, 0] = true;
            timeBoard[time, row, initialCols-1] = true;
        }
        for (int col = 0; col < initialCols; col++) {
            timeBoard[time, 0, col] = true;
            timeBoard[time, initialRows -1, col] = true;
        }

        // Start position
        timeBoard[time, 0, 1] = false;

        // Finish position
        timeBoard[time, initialRows-1, initialCols -2] = false;

        for (int row = 0; row < rows; row++) {
            for (int col = 0; col < cols; col++) {

                var c = board[row,col];
                if (c == '>') {
                    timeBoard[time, row+1, mod(col + time,cols)+1] = true;
                } else if (c == '<') {
                    timeBoard[time, row+1, mod(col - time, cols)+1] = true;
                } else if (c == '^') {
                    timeBoard[time, mod(row - time, rows)+1, col+1] = true;
                } else if (c == 'v') {
                    timeBoard[time, mod(row + time, rows)+1, col+1] = true;
                }
            }
        }
    }

    if (debug) {
        for (int time = 0; time < timeSteps; time++) {
            Console.WriteLine($"Time: {time}");
            Console.WriteLine(BoardToString(timeBoard, time));
        }
    }

    return timeBoard;
}

static int mod(int x, int m) {
    int r = x % m;
    return r < 0 ? r + m : r;
}

int ShortestPath(bool[,,] timeBoard, (int, int, int) start, (int, int, int) end) {
    var dist = new Dictionary<(int,int,int), int>();
    var Q = new PriorityQueue<(int, int, int), int>();

    Q.Enqueue(start, 0);
    dist[(start)] = 0;

    while (Q.Count > 0) {
        var u = Q.Dequeue();
        if (debug) {
            Console.WriteLine($"Considering {u}");
        }
        var time = u.Item1;
        var row = u.Item2;
        var col = u.Item3;

        if (u.Item2 == end.Item2 && u.Item3 == end.Item3) {
            return dist[u];
        }

        var nextTime = time + 1;
        var timeBoardIndex = mod(nextTime, timeBoard.GetLength(0));
        var neighbors = new List<(int, int, int)>();
        if (row != 0) {
            // up
            if (row != 0 && !timeBoard[timeBoardIndex, row - 1, col]) {
                neighbors.Add((nextTime, row - 1, col));
            }
        }
        if (row != timeBoard.GetLength(1) - 1) {
            // down
            if (!timeBoard[timeBoardIndex, row + 1, col]) {
                neighbors.Add((nextTime, row + 1, col));
            }
        }

        //left
        if (!timeBoard[timeBoardIndex, row, col - 1 ]) {
            neighbors.Add((nextTime, row, col - 1));
        }
        //right
        if (!timeBoard[timeBoardIndex, row, col + 1 ]) {
            neighbors.Add((nextTime, row, col + 1));
        }
        // stay
        if (!timeBoard[timeBoardIndex, row, col]) {
            neighbors.Add((nextTime, row, col));
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


static String BoardToString(bool[,,] board, int time) {
    StringBuilder sb = new StringBuilder();

    for (int row = 0; row < board.GetLength(1); row++) {
        for (int col = 0; col < board.GetLength(2); col++) { 
            sb.Append(board[time, row,col] ? "#" : ".");
        }
        sb.Append("\n");
    }
    return sb.ToString();
    
}



