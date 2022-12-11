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

        var array = lines.Select(l => l.Select(c => c - '0').ToArray()).ToArray();

        var twoDArray = new int[array.Length, array[0].Length];
        for (var i = 0; i < array.Length; i++)
        {
            for (var j = 0; j < array[0].Length; j++)
            {
                twoDArray[i,j] = array[i][j];
            }
        }

        Console.Out.WriteLine($"Parse time: {sw.ElapsedMilliseconds}");
        //Part1(twoDArray);
        Part2(twoDArray, sw);

        Console.Out.WriteLine($"Total time {sw.ElapsedMilliseconds}");
    }

    static void Part1(int[,] array) {
       var dist = new int[array.GetLength(0), array.GetLength(1)];
        var prev = new (int, int)[array.GetLength(0), array.GetLength(1)];
        var target = (array.GetLength(0)-1, array.GetLength(1)-1);
       List<(int, int)> Q = new List<(int, int)>();

       for (int row = 0; row < array.GetLength(0); row++) {
           for (int col = 0; col < array.GetLength(1); col++) {
            dist[row, col] = int.MaxValue;
            prev[row, col] = (-1, -1);
            Q.Add((row, col));
           }
       }
       dist[0,0] = 0;
       while (Q.Any()) {
           var u = Q.MinBy(q => dist[q.Item1, q.Item2]);
           Q.Remove(u);

           if (u == target) {
               break;
           }

            var row = u.Item1;
            var col = u.Item2;

           var neighbors = new List<(int, int)>();
           if (row != 0) {
               neighbors.Add((row -1, col));
           }
           if (row != array.GetLength(0) - 1) {
               neighbors.Add((row +1, col));
           }
           if (col != 0) {
               neighbors.Add((row, col -1));
           }
           if (col != array.GetLength(1) - 1) {
               neighbors.Add((row, col +1));
           }
           var neighborsInQ = neighbors.Where(n => Q.Contains(n));
           foreach (var v in neighborsInQ) {
               var alt = dist[u.Item1, u.Item2] + array[v.Item1, v.Item2];
               if (alt < dist[v.Item1, v.Item2]) {
                   dist[v.Item1, v.Item2] = alt;
                   prev[v.Item1, v.Item2] = u;
               }
           }
       }

        var sum = 0;
        var u2 = target;
        while (u2 != (-1, -1)) {
            sum += array[u2.Item1,u2.Item2];
            u2 = prev[u2.Item1,u2.Item2];
        }

        sum -= array[0,0];
        Console.Out.WriteLine($"Sum: {sum}");
    }

    static void Part2(int[,] smallArray, Stopwatch sw) {

        var array = new int[smallArray.GetLength(0)*5, smallArray.GetLength(1)*5];

        for (int rowCopy = 0; rowCopy < 5; rowCopy++) {
            for (int colCopy = 0; colCopy < 5; colCopy++) {
                int higher = rowCopy + colCopy;
                
                for(int row = 0; row < smallArray.GetLength(0); row++) {
                    for (int col = 0; col < smallArray.GetLength(1); col++) {
                        var newVal = smallArray[row, col];
                        newVal += higher;
                        newVal = (newVal - 1) % 9 + 1;
                        array[rowCopy*smallArray.GetLength(0) + row, colCopy*smallArray.GetLength(1) + col] = newVal;
                    }
                }
            }
        }


        var dist = new int[array.GetLength(0), array.GetLength(1)];
        var prev = new (int, int)[array.GetLength(0), array.GetLength(1)];
        var target = (array.GetLength(0)-1, array.GetLength(1)-1);
        //LinkedList<(int, int)> Q = new LinkedList<(int, int)>();
        List<(int, int)> Q = new List<(int, int)>();

        for (int row = 0; row < array.GetLength(0); row++) {
           for (int col = 0; col < array.GetLength(1); col++) {
            dist[row, col] = int.MaxValue;
            prev[row, col] = (-1, -1);
            //Q.AddLast((row, col));
            Q.Add((row, col));
           }
        }
       dist[0,0] = 0;
       Console.Out.WriteLine($"Total Q: {array.GetLength(0) * array.GetLength(1)}");
       var counter = 0;
       while (Q.Count > 0) {
           if (counter++ % 1000 == 0) Console.Out.WriteLine(counter);
           var u = Q.MinBy(q => dist[q.Item1, q.Item2]);
           Q.Remove(u);

           if (u == target) {
               break;
           }

            var row = u.Item1;
            var col = u.Item2;

           var neighbors = new List<(int, int)>();
           if (row != 0) {
               neighbors.Add((row -1, col));
           }
           if (row != array.GetLength(0) - 1) {
               neighbors.Add((row +1, col));
           }
           if (col != 0) {
               neighbors.Add((row, col -1));
           }
           if (col != array.GetLength(1) - 1) {
               neighbors.Add((row, col +1));
           }
           var neighborsInQ = neighbors.Where(n => Q.Contains(n));
           foreach (var v in neighborsInQ) {
               var alt = dist[u.Item1, u.Item2] + array[v.Item1, v.Item2];
               if (alt < dist[v.Item1, v.Item2]) {
                   dist[v.Item1, v.Item2] = alt;
                   prev[v.Item1, v.Item2] = u;
               }
           }
       }

        var sum = 0;
        var u2 = target;
        while (u2 != (-1, -1)) {
            sum += array[u2.Item1,u2.Item2];
            u2 = prev[u2.Item1,u2.Item2];
        }

        sum -= array[0,0];
        Console.Out.WriteLine($"Sum: {sum}");
  
    }

   
}
