#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using MoreLinq;


bool sample = false;


string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();


//Part1(lines);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(string[] lines)
{
    var mirror = lines.Select(l => l.Select(c => c).ToArray()).ToArray();
    
    for (int row = 1; row < mirror.Length; row++) {
        for (int col = 0; col < mirror[0].Length; col++) {
            if (mirror[row][col] == 'O') {
                var newPos = 0;
                for (int row2 = row-1; row2>= 0; row2--) {
                    if (mirror[row2][col] != '.') {
                        newPos = row2+1;
                        break;
                    }
                }
               if (newPos != row) {
                mirror[newPos][col] = 'O';
                mirror[row][col] = '.';
               }
            }
        }
    }

    var load = 0L;
    for (int row = 0; row < mirror.Length; row++) {
        for (int col = 0; col < mirror[0].Length; col++) {
            if (mirror[row][col] == 'O') {
                load += mirror.Length - row;
            }
        }
    }


    Console.Out.WriteLine($"Load is {load}");


}

void Part2(string[] lines)
{
    var mirror = lines.Select(l => l.Select(c => c).ToArray()).ToArray();


    List<char[][]> mirrors = new();

    for (int ii = 0; ii < 1000; ii++)
    {
        Cycle(mirror);

        // PrintMirror(mirror);
        
    }

    Console.WriteLine($"Ran 1000 spins");

    var baseMirror = Copy(mirror);
    int cycle = -1;
    for (int ii = 0; ii < 100; ii++)
    {
        Cycle(mirror);

        if (IsEqual(mirror, baseMirror)) {
            cycle = ii + 1;
            break;
        }
    }
    if (cycle == -1) {
        throw new Exception("Didn't find cycle");
    }

    Console.WriteLine($"Cycle is {cycle}");


    var remainder = (1000000000 - 1000 - cycle) % cycle;

    for (int ii = 0; ii < remainder; ii++)
    {
        Cycle(mirror);
    }

    var load = 0L;
    for (int row = 0; row < mirror.Length; row++)
    {
        for (int col = 0; col < mirror[0].Length; col++)
        {
            if (mirror[row][col] == 'O')
            {
                load += mirror.Length - row;
            }
        }
    }


    Console.Out.WriteLine($"Load is {load}");

}

static char[][] Copy(char[][] original) {
    return original.Select(row => row.Clone() as char[]).ToArray();
}

static bool IsEqual(char[][] a, char[][] b) {
    for (int row = 0; row < a.Length; row++)
    {
        for (int col = 0; col < a[0].Length; col++)
        {
            if (a[row][col] != b[row][col]) {
                return false;
            }
        }
    }
    return true;
}

static void PrintMirror(char[][] mirror)
{
    for (int row = 0; row < mirror.Length; row++)
    {
        for (int col = 0; col < mirror[0].Length; col++)
        {
            Console.Write(mirror[row][col]);
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

static void Cycle(char[][] mirror)
{
    // North

    for (int row = 1; row < mirror.Length; row++)
    {
        for (int col = 0; col < mirror[0].Length; col++)
        {
            if (mirror[row][col] == 'O')
            {
                var newPos = 0;
                for (int row2 = row - 1; row2 >= 0; row2--)
                {
                    if (mirror[row2][col] != '.')
                    {
                        newPos = row2 + 1;
                        break;
                    }
                }
                if (newPos != row)
                {
                    mirror[newPos][col] = 'O';
                    mirror[row][col] = '.';
                }
            }
        }
    }
    //PrintMirror(mirror);

    // West
    for (int col = 1; col < mirror[0].Length; col++)
    {
        for (int row = 0; row < mirror.Length; row++)
        {
            if (mirror[row][col] == 'O')
            {
                var newPos = 0;
                for (int col2 = col - 1; col2 >= 0; col2--)
                {
                    if (mirror[row][col2] != '.')
                    {
                        newPos = col2 + 1;
                        break;
                    }
                }
                if (newPos != col)
                {
                    mirror[row][newPos] = 'O';
                    mirror[row][col] = '.';
                }
            }
        }
    }
    //PrintMirror(mirror);

    // South
    for (int row = mirror.Length - 2; row >= 0; row--)
    {
        for (int col = 0; col < mirror[0].Length; col++)
        {
            if (mirror[row][col] == 'O')
            {
                var newPos = mirror.Length - 1;
                for (int row2 = row + 1; row2 < mirror.Length; row2++)
                {
                    if (mirror[row2][col] != '.')
                    {
                        newPos = row2 - 1;
                        break;
                    }
                }
                if (newPos != row)
                {
                    mirror[newPos][col] = 'O';
                    mirror[row][col] = '.';
                }
            }
        }
    }
    //PrintMirror(mirror);

    // East
    for (int col = mirror[0].Length - 2; col >= 0; col--)
    {
        for (int row = 0; row < mirror.Length; row++)
        {
            if (mirror[row][col] == 'O')
            {
                var newPos = mirror[0].Length - 1;
                for (int col2 = col + 1; col2 < mirror.Length; col2++)
                {
                    if (mirror[row][col2] != '.')
                    {
                        newPos = col2 - 1;
                        break;
                    }
                }
                if (newPos != col)
                {
                    mirror[row][newPos] = 'O';
                    mirror[row][col] = '.';
                }
            }
        }
    }
}