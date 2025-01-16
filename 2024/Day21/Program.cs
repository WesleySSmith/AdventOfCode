#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
//using MoreLinq;
using MoreLinq;

bool sample = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt").ToArray();
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


Dictionary<char, RC> keypadD =new()
{
    ['7'] = new RC(0,0),
    ['8'] = new RC(0,1),
    ['9'] = new RC(0,2),
    ['4'] = new RC(1,0),
    ['5'] = new RC(1,1),
    ['6'] = new RC(1,2),
    ['1'] = new RC(2,0),
    ['2'] = new RC(2,1),
    ['3'] = new RC(2,2),
    ['0'] = new RC(3,1),
    ['A'] = new RC(3,2),
};

bool[,] keypadBoard = new bool[4,3];
foreach (var kvp in keypadD) {
    var rc = kvp.Value;
    keypadBoard[rc.Row, rc.Col] = true;
}

Dictionary<char, RC> dirKeypadD =new()
{
    ['^'] = new RC(0,1),
    ['A'] = new RC(0,2),
    ['<'] = new RC(1,0),
    ['v'] = new RC(1,1),
    ['>'] = new RC(1,2),
};

bool[,] dirKeypadBoard = new bool[2,3];
foreach (var kvp in dirKeypadD) {
    var rc = kvp.Value;
    dirKeypadBoard[rc.Row, rc.Col] = true;
}

Dictionary<(char, char, int), long> R2Memo = new();

sw.Restart();
Part1();
Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");
sw.Restart();
Part2();
Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");

void Part1()
{
    var acc = 0;
    foreach (var line in lines) {
       acc += Complexity1(line);
    }
    Console.Out.WriteLine($"Part 1: {acc}");
}

int Complexity1(string line) {
    Console.WriteLine();
    Console.WriteLine(line);
    var possiblePathsLevel0 = Solve1(keypadD, keypadBoard, line.ToArray());

    var totalList = new List<List<char>>();
    foreach (var possiblePath in possiblePathsLevel0) {
        totalList.AddRange(R(possiblePath, 1));
    }
    
    var bestPath = totalList.OrderBy(l => l.Count).First();
    var shortestLen = bestPath.Count();
    var score = shortestLen * int.Parse(line[..^1]);

    Console.WriteLine($"Best path for {line} is {string.Join("", bestPath)} (len: {shortestLen}) for score {score}");

    return score;
}

List<List<char>> R(List<char> line, int depth) {
    
    var possiblePaths = Solve1(dirKeypadD, dirKeypadBoard, line);
    if (depth == 2) {
        return possiblePaths;
    } else {
        var totalList = new List<List<char>>();
        foreach (var possiblePath in possiblePaths) {
            totalList.AddRange(R(possiblePath, depth + 1));
        }
        return totalList;
    }
}

List<List<char>> ComputeAllGoodPathsBetweenTwoPositions1(Dictionary<char, RC> map, bool[,] board, char c1, char c2) {
    var paths = AllShortestPaths1(board, map[c1], map[c2]);

    var potentialPaths = paths.Select(path => {
        var potentialPath = path.Pairwise((p1, p2) => {
            if (p1.Row > p2.Row) return '^';
            if (p1.Row < p2.Row) return 'v';
            if (p1.Col > p2.Col) return '<';
            if (p1.Col < p2.Col) return '>';
            throw new Exception("Unexpected 1");
        }).Append('A').ToList();
        
        // A path like ^^^>>> is better than ^>^>^>, because it's expensive
        // for the upper robot to switch to a different key.
        // So, only return paths with the fewest switches of characters.
        var score = 0;
        char last = ' ';
        for (int i = 0; i < potentialPath.Count; i++) {
            if (potentialPath[i] != last) {
                score++;
                last = potentialPath[i];
            }
        }

        return (potentialPath, score);
    })
    .OrderBy(p => p.score);

    var bestScore = potentialPaths.First().score;

    return potentialPaths.TakeWhile(p => p.score == bestScore).Select(p => p.potentialPath).ToList();
    
}


List<List<char>> Solve1(Dictionary<char, RC> map, bool[,] board, IList<char> line) {

    var possibleMovesAtEachStep = line.Prepend('A').Pairwise((c1, c2) => ComputeAllGoodPathsBetweenTwoPositions1(map, board, c1, c2)).ToList();
    
    List<List<char>> allPossiblePaths = [];
    Stack<(int, List<char>)> stack = new();
    stack.Push((-1, []));

    while (stack.Count > 0) {
        var n = stack.Pop();

        var idx = n.Item1 + 1;
        if (idx > possibleMovesAtEachStep.Count -1) {
            allPossiblePaths.Add(n.Item2);
            continue;
        }

        var prevs = possibleMovesAtEachStep[idx];

        if (prevs.Count == 1) {
            n.Item2.AddRange(prevs[0]);
            stack.Push((idx, n.Item2));
            continue;
        }

        foreach(var prevN in prevs) {
            List<char> newList = [..n.Item2, ..prevN];
            stack.Push((idx, newList));
        }
    } 

    return allPossiblePaths;
}



List<List<RC>> AllShortestPaths1(bool[,] board, RC start, RC end) {

    var minRow = 0;
    var maxRow = board.GetLength(0) -1;
    var minCol = 0;
    var maxCol = board.GetLength(1) -1;

    var prev = new Dictionary<RC, List<RC>>();
    var dist = new Dictionary<RC, int>();
    var Q = new PriorityQueue<RC, int>();

    Q.Enqueue(start, 0);
    dist[(start)] = 0;
    prev.Add(start, []);

    while (Q.Count > 0) {
        var u = Q.Dequeue();
        var row = u.Row;
        var col = u.Col;

        if (row == end.Row && col == end.Col) {
            continue;
        }

        var neighbors = new List<RC>();

        // North
        if (row > minRow && board[row -1, col]) {
            neighbors.Add(new RC(row - 1, col));
        }

        // South
        if (row < maxRow && board[row+1, col]) {
            neighbors.Add(new RC(row + 1, col));
        }

        //West
        if (col > minCol && board[row, col-1]) {
            neighbors.Add(new RC(row, col - 1));
        }
        //East
        if (col < maxCol && board[row, col+1]) {
            neighbors.Add(new RC(row, col + 1));
        }
        
        foreach (var v in neighbors) {
            var alt = dist[u] + 1;
            if (alt <= (dist.TryGetValue(v, out var d) ? d : int.MaxValue)) {
                dist[v] = alt;
                if (!prev.TryGetValue(v, out var prevList)) {
                    prev[v] = prevList = [];
                }
                prevList.Add(u);
                if (Q.UnorderedItems.All(i => i.Element != v)) {
                    Q.Enqueue(v, alt);
                }
            }
        }
    }


    List<List<RC>> allShortestPaths = [];
    Stack<(RC, List<RC>)> stack = new();
    stack.Push((end, [end]));

    while (stack.Count > 0) {
        var n = stack.Pop();

        var prevs = prev[n.Item1];

        if (prevs.Count == 0) {
            n.Item2.Reverse();
            allShortestPaths.Add(n.Item2);
            continue;
        }

        if (prevs.Count == 1) {
            n.Item2.Add(prevs[0]);
            stack.Push((prevs[0], n.Item2));
            continue;
        }

        foreach(var prevN in prevs) {
            List<RC> newList = [..n.Item2, prevN];
            stack.Push((prevN, newList));
        }
    } 
    return allShortestPaths; 
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
  var acc = 0L;
    foreach (var line in lines) {
       acc += Complexity2(line);
    }
    Console.Out.WriteLine($"Part 2: {acc}");
}


long Complexity2(string line) {

    var shortestLen = long.MaxValue;
    var possibleKeyboardPaths = Solve1(keypadD, keypadBoard, line.ToList());
    foreach (var possibleKeyboardPath in possibleKeyboardPaths) {

        var thisScore = possibleKeyboardPath.Prepend('A').Pairwise((a,b) => {
            return R2(a, b, 25);
        }).Sum();

        //Console.WriteLine($"Best path for {string.Join("", possibleKeyboardPath)} is (len: {thisScore})");
        shortestLen = Math.Min(shortestLen, thisScore);
    };
    
    var score = shortestLen * int.Parse(line[..^1]);

    Console.WriteLine($"Best path for {line} is (len: {shortestLen}) for score {score}");

    return score;
}


long R2(char c1, char c2, int level) {
    
    var memoKey = (c1, c2, level);
    if (R2Memo.TryGetValue(memoKey, out var memoResult)) {
        return memoResult;
    }

    var paths = ComputeAllGoodPathsBetweenTwoPositions1(dirKeypadD, dirKeypadBoard, c1, c2);

    if (level == 1) {
        return paths.First().Count;
    }

    var best = long.MaxValue;
    foreach (var path in paths) {

        var total = path.Prepend('A').Pairwise((a,b) => {
            return R2(a, b, level -1);
        }).Sum();

        best = Math.Min(best, total);
    }

    R2Memo.Add(memoKey, best);

    return best;

    // //var memoKey = (new String(line.ToArray()), level);
    // // if (R2Memo.TryGetValue(memoKey, out var memoResult)) {
    // //     //Console.WriteLine($"Cache Hit: {memoKey}: {memoResult}");
    // //     return memoResult;
    // // } else {
    // //     //Console.WriteLine($"Cache Miss: {memoKey}");
    // // }

    // List<List<char>> possiblePaths = level == 3 ? [line] : Solve1(dirKeypadD, dirKeypadBoard, line);

    // var bestScore = int.MaxValue;
    // foreach (var possiblePath in possiblePaths) {

    //     var score = possiblePath.Prepend('A').Pairwise((p1, p2) => {
    //         return R2(new List<char>() {p1, p2}, level-1);
    //     }).Sum();

    //     bestScore = Math.Min(bestScore, score);
    // }

    // //R2Memo.Add(memoKey, bestScore);
    // Console.WriteLine($"R2 result: {memoKey}: {bestScore}");
    // return bestScore;
}

record RC(int Row, int Col);

