#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
//using MoreLinq;


bool sample = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
//Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

//Part1(lines);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(string[] lines)
{
    
    List<List<int>> list = new List<List<int>>();

    var rules = new List<(int before, int after)>();
    bool part1 = true;
    for (int ii = 0; ii < lines.Length; ii++) {
        var line = lines[ii];
        if (line.Count() == 0) {
            part1 = false;
            continue;
        }
        if (part1) {
            var rule = line.Split('|');
            rules.Add((int.Parse(rule[0]), int.Parse(rule[1])));
        }
        else {
            list.Add(line.Split(",").Select(int.Parse).ToList());
        }
    }


    var lookup = rules.ToLookup(s => s.before, s=> s.after);

    long middleSum = 0;
    foreach (var update in list) {

        var seenPages = new bool[100];
        for (int pageIndex = 0; pageIndex < update.Count; pageIndex++) {
            var page = update[pageIndex];

            var afters = lookup[page];

            foreach (var after in afters) {
                if (seenPages[after]) {
                    goto next;
                }
            }
            seenPages[page] = true;
        }

        var middle = update[update.Count()/2];
        middleSum += middle;

        next:;
    }

    Console.Out.WriteLine($"Part 1: {middleSum}");
}


void Part2(string[] lines) {
    List<List<int>> list = new List<List<int>>();

    var rules = new List<(int before, int after)>();
    bool part1 = true;
    for (int ii = 0; ii < lines.Length; ii++) {
        var line = lines[ii];
        if (line.Count() == 0) {
            part1 = false;
            continue;
        }
        if (part1) {
            var rule = line.Split('|');
            rules.Add((int.Parse(rule[0]), int.Parse(rule[1])));
        }
        else {
            list.Add(line.Split(",").Select(int.Parse).ToList());
        }
    }


    var lookup = rules.ToLookup(s => s.before, s=> s.after);

    var outOfOrder = new List<List<int>>();

    
    foreach (var update in list) {

        var seenPages = new bool[100];
        for (int pageIndex = 0; pageIndex < update.Count; pageIndex++) {
            var page = update[pageIndex];

            var afters = lookup[page];

            foreach (var after in afters) {
                if (seenPages[after]) {
                    goto next;
                }
            }
            seenPages[page] = true;
        }

       
        continue;

        next:
        outOfOrder.Add(update);  
    }


    long middleSum = 0;
    
    foreach (var update in outOfOrder) {
    
        // Create a subset of the rules where both elements are in the update
        var pagesInUpdate = new bool[100];
        foreach (var page in update) {
            pagesInUpdate[page] = true;
        }
        var relevantRules = rules.Where(r => pagesInUpdate[r.after] && pagesInUpdate[r.before]).ToList();
        var relevantRulesAfterSet = relevantRules.Select(r=> r.after).ToHashSet();

        var newUpdate = new List<int>();
        var numPagesInUpdate = update.Count;
        for (int ii = 0; ii < numPagesInUpdate/2 + 1; ii++) {

            // Find the page that's not after anything else in the rules
            foreach (var page in update) {
                if (!relevantRulesAfterSet.Contains(page)) {
                    newUpdate.Add(page);

                    relevantRules = relevantRules.Where(r => r.before != page).ToList();
                    relevantRulesAfterSet = relevantRules.Select(r=> r.after).ToHashSet();

                    update.Remove(page);
                    goto next;
                }
            }   
            throw new Exception("Not found");
            next:;
        }
        
        // We stopped when we got to the middle element, so the middle is the last one in newUpdate
        var middle = newUpdate[^1];
        middleSum += middle;

    }

    Console.Out.WriteLine($"Part 2: {middleSum}");
}