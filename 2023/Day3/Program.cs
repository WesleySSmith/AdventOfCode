#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
//using MoreLinq;


bool sample = false;


string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

//lines = lines[3..5];
//Part1(lines);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(string[] lines)
{ 

    char[,] expandedMap = new char[(lines.Length + 2), (lines[0].Length + 2)];

    for (var row = -1; row <= lines.Length; row++) {
        for (var col = -1; col <= lines[0].Length; col++) {

            if (row == -1 || row == lines.Length || col == -1 || col == lines[0].Length) {
                expandedMap[row + 1, col + 1] = '.';
            } else {
                expandedMap[row+1, col+1] = lines[row][col];
            }
        }
    }

    var symbolMap = GetSymbolMap(expandedMap);


    long sum = 0;
    for (var row = 1; row < expandedMap.GetLength(0); row++) {

        int? numStartCol = null;
        int? numEndCol = null;
        string num = "";

        for (var col = 1; col < expandedMap.GetLength(1); col++) {
           
            char c = expandedMap[row,col];
            //Console.WriteLine($"{row},{col}: {c}");
            if (c >= '0' && c <= '9') {
                if (numStartCol == null) {
                    numStartCol = col;
                    //Console.WriteLine($"NumStartCol: {numStartCol}");
                }
                num += c.ToString();
            } else if (numStartCol != null) {
                numEndCol = col-1;

                //Console.WriteLine($"NumEndCol: {numEndCol}");

                bool foundSymbol = false;
                for (var check = numStartCol.Value; check <= numEndCol.Value; check++) {
                    if (symbolMap[row, check]) {
                        foundSymbol = true;
                        break;
                    }
                }

                if (foundSymbol) {
                    Console.WriteLine($"Adding {num}");
                    sum += int.Parse(num);
                }

                //Console.WriteLine("Resetting");
                numStartCol = null;
                numEndCol = null;
                num = "";
            }
        }
    }


    Console.Out.WriteLine($"Sum is {sum}");
}

bool[,] GetSymbolMap(char[,] lines) {
   var minRowIndex = 0;
   var minColIndex = 0;
   var maxRowIndex = lines.GetLength(0);
   var maxColIndex = lines.GetLength(1);
   bool[,] symbolMap = new bool[maxRowIndex, maxColIndex];
    for (var row = 0; row < maxRowIndex; row++) {
        for (var col = 0; col < maxColIndex; col++) {
            char c = lines[row,col];
            if (!(c == '.' || (c >= '0' && c <= '9'))) {
                symbolMap[row-1, col -1] = true;
                symbolMap[row-1, col] = true;
                symbolMap[row-1, col +1] = true;
                symbolMap[row, col -1] = true;
                symbolMap[row, col] = true;
                symbolMap[row, col +1] = true;
                symbolMap[row+1, col -1] = true;
                symbolMap[row+1, col] = true;
                symbolMap[row+1, col +1] = true;
            }
        }
    }

    return symbolMap;


}



int?[,] GetGearMap(char[,] lines) {
   var maxRowIndex = lines.GetLength(0);
   var maxColIndex = lines.GetLength(1);
   var symbolMap = new int?[maxRowIndex, maxColIndex];
    for (var row = 0; row < maxRowIndex; row++) {
        for (var col = 0; col < maxColIndex; col++) {
            char c = lines[row,col];
            if (c == '*') {
                var v = row * 1000 + col;
                symbolMap[row-1, col -1] = v;
                symbolMap[row-1, col] = v;
                symbolMap[row-1, col +1] = v;
                symbolMap[row, col -1] = v;
                symbolMap[row, col] = v;
                symbolMap[row, col +1] = v;
                symbolMap[row+1, col -1] = v;
                symbolMap[row+1, col] = v;
                symbolMap[row+1, col +1] = v;
            }
        }
    }

    return symbolMap;


}

void Part2(string[] lines)
{
    char[,] expandedMap = new char[(lines.Length + 2), (lines[0].Length + 2)];

    for (var row = -1; row <= lines.Length; row++) {
        for (var col = -1; col <= lines[0].Length; col++) {

            if (row == -1 || row == lines.Length || col == -1 || col == lines[0].Length) {
                expandedMap[row + 1, col + 1] = '.';
            } else {
                expandedMap[row+1, col+1] = lines[row][col];
            }
        }
    }

    var symbolMap = GetGearMap(expandedMap);

    // for (int ii = 0; ii < symbolMap.GetLength(0); ii++) {
    //     for (int jj = 0; jj < symbolMap.GetLength(1); jj++) {
    //         var v = symbolMap[ii,jj];
    //         if (v.HasValue) {
    //             Console.Write($"({v.Value.Item1},{v.Value.Item2})");
    //         } else {
    //             Console.Write($"( , )");
    //         }
            
    //     }
    //     Console.WriteLine();
    // }



    long sum = 0;
    var map = new Dictionary<int, int>();
    for (var row = 1; row < expandedMap.GetLength(0); row++) {

        int? numStartCol = null;
        int? numEndCol = null;
        string num = "";

      

        for (var col = 1; col < expandedMap.GetLength(1); col++) {
           
            char c = expandedMap[row,col];
            //Console.WriteLine($"{row},{col}: {c}");
            if (c >= '0' && c <= '9') {
                if (numStartCol == null) {
                    numStartCol = col;
                    //Console.WriteLine($"NumStartCol: {numStartCol}");
                }
                num += c.ToString();
            } else if (numStartCol != null) {
                numEndCol = col-1;

                //Console.WriteLine($"NumEndCol: {numEndCol}");

                int? foundSymbol = null;
                for (var check = numStartCol.Value; check <= numEndCol.Value; check++) {
                    if (symbolMap[row, check].HasValue) {
                        foundSymbol = symbolMap[row, check];
                        break;
                    }
                }

                if (foundSymbol.HasValue) {
                    //Console.WriteLine($"foundSymbol: {foundSymbol.Value}");
                    if (map.TryGetValue(foundSymbol.Value, out int value)) {
                        //Console.WriteLine("In map already");
                        var first = value;
                        var second = int.Parse(num);
                        var ratio = first * second;
                        sum += ratio;
                    } else {
                        map.Add(foundSymbol.Value, int.Parse(num));
                        //Console.WriteLine($"Adding to map {foundSymbol.Value} = {int.Parse(num)}; Keys now {map.Keys.Count}");
                    }
                }

                //Console.WriteLine("Resetting");
                numStartCol = null;
                numEndCol = null;
                num = "";
            }
        }
    }


    Console.Out.WriteLine($"Sum is {sum}");
}

