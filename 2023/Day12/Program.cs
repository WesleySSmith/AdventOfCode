#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;
using MoreLinq;


bool sample = false;
bool debug = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

Dictionary<string, long> memo = new();

//Part1(lines, 1);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(string[] lines, int dupes)
{
    
    var totalPossibles = 0;
    foreach (var line in lines) {

        Console.WriteLine(line);
        var parts = line.Split(" ").ToArray();
        var pattern = parts[0];
        var groups = parts[1].Split(",").Select(int.Parse);

        pattern = string.Join("?", Enumerable.Repeat(pattern, dupes));
        groups = groups.Repeat(dupes);



        var possibles = 0;

        Queue<(string, int)> q = new Queue<(string, int)>();
        q.Enqueue((pattern, 0));

        while (q.Count > 0) {

           var p = q.Dequeue();
           var s = p.Item1;
           var n = p.Item2;

            // Check if legal so far

            // figure groups
            var groupsSoFar = s[0..n].GroupAdjacent(s => s).Where(g => g.Key == '#').Select(g => g.Count());

            var inGroup = n == 0 ? false : s[n-1] == '#';
            var lastCheck = n == s.Length;

            if (lastCheck) {
                if (groups.SequenceEqual(groupsSoFar)) {
                    //Console.WriteLine(s);
                    possibles++;
                }
                continue;
            }

            var compare = groups.Zip(groupsSoFar).Aggregate((Result: true, Index: 0), (agg, val) => {
                if (!agg.Result) {
                    return (false, agg.Index+1);
                }

                // Checking the last match
                if (agg.Index == groupsSoFar.Count() - 1) {
                    return (val.First >= val.Second, val.First == val.Second ? 1 : 0);
                }
                return (val.First == val.Second, agg.Index+1);
            } );

            if (!compare.Result) {
                continue;
            }

            if (s[n] == '?') {

                var nextWithDot = (s[0..n] + '.' + s[(n+1)..], n+1);
                var nextWithHash = (s[0..n] + '#' + s[(n+1)..], n+1);

                if (inGroup) {
                    if (compare.Index == 1) { /* Match is exact so far - need to be done with group */
                        q.Enqueue(nextWithDot);
                    } else {
                        /* Need more #'s to match */
                        q.Enqueue(nextWithHash);
                    }
                } else {
                    q.Enqueue(nextWithDot);
                    q.Enqueue(nextWithHash);
                }
            } else {
                q.Enqueue((s, n+1));
            }
        }

        Console.WriteLine($"Possibles: {possibles}");
        totalPossibles += possibles;
    }
    
    Console.Out.WriteLine($"Total Possibles is {totalPossibles}.");


}

int hits = 0;
int misses = 0;

void Part2(string[] lines)
{
    var totalPossibles = 0L;
    var dupes = 5;
    foreach (var line in lines) {
        hits = 0;
        misses = 0;
        Console.WriteLine(line);
        var parts = line.Split(" ").ToArray();
        var pattern = parts[0];
        var groups = parts[1].Split(",").Select(int.Parse);

        pattern = string.Join("?", Enumerable.Repeat(pattern, dupes));
        groups = groups.Repeat(dupes);



        var possibles = P(pattern, groups.ToArray());
        totalPossibles += possibles;
         Console.WriteLine($"Possibles: {possibles}  (H: {hits}, M: {misses})");
    }
    
    Console.Out.WriteLine($"Total Possibles is {totalPossibles}.");
}



long Pmemo(string s, int[] groups) {
    var key = $"{s}:{string.Join(',', groups)}";
    if (memo.TryGetValue(key, out var result)) {
        hits++;
        return result;
    }
    misses++;
    result = P(s, groups);
    memo.Add(key, result);
    return result;
}

long P(string s, int[] groups) {

    if (groups.Length == 0) {
        if (s.Length == 0 || s.All(c => c is '.' or '?')) {
            if (debug) Console.WriteLine($"P({s} {string.Join(',', groups)}): {1} Z");
            return 1;
        } else {
            if (debug) Console.WriteLine($"P({s} {string.Join(',', groups)}): {0} Y");
            return 0;
        }
    }

    if (s.Length == 0) {
        if (debug) Console.WriteLine($"P({s} {string.Join(',', groups)}): {0} A");
        return 0;
    }


    int g = groups[0];

    if (s[0] == '#') {
        // We're starting a new segment and have no choices.  See if we can match:
        if ( s.Length < g || s[0..g].Any(c => c == '.')) {
            // No way it can match.
            if (debug) Console.WriteLine($"P({s} {string.Join(',', groups)}): {0} B");
            return 0;
        } else {
            // It can match. 

            // See if it's at the end of the string
            if (groups.Length == 1 && s.Length == g) { 
                if (debug) Console.WriteLine($"P({s} {string.Join(',', groups)}): {1} C");
                return 1;
            }

            // See if it's at the end of string, but there are more groups to match
            if (s.Length == g) {
                if (debug) Console.WriteLine($"P({s} {string.Join(',', groups)}): {0} CD");
                return 0;
            }
            //  See if it's followed by a gap or end of string
            if (s[g] is '.' or '?') {
                var rest = Pmemo(s[(g+1)..], groups[1..]);
                if (debug) Console.WriteLine($"P({s} {string.Join(',', groups)}): {rest} D");
                return rest;
            }

            // It must be followed by a '#', which isn't a match
            if (debug) Console.WriteLine($"P({s} {string.Join(',', groups)}): {0} E");
            return 0;
        }
    } else if (s[0] == '.') {
        var nextInteresting = s.IndexOfAny(['#', '?']);
        if (nextInteresting == -1) {
            // We have more groups to match, but no more interesting string to use
            if (debug) Console.WriteLine($"P({s} {string.Join(',', groups)}): {0} EF");
            return 0;
        }

        var rest = Pmemo(s[nextInteresting..], groups);
        if (debug) Console.WriteLine($"P({s} {string.Join(',', groups)}): {rest} F");
        return rest;
    } else {
        // We're at a '?'.  Try as both a ',' and a '#'
        var rest = Pmemo('#'+s[1..], groups) + Pmemo('.'+s[1..], groups);
        if (debug) Console.WriteLine($"P({s} {string.Join(',', groups)}): {rest} G");
        return rest;
    }
}

