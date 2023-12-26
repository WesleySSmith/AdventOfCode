#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Text;
//using MoreLinq;

bool sample = false;
bool debug = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt"); 
if (debug) Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

var blueprints = lines.Select(Blueprint.Parse).ToArray();
Dictionary<(int TimeLeft, ulong Robots, ulong Available), Answer> memo = new();
var bestOptionSoFar = int.MinValue;

Part1(blueprints);
//Part2(blueprints);


Console.Out.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");

long SumOfPowers(int basee, int depth) {
    return ((long)Math.Pow(basee, depth+1)) / basee - 1;
}

void Part1(Blueprint[] blueprints) {

var score = 0;
var index = 0;
    foreach (var blueprint in blueprints) {
        ++index;
        Global.Blueprint = blueprint;
        
        bestOptionSoFar = int.MinValue;
        memo.Clear();
        var depth = 24;

        var startingRobots = Global.Assemble(1, 0, 0, 0);
        var s = Global.DecodeBits(startingRobots);
        var answer = Dfs(depth, startingRobots, Global.Assemble(0, 0, 0, 0));

        var qualityLevel = answer.Geodes * index;
        score += qualityLevel;
        Console.WriteLine($"{index} * {answer.Geodes}:  {qualityLevel}");
        //Console.WriteLine(answer.Log);
    }
    Console.WriteLine($"Part 1 score: {score}");
    
}


void Part2(Blueprint[] blueprints) {

var score = 1;
var index = 0;
    foreach (var blueprint in blueprints.Take(3)) {
        ++index;
        Global.Blueprint = blueprint;
        
        bestOptionSoFar = int.MinValue;
        memo.Clear();
        var depth = 32;
        //searchSpace = SumOfPowers(5, depth);

        var startingRobots = Global.Assemble(1, 0, 0, 0);
        var answer = Dfs(depth, startingRobots, Global.Assemble(0, 0, 0, 0));

        score *= answer.Geodes;
        Console.WriteLine($"{index}: {answer.Geodes}");
        //Console.WriteLine(answer.Log);
    }
    Console.WriteLine($"Part 2 score: {score}");
}



Answer Dfs(int timeLeft, ulong robots, ulong available) {

    if (timeLeft == 1) {
        return new Answer(Global.GetGeode(robots) /*, $"[{timeLeft}]: Robots: {DecodeBits(robots)}; Available: {DecodeBits(available)}\n{bestOption.Log}"*/);
    }

    string log = null;
    if (debug) {
        log = $"[{timeLeft}] - Robots: {robots} Available: {available}";
    }

    if (timeLeft == 0) {
        throw new Exception("Shouldn't be here");
    }
    
    var memoKey = (timeLeft, robots, available);


   



    int upperBoundGeodesProduced = (Global.GetGeode(available) + Global.GetGeode(robots) * timeLeft) + (timeLeft *  (timeLeft - 1) / 2);

    if (upperBoundGeodesProduced <= bestOptionSoFar) {
        return new Answer(-1 /*, "Aborting due to upper bound"*/);
    }

    if (memo.TryGetValue(memoKey, out var answer)) {

        return answer;
    }

    List<Answer> options = new();

    Answer bestOption;

    var nowAvailableAfterProduction = robots + available;

    var maxConsumableInRemainingTime = Global.Blueprint.MaxConsumableInTimestep * (ulong)timeLeft;

    // Throw away any resources that I can never use
    nowAvailableAfterProduction = Global.Assemble(
        Math.Min(Global.GetOre(nowAvailableAfterProduction), Global.GetOre(maxConsumableInRemainingTime)),
        Math.Min(Global.GetClay(nowAvailableAfterProduction), Global.GetClay(maxConsumableInRemainingTime)),
        Math.Min(Global.GetObsidian(nowAvailableAfterProduction), Global.GetObsidian(maxConsumableInRemainingTime)),
        Global.GetGeode(nowAvailableAfterProduction));

    foreach(var robotOption in Global.Blueprint.Robots) {
        if (
            Global.GetOre(available) >= robotOption.OreCost
            && Global.GetClay(available) >= robotOption.ClayCost
            && Global.GetObsidian(available) >= robotOption.ObsidianCost
            ) {

            var shouldMake = (robotOption.Produces == Global.Ore && Global.GetOre(robots) < Global.GetOre(Global.Blueprint.MaxConsumableInTimestep))
            ||  (robotOption.Produces == Global.Clay && Global.GetClay(robots) < Global.GetClay(Global.Blueprint.MaxConsumableInTimestep))
            ||  (robotOption.Produces == Global.Obsidian && Global.GetObsidian(robots) < Global.GetObsidian(Global.Blueprint.MaxConsumableInTimestep))
            ||  (robotOption.Produces == Global.Geode);

            if  (shouldMake) {
                var nowAvailable = nowAvailableAfterProduction - Global.Assemble(robotOption.OreCost, robotOption.ClayCost, robotOption.ObsidianCost, 0);

                var nowRobots = robots + Global.Assemble(
                    robotOption.Produces == Global.Ore ? (ushort)1 : (ushort)0,
                    robotOption.Produces == Global.Clay ? (ushort)1 : (ushort)0,
                    robotOption.Produces == Global.Obsidian ? (ushort)1 : (ushort)0,
                    robotOption.Produces == Global.Geode ? (ushort)1 : (ushort)0);
                options.Add(Dfs(timeLeft - 1, nowRobots, nowAvailable));
            }
        }
    }

    if (options.Count != 4) { // Only make nothing if we can't make everything else
        options.Add(Dfs(timeLeft - 1, robots, nowAvailableAfterProduction));
    }

    bestOption = options.MaxBy(o => o.Geodes);

    
    var totalGeodes = Global.GetGeode(robots) + bestOption.Geodes;
    var result = new Answer(totalGeodes /*, $"[{timeLeft}]: Robots: {DecodeBits(robots)}; Available: {DecodeBits(available)}\n{bestOption.Log}"*/);

    memo.Add(memoKey, result);


    var totalGeodesTotal = totalGeodes + Global.GetGeode(available);
    if (bestOptionSoFar < totalGeodesTotal) {
        bestOptionSoFar = totalGeodesTotal;
        if (debug) {Console.WriteLine($"Best so far now: {bestOptionSoFar}");}
    }

    return result;
}

record Blueprint(Robot[] Robots, ulong MaxConsumableInTimestep) {

    public static Blueprint Parse(string s) {
        var s1 = s.Split(':');
        var r = s1[1].Split('.');

        var robots = new[] {
                Robot.Parse(Global.Ore, r[0]),
                Robot.Parse(Global.Clay, r[1]),
                Robot.Parse(Global.Obsidian, r[2]),
                Robot.Parse(Global.Geode, r[3])
        };

        var maxConsumable = Global.Assemble(
            robots.Select(r => r.OreCost).Max(),
            robots.Select(r => r.ClayCost).Max(),
            robots.Select(r => r.ObsidianCost).Max(),
            (ushort) 0);

        return new Blueprint(robots, maxConsumable);
    }

}

record Robot(int Produces, ushort OreCost, ushort ClayCost, ushort ObsidianCost) {
    public static Robot Parse(int product, string s) {
        var s1 = s.Split("costs ");
        var s2 = s1[1].Split("and ");
        
        return new Robot(product,
            ushort.Parse(s2[0].Split(" ")[0]),
            product == Global.Obsidian ? ushort.Parse(s2[1].Split(" ")[0]) : (ushort)0,
            product == Global.Geode ? ushort.Parse(s2[1].Split(" ")[0]) : (ushort)0
        );
    }
}

//record Answer(int Geodes, string Log);
record Answer(int Geodes);

static class Global {

    public const int Ore = 1;
    public const int Clay = 2;
    public const int Obsidian = 3;
    public const int Geode = 4;

    public static Blueprint Blueprint;

    public static ulong Assemble(ushort ore, ushort clay, ushort obsidian, ushort geode) {
        return 0UL | (ulong)ore << (16 * 3) | (ulong)clay << (16 * 2) | (ulong)obsidian << (16 * 1) | (ulong)geode;
    }

    public static string DecodeBits(ulong all) {
        return $"Ore: {GetOre(all)}, Clay: {GetClay(all)}, Obsidian: {GetObsidian(all)}, Geode: {GetGeode(all)}";
    }

    public static ushort GetOre(ulong all) {
        return (ushort)((all & 0xFFFF000000000000) >> (16 * 3));
    }
    public static ushort GetClay(ulong all) {
        return (ushort)((all & 0xFFFF00000000) >> (16 * 2));
    }
    public static ushort GetObsidian(ulong all) {
        return (ushort)((all & 0xFFFF0000) >> 16);
    }
    public static ushort GetGeode(ulong all) {
        return (ushort)(all & 0xFFFF);
    }
}
