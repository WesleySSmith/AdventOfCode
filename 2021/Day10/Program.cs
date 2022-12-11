//using MoreLinq;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Numerics;

public class Day8 {

    public static void Main() {

        string[] lines = File.ReadAllLines("input.txt"); 
        //string[] lines = File.ReadAllLines("sample.txt"); 
        Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");
        var sw = Stopwatch.StartNew();

        var codeLines = lines.Select(l => l.Select(c => c).ToList()).ToList();

        Console.Out.WriteLine($"Parse time: {sw.ElapsedMilliseconds}");
        //Part1(codeLines);
        Part2(codeLines);

        Console.Out.WriteLine($"Total time {sw.ElapsedMilliseconds}");
    }

    static void Part1(List<List<char>> lines) {
        var score = lines.Select(FindError).Select(Score).Sum();
        Console.Out.WriteLine($"Score: {score}");
    }

    static void Part2(List<List<char>> lines) {
        var incomplete = lines.Where(l => FindError(l) == null);
        
        var scores = incomplete
        .Select(FindMissing)
        .Select(m => m.Select(c => c).Aggregate(0L, (acc, v) => acc * 5 + v switch {
            ')' => 1,
            ']' => 2,
            '}' => 3,
            '>' => 4,
            _ => throw new Exception($"@@@: {v}")
            }))
        .OrderBy(s => s)
        .ToList();

        if (scores.Count() % 2 == 0) {
            throw new Exception($"###: {scores.Count()}");
        }

        var middle = scores[scores.Count() / 2];
        Console.Out.WriteLine($"Middle score: {middle}");
    }

    static char? FindError(List<char> line) {
        var s = new Stack<char>();
        foreach(var c in line) {
            if (c is '{' or '(' or '[' or '<') {
                s.Push(c);
            } else {
                var pop = s.Pop();
                var expected = pop switch {
                    '{' => '}',
                    '(' => ')',
                    '[' => ']',
                    '<' => '>',
                    _ => throw new Exception($"!!!: {pop}")
                };
                if (c != expected)
                {
                    return c;
                }
            }
        }
        return null;
    }

    static List<char> FindMissing(List<char> line) {
        var s = new Stack<char>();
        foreach(var c in line) {
            if (c is '{' or '(' or '[' or '<') {
                s.Push(c);
            } else {
                s.Pop();
            }
        }
        return s.Select(c => c switch {
                    '{' => '}',
                    '(' => ')',
                    '[' => ']',
                    '<' => '>',
                    _ => throw new Exception($"%%%: {c}")
                    }).ToList();
    }

    static int Score(char? invalidChar) => 
        invalidChar switch {
            ')' => 3,
            ']' => 57,
            '}' => 1197,
            '>' => 25137,
            null => 0,
            _ => throw new Exception($"???: {invalidChar}")
        };
}

















