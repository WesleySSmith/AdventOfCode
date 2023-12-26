#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Text;
//using MoreLinq;

bool sample = false;
bool debug = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt"); 
if (debug) Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

const int WIDTH = 7;

var jets = lines[0].ToArray();

var s1 = new bool[1,4] {{true, true, true, true }};
var s2 = new bool[3,3] {{false, true, false }, {true, true, true }, {false, true, false }};
var s3 = new bool[3,3] { {true, true, true }, {false, false, true }, {false, false, true },};
var s4 = new bool[4,1] {{true}, {true}, {true}, {true} };
var s5 = new bool[2,2] {{true, true}, {true, true }};

var Shapes = new[] {
    s1, s2, s3, s4, s5
};

//Part1(jets);
Part2(jets);


Console.Out.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");


void Part1(char[] jets) {

    int WIDTH = 7;
    int HEIGHT = 2022*4 + 10;
    bool[,] board = new bool[HEIGHT, WIDTH + 2];
    
    //floor
    for (var i = 0; i< WIDTH+2;i++) {
        board[0,i] = true;
    }

    // walls
    for (var j = 0; j< HEIGHT;j++) {
        board[j, 0] = true;
        board[j, WIDTH + 1] = true;
    }

    var highRockRow = 0;


    int rocksLeft = 2022;

    var jetEnum = Enumerable.Range(0, int.MaxValue).SelectMany(r => jets).GetEnumerator();
    var shapeEnum = Enumerable.Range(0,int.MaxValue).SelectMany(r => Shapes).GetEnumerator();

    while (rocksLeft > 0) {

        if (debug) Console.WriteLine($"Rocks left: {rocksLeft}");
        rocksLeft--;

        var rockOriginRow = highRockRow + 4;
        var rockOriginCol = 1 + 2;
        
        shapeEnum.MoveNext();
        var rockShape = shapeEnum.Current;

        if (debug) Console.WriteLine($"Rocks starts at: {rockOriginRow},{rockOriginCol}");

        while (true) {
            if (debug) Console.WriteLine($" Turn");
            // Jet moves rock
            jetEnum.MoveNext();
            var jet = jetEnum.Current;

            int proposedRockOriginCol = rockOriginCol + (jet == '<' ? -1 : +1);
            int proposedRockOriginRow = rockOriginRow;

            if (!Collides(board, rockShape, proposedRockOriginRow, proposedRockOriginCol)) {
                rockOriginCol = proposedRockOriginCol;
                rockOriginRow = proposedRockOriginRow;
                if (debug) Console.WriteLine($"  Rocks moves horiz to : {rockOriginRow},{rockOriginCol}");
            }

            // Rock falls

            proposedRockOriginCol = rockOriginCol;
            proposedRockOriginRow = rockOriginRow - 1;

            if (!Collides(board, rockShape, proposedRockOriginRow, proposedRockOriginCol)) {
                rockOriginCol = proposedRockOriginCol;
                rockOriginRow = proposedRockOriginRow;
                if (debug) Console.WriteLine($"  Rocks falls to : {rockOriginRow},{rockOriginCol}");
            } else {
                highRockRow = Math.Max(highRockRow, rockOriginRow + rockShape.GetLength(0) -1);
                UpdateBoard(board, rockShape, rockOriginRow, rockOriginCol);
                if (debug) Console.WriteLine($"  Rocks stops at : {rockOriginRow},{rockOriginCol}. High Rock: {highRockRow}");
                break;
            }

        }
    }

    //PrintBoard(board);
    Console.WriteLine($"Part 1 tower: {highRockRow}");

}


void Part2(char[] jets) {

    
    int HEIGHT = jets.Length * Shapes.Length * 100;
    bool[,] board = new bool[HEIGHT, WIDTH + 2];
    
    var jetIndex = 0;
    var shapeIndex = 0;

    bool[,] GetShape() {
        var shape = Shapes[shapeIndex];
        shapeIndex = (shapeIndex + 1) % Shapes.Length;
        return shape;
    }

    char GetJet() {
        var jet = jets[jetIndex];
        jetIndex = (jetIndex + 1) % jets.Length;
        return jet;
    }

    string GetProfile(int highRockRow) {
        var result = new int[WIDTH];
        
        for (int i = 0; i < WIDTH; i++) {
            var rowToCheck = highRockRow;
            while (rowToCheck >= 0) {
                if (board[rowToCheck, i+1]) {
                    result[i] = (int)(highRockRow - rowToCheck);
                    break;
                }
                rowToCheck--;
            }
        }
        return string.Join(",", result);
    }

    //floor 
    for (var i = 0; i< WIDTH+2;i++) {
        board[0,i] = true;
    }

    // walls
    for (var j = 0; j< HEIGHT;j++) {
        board[j, 0] = true;
        board[j, WIDTH + 1] = true;
    }

    int highRockRow = 0;
    const ulong ROCKS = 1000000000000;


    Dictionary<(string TopProfile, int JetIndex, int ShapeIndex),(int HighRockRow, int RockCount)> seen = new();  

    var rowOffset = 0UL;
    for (ulong rockCount = 0; rockCount < ROCKS; rockCount++) {

        if (debug) Console.WriteLine($"Rocks dropped: {rockCount}");

        var rockOriginRow = highRockRow + 4;
        var rockOriginCol = 1 + 2;
        
        var rockShape = GetShape();

        if (debug) Console.WriteLine($"Rocks starts at: {rockOriginRow},{rockOriginCol}");

        while (true) {
            if (debug) Console.WriteLine($" Turn");
            // Jet moves rock
            var jet = GetJet();

            int proposedRockOriginCol = rockOriginCol + (jet == '<' ? -1 : +1);
            int proposedRockOriginRow = rockOriginRow;

            if (!Collides(board, rockShape, proposedRockOriginRow, proposedRockOriginCol)) {
                rockOriginCol = proposedRockOriginCol;
                rockOriginRow = proposedRockOriginRow;
                if (debug) Console.WriteLine($"  Rocks moves horiz to : {rockOriginRow},{rockOriginCol}");
            }

            // Rock falls

            proposedRockOriginCol = rockOriginCol;
            proposedRockOriginRow = rockOriginRow - 1;

            if (!Collides(board, rockShape, proposedRockOriginRow, proposedRockOriginCol)) {
                rockOriginCol = proposedRockOriginCol;
                rockOriginRow = proposedRockOriginRow;
                if (debug) Console.WriteLine($"  Rocks falls to : {rockOriginRow},{rockOriginCol}");
            } else {
                highRockRow = Math.Max(highRockRow, rockOriginRow + rockShape.GetLength(0) -1);
                UpdateBoard(board, rockShape, rockOriginRow, rockOriginCol);
                if (debug) Console.WriteLine($"  Rocks stops at : {rockOriginRow},{rockOriginCol}. High Rock: {highRockRow}");
                
                break;
            }
        }

        var seenKey = (GetProfile(highRockRow), jetIndex, shapeIndex);
        if (debug) Console.WriteLine($"[{rockCount.ToString().PadLeft(5)}]: {MemoKeyToString(seenKey)}");
        if (seen != null && seen.TryGetValue(seenKey, out var memoValue)) {
            var blockSizeInRows = highRockRow - memoValue.HighRockRow;
            var blockSizeInRocks = rockCount - (ulong)memoValue.RockCount;

            var rocksRemainingToStack = ROCKS - rockCount;
            var blocksToSkip = rocksRemainingToStack / blockSizeInRocks;

            var rocksToSkip = blocksToSkip * blockSizeInRocks;
            rockCount += rocksToSkip;
            rowOffset = blocksToSkip * (ulong)blockSizeInRows;
            seen = null;
            continue;
        } else {
            if (seen != null) {
                seen.Add(seenKey, (highRockRow, (int)rockCount));
            }
        }
    }

    //PrintBoard(board);
    Console.WriteLine($"Part 2 tower: {(ulong)highRockRow + rowOffset}");
}



void UpdateBoard(bool[,] board, bool[,] shape, int originRow, int originCol)
{
    int shapeHeight = shape.GetLength(0);
    int shapeWidth = shape.GetLength(1);

    for (var row = 0; row < shapeHeight; row++) {
        for (var col = 0; col < shapeWidth; col++) {
            if (shape[row,col]) {
                 board[originRow + row, originCol + col] = true;
            }
        }
    }
}

bool Collides(bool[,] board, bool[,] shape, int originRow, int originCol) {

    int shapeHeight = shape.GetLength(0);
    int shapeWidth = shape.GetLength(1);

    for (var row = 0; row < shapeHeight; row++) {
        for (var col = 0; col < shapeWidth; col++) {
            if (shape[row,col] && board[originRow + row, originCol + col]) {
                return true;
            }
        }
    }
    return false;
}

void PrintBoard(bool[,] board, int? start = null) {
    for (int i = start.HasValue ? start.Value : board.GetLength(0) -1; i >= 0; i--) {
        var s = (i-1).ToString().PadLeft(5) + " ";
        for (int j = 0; j < board.GetLength(1); j++) {
            s += board[i,j] ? (j == 0 || j == 8 ? "|" : '#') : '.';
        }
        Console.WriteLine(s);
    }
}

string MemoKeyToString((string TopProfile, int JetIndex, int ShapeIndex) memoKey) {
    return $"jetIndex: {memoKey.JetIndex}, shapeIndex: {memoKey.ShapeIndex}, profile: {memoKey.TopProfile}";
}

