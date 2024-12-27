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

//Part1(lines);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");



void Part1(string[] lines)
{
    List<string> mapLines = new();
    int i = 0;
    while (lines[i].Length > 0) {
        mapLines.Add(lines[i]);
        i++;
    }
    i++;

    List<char> instructions = new ();
    while (i < lines.Length) {
        instructions.AddRange(lines[i]);
        i++;
    }

    var numRows = mapLines.Count;
    var numCols = mapLines[0].Count();
    var map = new char[numRows, numCols];

    RC robotRc = null;
    for (int r = 0; r < numRows; r++)
    {
        for (int c = 0; c < numCols; c ++) {
            var ch = map[r, c] = lines[r][c];
            if (ch == '@') {
                robotRc = new RC(r, c);
            }
        }
    }

    //PrintMap(map);

    foreach(var move in instructions) {
        switch (move) {
           case '^':
            for (i = robotRc.Row -1; i >= 0; i--) {
                var ch = map[i, robotRc.Col];
                if (ch == '#') {
                    // Wall
                    break;
                } if (ch == 'O') {
                    // Box
                } else if (ch == '.') {
                    // Space.  The Robot can move

                    for (int j = i; j < robotRc.Row; j++) {
                        map[j, robotRc.Col] = map[j+1, robotRc.Col];
                    }
                    map[robotRc.Row, robotRc.Col] = '.';
                    robotRc.Row--;
                    
                    break;

                }
            }
            break;

            case 'v':
            for (i = robotRc.Row +1; i < numRows; i++) {
                var ch = map[i, robotRc.Col];
                if (ch == '#') {
                    // Wall
                    break;
                } if (ch == 'O') {
                    // Box
                } else if (ch == '.') {
                    // Space.  The Robot can move

                    for (int j = i; j > robotRc.Row; j--) {
                        map[j, robotRc.Col] = map[j-1, robotRc.Col];
                    }

                    map[robotRc.Row, robotRc.Col] = '.';
                    robotRc.Row++;

                    break;

                }
            }
             break;


            case '<':
            for (i = robotRc.Col -1; i >= 0; i--) {
                var ch = map[robotRc.Row, i];
                if (ch == '#') {
                    // Wall
                    break;
                } if (ch == 'O') {
                    // Box
                } else if (ch == '.') {
                    // Space.  The Robot can move

                    for (int j = i; j < robotRc.Col; j++) {
                        map[robotRc.Row, j] = map[robotRc.Row, j+1];
                    }

                    map[robotRc.Row, robotRc.Col] = '.';
                    robotRc.Col--;
                    break;

                }
            }
            break;

            case '>':
            for (i = robotRc.Col +1; i < numCols; i++) {
                var ch = map[robotRc.Row, i];
                if (ch == '#') {
                    // Wall
                    break;
                } if (ch == 'O') {
                    // Box
                } else if (ch == '.') {
                    // Space.  The Robot can move

                    for (int j = i; j > robotRc.Col; j--) {
                        map[robotRc.Row, j] = map[robotRc.Row, j-1];
                    }
                    map[robotRc.Row, robotRc.Col] = '.';
                    robotRc.Col++;
                    break;

                }
            }
             break;
        }

        //Console.Out.WriteLine($"Move {move}");
        //PrintMap(map);

    }


    var acc = 0L;
    for (int r = 0; r < numRows; r++)
    {
        for (int c = 0; c < numCols; c ++) {
            var ch = map[r, c];
            if (ch == 'O') {
                var gps = r * 100 + c;
                acc += gps;
            }
        }
    }


    Console.Out.WriteLine($"Part 1: {acc}");



}




void Part2(string[] lines) {

    List<string> mapLines = new();
    int i = 0;
    while (lines[i].Length > 0) {
        mapLines.Add(lines[i]);
        i++;
    }
    i++;

    List<char> instructions = new ();
    while (i < lines.Length) {
        instructions.AddRange(lines[i]);
        i++;
    }

    var numRows = mapLines.Count;
    var numCols = mapLines[0].Count();
    var map = new char[numRows, numCols * 2];

    RC robotRc = null;
    for (int r = 0; r < numRows; r++)
    {
        for (int c = 0; c < numCols; c ++) {


            var ch = lines[r][c];

            switch (ch) {
                case '.':
                    map[r, c*2] = '.';
                    map[r, c*2+1] = '.';
                    break;
                case 'O':
                    map[r, c*2] = '[';
                    map[r, c*2+1] = ']';
                    break;
                case '#':
                    map[r, c*2] = '#';
                    map[r, c*2+1] = '#';
                    break;
                case '@':
                    map[r, c*2] = '@';
                    map[r, c*2+1] = '.';
                    break;
            }
            if (ch == '@') {
                robotRc = new RC(r, c*2);
            }
        }
    }

    //PrintMap2(map);
    numCols *= 2;

    foreach(var move in instructions) {
        switch (move) {
           case '^':
            {
                var rcNext = robotRc with {Row = robotRc.Row -1};
                var chNext = map[rcNext.Row, rcNext.Col];
                switch (chNext) {
                    case '.':
                        // Space.  The Robot can move
                        map[rcNext.Row, rcNext.Col] = '@';
                        map[robotRc.Row, robotRc.Col] = '.';
                        robotRc = rcNext;
                        break;
                    case '[':
                    case ']':

                        List<RC> boxesToPush = [];
                        Stack<RC> boxesToPushStack = new();

                        var boxRc = chNext switch {
                            '[' => rcNext,
                            ']' => rcNext with {Col = rcNext.Col -1}
                        };
                        
                        boxesToPushStack.Push(boxRc);
                        while (boxesToPushStack.Any()) {
                            
                            var popped = boxesToPushStack.Pop();
                            boxesToPush.Add(popped);

                            if (map[popped.Row-1, popped.Col] == '#' || map[popped.Row-1, popped.Col+1] == '#') {
                                // Wall above
                                goto noPush;
                            }

                            if (map[popped.Row-1, popped.Col] == '[') {
                                //Another box directly above
                                boxesToPushStack.Push(new RC(popped.Row-1, popped.Col));
                            } else {
                                if (map[popped.Row-1, popped.Col] == ']') {
                                    //Another box to the above left
                                    boxesToPushStack.Push(new RC(popped.Row-1, popped.Col-1));
                                }
                                if (map[popped.Row-1, popped.Col+1] == '[') {
                                    //Another box to the above right
                                    boxesToPushStack.Push(new RC(popped.Row-1, popped.Col+1));
                                }
                            }
                        }

                        foreach (var boxToPush in boxesToPush.OrderBy(x => x.Row).ThenBy(x => x.Col)) {
                            map[boxToPush.Row -1, boxToPush.Col] = '[';
                            map[boxToPush.Row -1, boxToPush.Col+1] = ']';
                            map[boxToPush.Row, boxToPush.Col] = '.';
                            map[boxToPush.Row, boxToPush.Col+1] = '.';
                        }

                        map[rcNext.Row, rcNext.Col] = '@';
                        map[robotRc.Row, robotRc.Col] = '.';
                        robotRc.Row--;

                    break;


                }
            }

            break;

            case 'v':
            {
                var rcNext = robotRc with {Row = robotRc.Row +1};
                var chNext = map[rcNext.Row, rcNext.Col];
                switch (chNext) {
                    case '.':
                        // Space.  The Robot can move
                        map[rcNext.Row, rcNext.Col] = '@';
                        map[robotRc.Row, robotRc.Col] = '.';
                        robotRc = rcNext;
                        break;
                    case '[':
                    case ']':

                        List<RC> boxesToPush = [];
                        Stack<RC> boxesToPushStack = new();

                        var boxRc = chNext switch {
                            '[' => rcNext,
                            ']' => rcNext with {Col = rcNext.Col -1}
                        };
                        
                        boxesToPushStack.Push(boxRc);
                        while (boxesToPushStack.Any()) {
                            
                            var popped = boxesToPushStack.Pop();
                            boxesToPush.Add(popped);

                            if (map[popped.Row+1, popped.Col] == '#' || map[popped.Row+1, popped.Col+1] == '#') {
                                // Wall below
                                goto noPush;
                            }

                            if (map[popped.Row+1, popped.Col] == '[') {
                                //Another box directly below
                                boxesToPushStack.Push(new RC(popped.Row+1, popped.Col));
                            } else {
                                if (map[popped.Row+1, popped.Col] == ']') {
                                    //Another box to the below left
                                    boxesToPushStack.Push(new RC(popped.Row+1, popped.Col-1));
                                }
                                if (map[popped.Row+1, popped.Col+1] == '[') {
                                    //Another box to the below right
                                    boxesToPushStack.Push(new RC(popped.Row+1, popped.Col+1));
                                }
                            }
                        }

                        foreach (var boxToPush in boxesToPush.OrderByDescending(x => x.Row).ThenBy(x => x.Col)) {
                            map[boxToPush.Row +1, boxToPush.Col] = '[';
                            map[boxToPush.Row +1, boxToPush.Col+1] = ']';
                            map[boxToPush.Row, boxToPush.Col] = '.';
                            map[boxToPush.Row, boxToPush.Col+1] = '.';
                        }

                        map[rcNext.Row, rcNext.Col] = '@';
                        map[robotRc.Row, robotRc.Col] = '.';
                        robotRc.Row++;

                    break;
                }
            }
             break;


            case '<':
            for (i = robotRc.Col -1; i >= 0; i--) {
                var ch = map[robotRc.Row, i];
                if (ch == '#') {
                    // Wall
                    break;
                } if (ch == ']') {
                    // Box
                } else if (ch == '.') {
                    // Space.  The Robot can move

                    for (int j = i; j < robotRc.Col; j++) {
                        map[robotRc.Row, j] = map[robotRc.Row, j+1];
                    }

                    map[robotRc.Row, robotRc.Col] = '.';
                    robotRc.Col--;
                    break;

                }
            }
            break;

            case '>':
            for (i = robotRc.Col +1; i < numCols; i++) {
                var ch = map[robotRc.Row, i];
                if (ch == '#') {
                    // Wall
                    break;
                } if (ch == '[') {
                    // Box
                } else if (ch == '.') {
                    // Space.  The Robot can move

                    for (int j = i; j > robotRc.Col; j--) {
                        map[robotRc.Row, j] = map[robotRc.Row, j-1];
                    }
                    map[robotRc.Row, robotRc.Col] = '.';
                    robotRc.Col++;
                    break;

                }
            }
             break;
        }
        noPush:;

        //Console.Out.WriteLine($"Move {move}");
        //PrintMap2(map);

    }


    var acc = 0L;
    for (int r = 0; r < numRows; r++)
    {
        for (int c = 0; c < numCols; c ++) {
            var ch = map[r, c];
            if (ch == '[') {
                var gps = r * 100 + c;
                acc += gps;
            }
        }
    }


    Console.Out.WriteLine($"Part 2: {acc}");
}


void PrintMap1(char[,] map) {
    var numRows = map.GetLength(0);
    var numCols = map.GetLength(1);

    for (int r = 0; r < numRows; r++)
    {
        Console.WriteLine();
        for (int c = 0; c < numCols; c ++) {
            var ch = map[r, c];
            if (ch == '@') {
                Console.Write(YELLOW_BG + '@' + NORMAL_BG);
            } else if (ch == '#') {
                Console.Write(RED_BG + "#" + NORMAL_BG);
            }
            else if (ch == 'O') {
                Console.Write(GREEN_BG + 'O' + NORMAL_BG);
            }
            else if (ch == '.') {
                Console.Write('.');
            }
        }
    }
    Console.WriteLine();
}


void PrintMap2(char[,] map) {
    var numRows = map.GetLength(0);
    var numCols = map.GetLength(1);

    for (int r = 0; r < numRows; r++)
    {
        Console.WriteLine();
        for (int c = 0; c < numCols; c ++) {
            var ch = map[r, c];
            if (ch == '@') {
                Console.Write(YELLOW_BG + '@' + NORMAL_BG);
            } else if (ch == '#') {
                Console.Write(RED_BG + "#" + NORMAL_BG);
            }
            else if (ch == '[' || ch == ']') {
                Console.Write(GREEN_BG + ch + NORMAL_BG);
            }
            else if (ch == '.') {
                Console.Write('.');
            }
        }
    }
    Console.WriteLine();
}

record RC {

    public RC(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public int Row;
    public int Col;
};

record Robot(RC Pos, RC Velocity) {

}

