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

//Part1(lines);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(string[] lines)
{
    Cell[,] contraption = InitContraption(lines, minRow, maxRow, minCol, maxCol);

    var initialLight = new Light { ArrivingAtCell = contraption[0, 0], Moving = Dir.E };
    int energized = CountEnergized(minRow, maxRow, minCol, maxCol, contraption, initialLight);

    Console.Out.WriteLine($"Energized is {energized}.");
    
}


void Part2(string[] lines)
{
    Cell[,] contraption = InitContraption(lines, minRow, maxRow, minCol, maxCol);

    var best = 0;
    for (int row = minRow; row <= maxRow; row++)
    {
        ResetContraption(contraption, minRow, maxRow, minCol, maxCol);
        var initialLight = new Light { ArrivingAtCell = contraption[row, 0], Moving = Dir.E };
        int energized = CountEnergized(minRow, maxRow, minCol, maxCol, contraption, initialLight);
        best = Math.Max(best, energized);

        ResetContraption(contraption, minRow, maxRow, minCol, maxCol);
        initialLight = new Light { ArrivingAtCell = contraption[row, maxCol], Moving = Dir.W };
        energized = CountEnergized(minRow, maxRow, minCol, maxCol, contraption, initialLight);
        best = Math.Max(best, energized);
    }

    for (int col = minCol; col <= maxCol; col++)
    {

        ResetContraption(contraption, minRow, maxRow, minCol, maxCol);
        var initialLight = new Light { ArrivingAtCell = contraption[minRow, col], Moving = Dir.S };
        int energized = CountEnergized(minRow, maxRow, minCol, maxCol, contraption, initialLight);
        best = Math.Max(best, energized);

        ResetContraption(contraption, minRow, maxRow, minCol, maxCol);
        initialLight = new Light { ArrivingAtCell = contraption[maxRow, col], Moving = Dir.N };
        energized = CountEnergized(minRow, maxRow, minCol, maxCol, contraption, initialLight);
        best = Math.Max(best, energized);

        
    }

    Console.Out.WriteLine($"BEST Energized is {best}.");

}


void Enqueue(Queue<Light> lights, Cell cell, Dir nextDir) {
    var nextCell = cell.Neighbor(nextDir);
    if (nextCell != null) {
        lights.Enqueue(new Light{ArrivingAtCell = nextCell, Moving = nextDir});
    }
}


void MoveLight(Queue<Light> lights, Light light)
    {
        var cell = light.ArrivingAtCell;
        var dir = light.Moving;
        var ch = cell.Char;
        var wasEnergized = cell.Energized[(int)dir];
        if (!wasEnergized)
        {
            cell.Energized[(int)dir] = true;
        }
        else
        {
            return;
        }
        if (ch == '.')
        {
            Enqueue(lights, cell, dir);
        }
        else if (ch == '/')
        {
            var nextDir = dir switch
            {
                Dir.N => Dir.E,
                Dir.S => Dir.W,
                Dir.W => Dir.S,
                Dir.E => Dir.N
            };
            Enqueue(lights, cell, nextDir);
        }
        else if (ch == '\\')
        {
            var nextDir = dir switch
            {
                Dir.N => Dir.W,
                Dir.S => Dir.E,
                Dir.W => Dir.N,
                Dir.E => Dir.S
            };
            Enqueue(lights, cell, nextDir);
        }
        else if (ch == '|')
        {
            if (dir is Dir.N or Dir.S)
            {
                Enqueue(lights, cell, dir);
            }
            else
            {
                Enqueue(lights, cell, Dir.N);
                Enqueue(lights, cell, Dir.S);
            }
        }
        else if (ch == '-')
        {
            if (dir is Dir.E or Dir.W)
            {
                Enqueue(lights, cell, dir);
            }
            else
            {
                Enqueue(lights, cell, Dir.E);
                Enqueue(lights, cell, Dir.W);
            }
        }
    }

    int CountEnergized(int minRow, int maxRow, int minCol, int maxCol, Cell[,] contraption, Light initialLight)
    {
        var lights = new Queue<Light>();
        lights.Enqueue(initialLight);

        while (lights.TryDequeue(out var light))
        {
            MoveLight(lights, light);
        }

        var energized = 0;
        for (int row = minRow; row <= maxRow; row++)
        {
            for (int col = minCol; col <= maxCol; col++)
            {
                var cell = contraption[row, col];
                energized += cell.Energized.Any(e => e) ? 1 : 0;
            }
        }

        return energized;
    }


void ResetContraption(Cell[,] contraption, int minRow, int maxRow, int minCol, int maxCol)
{
    for (int row = minRow; row <= maxRow; row++)
    {
        for (int col = minCol; col <= maxCol; col++)
        {
            Array.Clear(contraption[row,col].Energized);
        }
    }
}

Cell[,] InitContraption(string[] lines, int minRow, int maxRow, int minCol, int maxCol)
{
    var contraption = new Cell[lines.Length, lines[0].Length];
    for (int row = minRow; row <= maxRow; row++)
    {
        for (int col = minCol; col <= maxCol; col++)
        {
            contraption[row, col] = new Cell(lines[row][col], row, col);
        }
    }

    for (int row = minRow; row <= maxRow; row++)
    {
        for (int col = minCol; col <= maxCol; col++)
        {
            var cell = contraption[row, col];
            if (cell.R > minRow)
            {
                cell.N = contraption[cell.R - 1, cell.C];
            }
            if (cell.R < maxRow)
            {
                cell.S = contraption[cell.R + 1, cell.C];
            }
            if (cell.C > minCol)
            {
                cell.W = contraption[cell.R, cell.C - 1];
            }
            if (cell.C < maxCol)
            {
                cell.E = contraption[cell.R, cell.C + 1];
            }
        }
    }

    return contraption;
}



record Cell {
    public char Char;

    public Cell N;
    public Cell S;
    public Cell E;
    public Cell W;


    public int R;
    public int C;
    public bool[] Energized = new bool[4];



    public Cell(char ch, int r, int c) {
        Char = ch;
        R = r;
        C = c;
    }

    public Cell Neighbor(Dir d) => 
    d switch
    {
        Dir.N => N,
        Dir.S => S,
        Dir.E => E,
        Dir.W => W
    };



}

record Light {
    public Cell ArrivingAtCell;
    public Dir Moving;
}

public enum Dir {
    N, S, E, W
}