//using MoreLinq;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Numerics;

public class Day18 {

    public static bool Debug = false;

    public static List<(int, int, int)> constants = new List<(int, int, int)> {
        (1, 12, 4),
        (1, 11, 11),
        (1, 13, 5),
        (1, 11, 11),
        (1, 14, 14),
        (26, -10, 7),
        (1, 11, 11),
        (26, -9, 4),
        (26, -3, 6),
        (1, 13, 5),
        (26, -5, 9),
        (26, -10, 12),
        (26, -4, 14),
        (26, -5, 14),
    };

    public static void Main() {
        var sw = Stopwatch.StartNew();
        Console.Out.WriteLine($"Parse time: {sw.ElapsedMilliseconds}");

   

        Part1(constants);
        //Part2(instructions);

        Console.Out.WriteLine($"Total time {sw.ElapsedMilliseconds}");
    }

    static Dictionary<(int step, long z), List<byte[]>> memo = new();

    static void Part1(List<(int, int, int)> constants) {
        
        List<byte[]> digits = AllValidSuffix(new byte[14], 0, 0);

        if (digits == null) {
            Console.Out.WriteLine("No valid found");
        } else {
            var valid = digits.Select(v => v.Aggregate(0L, (acc, d) => {
                acc *= 10;
                acc += d;
                return acc;
            })).OrderBy(d => d);

            Console.Out.WriteLine($"Found {valid.Count()} valid.  Min: {valid.First()}, Max: {valid.Last()}");
        }

    }

    static int counter = 0;
    static int memoHits = 0;
    static List<byte[]> AllValidSuffix(byte[] digits, int step, long currentZ) {

        if (counter++ % 10000000 == 0) {
            Console.Out.WriteLine($"AllValid: counter: {counter}, digits: {string.Join("", digits.Select(d => d.ToString()))}, step: {step}, currentZ: {currentZ}, memoHits: {memoHits}, memoLength: {memo.Count()}");
        }

        bool found = memo.TryGetValue((step, currentZ), out var m);
        if (found) {
            memoHits++;
            return m;
        }
       
        if (step == 14) {
            if (currentZ == 0) {
                return new List<byte[]> {};
            }
            return null;
        }
        List<byte[]> result = null;
        for (byte w = 1; w <= 9; w++) {
            var nextZ = NextZ(constants[step], w, currentZ);
            var newDigits = new byte[14];
            Array.Copy(digits, newDigits, 14);
            newDigits[step] = w;
            var av = AllValidSuffix(newDigits, step+1, nextZ);
            if (av != null) {
                if (av.Count == 0) {
                    return new List<byte[]> {new byte[] {w}};
                } else {
                    result ??= new();
                    foreach (var v in av) {
                        var n = new byte[v.Length+1];
                        n[0] = w;
                        Array.Copy(v, 0, n, 1, v.Length);
                        result.Add(n);
                    }
                }
            }
        }
        memo[(step, currentZ)] = result;
        return result;
    }

    

    static long NextZ((int a, int b, int c) constants, int w, long z) {
        var x = z % 26 + constants.b;
        z /= constants.a;
        x = x == w ? 0 : 1;
        var y = 25 * x + 1;
        z *= y;
        y = (w + constants.c) * x;
        z += y;
        return z;
    }

   
    static void Part2(List<(int, int, int)> constants) {
       
      
    }


}
