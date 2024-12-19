#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using MoreLinq;
//using MoreLinq;

bool sample = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
//Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

//Part1(lines);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(string[] lines)
{
    int numRows = lines.Length;
    int numCols = lines[0].Length;
    List<(char c, RC rc)> antennas = new();
    for (int row = 0; row < numRows; row++) {
        for (int col = 0; col < numCols; col++) {
            var c = lines[row][col];
            if (c != '.') {
                antennas.Add((c, new RC(row, col)));
            }
        }
    }

    HashSet<RC> antinodes = new();
    var lookup = antennas.ToLookup(a => a.c, a => a.rc);

    foreach (var antennaType in lookup) {
        var antennasOfOneType = antennaType.ToArray();

        for (int i = 0; i < antennasOfOneType.Length; i++) {
            for (int j = i + 1; j < antennasOfOneType.Length; j++) {

                var deltaRow = antennasOfOneType[i].row - antennasOfOneType[j].row;
                var deltaCol = antennasOfOneType[i].col - antennasOfOneType[j].col;

                var possible1 = new RC(antennasOfOneType[i].row + deltaRow, antennasOfOneType[i].col + deltaCol);
                var possible2 = new RC(antennasOfOneType[i].row - deltaRow*2, antennasOfOneType[i].col - deltaCol*2);

                if (possible1.InBounds(numRows, numCols)) {
                    antinodes.Add(possible1);
                }
                if (possible2.InBounds(numRows, numCols)) {
                    antinodes.Add(possible2);
                }
            }
        }
    }

    // foreach(var antinode in antinodes.OrderBy(x => x.row).ThenBy(x => x.col)) {
    //     Console.Out.WriteLine(antinode);
    // }
    Console.Out.WriteLine($"Part 1: {antinodes.Count()}");
}


void Part2(string[] lines) {
    int numRows = lines.Length;
    int numCols = lines[0].Length;
    List<(char c, RC rc)> antennas = new();
    for (int row = 0; row < numRows; row++) {
        for (int col = 0; col < numCols; col++) {
            var c = lines[row][col];
            if (c != '.') {
                antennas.Add((c, new RC(row, col)));
            }
        }
    }

    HashSet<RC> antinodes = new();
    var lookup = antennas.ToLookup(a => a.c, a => a.rc);

    foreach (var antennaType in lookup) {
        var antennasOfOneType = antennaType.ToArray();

        for (int i = 0; i < antennasOfOneType.Length; i++) {
            for (int j = i + 1; j < antennasOfOneType.Length; j++) {

                var deltaRow = antennasOfOneType[i].row - antennasOfOneType[j].row;
                var deltaCol = antennasOfOneType[i].col - antennasOfOneType[j].col;

                var idx = 0;
                bool added;
                do {
                    added = false;
                    var possible1 = new RC(antennasOfOneType[i].row + deltaRow*idx, antennasOfOneType[i].col + deltaCol*idx);
                    var possible2 = new RC(antennasOfOneType[i].row - deltaRow*idx, antennasOfOneType[i].col - deltaCol*idx);

                    if (possible1.InBounds(numRows, numCols)) {
                        antinodes.Add(possible1);
                        added = true;
                    }
                    if (possible2.InBounds(numRows, numCols)) {
                        antinodes.Add(possible2);
                        added = true;
                    }
                    idx++;
                } while (added);
            }
        }
    }

    // foreach(var antinode in antinodes.OrderBy(x => x.row).ThenBy(x => x.col)) {
    //     Console.Out.WriteLine(antinode);
    // }
    Console.Out.WriteLine($"Part 2: {antinodes.Count()}"); 
}

record RC(int row, int col) {
    public bool InBounds(int numRows, int numCols) {
        return row >= 0 && row < numRows && col >= 0 && col < numCols;
    }
}

