#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using MoreLinq;


bool sample = false;
bool debug = false;

string NORMAL_BG      = Console.IsOutputRedirected ? "" : "\x1b[49m";
string RED_BG        = Console.IsOutputRedirected ? "" : "\x1b[41m";
string GREEN_BG      = Console.IsOutputRedirected ? "" : "\x1b[42m";
string YELLOW_BG      = Console.IsOutputRedirected ? "" : "\x1b[43m";
string X1_BG      = Console.IsOutputRedirected ? "" : "\x1b[44m";
string X2_BG      = Console.IsOutputRedirected ? "" : "\x1b[45m";

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "gavin.txt");
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

List<Brick> bricks = [];
foreach (var kvp in lines.Index()) {
    var line = kvp.Value;
    var index = kvp.Key;
    var splits = line.Split('~').Select(p => p.Split(',').Select(int.Parse));
    var p1 = splits.ElementAt(0);
    var p2 = splits.ElementAt(1);
    bricks.Add(new Brick(
        new Point(p1.ElementAt(0), p1.ElementAt(1), p1.ElementAt(2)),
        new Point(p2.ElementAt(0), p2.ElementAt(1), p2.ElementAt(2)),
        index
    ));
}

//Part1(bricks);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(List<Brick> bricks)
{

    var maxDims = bricks.Select(b => b.P2).Aggregate((0, 0, 0), ((agg, p) => (Math.Max(agg.Item1, p.X), Math.Max(agg.Item2, p.Y), Math.Max(agg.Item3, p.Z))));

    int?[,,] space = new int?[maxDims.Item1 + 1, maxDims.Item2 + 1, maxDims.Item3 + 1];

    InitSpaceFromBricks(bricks, space);

    if (debug) PrintSpace(space);
    BricksFall(bricks, space);

    if (debug) PrintSpace(space);

    var bricksCouldDisintegrate = CheckCanSafelyDisintegrateBricks(bricks, debug, space);

    Console.WriteLine($"Bricks that could disintegrate: {bricksCouldDisintegrate.Count}");
}

void PrintSpace(int?[,,] space) {

    for (int z = space.GetLength(2) -1; z >=0; z--) {
        for (int y = 0; y < space.GetLength(1); y++) {
            for (int x = 0; x < space.GetLength(0); x++) {
                var b = space[x,y,z];

                Console.Write((b.HasValue ? b.Value.ToString() : "").PadLeft(3));
            }
            Console.Write($" {GREEN_BG}|{NORMAL_BG} ");
        }
        Console.WriteLine();
    }

    Console.WriteLine("\n");

}

void Part2(string[] lines)
{
    var maxDims = bricks.Select(b => b.P2).Aggregate((0, 0, 0), ((agg, p) => (Math.Max(agg.Item1, p.X), Math.Max(agg.Item2, p.Y), Math.Max(agg.Item3, p.Z))));

    int?[,,] space = new int?[maxDims.Item1 + 1, maxDims.Item2 + 1, maxDims.Item3 + 1];

    InitSpaceFromBricks(bricks, space);

    if (debug) PrintSpace(space);
    BricksFall(bricks, space);

    if (debug) PrintSpace(space);

    var bricksCouldDisintegrate = new HashSet<int>(CheckCanSafelyDisintegrateBricks(bricks, debug, space));

    var bricksThatWillCauseChainReaction = bricks.Where(b => !bricksCouldDisintegrate.Contains(b.Num)).ToList();

    long acc =0;
    List<(int BrickNum, int NumFall)> log = new();
    foreach (var brickThatWillCauseChainReaction in bricksThatWillCauseChainReaction) {
        var altBricks = bricks.Where(b => b != brickThatWillCauseChainReaction).Select(b => new Brick(b.P1, b.P2, b.Num)).ToList();
        var altSpace = new int?[maxDims.Item1 + 1, maxDims.Item2 + 1, maxDims.Item3 + 1];
        InitSpaceFromBricks(altBricks, altSpace);
        var numBricksThatFell = BricksFall(altBricks, altSpace);
        log.Add((brickThatWillCauseChainReaction.Num, numBricksThatFell));
        acc += numBricksThatFell;
    }

    Console.WriteLine($"Sum num bricks would fall: {acc}");

    Console.WriteLine(string.Join("\n", log.OrderBy(l => l.BrickNum).Select(l => $"{l.BrickNum}: {l.NumFall}")));
}



static void InitSpaceFromBricks(List<Brick> bricks, int?[,,] space)
{
    foreach (Brick brick in bricks)
    {
        var points = brick.Points;
        foreach (var point in points)
        {
            space[point.X, point.Y, point.Z] = brick.Num;
        }
    }
}

static int BricksFall(List<Brick> bricks, int?[,,] space)
{
    int bricksThatFell = 0;
    var bottomUpList = bricks.OrderBy(b => b.P1.Z).ToList();

    foreach (var brick in bottomUpList)
    {
        // find bottom cross section:
        var lowestZ = brick.P1.Z;
        if (lowestZ == 1)
        {
            // already at bottom
            continue;
        }

        var allBrickPoints = brick.Points;
        var bottomCrossSection = allBrickPoints.Where(b => b.Z == lowestZ).ToList();

        // use the space[] to see if it could move down

        var possibleZ = lowestZ - 1;
        while (possibleZ > 0)
        {

            foreach (var point in bottomCrossSection)
            {
                if (space[point.X, point.Y, possibleZ].HasValue)
                {
                    goto noFarther;
                }
            }
            // was okay to move down
            possibleZ--;
        }
    noFarther:;
        var lowestPossibleZ = possibleZ + 1;
        var unitsToFall = lowestZ - lowestPossibleZ;

        if (unitsToFall > 0)
        {

            bricksThatFell++;
            // Move the brick
            foreach (var p in allBrickPoints)
            {
                space[p.X, p.Y, p.Z] = null;
            }

            brick.P1 = new Point(brick.P1.X, brick.P1.Y, brick.P1.Z - unitsToFall);
            brick.P2 = new Point(brick.P2.X, brick.P2.Y, brick.P2.Z - unitsToFall);

            var newBrickPoints = brick.Points;
            foreach (var p in newBrickPoints)
            {
                space[p.X, p.Y, p.Z] = brick.Num;
            }
        }
    }
    return bricksThatFell;
}


static List<int> CheckCanSafelyDisintegrateBricks(List<Brick> bricks, bool debug, int?[,,] space)
{
    List<int> bricksThatCanDisinegrate = new();
    foreach (var brick in bricks)
    {

        if (debug) Console.WriteLine($"Check {brick.Num}");
        HashSet<int> bricksThatMightMove = new();

        // find all bricks on the row above the top of this brick
        var highestZ = brick.P2.Z;

        var allBrickPoints = brick.Points;
        var topCrossSection = allBrickPoints.Where(b => b.Z == highestZ).ToList();

        foreach (var point in topCrossSection)
        {
            var cell = space[point.X, point.Y, point.Z + 1];

            if (cell.HasValue)
            {
                bricksThatMightMove.Add(cell.Value);
            }
        }

        if (debug) Console.WriteLine($" Other bricks to check {string.Join(", ", bricksThatMightMove.Select(bNum => bNum.ToString()))}");
        foreach (var brickThatMightMove in bricksThatMightMove.Select(bNum => bricks.Single(b => b.Num == bNum)))
        {
            var bottomCrossSection = brickThatMightMove.Points.Where(b => b.Z == highestZ + 1).ToList();
            foreach (var point in bottomCrossSection)
            {
                var s = space[point.X, point.Y, highestZ];
                if (s.HasValue && s.Value != brick.Num)
                {
                    if (debug) Console.WriteLine($"  {brickThatMightMove.Num} Supported by {s.Value}");
                    goto checkNextBrickThatMightMove;
                }
            }
            // Brick not supported
            if (debug) Console.WriteLine($" Brick not supported: {brickThatMightMove.Num}");
            goto checkNextBrick;
        checkNextBrickThatMightMove:;
        }

        bricksThatCanDisinegrate.Add(brick.Num);
        if (debug) Console.WriteLine($" Could disintegrate {brick.Num}");

        checkNextBrick:;
    }

    return bricksThatCanDisinegrate;
}

record Point(int X, int Y, int Z);

record Brick
 {
    public Brick(Point P1, Point P2, int Num)
    {
        this.P1 = P1;
        this.P2 = P2;
        this.Num = Num;
    }

    public Point P1;
    public Point P2;

    public int Num;

    public List<Point> Points {
        get {
            var dX = P2.X - P1.X;
            var dY = P2.Y - P1.Y;
            var dZ = P2.Z - P1.Z;

            if (dX == 0 && dY == 0 && dZ == 0) {
                return [P1];
            }

            if (dX > 0) {
                return Enumerable.Range(0, dX+1).Select(n => new Point(P1.X + n, P1.Y, P1.Z)).ToList();
            }
            if (dY > 0) {
                return Enumerable.Range(0, dY+1).Select(n => new Point(P1.X, P1.Y + n, P1.Z)).ToList();
            }
            return Enumerable.Range(0, dZ+1).Select(n => new Point(P1.X, P1.Y, P1.Z + n)).ToList();
        }
    }
    public bool Intersects(Brick b) {
        var points1 = this.Points;
        var points2 = b.Points;

        return points1.Intersect(points2).Any();
    }

    public bool TouchingGround {
        get {
            return P1.Z == 1;
        }
    }

   
    
 }
