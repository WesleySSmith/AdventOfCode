#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Diagnostics;
using MoreLinq;

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

var moves = lines.Select(Move.Parse);

//Part1(moves);
Part2(moves);

Console.Out.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");

static void Part1(IEnumerable<Move> moves) {
   
    HashSet<Point> tailPoints = new HashSet<Point>();
    var count = 0;
    Point head = new Point(0,0);
    Point tail = new Point(0,0);
    tailPoints.Add(tail);
    foreach(var move in moves) {
        for (var ii = 0; ii < move.Steps; ii++){

            head = move.Direction switch {
                'R' => head with {X = (head.X + 1)},
                'L' => head with {X = (head.X - 1)},
                'U' => head with {Y = (head.Y + 1)},
                'D' => head with {Y = (head.Y - 1)},
            };

            if (head == tail) {
                // Nothing to do
            }
            else if (head.X == tail.X) {
                
                if (head.Y - tail.Y > 1) {
                    // tail moves up
                    tail = tail with {Y = tail.Y + 1};
                }
                else if (head.Y - tail.Y < -1) {
                    //tail moves down
                    tail = tail with {Y = tail.Y - 1};
                }
            } else if (head.Y == tail.Y) {
                if (head.X - tail.X > 1) {
                    // tail moves right
                    tail = tail with {X = tail.X + 1};
                }
                else if (head.X - tail.X < -1) {
                    //tail moves left
                    tail = tail with {X = tail.X - 1};
                }
            } else {
                if (head.X - tail.X > 0 && head.Y - tail.Y > 0 
                && (head.X - tail.X > 1 || head.Y - tail.Y > 1)) {
                    // tail moves up and right
                    tail = tail with {X = tail.X + 1, Y = tail.Y + 1};
                } 
                else 
                if (head.X - tail.X > 0 && head.Y - tail.Y < 0 
                && (head.X - tail.X > 1 || head.Y - tail.Y < -1))
                {
                    // tail moves down and right
                    tail = tail with {X = tail.X + 1, Y = tail.Y - 1};
                }
                else 
                if (head.X - tail.X <  0 && head.Y - tail.Y > 0 
                && (head.X - tail.X < -1 || head.Y - tail.Y > 1)) 
                {
                    // tail moves up and left
                    tail = tail with {X = tail.X - 1, Y = tail.Y + 1};
                } 
                else 
                if (head.X - tail.X < 0  && head.Y - tail.Y < 0 
                && (head.X - tail.X < -1 || head.Y - tail.Y < -1)) 
                {
                    // tail moves down and left
                    tail = tail with {X = tail.X - 1, Y = tail.Y - 1};
                } 
            }

            tailPoints.Add(tail);

            //Console.Out.WriteLine($"[{count++}] head: {head}, tail: {tail}");

        }
    }
    Console.Out.WriteLine($"Part 1 Count: {tailPoints.Count()}");
    Console.Out.WriteLine($"head: {head}, tail: {tail}");
}


static void Part2(IEnumerable<Move> moves) {
   
    HashSet<Point> tailPoints = new HashSet<Point>();
    var count = 0;
    Point realHead = new Point(0,0);
    Point[] tails = new Point[9];
    for (int tailCounter = 0; tailCounter < tails.Length; tailCounter++ ) {
        tails[tailCounter] = new Point(0,0);
    }
    tailPoints.Add(tails.Last());
    foreach(var move in moves) {
        for (var ii = 0; ii < move.Steps; ii++){

            realHead = move.Direction switch {
                'R' => realHead with {X = (realHead.X + 1)},
                'L' => realHead with {X = (realHead.X - 1)},
                'U' => realHead with {Y = (realHead.Y + 1)},
                'D' => realHead with {Y = (realHead.Y - 1)},
            };

            var leader = realHead;

            for (int tailCounter = 0; tailCounter < tails.Length; tailCounter++ ) {
                var follower = tails[tailCounter];
                if (leader == follower) {
                    // Nothing to do
                }
                else if (leader.X == follower.X) {
                    
                    if (leader.Y - follower.Y > 1) {
                        // tail moves up
                        follower = follower with {Y = follower.Y + 1};
                    }
                    else if (leader.Y - follower.Y < -1) {
                        //tail moves down
                        follower = follower with {Y = follower.Y - 1};
                    }
                } else if (leader.Y == follower.Y) {
                    if (leader.X - follower.X > 1) {
                        // tail moves right
                        follower = follower with {X = follower.X + 1};
                    }
                    else if (leader.X - follower.X < -1) {
                        //tail moves left
                        follower = follower with {X = follower.X - 1};
                    }
                } else {
                    if (leader.X - follower.X > 0 && leader.Y - follower.Y > 0 
                    && (leader.X - follower.X > 1 || leader.Y - follower.Y > 1)) {
                        // tail moves up and right
                        follower = follower with {X = follower.X + 1, Y = follower.Y + 1};
                    } 
                    else 
                    if (leader.X - follower.X > 0 && leader.Y - follower.Y < 0 
                    && (leader.X - follower.X > 1 || leader.Y - follower.Y < -1))
                    {
                        // tail moves down and right
                        follower = follower with {X = follower.X + 1, Y = follower.Y - 1};
                    }
                    else 
                    if (leader.X - follower.X <  0 && leader.Y - follower.Y > 0 
                    && (leader.X - follower.X < -1 || leader.Y - follower.Y > 1)) 
                    {
                        // tail moves up and left
                        follower = follower with {X = follower.X - 1, Y = follower.Y + 1};
                    } 
                    else 
                    if (leader.X - follower.X < 0  && leader.Y - follower.Y < 0 
                    && (leader.X - follower.X < -1 || leader.Y - follower.Y < -1)) 
                    {
                        // tail moves down and left
                        follower = follower with {X = follower.X - 1, Y = follower.Y - 1};
                    } 
                }
                tails[tailCounter] = follower;
                leader = follower;
            }

            tailPoints.Add(tails.Last());

            //Console.Out.WriteLine($"[{count++}] head: {realHead}, tails: \n{string.Join("\n", tails.Select((t,i) => $"{i}: {t}"))}");

        }
    }
    Console.Out.WriteLine($"Part 2 Count: {tailPoints.Count()}");
    //Console.Out.WriteLine($"head: {head}, tail: {tail}");
}



public record Move {
    public char Direction;
    public byte Steps;

    public static Move Parse(string s) {
        return new Move {
            Direction = s[0],
            Steps = byte.Parse(s[2..])
        };
    }
}

public record Point(int X, int Y) {
}

