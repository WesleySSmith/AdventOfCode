//using MoreLinq;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Numerics;

public class Day11 {

    public static void Main() {

        string[] lines = File.ReadAllLines("input.txt"); 
        //string[] lines = File.ReadAllLines("sample.txt"); 
        Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");
        var sw = Stopwatch.StartNew();

        var coords = lines.Where(l => l.Length > 1 && char.IsDigit(l[0])).Select(l => l.Split(',')).Select(c => (int.Parse(c[0]), int.Parse(c[1])));
        var instructions = lines.Where(l => l.Length > 1 && l[0] == 'f').Select(l => l.Substring(11).Split('=')).Select(c => (c[0][0], int.Parse(c[1])));
        
        Console.Out.WriteLine($"Parse time: {sw.ElapsedMilliseconds}");
        Part2(coords, instructions);
        //Part2(coords, instructions);

        Console.Out.WriteLine($"Total time {sw.ElapsedMilliseconds}");
    }

    static void Part1(IEnumerable<(int x, int y)> coords, IEnumerable<(char direction, int)> instructions) {
        int maxX = coords.Max(coord => coord.x);
        int maxY = coords.Max(coord => coord.y);
        var paper = new bool[maxY + 1, maxX + 1];
        foreach(var coord in coords) {
            paper[coord.y, coord.x] = true;
        }

        var instruction = instructions.First();
        if (instruction.direction == 'y') {
            var newMaxX = maxX;
            var newMaxY = maxY / 2 - 1;
            var newPaper = new bool[newMaxY + 1, newMaxX + 1];

            for (int x = 0; x <= newMaxX; x++) {
                for (int y = 0; y <= newMaxY; y++) {
                    newPaper[y, x] = paper[y,x];
                }
            }
            for (int x = 0; x <= newMaxX; x++) {
                for (int y = 0; y <= newMaxY; y++) {
                    newPaper[y, x] |= paper[maxY - y ,x];
                }
            }

            paper = newPaper;

        } else {
            var newMaxY = maxY;
            var newMaxX = maxX / 2 - 1;
            var newPaper = new bool[newMaxY + 1, newMaxX + 1];

            for (int x = 0; x <= newMaxX; x++) {
                for (int y = 0; y <= newMaxY; y++) {
                    newPaper[y, x] = paper[y,x];
                }
            }
            for (int x = 0; x <= newMaxX; x++) {
                for (int y = 0; y <= newMaxY; y++) {
                    newPaper[y, x] |= paper[y ,maxX - x];
                }
            }

            paper = newPaper;
            maxX = newMaxX;
            maxY = newMaxY;
        }

        int count = 0;
        for (var y = 0; y < paper.GetLength(0); y++) {
            for (var x = 0; x < paper.GetLength(1); x++) {
                count += paper[y,x] ? 1 : 0;
            }
        }

        Console.Out.WriteLine($"Dots: {count}");
    }

    static void Part2(IEnumerable<(int x, int y)> coords, IEnumerable<(char direction, int)> instructions) {
        int maxX = coords.Max(coord => coord.x);
        int maxY = coords.Max(coord => coord.y);
        var paper = new bool[maxY + 1, maxX + 1];
        foreach(var coord in coords) {
            paper[coord.y, coord.x] = true;
        }

        foreach(var instruction in instructions) {

            int newMaxX;
            int newMaxY;
            bool[,] newPaper;
            if (instruction.direction == 'y') {
                newMaxX = maxX;
                newMaxY = maxY / 2 - 1;
                newPaper = new bool[newMaxY + 1, newMaxX + 1];

                for (int x = 0; x <= newMaxX; x++) {
                    for (int y = 0; y <= newMaxY; y++) {
                        newPaper[y, x] = paper[y,x] || paper[maxY - y, x];
                    }
                }
            } else {
                newMaxY = maxY;
                newMaxX = maxX / 2 - 1;
                newPaper = new bool[newMaxY + 1, newMaxX + 1];

                for (int x = 0; x <= newMaxX; x++) {
                    for (int y = 0; y <= newMaxY; y++) {
                        newPaper[y, x] = paper[y,x] || paper[y ,maxX - x];
                    }
                }
            }
            paper = newPaper;
            maxX = newMaxX;
            maxY = newMaxY;
        }

        for (var y = 0; y < paper.GetLength(0); y++) {
            string line = "";
            for (var x = 0; x < paper.GetLength(1); x++) {
                line += paper[y,x] ? "#" : " ";
            }
            Console.Out.WriteLine(line);
        }
    }
}
