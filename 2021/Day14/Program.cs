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

        var template = lines.First();
        var rules = lines.Skip(2).Select(l => l.Split(" -> ")).Select(p => (p[0], p[1]));
        
        Console.Out.WriteLine($"Parse time: {sw.ElapsedMilliseconds}");
        //Part1(template, rules);
        Part2(template, rules, sw);

        Console.Out.WriteLine($"Total time {sw.ElapsedMilliseconds}");
    }

    static void Part1(string template, IEnumerable<(string pair, string insertion)> ruleList) {
       
        var rules = ruleList.ToDictionary(r => r.pair, r => r.insertion);
        
        StringBuilder nextTemplate = new StringBuilder();
        for (int ii = 0; ii < 40; ii++) {

           for (var pairStart = 0; pairStart <= template.Length - 2; pairStart++) {
               var pair = template[pairStart..(pairStart+2)];
               nextTemplate.Append(template[pairStart]);
               if (rules.TryGetValue(pair, out var insertion)) {
                   nextTemplate.Append(insertion);
               }
           }
           nextTemplate.Append(template[^1]);

            template = nextTemplate.ToString();
            //Console.Out.WriteLine($"After {ii + 1}: {nextTemplate.ToString()}");
            nextTemplate.Clear();
        }
        var groupCounts = template.GroupBy(g => g).Select(g => g.Count()).OrderByDescending(g => g);
        var mostCommon = groupCounts.First();
        var leastCommon = groupCounts.Last();
        Console.Out.WriteLine($"Score: {mostCommon - leastCommon}");
    }

    static void Part2(string template, IEnumerable<(string pair, string insertion)> ruleList, Stopwatch sw) {
        Console.Out.WriteLine($"Part 2 {sw.ElapsedMilliseconds}");
        var rules = ruleList.ToDictionary(r => r.pair, r => r.insertion);
        
        var letterCounts = new long[26];
        var nextPairCountsX = ruleList.ToDictionary(r => r.pair, r => 0L);
        var pairCounts = new Dictionary<string, long>(nextPairCountsX);
        for (int pairStart = 0; pairStart < template.Length -1; pairStart++) {
            var pair = template[pairStart..(pairStart+2)];
            pairCounts[pair]++;
        }

        foreach (var c in template) {
            letterCounts[c - 'A']++;
        }

        Console.Out.WriteLine($"Before loop {sw.ElapsedMilliseconds}");

        for (int ii = 0; ii < 40; ii++) {

            var nextPairCounts = new Dictionary<string, long>(nextPairCountsX);

            foreach(var pair in pairCounts.Keys) {
                var currentCount = pairCounts[pair];
                var insertion = rules[pair];
                letterCounts[insertion[0] - 'A'] += currentCount;
                var newPair1 = pair[0] + insertion;
                var newPair2 = insertion + pair[1];
                nextPairCounts[newPair1]+= currentCount;
                nextPairCounts[newPair2]+= currentCount;
            }

            pairCounts = nextPairCounts;
            //Console.Out.WriteLine($"{ii}");
          
        }
       
        Console.Out.WriteLine($"Score: {letterCounts.Max() - letterCounts.Min()}");
Console.Out.WriteLine($"After loop {sw.ElapsedMilliseconds}");        
    }
}
