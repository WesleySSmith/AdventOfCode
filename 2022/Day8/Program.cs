#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Diagnostics;
using MoreLinq;

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

//var trees = lines.Select(l => l.Select(c => new Tree {Height = (byte)(c - '0')}).ToArray()).ToArray();
var trees = lines.Select(l => l.Select(c => (byte)(c - '0')).ToArray()).ToArray();

//Part1(trees);
Part2(trees);

Console.Out.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");

static void Compute(Tree[][] trees) {
    
    Tree tree;
    short max;
    for (int row = 0; row < trees.Length; row++) {
        //Left
        max = -1;
        for (int col = 0; col < trees[0].Length; col++) {
            tree = trees[row][col];
            if (tree.Height > max) {
                tree.L = true;
                max = tree.Height;
            }
        }

        //Right
        max = -1;
        for (int col = trees[0].Length -1; col >= 0 ; col--) {
            tree = trees[row][col];
            if (tree.Height > max) {
                tree.R = true;
                max = tree.Height;
            }
        }
    }

    for (int col = 0; col < trees[0].Length; col++) {
        //Top
        max = -1;
        for (int row = 0; row < trees.Length; row++) {
            tree = trees[row][col];
            if (tree.Height > max) {
                tree.T = true;
                max = tree.Height;
            }
        }

        //Bottom
        max = -1;
        for (int row = trees.Length -1; row >= 0 ; row--) {
            tree = trees[row][col];
            if (tree.Height > max) {
                tree.B = true;
                max = tree.Height;
            }
        }
    }
}


static void Part1(Tree[][] trees) {
   
   Compute(trees);

   var count = 0;
    for (int row = 0; row < trees.Length; row++) {
        for (int col = 0; col < trees[0].Length; col++) {
            var tree = trees[row][col];
            if (tree.L || tree.R || tree.T || tree.B) {
                count++;
            }
        }
    }
   
    Console.Out.WriteLine($"Part 1 Count: {count}");
}

static void Part2(byte[][] trees) {

   var best = 0;

    for (int row = 0; row < trees.Length; row++) {
        for (int col = 0; col < trees[0].Length; col++) {
            var tree = trees[row][col];
            
            // Move right
            var scoreR = 0;
            var c2 = col + 1;
            while (c2 < trees[0].Length) {
                scoreR++;
                var nextTree = trees[row][c2];
                if (nextTree >= tree) {
                    break;
                }
                c2++;
            }

            // Move left
            var scoreL = 0;
            c2 = col - 1;
            while (c2 >= 0) {
                scoreL++;
                var nextTree = trees[row][c2];
                if (nextTree >= tree) {
                    break;
                }
                c2--;
            }

            // Move down
            var scoreD = 0;
            var r2 = row + 1;
            while (r2 < trees.Length) {
                scoreD++;
                var nextTree = trees[r2][col];
                if (nextTree >= tree) {
                    break;
                }
                r2++;
            }

            // Move up
            var scoreU = 0;
            r2 = row - 1;
            while (r2 >= 0) {
                scoreU++;
                var nextTree = trees[r2][col];
                if (nextTree >= tree) {
                    break;
                }
                r2--;
            }
            
            
            var score = scoreL * scoreR * scoreD * scoreU;
            if (score > best) {
                best = score;
            }
        }
    }
   
    Console.Out.WriteLine($"Part 2 Score: {best}");
}



public record Tree {
    public byte Height {get; init;}
    public bool L {get; set;}
    public bool R {get; set;}
    public bool T {get; set;}
    public bool B {get; set;}
}

