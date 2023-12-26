#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using MoreLinq;


bool sample = false;


string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();


//Part1(lines);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(string[] lines) {
   long sum = 0;
   foreach (var line in lines) {
        var nums = line.Split(" ").Select(int.Parse).ToArray();
        var a = new int[nums.Length,nums.Length + 1];
        for (int c = 0; c < nums.Length;c++) {
            a[0, c] = nums[c];
        }

        int r;
        for (r = 1; r < nums.Length; r++) {
            bool allZero = true;
            for (int c = 0; c < nums.Length - r; c++) {
                var diff = a[r-1,c+1] - a[r-1, c];
                a[r, c] = diff;
                if (diff != 0) {
                    allZero = false;
                }

            }
            if (allZero) {
                break;
            }
        }

        for (var r2 = r -1; r2 >= 0; r2--) {
            var destCol = nums.Length - r2;
            a[r2, destCol] = a[r2+1, destCol-1] + a[r2, destCol-1];
        }

        var nextSeq = a[0, a.GetLength(1)-1];
        Console.WriteLine($"{nextSeq}");
        sum += nextSeq;
   }
    Console.Out.WriteLine($"Sum is {sum}");

}

void Part2(string[] lines)
{
    long sum = 0;
    foreach (var line in lines) {
        var nums = line.Split(" ").Select(int.Parse).ToArray();
        var a = new int[nums.Length,nums.Length + 1];
        for (int c = 0; c < nums.Length;c++) {
            a[0, c] = nums[c];
        }

        int r;
        for (r = 1; r < nums.Length; r++) {
            bool allZero = true;
            for (int c = 0; c < nums.Length - r; c++) {
                var diff = a[r-1,c+1] - a[r-1, c];
                a[r, c] = diff;
                if (diff != 0) {
                    allZero = false;
                }

            }
            if (allZero) {
                break;
            }
        }

        for (var r2 = r - 1; r2 >= 0; r2--) {
            var destCol = nums.Length;
            a[r2, destCol] = a[r2, 0] - a[r2+1, destCol];
        }

        var prevSeq = a[0, a.GetLength(1)-1];
        Console.WriteLine($"{prevSeq}");
        sum += prevSeq;
   }
    Console.Out.WriteLine($"Sum is {sum}");  
   
}

/*
10  13  16  21  30  45     5  
3   3   5   9  15           5   
0   2   4   6               -2   
2   2   2                   2   
0   0                       0   
*/