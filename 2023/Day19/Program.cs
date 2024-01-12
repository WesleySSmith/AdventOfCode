#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Text;
using MoreLinq;
//using MoreLinq.Extensions;


bool sample = false;


string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();


var splits = lines.Segment(l => l == "").ToList();

 Rule A = new Rule {Dest = "A"};
 Rule R = new Rule {Dest = "R"};

var workflowDict = splits[0].Select(ParseWorkflow).ToDictionary(w => w.Name);
var parts = splits[1].Skip(1).Select(ParsePart).ToList();

//Part1(workflowDict, parts);
Part2(workflowDict);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(Dictionary<string, Workflow> workflows, List<Part> parts)
{
    
    var inPart = workflows["in"];

    List<Part> accepted = new List<Part>();

    foreach (var part in parts) {

        var workflow = inPart;
        while (true)
        {
            foreach (var rule in workflow.Rules) {
                
                bool ruleMatches;
                if (!rule.Cat.HasValue)  {
                    ruleMatches = true;
                } else {
                    var rating = part[rule.Cat.Value];
                    ruleMatches = rule.Op switch {Op.gt => rating > rule.Operand, Op.lt => rating < rule.Operand};
                }
                if (!ruleMatches) {
                    continue;
                } else {
                    if (rule.Dest == "A") {
                            accepted.Add(part);
                        goto nextPart;
                    } else if (rule.Dest == "R") {
                        goto nextPart;
                    } else {
                        workflow = workflows[rule.Dest];
                        goto nextWorkflow;
                    }
                }
            }
nextWorkflow:;            
        }
nextPart:;        
    }

    var score = accepted.Sum(ac => ac[Cat.x] + ac[Cat.m] + ac[Cat.a] + ac[Cat.s]);

    Console.Out.WriteLine($"Score is {score}.");


}

void Part2(Dictionary<string, Workflow> workflows)
{

    var workflow = workflows["in"];

    var allRatings = PartRanges2.All;

    var score = CountDistinctCombinations(workflows["in"], allRatings);

    Console.Out.WriteLine($"Score is {score}.");

}

long CountDistinctCombinations(Workflow workflow, PartRanges2 validRanges) {

   
    var ratingsForThisMatch = validRanges with {};
    var nextRatings = validRanges with {};
    var acceptingRanges = 0L;
    foreach (var rule in workflow.Rules) {

        if (rule.Cat.HasValue) {
            if (rule.Op == Op.gt) {
                ratingsForThisMatch[rule.Cat.Value] = nextRatings[rule.Cat.Value].And(Range2.GreaterThan(rule.Operand));
                nextRatings[rule.Cat.Value] = nextRatings[rule.Cat.Value].And(Range2.LessThan(rule.Operand + 1));
            } else {
                ratingsForThisMatch[rule.Cat.Value] = nextRatings[rule.Cat.Value].And(Range2.LessThan(rule.Operand));
                nextRatings[rule.Cat.Value] = nextRatings[rule.Cat.Value].And(Range2.GreaterThan(rule.Operand - 1));
            }
        } else {
            // Final rule
            ratingsForThisMatch = nextRatings with {};
        }

        long ruleAcceptingRanges;
        if (ratingsForThisMatch.Any) {

            if (rule.Dest == A.Dest) {
                long volume = ratingsForThisMatch.Volume();
                ruleAcceptingRanges = volume;
                //Console.WriteLine($"Accepting {volume} for rule: {rule} int workflow: {workflow.Name}");
            }
            else if (rule.Dest == R.Dest) {
                ruleAcceptingRanges = 0;
            } else {
                ruleAcceptingRanges = CountDistinctCombinations(workflowDict[rule.Dest], ratingsForThisMatch);
            }
            acceptingRanges += ruleAcceptingRanges;
        }
        ratingsForThisMatch = nextRatings with {};

    }
    return acceptingRanges;
}



Workflow ParseWorkflow(string s) {
    var splits = s.Split(['{', '}']);
    return new Workflow {
        Name = splits[0],
        Rules = ParseRules(splits[1])
    };
}

List<Rule> ParseRules(string s) {
    var splits = s.Split(',');
    return splits.Select(ParseRule).ToList();
}

Rule ParseRule(string s) {
    if (!s.Contains(':')) {
       return s switch {
                "A" => A,
                "R" => R,
                _ => new Rule {Dest = s}
            };
    }
    var splits = s[2..].Split(':');
    return new Rule {
        Cat = Enum.Parse<Cat>(s[0].ToString()),
        Op = s[1] switch {'<' => Op.lt, '>' => Op.gt },
        Operand = int.Parse(splits[0]),
        Dest = splits[1]
    };
}

Part ParsePart(string s) {
    var splits = s.Trim()[1..^1].Split([',','=']);
    var part = new Part();
    part[Cat.x] = int.Parse(splits[1]);
    part[Cat.m] = int.Parse(splits[3]);
    part[Cat.a] = int.Parse(splits[5]);
    part[Cat.s] = int.Parse(splits[7]);
    return part;
}

public record Workflow {
    public string Name;
    public List<Rule> Rules;
}
public record Rule {
    public Cat? Cat;
    public Op Op;
    public int Operand;
    public string Dest;
}


public enum Cat {
    x,m,a,s
}

public enum Op {
    gt,lt,R,A
}

record Part {
    private int[] Ratings = new int[4];

     public int this[Cat cat]
    {
        get
        {
            return Ratings[(int)cat];
        }
        set
        {
           Ratings[(int)cat] = value;
        }
    }
}

sealed record class PartRanges2 {

     private Range2[] Ranges;

    public PartRanges2()
    {
        Ranges = new Range2[4];
        X = new Range2();
        M = new Range2();
        A = new Range2();
        S = new Range2();

    }

    public PartRanges2(PartRanges2 other) {
        Ranges = new Range2[4];
        Ranges[0] = other.Ranges[0];
        Ranges[1] = other.Ranges[1];
        Ranges[2] = other.Ranges[2];
        Ranges[3] = other.Ranges[3];
    }


    public Range2 this[Cat cat]
    {
        get
        {
            return Ranges[(int)cat];
        }
        set
        {
           Ranges[(int)cat] = value;
        }
    }

    public Range2 X {
        get {
            return Ranges[(int)Cat.x];
        }
        set {
            Ranges[(int)Cat.x] = value;
        }
    }

    public Range2 M {
        get {
            return Ranges[(int)Cat.m];
        }
        set {
            Ranges[(int)Cat.m] = value;
        }
    }

    public Range2 A {
        get {
            return Ranges[(int)Cat.a];
        }
        set {
            Ranges[(int)Cat.a] = value;
        }
    }

    public Range2 S {
        get {
            return Ranges[(int)Cat.s];
        }
        set {
            Ranges[(int)Cat.s] = value;
        }
    }

    public bool Any {
        get {
            return X.Range > 0 || M.Range > 0 || A.Range > 0 || S.Range > 0;
        }
    }

    public static PartRanges2 All {
        get {
            return new PartRanges2 {
                X = Range2.All(),
                M = Range2.All(),
                A = Range2.All(),
                S = Range2.All(),
            };
        }
    }

    public long Volume() {
       return (long)X.Range * M.Range * A.Range * S.Range;
    }

}

public record struct Range2 {
    private readonly bool[] bits = new bool[4000];

    public Range2()
    {
    }


    public static Range2 All() {
        var r = new Range2();
        for (int ii = 0; ii < r.bits.Length; ii++) {
            r.bits[ii] = true;
        }
        return r;
    }

    public int Range {
        get {
            var c = 0;
            for (int ii = 0; ii < this.bits.Length; ii++) {
                c +=  this.bits[ii] ? 1 : 0;
            }
            return c;
        }
    }

    public Range2 And(Range2 other) {
        var newRange = new Range2();
        for (int ii = 0; ii < this.bits.Length; ii++) {
            newRange.bits[ii] = this.bits[ii] && other.bits[ii];
        }
        return newRange;
    }

    public Range2 Or(Range2 other) {
        var newRange = new Range2();
        for (int ii = 0; ii < this.bits.Length; ii++) {
            newRange.bits[ii] = this.bits[ii] || other.bits[ii];
        }
        return newRange;
    }

    public static Range2 LessThan(int v) {
        var newRange = new Range2();
        for (int ii = 0; ii < v - 1; ii++) {
            newRange.bits[ii] = true;
        }
        return newRange;
    }

     public static Range2 GreaterThan(int v) {
        var newRange = new Range2();
        for (int ii = v; ii < newRange.bits.Length; ii++) {
            newRange.bits[ii] = true;
        }
        return newRange;
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder(4000);
        for (int ii = 0; ii < this.bits.Length; ii++) {
            sb.Append(bits[ii]? '1' : '_');
        }
        return sb.ToString();
    }
    
}


