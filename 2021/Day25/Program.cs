//using MoreLinq;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Numerics;

public class Day18 {

    public static bool Debug = false;

   
    public static void Main() {
        var sw = Stopwatch.StartNew();
        

        string[] lines = File.ReadAllLines("input.txt"); 
        //string[] lines = File.ReadAllLines("sample.txt"); 
        Console.Out.WriteLine($"Parse time: {sw.ElapsedMilliseconds}");
        Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

        var input = lines.Select(l => l.Select(c => c).ToArray()).ToArray();

        char[,] board = new char[input.Length, input[0].Length];
        for (var r = 0; r < board.GetLength(0); r++){
            for (var c = 0; c < board.GetLength(1); c++){
                board[r,c] = input[r][c];
            }
        }

        Part1(board);
        //Part2(instructions);

        Console.Out.WriteLine($"Total time {sw.ElapsedMilliseconds}");
    }

    static Dictionary<(int step, long z), List<byte[]>> memo = new();

    static void Part1(char[,] seafloor) {
        int width = seafloor.GetLength(1);
        int height = seafloor.GetLength(0);
        var moved = true;
        int steps = 0;
        while (moved) {
            steps++;
            Console.Out.WriteLine($"Step {steps}");
            moved = false;
            var next = new char[height, width];
            for (var r = 0; r < height; r++) {
                for (var c = 0; c < width; c++) {
                    next[r,c] = seafloor[r,c];
                }
            }
            for (var r = 0; r < height; r++) {
                for (var c = 0; c < width; c++) {
                    
                    if (seafloor[r,c] == '>') {

                        var eastC = c + 1;
                        if (eastC == width) {
                            eastC = 0;
                        }
                        if (seafloor[r, eastC] == '.') {
                            next[r,c] = '.';
                            next[r, eastC] = '>';
                            moved = true;
                        }
                    }
                }
            }
            seafloor = next;
            next = new char[height, width];
            for (var r = 0; r < height; r++) {
                for (var c = 0; c < width; c++) {
                    next[r,c] = seafloor[r,c];
                }
            }
             for (var r = 0; r < height; r++) {
                for (var c = 0; c < width; c++) {
                    
                    if (seafloor[r,c] == 'v') {

                        var southR = r + 1;
                        if (southR == height) {
                            southR = 0;
                        }
                        if (seafloor[southR, c] == '.') {
                            next[r,c] = '.';
                            next[southR, c] = 'v';
                            moved = true;
                        }
                    }
                }
            }
            seafloor = next;
        }
        Console.Out.WriteLine($"Steps: {steps}");
    }

   
    static void Part2(List<(int, int, int)> constants) {
       
      
    }


}
