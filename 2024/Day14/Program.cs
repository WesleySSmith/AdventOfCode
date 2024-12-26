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

//Part1(lines);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");



void Part2(string[] lines)
{
    var numRows = sample ? 7 : 103 ; // lines.Length;
    var numCols = sample ? 11 : 101; // lines[0].Length;

    var regex = new Regex(@"p=([-0-9]+),([-0-9]+) v=([-0-9]+),([-0-9]+)");

    var robots = lines.Select(l => {
        var matches = regex.Match(l);
        return new Robot(
            new RC(int.Parse(matches.Groups[2].Value), int.Parse(matches.Groups[1].Value)),
            new RC(int.Parse(matches.Groups[4].Value), int.Parse(matches.Groups[3].Value))
        );
    }).ToList();

    for(int i = 0; i < 10_000; i++) {

        foreach(var robot in robots) {
            robot.Pos.Row = mod(robot.Pos.Row + robot.Velocity.Row, numRows);
            robot.Pos.Col = mod(robot.Pos.Col + robot.Velocity.Col, numCols);
        }

   if (i % 101 == 19 && i % 103 == 89) {
            var map = new bool[numRows, numCols];
            foreach(var robot in robots) {
                map[robot.Pos.Row, robot.Pos.Col] = true;
            }

    //Console.Clear();
    Console.Out.WriteLine($"i: {i}");
            for(int r = 0; r < numRows; r++) {
                Console.Out.WriteLine();
                for (int c = 0; c < numCols; c++) {
                    Console.Out.Write(map[r, c] ? '*' : ' ');
                }
            }
            Console.Out.WriteLine();

Console.Out.WriteLine();
//Thread.Sleep(300);
   }

    }

}


/*

Horizontal: 89
Vertical: 19, 120

*/

void Part1(string[] lines) {
    var numRows = sample ? 7 : 103 ; // lines.Length;
    var numCols = sample ? 11 : 101; // lines[0].Length;

    var r = new Regex(@"p=([-0-9]+),([-0-9]+) v=([-0-9]+),([-0-9]+)");

    var robots = lines.Select(l => {
        var matches = r.Match(l);
        return new Robot(
            new RC(int.Parse(matches.Groups[2].Value), int.Parse(matches.Groups[1].Value)),
            new RC(int.Parse(matches.Groups[4].Value), int.Parse(matches.Groups[3].Value))
        );
    }).ToList();

    for(int i = 0; i < 100; i++) {

        foreach(var robot in robots) {
            robot.Pos.Row = mod(robot.Pos.Row + robot.Velocity.Row, numRows);
            robot.Pos.Col = mod(robot.Pos.Col + robot.Velocity.Col, numCols);
        }
    }

    var tl = 0;
    var tr = 0;
    var bl = 0;
    var br = 0;
    foreach(var robot in robots) {
        if (robot.Pos.Row < numRows / 2 && robot.Pos.Col < numCols / 2) {
            tl++;
        } else if (robot.Pos.Row > numRows / 2 && robot.Pos.Col < numCols / 2) {
            bl++;
        } else if (robot.Pos.Row < numRows / 2 && robot.Pos.Col > numCols / 2) {
            tr++;
        } else if (robot.Pos.Row > numRows / 2 && robot.Pos.Col > numCols / 2) {
            br++;
        }
    }

    var score = tl * tr * bl * br;

    Console.Out.WriteLine($"Part 1: {score}");
}

int mod(int x, int m) {
    return (x%m + m)%m;
}

class RC {

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
