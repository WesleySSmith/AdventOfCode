//using MoreLinq;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Numerics;

public class Day8 {

    public static string[] SegmentsAsStrings;
    static List<List<int>> allMappings;

    public static void Main() {

        string[] lines = File.ReadAllLines("input.txt"); 
        //string[] lines = File.ReadAllLines("sample.txt"); 
        Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");
        var sw = Stopwatch.StartNew();

        var entries = lines
            .Select(l => l.Split(" | "))
            .Select(p => (p.ElementAt(0), p.ElementAt(1)))
            .Select(t => (patterns: t.Item1.Split(' '), outputs: t.Item2.Split(' ')))
            .ToList();

        string[] segments = new string[] {
            /* 0 */ "abcefg",
            /* 1 */ "cf",
            /* 2 */ "acdeg",
            /* 3 */ "acdfg",
            /* 4 */ "bcdf",
            /* 5 */ "abdfg",
            /* 6 */ "abdefg",
            /* 7 */ "acf",
            /* 8 */ "abcdefg",
            /* 9 */ "abcdfg",
        };

        var segmentsAsArrays = segments.Select(s => s.Select(FromChar).ToArray());

        SegmentsAsStrings = segmentsAsArrays.Select(ArrayAsString).ToArray();

        allMappings = AllMappings();

       

        //Part1(entries);
        Part2(entries);

        Console.Out.WriteLine($"Total time {sw.ElapsedMilliseconds}");

    }

    static string ArrayAsString(IEnumerable<int> a) {
        return a.Aggregate("", (acc, n) => acc + n.ToString());
    }

    static int ArrayAsInt(int[] a) {
        return a.Aggregate(0, (acc, n) => acc * 10 + n);
    }

    static void Part1(IList<(string[] patterns, string[] outputs)> entries) {
        var count = entries
            .Select(entry => entry.outputs.Count(o => o.Length switch { 
            2 or 4 or 3 or 7 => true,
            _ => false}))
            .Sum();
        Console.Out.WriteLine($"Count: {count}");
        
    }

    static void Part2(IList<(string[] patterns, string[] outputs)> entries) {
        var total = entries.Select(ValueForEntry).Sum();
        Console.Out.WriteLine($"Total: {total}");
    }



    static int FromChar(char c) {
        return c - 'a';
    }
    int ToChar(int i) {
        return 'a' + i;
    }

    static int[] FromString(string s) {
        return s.OrderBy(c => c).Select(c => c - 'a').ToArray();
    }

    static int ValueForEntry((string[] patterns, string[] outputs) entry) {
        
        foreach (var map in allMappings) {
            foreach (var pattern in entry.patterns) {
                var p = FromString(pattern);
                var possibleAnswer = Map(p, map);
                if (!ValidPattern(possibleAnswer)) {
                    goto next_map;
                }
            }
            // They all pass, find the output
            var digits = entry.outputs.Select(
                s => Array.IndexOf(SegmentsAsStrings, ArrayAsString(Map(FromString(s), map).OrderBy(i => i))))
                .ToArray();
            return ArrayAsInt(digits);
        next_map:;
        }
        throw new Exception("Shouldn't get here");
    }


    static bool ValidPattern(int[] possibleDigit) {
        return SegmentsAsStrings.Contains(ArrayAsString(possibleDigit.OrderBy(i => i)));
    }

    static List<List<int>> AllMappings() {
          return AllMappings(new [] {0,1,2,3,4,5,6});     
    }

    static List<List<int>> AllMappings(IEnumerable<int> choices) {
        if (choices.Count() == 1) {
            return new List<List<int>> {new List<int>(choices)};
        }
        var l = new List<List<int>>();
        foreach (var choice in choices) {
            var remain = AllMappings(choices.Where(c => c != choice));
            remain.ForEach(r => r.Add(choice));
            l.AddRange(remain);
        }
        return l;
    }

    static int[] Map(int[] input, List<int> mapping) {
        var result = new int[input.Length];
        for (int ii = 0; ii < result.Length; ii++) {
            result[ii] = mapping[input[ii]];
        }
        return result;
        //return input.Select(i => mapping[i]).ToArray();
    }

}

















