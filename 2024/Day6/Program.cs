#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
//using MoreLinq;


string NORMAL_BG      = Console.IsOutputRedirected ? "" : "\x1b[49m";
string RED_BG        = Console.IsOutputRedirected ? "" : "\x1b[41m";
string GREEN_BG      = Console.IsOutputRedirected ? "" : "\x1b[42m";
string YELLOW_BG      = Console.IsOutputRedirected ? "" : "\x1b[43m";
string X1_BG      = Console.IsOutputRedirected ? "" : "\x1b[44m";
string X2_BG      = Console.IsOutputRedirected ? "" : "\x1b[45m";

bool sample = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
//Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

//Part1(lines);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(string[] lines)
{
    
    var map = lines.Select(l => l.Select(c => c).ToArray()).ToArray();

    int numRows = map.Length;
    int numCols = map[0].Length;

    int currentRow = -1;
    int currentCol = -1;
    Direction direction = (Direction)(-1);

    for (int row = 0; row < numRows; row++) {
        for (int col = 0; col < numCols; col++) {
            if (map[row][col] == '^') {
                currentRow = row;
                currentCol = col;
                direction = Direction.Up;
                map[row][col] = 'X';
                break;
            }
        }
    }

    while (true) {
        //PrintMap(map, currentRow, currentCol, direction);

        int nextCol = direction switch { Direction.Up or Direction.Down => currentCol, Direction.Left => currentCol -1, Direction.Right => currentCol + 1};   
        int nextRow = direction switch { Direction.Up => currentRow -1, Direction.Down => currentRow + 1, Direction.Left or Direction.Right => currentRow};

if (!(nextRow >= 0 && nextRow < numRows && nextCol >= 0 && nextCol < numCols)) {
break;
}
        switch (map[nextRow][nextCol]) {
            case '.':
            case 'X':
                map[nextRow][nextCol] = 'X';
                currentCol = nextCol;
                currentRow = nextRow;
                break;
            case '#':
                direction = direction switch {
                     Direction.Up => Direction.Right,
                     Direction.Right => Direction.Down,
                     Direction.Down => Direction.Left,
                     Direction.Left => Direction.Up
                };
                break;
        }
    }

    var count = 0;
    for (int row = 0; row < numRows; row++) {
        for (int col = 0; col < numCols; col++) {
            if (map[row][col] == 'X') {
                count++;
            }
        }
    }

    Console.Out.WriteLine($"Part 1: {count}");
}


void Part2(string[] lines) {
    
    var originalmap = lines.Select(l => l.Select(c => new Position {Char = c}).ToArray()).ToArray();

    int numRows = originalmap.Length;
    int numCols = originalmap[0].Length;

    int startRow = -1;
    int startCol = -1;
    Direction startDirection = (Direction)(-1);

    for (int row = 0; row < numRows; row++) {
        for (int col = 0; col < numCols; col++) {
            if (originalmap[row][col].Char == '^') {
                startRow = row;
                startCol = col;
                startDirection = Direction.Up;
                break;
            }
        }
    }

    int numLoops = 0;
    for (int row = 0; row < numRows; row++) {
        for (int col = 0; col < numCols; col++) {

            // if (!(row == 6 && col == 3)){
            //     continue;
            // }
            
            if (originalmap[row][col].Char is '#' or '^') {
                // Can't put a block here, as there's already one, or it's start position
                continue;
            }
            
            //Console.WriteLine("----------------------");
    
            var map = lines.Select(l => l.Select(c => new Position {Char = c}).ToArray()).ToArray();

            map[startRow][startCol].Char = 'X';
            map[row][col].Char = 'O';
            int currentRow = startRow;
            int currentCol = startCol;
            Direction direction = startDirection;

            while (true) {
                //PrintMap2(map, currentRow, currentCol, direction);

                int nextCol = direction switch { Direction.Up or Direction.Down => currentCol, Direction.Left => currentCol -1, Direction.Right => currentCol + 1};   
                int nextRow = direction switch { Direction.Up => currentRow -1, Direction.Down => currentRow + 1, Direction.Left or Direction.Right => currentRow};

                if (!(nextRow >= 0 && nextRow < numRows && nextCol >= 0 && nextCol < numCols)) {
                    goto foundNoLoop;
                }
                switch (map[nextRow][nextCol].Char) {
                    case '.':
                    case 'X':
                        var pos = map[nextRow][nextCol];
                        pos.Char = 'X';
                        currentCol = nextCol;
                        currentRow = nextRow;
                        switch (direction) {
                            case Direction.Up:
                                if (pos.Up) { goto foundLoop;}
                                pos.Up = true;
                                break;
                            case Direction.Down:
                                if (pos.Down) { goto foundLoop;}
                                pos.Down = true;
                                break;
                            case Direction.Right:
                                if (pos.Right) { goto foundLoop;}
                                pos.Right = true;
                                break;
                            case Direction.Left:
                                if (pos.Left) { goto foundLoop;}
                                pos.Left = true;
                                break;
                        }
                        break;
                    case '#':
                    case 'O':
                        direction = direction switch {
                            Direction.Up => Direction.Right,
                            Direction.Right => Direction.Down,
                            Direction.Down => Direction.Left,
                            Direction.Left => Direction.Up
                        };
                        break;
                }
            }

            foundLoop:
            numLoops++;
            foundNoLoop:;
        }
    }

    Console.Out.WriteLine($"Part 2: {numLoops}");
}



void PrintMap(char[][] map, int currentRow, int currentCol, Direction direction) {
    Console.WriteLine($"Direction: {direction}");
    int numRows = map.Length;
    int numCols = map[0].Length;

    
    for (int row = 0; row < numRows; row++) {
        for (int col = 0; col < numCols; col++) {
            bool currentPos = row == currentRow && col == currentCol;
            if (currentPos) {
                  Console.Write(GREEN_BG);
            }
            Console.Write(map[row][col]);
            if (currentPos) {
                Console.Write(NORMAL_BG);
            }
        }
        Console.WriteLine();
    }
    
    Console.WriteLine();
    Console.WriteLine();
}

void PrintMap2(Position[][] map, int currentRow, int currentCol, Direction direction) {
    Console.WriteLine($"Direction: {direction}");
    int numRows = map.Length;
    int numCols = map[0].Length;

    
    for (int row = 0; row < numRows; row++) {
        for (int col = 0; col < numCols; col++) {
            bool currentPos = row == currentRow && col == currentCol;
            if (currentPos) {
                  Console.Write(GREEN_BG);
            }
            Console.Write(map[row][col].Char);
            if (currentPos) {
                Console.Write(NORMAL_BG);
            }
        }
        Console.WriteLine();
    }
    
    Console.WriteLine();
    Console.WriteLine();
}

record Position {
    public char Char;
    public bool Left;
    public bool Right;
    public bool Up;
    public bool Down;
}

enum Direction {
    Up,
    Down,
    Left,
    Right
}