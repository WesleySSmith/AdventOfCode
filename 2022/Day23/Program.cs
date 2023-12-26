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

bool[][] board = lines.Select(line => line.Select(c => c == '#').ToArray()).ToArray();
Stopwatch sw;

sw = Stopwatch.StartNew();
//Part1(board);
Part2(board);

Console.Out.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");

void Part1(bool[][] board) {
   
    var rows = board.Count();
    var cols = board[0].Count();
    var bigBoard = new bool[rows*3, cols * 3];
    
    for (int row = 0; row < rows; row++) {
        for (int col = 0; col < cols; col++) {
            bigBoard[row + rows, col + cols] = board[row][col];
        }
    }

    var proposals = new Dictionary<(int, int), List<(int, int)>>();
    int round = 0;
    while (true) {

        proposals.Clear();

        for (int row = 1; row < bigBoard.GetLength(0) -1; row++) {
            for (int col = 1; col < bigBoard.GetLength(1) -1; col++) {
                if (bigBoard[row,col]) {

                    // Check for any neighbors
                    if (!(
                           bigBoard[row-1, col-1] 
                        || bigBoard[row-1, col+0] 
                        || bigBoard[row-1, col+1]
                        || bigBoard[row+0, col-1]
                        || bigBoard[row+0, col+1]
                        || bigBoard[row+1, col-1]
                        || bigBoard[row+1, col+0]
                        || bigBoard[row+1, col+1]
                        )) {
                            goto nextElf;
                    }


                    // Check first direction
                    
                    for (int attempt = 0; attempt < 4; attempt++) {
                        var direction = (Direction)((attempt + round) % 4);

                        switch (direction) {
                            case Direction.N:
                                if (   !bigBoard[row-1, col-1] 
                                    && !bigBoard[row-1, col+0] 
                                    && !bigBoard[row-1, col+1]) {
                                        AddToList(proposals, (row -1, col), (row, col));
                                        goto nextElf;
                                    }
                                break;
                            case Direction.S:
                                if (   !bigBoard[row+1, col-1] 
                                    && !bigBoard[row+1, col+0] 
                                    && !bigBoard[row+1, col+1]) {
                                        AddToList(proposals, (row +1, col), (row, col));
                                        goto nextElf;
                                    }
                                break;
                            case Direction.W:
                                if (   !bigBoard[row-1, col-1] 
                                    && !bigBoard[row+0, col-1] 
                                    && !bigBoard[row+1, col-1]) {
                                        AddToList(proposals, (row, col-1), (row, col));
                                        goto nextElf;
                                    }
                                break;
                            case Direction.E:
                                if (   !bigBoard[row-1, col+1] 
                                    && !bigBoard[row+0, col+1] 
                                    && !bigBoard[row+1, col+1]) {
                                        AddToList(proposals, (row, col+1), (row, col));
                                        goto nextElf;
                                    }
                                break;
                        }
                    }
                }
                nextElf:;
            }
        }


        if (proposals.Any())  {
            foreach (var proposal in proposals) {
                if (proposal.Value.Count == 1) {
                    if (proposal.Key.Item1 == 0 || proposal.Key.Item1 == bigBoard.GetLength(0) -1
                    || proposal.Key.Item2 == 0 || proposal.Key.Item2 == bigBoard.GetLength(1) -1
                    ) {
                        throw new Exception("Board overflow");
                    }
                    bigBoard[proposal.Key.Item1, proposal.Key.Item2] = true;
                    bigBoard[proposal.Value[0].Item1, proposal.Value[0].Item2] = false;
                }
            }
        } else {
            break;
        }

        if (debug) {
            Console.WriteLine($"Round: {round + 1}\n{BoardToString(bigBoard)}");
        }

        if (round == 9) {
            break;
        }
        round++;
    }

    int minRow = int.MaxValue;
    int maxRow = int.MinValue;
    int minCol = int.MaxValue;
    int maxCol = int.MinValue;

     for (int row = 0; row < bigBoard.GetLength(0); row++) {
        for (int col = 0; col < bigBoard.GetLength(1); col++) {
            if (bigBoard[row,col]) {
                if (row < minRow) {
                    minRow = row;
                } 
                if (row > maxRow) {
                    maxRow = row;
                }
                if (col < minCol) {
                    minCol = col;
                }
                if (col > maxCol) {
                    maxCol = col;
                }
            }
        }
     }

    var emptyCount = 0;
    for (int row = minRow; row <= maxRow; row++) {
        for (int col = minCol; col <= maxCol; col++) {
            if (!bigBoard[row,col]) {
                emptyCount++;
            }
        }
    }

    Console.WriteLine($"Part 1 Empty Count: {emptyCount}");



}


void Part2(bool[][] board) {
   
    var rows = board.Count();
    var cols = board[0].Count();
    var bigBoard = new bool[rows*3, cols * 3];
    
    for (int row = 0; row < rows; row++) {
        for (int col = 0; col < cols; col++) {
            bigBoard[row + rows, col + cols] = board[row][col];
        }
    }

    var proposals = new Dictionary<(int, int), List<(int, int)>>();
    int round = 0;
    while (true) {

        proposals.Clear();

        for (int row = 1; row < bigBoard.GetLength(0) -1; row++) {
            for (int col = 1; col < bigBoard.GetLength(1) -1; col++) {
                if (bigBoard[row,col]) {

                    // Check for any neighbors
                    if (!(
                           bigBoard[row-1, col-1] 
                        || bigBoard[row-1, col+0] 
                        || bigBoard[row-1, col+1]
                        || bigBoard[row+0, col-1]
                        || bigBoard[row+0, col+1]
                        || bigBoard[row+1, col-1]
                        || bigBoard[row+1, col+0]
                        || bigBoard[row+1, col+1]
                        )) {
                            goto nextElf;
                    }


                    // Check first direction
                    
                    for (int attempt = 0; attempt < 4; attempt++) {
                        var direction = (Direction)((attempt + round) % 4);

                        switch (direction) {
                            case Direction.N:
                                if (   !bigBoard[row-1, col-1] 
                                    && !bigBoard[row-1, col+0] 
                                    && !bigBoard[row-1, col+1]) {
                                        AddToList(proposals, (row -1, col), (row, col));
                                        goto nextElf;
                                    }
                                break;
                            case Direction.S:
                                if (   !bigBoard[row+1, col-1] 
                                    && !bigBoard[row+1, col+0] 
                                    && !bigBoard[row+1, col+1]) {
                                        AddToList(proposals, (row +1, col), (row, col));
                                        goto nextElf;
                                    }
                                break;
                            case Direction.W:
                                if (   !bigBoard[row-1, col-1] 
                                    && !bigBoard[row+0, col-1] 
                                    && !bigBoard[row+1, col-1]) {
                                        AddToList(proposals, (row, col-1), (row, col));
                                        goto nextElf;
                                    }
                                break;
                            case Direction.E:
                                if (   !bigBoard[row-1, col+1] 
                                    && !bigBoard[row+0, col+1] 
                                    && !bigBoard[row+1, col+1]) {
                                        AddToList(proposals, (row, col+1), (row, col));
                                        goto nextElf;
                                    }
                                break;
                        }
                    }
                }
                nextElf:;
            }
        }


        if (proposals.Any())  {
            foreach (var proposal in proposals) {
                if (proposal.Value.Count == 1) {
                    bigBoard[proposal.Key.Item1, proposal.Key.Item2] = true;
                    bigBoard[proposal.Value[0].Item1, proposal.Value[0].Item2] = false;
                }
            }
        } else {
            break;
        }

        if (debug) {
            Console.WriteLine($"Round: {round + 1}\n{BoardToString(bigBoard)}");
        }

        round++;
    }

    

    Console.WriteLine($"Part 2 Round Count: {round + 1}");



}


static String BoardToString(bool[,] board) {
    StringBuilder sb = new StringBuilder();

    for (int row = 0; row < board.GetLength(0); row++) {
        for (int col = 0; col < board.GetLength(1); col++) { 
            sb.Append(board[row,col] ? "#" : ".");
        }
        sb.Append("\n");
    }
    return sb.ToString();
    
}
static void AddToList(Dictionary<(int, int), List<(int, int)>> dictionary, (int,int)key, (int,int) value) {
    var found = dictionary.TryGetValue(key, out var list);
    if (!found) {
        var newList = new List<(int, int)>() {value};
        dictionary.Add(key, newList);
    } else {
        list.Add(value);
    }
}

enum  Direction
{
    N,
    S,
    W,
    E
}





