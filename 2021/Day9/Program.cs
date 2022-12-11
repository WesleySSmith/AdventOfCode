//using MoreLinq;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Numerics;

public class Day8 {

    public static string[] SegmentsAsStrings;
    static List<List<int>> allMappings;

    public static void Main() {

        string[] lines = File.ReadAllLines("input.txt"); 
        //string[] lines = File.ReadAllLines("sample.txt"); 
        Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");
        var sw = Stopwatch.StartNew();

        var heightmap = lines.Select(l => l.Select(c => c - '0').ToArray()).ToArray();
        var height = heightmap.Count();
        var width = heightmap.First().Count();

        var heightArray = new int[height, width];
        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                heightArray[i,j] = heightmap[i][j];
            }
        }
        Console.Out.WriteLine($"Parse time: {sw.ElapsedMilliseconds}");
        //Part1(heightArray);
        Part2(heightArray);

        Console.Out.WriteLine($"Total time {sw.ElapsedMilliseconds}");
    }

    static void Part1(int[,] heightArray) {
        var width = heightArray.GetLength(1);
        var height = heightArray.GetLength(0);
        var lowPoints = new List<int>();
        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                var up = i == 0 ? int.MaxValue : heightArray[i-1,j];
                var down = i == height - 1 ? int.MaxValue : heightArray[i+1,j];
                var left = j == 0 ? int.MaxValue : heightArray[i, j-1];
                var right = j == width -1 ? int.MaxValue : heightArray[i, j+1];
                var n = heightArray[i,j];
                if (n < up && n < down && n < left && n < right) {
                    lowPoints.Add(n);
                }
            }
        }
        
        var riskLevel = lowPoints.Sum(p => p+1);
        Console.Out.WriteLine($"Risk Level: {riskLevel}");
    }

    static void Part2(int[,] heightArray) {
        var width = heightArray.GetLength(1);
        var height = heightArray.GetLength(0);
        var lowPoints = new List<(int, int)>();
        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                var up = i == 0 ? int.MaxValue : heightArray[i-1,j];
                var down = i == height - 1 ? int.MaxValue : heightArray[i+1,j];
                var left = j == 0 ? int.MaxValue : heightArray[i, j-1];
                var right = j == width -1 ? int.MaxValue : heightArray[i, j+1];
                var n = heightArray[i,j];
                if (n < up && n < down && n < left && n < right) {
                    lowPoints.Add((i, j));
                }
            }
        }
        var sizes = new List<int>(lowPoints.Count);
        foreach (var lowPoint in lowPoints) {
            Stack<(int row,int col)> toInvestigate = new Stack<(int, int)>();
            toInvestigate.Push(lowPoint);
            int count = 0;
            while(toInvestigate.Any()) {
                var pt = toInvestigate.Pop();
                if (heightArray[pt.row, pt.col] != 9) {
                    count++;
                    if (pt.row != 0) toInvestigate.Push((pt.row-1, pt.col));
                    if (pt.row != height - 1) toInvestigate.Push((pt.row + 1, pt.col));
                    if (pt.col != 0 )  toInvestigate.Push((pt.row, pt.col -1));
                    if (pt.col != width - 1 ) toInvestigate.Push((pt.row, pt.col + 1));
                    heightArray[pt.row, pt.col] = 9;
                }
            }
            sizes.Add(count);
        }
        var answer = sizes.OrderByDescending(s => s).Take(3).Aggregate(1, (acc, n) => acc * n);
        Console.Out.WriteLine($"Largest 3: {answer}");
    }
}

















