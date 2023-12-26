#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using MoreLinq;


bool sample = false;


string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();


//Part1(lines);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(string[] lines)
{
    var hands = lines.Select(l => l.Split(" ")).Select(x => (Hand: x[0], Bid: long.Parse(x[1])));
    var typeComparer = new TypeComparer1();
    var highComparer = new HighComparer1();
    var orderedHands = hands.OrderByDescending(x => x.Hand, typeComparer).ThenByDescending(x => x.Hand, highComparer);

    var score = orderedHands.Zip(Enumerable.Range(1, orderedHands.Count()).Reverse()).Sum(e => e.Second * e.First.Bid);

    Console.Out.WriteLine($"Score is {score}");

}



void Part2(string[] lines)
{
    var hands = lines.Select(l => l.Split(" ")).Select(x => (Hand: x[0], Bid: long.Parse(x[1])));
    var typeComparer = new TypeComparer2();
    var highComparer = new HighComparer2();
    var orderedHands = hands.OrderByDescending(x => x.Hand, typeComparer).ThenByDescending(x => x.Hand, highComparer);

    var score = orderedHands.Zip(Enumerable.Range(1, orderedHands.Count()).Reverse()).Sum(e => e.Second * e.First.Bid);

    Console.Out.WriteLine($"Score is {score}");
}
public class HighComparer1 : IComparer<string> {
    public int Compare(string a, string b)
    {
        for (int ii = 0; ii < a.Length; ii++) {
            var scoreA = Score(a[ii]);
            var scoreB = Score(b[ii]);

            if (scoreA > scoreB) return 1;
            if (scoreA < scoreB) return -1;
        }
        return 0;
    }

    public static int Score(char card) {
        return card switch {
            'A' => 14,
            'K' => 13,
            'Q' => 12,
            'J' => 11,
            'T' => 10,
            _ => card - '0'
        };
    }
}
public class TypeComparer1 : IComparer<string> {
    public int Compare(string a, string b)
    {
        var scoreA = Score(a);
        var scoreB = Score(b);

        if (scoreA == scoreB) return 0;
        if (scoreA > scoreB) return 1;
        return -1;

    }

    public static int Score(string hand) {
        var groups = hand.GroupBy(c => c);

        // 5 of a kind
        if (groups.Count() == 1 && groups.Single().Count() == 5) {
            return 7;
        }

        // 4 of a kind
        if (groups.Count() ==2 && groups.Any(g => g.Count() == 4)) {
            return 6;
        }

        //Full house
        if (groups.Count() ==2 && groups.Any(g => g.Count() == 3)) {
            return 5;
        }

        // Three of a kind
        if (groups.Count() ==3 && groups.Any(g => g.Count() == 3)) {
            return 4;
        }

        // Two pair
        if (groups.Count() == 3) {
            var groupsOrderedByCount = groups.OrderByDescending(g => g.Count());
            if (groupsOrderedByCount.ElementAt(0).Count() == 2
            && groupsOrderedByCount.ElementAt(1).Count() == 2) {
                return 3;
            }
        } 

        // One pair
        if (groups.Count() == 4 && groups.Any(g => g.Count() == 2)) {
            return 2;
        }

        // High card
        if (groups.Count() == 5) {
            return 1;
        }
        throw new Exception("Bad logic in Score");

    }
}





public class HighComparer2 : IComparer<string> {
    public int Compare(string a, string b)
    {
        for (int ii = 0; ii < a.Length; ii++) {
            var scoreA = Score(a[ii]);
            var scoreB = Score(b[ii]);

            if (scoreA > scoreB) return 1;
            if (scoreA < scoreB) return -1;
        }
        return 0;
    }

    public static int Score(char card) {
        return card switch {
            'A' => 14,
            'K' => 13,
            'Q' => 12,
            'J' => 1,
            'T' => 10,
            _ => card - '0'
        };
    }
}
public class TypeComparer2 : IComparer<string> {
    public int Compare(string a, string b)
    {
        var scoreA = Score(a);
        var scoreB = Score(b);

        if (scoreA == scoreB) return 0;
        if (scoreA > scoreB) return 1;
        return -1;

    }

    public static int Score(string hand) {
        var groups = hand.GroupBy(c => c);

        var jokerGroup = groups.SingleOrDefault(g => g.Key == 'J');
        var nonJokerGroups = groups.Where(g => g.Key != 'J');

        // 5 of a kind
        if (groups.Count() == 1) {
            return 7;
        }

        if (groups.Count() == 2 && jokerGroup != null) { 
            return 7;
        }

        // 4 of a kind
        if (groups.Count() == 2 && groups.Any(g => g.Count() == 4)) {
            return 6;
        }

        if (groups.Count() == 3 && jokerGroup != null) {
            var groupsOrderedByCount = nonJokerGroups.OrderByDescending(g => g.Count());
            if (groupsOrderedByCount.First().Count() + jokerGroup.Count() == 4) {
                return 6;
            }
        }

        //Full house
        if (groups.Count() == 2 && groups.Any(g => g.Count() == 3)) {
            return 5;
        }

        if (groups.Count() == 3 && jokerGroup != null) {
            var groupsOrderedByCount = nonJokerGroups.OrderByDescending(g => g.Count());
            if (groupsOrderedByCount.First().Count() + jokerGroup.Count() == 3) {
                return 5;
            }
        }

        // Three of a kind
        if (groups.Count() == 3 && groups.Any(g => g.Count() == 3)) {
            return 4;
        }

        if (groups.Count() == 4 && jokerGroup != null) {
            var groupsOrderedByCount = nonJokerGroups.OrderByDescending(g => g.Count());
            if (groupsOrderedByCount.First().Count() + jokerGroup.Count() == 3) {
                return 4;
            }
        }


        // Two pair
        if (groups.Count() == 3) {
            var groupsOrderedByCount = groups.OrderByDescending(g => g.Count());
            if (groupsOrderedByCount.ElementAt(0).Count() == 2
            && groupsOrderedByCount.ElementAt(1).Count() == 2) {
                return 3;
            }
        } 

         if (groups.Count() == 4 && jokerGroup != null && jokerGroup.Count() == 1) {
            var groupsOrderedByCount = nonJokerGroups.OrderByDescending(g => g.Count());
            if (groupsOrderedByCount.ElementAt(0).Count() == 2
            && groupsOrderedByCount.ElementAt(1).Count() == 1) {
                return 3;
            }
        } 


        // One pair
        if (groups.Count() == 4 && groups.Any(g => g.Count() == 2)) {
            return 2;
        }

        // One pair
        if (groups.Count() == 5 && jokerGroup != null && jokerGroup.Count() == 1) {
            return 2;
        }

        // High card
        if (groups.Count() == 5) {
            return 1;
        }
        throw new Exception("Bad logic in Score");

    }
}

// 249804174 Too low
