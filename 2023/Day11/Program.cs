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


Part1(lines);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(string[] lines)
{
    var minRow = 0;
    var minCol = 0;
    var maxRow = lines.Length - 1;
    var maxCol = lines[0].Length - 1;

    bool[,] map = new bool[maxRow + 1, maxCol +1];


    List<RC> galaxys = new();
    for (var row = minRow; row <= maxRow; row++) {
        for (var col = minCol; col <= maxCol; col++) {
            bool found = map[row,col] = lines[row][col] == '#';
            if (found) {
                galaxys.Add(new RC {Row = row, Col = col});
            }
        }
    }

    bool[] rowsWithNoGalaxys = new bool[maxRow+1];
    for (var row = minRow; row <= maxRow; row++) {
        var hasGalaxy = false;
        for (var col = minCol; col <= maxCol; col++) {
            if (map[row,col]) {
                hasGalaxy = true;
                break;
            }
        }
        if (!hasGalaxy) {
            rowsWithNoGalaxys[row] = true;;
        }
    }

    
    bool[] colsWithNoGalaxys = new bool[maxCol+1];
    for (var col = minCol; col <= maxCol; col++) {
        var hasGalaxy = false;
        for (var row = minRow; row <= maxRow; row++) {
            if (map[row,col]) {
                hasGalaxy = true;
                break;
            }
        }
        if (!hasGalaxy) {
            colsWithNoGalaxys[col] = true;
        }
    }

    long[,] DistanceBetweenRows = new long[maxRow+1, maxRow+1];
    for (int ii = 0; ii <= maxRow; ii++) {
        var dist = 0;
        for (int jj = ii; jj <= maxRow; jj++) {
            DistanceBetweenRows[ii,jj] = dist;
            DistanceBetweenRows[jj,ii] = dist;
            if (rowsWithNoGalaxys[jj]) {
                dist = dist + 1_000_000;
            } else {
                dist++;
            }
        }
    }

    long[,] DistanceBetweenCols = new long[maxCol+1, maxCol+1];
    for (int ii = 0; ii <= maxCol; ii++) {
        var dist = 0;
        for (int jj = ii; jj <= maxCol; jj++) {
            DistanceBetweenCols[ii,jj] = dist;
            DistanceBetweenCols[jj,ii] = dist;
            if (colsWithNoGalaxys[jj]) {
                dist = dist + 1_000_000;
            } else {
                dist++;
            }
        }
    }


    var totalLength = 0L;
    for (int g1 = 0; g1 < galaxys.Count(); g1++) {
        for (int g2 = g1 + 1; g2 < galaxys.Count(); g2++) {

            var gal1 = galaxys[g1];
            var gal2 = galaxys[g2];

            var deltaRow = DistanceBetweenRows[gal1.Row, gal2.Row];
            var deltaCol = DistanceBetweenCols[gal1.Col, gal2.Col];
            var dist = deltaRow + deltaCol;

            totalLength += dist;

        }
    }



    Console.Out.WriteLine($"Len is {totalLength}");


}

void Part2(string[] lines)
{


}

class RC {
    public int Row;
    public int Col;
}