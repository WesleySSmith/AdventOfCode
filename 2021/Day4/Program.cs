using MoreLinq;
using System.Text;
using System.Text.Json;



string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

var draws = lines[0].Split(',').Select(int.Parse);
var boardCount = (lines.Length - 1) / 6;
var boards = new List<Cell[,]>();
for (int ii = 0; ii < boardCount; ii++) {
    var board = new Cell[5,5];
    boards.Add(board);
    for (int row = 0; row < 5; row++) {
        var line = lines[2+ row + ii*6];
        var nums = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        for (int col = 0; col < 5; col++) {
            board[row, col] = new Cell {Value = nums[col]};
        }
    }
}

//Part1(draws, boards);
Part2(draws, boards);


static void Part1(IEnumerable<int> draws, List<Cell[,]> boards) {
   
    Console.Out.WriteLine($"Draws: {JsonSerializer.Serialize(draws)}");
    Console.Out.WriteLine($"Boards: \n{string.Join("\n", boards.Select(BoardToString).ToArray())}");

    Cell[,] winningBoard = null;
    int winningDraw = 0;
    foreach (var draw in draws) {
        foreach (var board in boards) {
            Mark(board, draw);
            if (IsWin(board)) {
                winningBoard = board;
                winningDraw = draw;
                goto win;
            }
        }
    }
win:
    Console.Out.WriteLine($"Winning board:\n{BoardToString(winningBoard)}");
    Console.Out.WriteLine($"Score: {ScoreBoard(winningBoard) * winningDraw}");
}

static void Part2(IEnumerable<int> draws, List<Cell[,]> boards) {
   
    Console.Out.WriteLine($"Draws: {JsonSerializer.Serialize(draws)}");
    Console.Out.WriteLine($"Boards: \n{string.Join("\n", boards.Select(BoardToString).ToArray())}");

    
    Cell[,] lastWinningBoard = null;
    int winningDraw = 0;
    foreach (var draw in draws) {
        List<Cell[,]> winningBoards = new List<Cell[,]>();
        foreach (var board in boards) {
            Mark(board, draw);
            if (IsWin(board)) {
                winningBoards.Add(board);
            }
        }
        boards = boards.Except(winningBoards).ToList();
        if (boards.Count == 0) {
            winningDraw = draw;
            lastWinningBoard = winningBoards.Single();
            goto win;
        }
    }
win:
    Console.Out.WriteLine($"Last winning board:\n{BoardToString(lastWinningBoard)}");
    Console.Out.WriteLine($"Score: {ScoreBoard(lastWinningBoard) * winningDraw}");
}

static void Mark(Cell[,] board, int val) {
    for (var r = 0; r < board.GetLength(0); r++) {
        for (var c = 0; c < board.GetLength(1); c++) {
            if (board[r,c].Value == val) {
                board[r,c].Marked = true;
            }
        }
    }
}

static bool IsWin(Cell[,] board) {
    for (var r = 0; r < board.GetLength(0); r++) {
        if (IsRowWin(board, r)) {
            return true;
        }
    }
    for (var c = 0; c < board.GetLength(1); c++) {
        if (IsColWin(board, c)) {
            return true;
        }
    }
    return false;
}

static bool IsRowWin(Cell[,] board, int row) {
    for (var c = 0; c < board.GetLength(1); c++) {
        var cell = board[row,c];
        if (!cell.Marked) {
            return false;
        }
    }
    return true;
}

static bool IsColWin(Cell[,] board, int col) {
    for (var r = 0; r < board.GetLength(0); r++) {
        var cell = board[r,col];
        if (!cell.Marked) {
            return false;
        }
    }
    return true;
}

static int ScoreBoard(Cell[,] board) {
    return board.Cast<Cell>().ToList().Aggregate(0, (acc, cell) => acc + (cell.Marked ? 0 : cell.Value));
}

static string BoardToString(Cell[,] board) {
    var sb = new StringBuilder();
    for (var r = 0; r < board.GetLength(0); r++) {
        for (var c = 0; c < board.GetLength(1); c++) {
            var cell = board[r,c];
            sb.Append(cell.Value);
            if (cell.Marked) {
                sb.Append("*");
            }
            if (c + 1 < board.GetLength(1)) {sb.Append(", ");}
        }
        sb.Append("\n");
    }
    return sb.ToString();
}

record Cell {
    public int Value;
    public bool Marked;
}







