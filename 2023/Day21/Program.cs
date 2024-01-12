#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Principal;
using MoreLinq;


bool sample = false;

string NORMAL      = Console.IsOutputRedirected ? "" : "\x1b[39m";
string RED         = Console.IsOutputRedirected ? "" : "\x1b[91m";
string GREEN       = Console.IsOutputRedirected ? "" : "\x1b[92m";

string NORMAL_BG      = Console.IsOutputRedirected ? "" : "\x1b[49m";
string RED_BG        = Console.IsOutputRedirected ? "" : "\x1b[41m";
string GREEN_BG      = Console.IsOutputRedirected ? "" : "\x1b[42m";
string YELLOW_BG      = Console.IsOutputRedirected ? "" : "\x1b[43m";
string X1_BG      = Console.IsOutputRedirected ? "" : "\x1b[44m";
string X2_BG      = Console.IsOutputRedirected ? "" : "\x1b[45m";

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();



//Part1(lines);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(string[] lines)
{

    var map = new bool[lines.Length + 2, lines[0].Length + 2];
    Point start = null;
    bool b = false;
    for (var ii = 0 ; ii < map.GetLength(0); ii++) {
        for (var jj = 0; jj < map.GetLength(1); jj++) {
            
            if (ii == 0 || ii == map.GetLength(0) - 1 || jj == 0 || jj == map.GetLength(1) - 1) {
                b = false;
            }
            else {
                var c = lines[ii-1][jj-1];
                if (c == 'S') {
                    start = new Point(ii,jj);
                } else {
                    b = c == '.';
                }
            }
            map[ii,jj] = b;
        }
    }
    HashSet<Point> currentList = new();
    currentList.Add(start);

    for (int ii = 0; ii < (sample ? 6 : 64); ii++) {
        HashSet<Point> nextList = new();

        foreach (var p in currentList) {
            Point n = new Point(p.X - 1, p.Y);
            if (map[n.X, n.Y]) {
                nextList.Add(n);
            }

            Point s = new Point(p.X + 1, p.Y);
            if (map[s.X, s.Y]) {
                nextList.Add(s);
            }

            Point e = new Point(p.X, p.Y + 1);
            if (map[e.X, e.Y]) {
                nextList.Add(e);
            }

            Point w = new Point(p.X, p.Y - 1);
            if (map[w.X, w.Y]) {
                nextList.Add(w);
            }
        }

        currentList = nextList;
    }
    
    Console.Out.WriteLine($"Len is {currentList.Count}");


}

void Part2(string[] lines)
{
    var map = new bool[lines.Length * 3, lines[0].Length * 3];
    Point start = new Point(map.GetLength(0) / 2, map.GetLength(1) / 2);
    bool b = false;
    for (var ii = 0 ; ii < map.GetLength(0); ii++) {
        for (var jj = 0; jj < map.GetLength(1); jj++) {
            var c = lines[ii % lines.Length][jj % lines[0].Length];
            if (c == 'S') {
                b = true;
            } else {
                b = c == '.';
            }
            map[ii,jj] = b;
        }
    }

    
    HashSet<Point> currentList = new();
    currentList.Add(start);

    for (int ii = 0; ii < (sample ? /*36*/ 10  : 201); ii++) {
        HashSet<Point> nextList = new();

        foreach (var p in currentList) {

            Point nextP;
            nextP = new Point(p.X - 1, p.Y); // N
            if (nextP.X >= 0 && nextP.X < map.GetLength(0) && nextP.Y >=0 && nextP.Y < map.GetLength(1) && map[nextP.X, nextP.Y]) {
                nextList.Add(nextP);
            }

            nextP = new Point(p.X + 1, p.Y); // S
            if (nextP.X >= 0 && nextP.X < map.GetLength(0) && nextP.Y >=0 && nextP.Y < map.GetLength(1) && map[nextP.X, nextP.Y]) {
                nextList.Add(nextP);
            }

            nextP = new Point(p.X, p.Y + 1); // E
            if (nextP.X >= 0 && nextP.X < map.GetLength(0) && nextP.Y >=0 && nextP.Y < map.GetLength(1) && map[nextP.X, nextP.Y]) {
                nextList.Add(nextP);
            }

            nextP = new Point(p.X, p.Y - 1); // W
            if (nextP.X >= 0 && nextP.X < map.GetLength(0) && nextP.Y >=0 && nextP.Y < map.GetLength(1) && map[nextP.X, nextP.Y]) {
                nextList.Add(nextP);
            }
        }

        currentList = nextList;
    }
    

    HashSet<Point> pointsInDiamondA = new HashSet<Point>();
    var boxDim = map.GetLength(0) / 3;

    var diamondRadius = boxDim / 2;
    for (int ii = 0; ii < boxDim; ii++) {
        for (int jj = 0; jj < boxDim; jj++) {
            if (Math.Abs(ii - diamondRadius) + Math.Abs(jj - diamondRadius) <= diamondRadius) {
                pointsInDiamondA.Add(new Point(start.X + ii - diamondRadius, start.Y + jj - diamondRadius));
            }
        }
    }

    HashSet<Point> pointsInDiamondB = new();
    for (int ii = 0; ii < boxDim-1; ii++) {
        for (int jj = 0; jj < boxDim-1; jj++) {
            if (Math.Abs(ii - (diamondRadius-0.5)) + Math.Abs(jj - (diamondRadius-0.5)) <= diamondRadius) {
                pointsInDiamondB.Add(new Point(start.X + ii - (boxDim -1), start.Y + jj + 1));
            }
        }
    }

    HashSet<Point> pointsInDiamondAp = new(pointsInDiamondA.Select(p => new Point(p.X - boxDim, p.Y)));
    HashSet<Point> pointsInDiamondBp = new(pointsInDiamondB.Select(p => new Point(p.X, p.Y -boxDim)));

    for (var ii = 0 ; ii < map.GetLength(0); ii++) {
        
        for (var jj = 0; jj < map.GetLength(1); jj++) { 

            
            var border = 
                ii % boxDim == 0 || jj % boxDim == 0 
             || ii % boxDim == boxDim - 1 || jj % boxDim == boxDim - 1;
            if (border) {
                Console.Write(GREEN_BG);
            }
            var p =new Point(ii, jj);
            if (pointsInDiamondA.Contains(p)) {
                Console.Write(RED_BG);
            } else if (pointsInDiamondB.Contains(p)) {
                Console.Write(YELLOW_BG);
            }
            else if (pointsInDiamondAp.Contains(p)) {
                Console.Write(X1_BG);
            }
            else if (pointsInDiamondBp.Contains(p)) {
                Console.Write(X2_BG);
            }
            var isSet = currentList.Contains(new Point(ii, jj));
            Console.Write(isSet ? 'O' : map[ii,jj] ? '.' : '#');
            Console.Write(NORMAL_BG);
        }
        Console.WriteLine();

    }

   var countInDiamondA = pointsInDiamondA.Sum(p => currentList.Contains(p) ? 1 : 0);
   var countInDiamondB = pointsInDiamondB.Sum(p => currentList.Contains(p) ? 1 : 0);
   var countInDiamondAp = pointsInDiamondAp.Sum(p => currentList.Contains(p) ? 1 : 0);
   var countInDiamondBp = pointsInDiamondBp.Sum(p => currentList.Contains(p) ? 1 : 0);

   Console.WriteLine($"count in A: {countInDiamondA}\ncount in B: {countInDiamondB}\ncount in A': {countInDiamondAp}\ncount in B': {countInDiamondBp}");
 
    var steps = 26501365;
    long n = (steps - (boxDim / 2)) / boxDim;
    n = (n * 2 + 1);
Console.WriteLine($"n: {n}");
   long numDiamonds = n * n;
   Console.WriteLine($"numDiamonds: {numDiamonds}");
   long numAs = ((n + 1) / 2) * ((n + 1) / 2);
   long numAps = ((n - 1) / 2) * ((n - 1) / 2);
   long numBs = numDiamonds / 4;
   long numBps = numDiamonds / 4;

    Console.WriteLine($"num As: {numAs}\nnum Bs: {numBs}\nnum A's: {numAps}\nnum B's: {numBps}\n");

   var totalCount = numAs * countInDiamondA + numBs * countInDiamondB + numAps * countInDiamondAp + numBps * countInDiamondBp;

   Console.WriteLine($"total {totalCount}");

}

record Point(int X, int Y) {}


//635_900_299_361_316