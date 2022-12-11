//using MoreLinq;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Numerics;

public class Day15 {

    public static void Main() {
        string[] lines = File.ReadAllLines("input.txt"); 
        //string[] lines = File.ReadAllLines("sample.txt"); 
        Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");
        var sw = Stopwatch.StartNew();

       var parts = lines[0]
        .Split(": ")[1]
        .Split(", ")
        .Select(s => s.Substring(2).Split("..").Select(n => int.Parse(n)).ToArray()).ToArray();
       
       int minX = parts[0][0];
       int maxX = parts[0][1];
       int minY = parts[1][0];
       int maxY = parts[1][1];

       
        Console.Out.WriteLine($"Parse time: {sw.ElapsedMilliseconds}");

        //Part1(minX, maxX, minY, maxY);
        Part2(minX, maxX, minY, maxY);

        Console.Out.WriteLine($"Total time {sw.ElapsedMilliseconds}");
    }

    static void Part1(int minX, int maxX, int minY, int maxY) {
      
       int? minDx = null;
       int maxDx = maxX;
       int? maxSteps = null;
       for (int dxii = 1; dxii <= maxX; dxii++) {
           int finalX = SumUpTo(dxii);
           if (minDx == null && finalX >= minX) {
               minDx = dxii;
           }
           if (finalX <= maxX) {
               maxSteps = dxii;
           }
       }

        (int x, int y) pos = (0,0);
        int dx, dy;
        int startDx = dx = minDx.Value;
        int startDy = dy = 0-minY -1;
        int overallMaxY = pos.y;
        while (pos.y + dy >= minY){
            NextPos(ref pos, ref dx, ref dy);
            overallMaxY = Math.Max(overallMaxY, pos.y);
        }
        
        if (InTarget(pos, minX, maxX, minY, maxY)) {
            Console.Out.WriteLine($"In target at {pos.x},{pos.y}  Initial velocity ({startDx},{startDy}) Max y: {overallMaxY}");
        } else {
            Console.Out.WriteLine($"MISS target at {pos.x},{pos.y} Initial velocity ({startDx},{startDy})");
        }
    }

    static void Part2(int minX, int maxX, int minY, int maxY) {
       int minDx = int.MaxValue;
       int maxDx = maxX;
       for (int dxii = 1; dxii <= maxX; dxii++) {
           int finalX = SumUpTo(dxii);
           if (finalX >= minX) {
               minDx = dxii;
               break;
           }
       }

       var maxDy = 0-minY -1;
       var minDy = minY;

        int goodShotCount = 0;
        for (int dx = minDx; dx <= maxDx; dx++) {
           for (int dy = minDy; dy <= maxDy; dy++) {
               if (GoodShot(dx, dy, minX, maxX, minY, maxY)) {
                   goodShotCount++;
               }
           }
        }
        Console.Out.WriteLine($"Num good shots: {goodShotCount}");
    }

    static bool GoodShot(int dx, int dy, int minX, int maxX, int minY, int maxY) {
        (int x, int y) pos = (0,0);
        while (pos.y + dy >= minY){
            NextPos(ref pos, ref dx, ref dy);
            if (InTarget(pos, minX, maxX, minY, maxY)) {
                return true;
            }
        }
        return false;

    }

    static void NextPos(ref (int x, int y) position, ref int dx, ref int dy) {
        position.x += dx;
        position.y += dy;
        dx = dx + dx switch {<0 => 1, 0 => 0, >0 => -1};
        dy--;
    }

    static bool InTarget((int x, int y) position, int minX, int maxX, int minY, int maxY) {
        return position.x >= minX && position.x <= maxX && position.y >= minY && position.y <= maxY;
    }

    static int SumUpTo(int x) {
        return x * ( x + 1) / 2;
    }

   

    
   
}
