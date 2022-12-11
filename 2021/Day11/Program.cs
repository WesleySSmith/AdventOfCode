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

        var energies = lines.Select(l => l.Select(c => c - '0').ToArray()).ToArray();
        var height = energies.Count();
        var width = energies.First().Count();

        var energyArray = new int[height, width];
        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                energyArray[i,j] = energies[i][j];
            }
        }
        Console.Out.WriteLine($"Parse time: {sw.ElapsedMilliseconds}");
        //Part1(energyArray);
        Part2(energyArray);

        Console.Out.WriteLine($"Total time {sw.ElapsedMilliseconds}");
    }

    static void Part1(int[,] energyArray) {
        SafeArray a = new SafeArray(energyArray);
        int flashes = 0;
        for (int ii = 0; ii < 100; ii++) {
            int flashesThisStep = Step(a);
            flashes += flashesThisStep;
            Console.Out.WriteLine($"Flashes step {ii}: {flashesThisStep}");
        }
        Console.Out.WriteLine($"Flashes: {flashes}");
    }

    static void Part2(int[,] energyArray) {
        SafeArray a = new SafeArray(energyArray);
        for (int ii = 0; ;ii++) {
            int flashesThisStep = Step(a);
            Console.Out.WriteLine($"Flashes step {ii + 1}: {flashesThisStep}");
            if (flashesThisStep == 100) {
                break;
            }
        }
        //Console.Out.WriteLine($"Flashes: {flashes}");
    }

    
    public class SafeArray {
        private int Height;
        private int Width;
        public int[,] Array;

        public SafeArray(int[,] array)
        {
            this.Height = array.GetLength(0);
            this.Width = array.GetLength(1);
            this.Array = array;
        }

        public void Inc(int r, int c){
            if (r >= 0 && r <= Height -1 && c >= 0 && c <= Width -1) {
                this.Array[r, c]++;
            }
        }

        public void IncAdjacent(int r, int c) {
            Inc(r-1, c-1);
            Inc(r-1, c);
            Inc(r-1, c+1);
            Inc(r, c-1);
            Inc(r, c+1);
            Inc(r+1, c-1);
            Inc(r+1, c);
            Inc(r+1, c+1);
        }

        public void IncAll() {
            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    Array[i, j]++;
                }
            }
        }
    }

    public static int Step(SafeArray state) {
        state.IncAll();
        int flashes = 0;
        while(true) {
            int flashesThisTime = 0;
            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    if (state.Array[i,j] > 9) {
                        flashesThisTime++;
                        state.Array[i,j] = int.MinValue;
                        state.IncAdjacent(i,j);
                    }
                }
            }
            if (flashesThisTime == 0) {
                break;
            }
            flashes += flashesThisTime;
        }

        for (var i = 0; i < 10; i++)
        {
            for (var j = 0; j < 10; j++)
            {
                if (state.Array[i,j] < 0) {
                    state.Array[i,j] = 0;
                }
            }
        }
        return flashes;
    }

    

    
}

















