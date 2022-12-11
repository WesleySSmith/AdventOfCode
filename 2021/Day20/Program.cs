//using MoreLinq;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Numerics;

public class Day18 {

    public static void Main() {
        string[] lines = File.ReadAllLines("input.txt"); 
        //string[] lines = File.ReadAllLines("sample.txt"); 
        Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");
        var sw = Stopwatch.StartNew();
     
        var algorithm = lines[0].Select(c => c == '#').ToArray();

        var image = lines.Skip(2).Select(l => l.Select(c => c == '#').ToArray()).ToArray();


        Console.Out.WriteLine($"Parse time: {sw.ElapsedMilliseconds}");

        //Part1(algorithm, image);
        Part2(algorithm, image);

        Console.Out.WriteLine($"Total time {sw.ElapsedMilliseconds}");
    }

    static void Part1(bool[] algorithm, bool[][] image) {
        
        bool[,] a = new bool[image.Length + 4, image[0].Length + 4];
        for (var r = 0; r < image.Length; r++) {
            for (var c = 0; c < image.Length; c++) {
                a[r+2, c+2] = image[r][c];
            }
        }

        SafeArray sa = new SafeArray(a, false);
        for (var ii = 0; ii < 2; ii++) {
            bool[,] dest = new bool[a.GetLength(0), a.GetLength(1)];
            for (var r = 0; r < a.GetLength(0); r++) {
                for (var c = 0; c < a.GetLength(1); c++) {
                    dest[r,c] = algorithm[sa.Get9(r, c)];
                }
            }
            sa = new SafeArray(dest, sa.Default ? algorithm[511] : algorithm[0]);
        }

        if (sa.Default) {
            throw new Exception("Infinitite lit");
        }

        a = sa.Array;
        int acc = 0;
        for (var r = 0; r < a.GetLength(0); r++) {
            for (var c = 0; c < a.GetLength(1); c++) {
                acc += a[r,c] ? 1 : 0;
            }
        }

        Console.Out.WriteLine($"Total lit: {acc}");
    }

    static void Part2(bool[] algorithm, bool[][] image) {
        bool[,] a = new bool[image.Length + 100, image[0].Length + 100];
        for (var r = 0; r < image.Length; r++) {
            for (var c = 0; c < image.Length; c++) {
                a[r+50, c+50] = image[r][c];
            }
        }

        SafeArray sa = new SafeArray(a, false);
        for (var ii = 0; ii < 50; ii++) {
            bool[,] dest = new bool[a.GetLength(0), a.GetLength(1)];
            for (var r = 0; r < a.GetLength(0); r++) {
                for (var c = 0; c < a.GetLength(1); c++) {
                    dest[r,c] = algorithm[sa.Get9(r, c)];
                }
            }
            sa = new SafeArray(dest, sa.Default ? algorithm[511] : algorithm[0]);
        }

        if (sa.Default) {
            throw new Exception("Infinitite lit");
        }

        a = sa.Array;
        int acc = 0;
        for (var r = 0; r < a.GetLength(0); r++) {
            for (var c = 0; c < a.GetLength(1); c++) {
                acc += a[r,c] ? 1 : 0;
            }
        }

        Console.Out.WriteLine($"Total lit: {acc}");
    }

    public class SafeArray {
        private int Height;
        private int Width;
        public bool[,] Array;

        public bool Default;

        public SafeArray(bool[,] array, bool @default)
        {
            this.Height = array.GetLength(0);
            this.Width = array.GetLength(1);
            this.Array = array;
            this.Default = @default;
        }

        public short Get(int r, int c){
            if (r >= 0 && r <= Height -1 && c >= 0 && c <= Width -1) {
                return this.Array[r, c] ? (short)1 : (short)0;
            } return Default ? (short)1 : (short)0;
        }

        public short Get9(int r, int c) {
            short n = 0;
            n <<= 1; n |= Get(r-1, c-1);
            n <<= 1; n |= Get(r-1, c);
            n <<= 1; n |= Get(r-1, c+1);
            n <<= 1; n |= Get(r, c-1);
            n <<= 1; n |= Get(r, c);
            n <<= 1; n |= Get(r, c+1);
            n <<= 1; n |= Get(r+1, c-1);
            n <<= 1; n |= Get(r+1, c);
            n <<= 1; n |= Get(r+1, c+1);
            return n;
        }
    }


    
   
}
