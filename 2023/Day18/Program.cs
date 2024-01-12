#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Text;
using MoreLinq;
using EmnumerableEx;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Drawing;
//using MoreLinq.Extensions;

string NORMAL      = Console.IsOutputRedirected ? "" : "\x1b[39m";
string RED         = Console.IsOutputRedirected ? "" : "\x1b[91m";
string GREEN       = Console.IsOutputRedirected ? "" : "\x1b[92m";


int RenderCount = 0;
int RenderSkip = 1000;
int RenderThrow = int.MaxValue; // RenderSkip + 5;

bool sample = false;

Console.WriteLine($"Testing {RED}red{NORMAL} and {GREEN}green{NORMAL}");
string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

// Part 1:
//var instructions = lines.Select(l => new Instruction { Dir = Enum.Parse<Dir>(l[0].ToString()), Len = int.Parse(l[2..].Split(' ')[0])}).ToList();

// Part 2:
var instructions = lines
    .Select(l => l.Split("#")[1])
    .Select(s => new Instruction { 
        Dir = s[5] switch {'0' => Dir.R, '1' => Dir.D, '2' => Dir.L, '3' => Dir.U},
        Len = Convert.ToInt64(s[0..5], 16), 
    }).ToList();



Render(instructions);
Part1(instructions);
//Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(List<Instruction> instructionsX)
{
    
    
    long areaAdded = 0;

    int mainLoopCount = 0;
    int iterationCount = 0;
    int advanceCount = 1;
    var ill = new LinkedList<Instruction>(instructionsX);

    int lastLen = 0;   
    while (ill.Count > 4) {

        if (ill.Count == lastLen) {

            // Advance one
            var lastN = ill.Last;
            ill.Remove(lastN);
            ill.AddFirst(lastN);

            Console.WriteLine($"++++++++++++++++++++++++++  Advances {advanceCount++}:");
            Render(ill);


            //throw new Exception("Too many main loops");
        }
        lastLen = ill.Count;

        Console.WriteLine($"******************* Looper {mainLoopCount++}");

        
        var illCurrent = ill.First;
        while (illCurrent != null) {


            iterationCount++;
            CalcInstructionBounds(ill);
            var i0 = illCurrent;
            var i1 = i0.Next ?? ill.First;
            var i2 = i1.Next ?? ill.First;
            var i3 = i2.Next ?? ill.First;
            var i4 = i3.Next ?? ill.First;

            var t1 = GetTurn(i0.Value.Dir, i1.Value.Dir);
            var t2 = GetTurn(i1.Value.Dir, i2.Value.Dir);
            var t3 = GetTurn(i2.Value.Dir, i3.Value.Dir);
            var t4 = GetTurn(i3.Value.Dir, i4.Value.Dir);

            var bumpS = t1.ToString() + t2.ToString() + t3.ToString();
            Console.WriteLine($"Found {bumpS}");
            if (bumpS == "RLL") {

                var preN = i0;
                var s1N = i1;
                var s2N = i2;
                var s3N = i3;
                var postN = i4;

                var pre = preN.Value;
                var s1 = s1N.Value;
                var s2 = s2N.Value;
                var s3 = s3N.Value;
                var post = postN.Value;

                pre.RenderChar = '0';
                s1.RenderChar = '1';
                s2.RenderChar = '2';
                s3.RenderChar = '3';
                post.RenderChar = '4';

                Render(ill);

               

                if (s1.Len == s3.Len) {

                    PointL p1 = new PointL(), p2 = new PointL();
                    if (pre.Dir is Dir.U or Dir.D) {
                        p1.X = p2.X = pre.MaxX;
                        p1.Y = pre.EndY;
                        p2.Y = post.StartY;
                    } else  {
                        p1.Y = p2.Y = pre.MaxY;
                        p1.X = pre.EndX;
                        p2.X = post.StartX;
                    }

                    if (!CheckForNoCrossovers(ill, p1, p2)) {
                        Console.WriteLine("Skipping - crossovers #1");
                        illCurrent = i1;  
                        goto after;   
                    }

                    var toAdd = (s2.Len - 1) * s1.Len;
                    Console.WriteLine($"Case 1 RLL-Equal Adds {toAdd}");
                    areaAdded += toAdd;
                    ill.Remove(s1N);
                    ill.Remove(s2N);
                    ill.Remove(s3N);
                    ill.Remove(postN);
                    pre.Len += s2.Len + post.Len;
                    illCurrent = preN.Next;
                } else {
                    var minWidth = Math.Min(s1.Len, s3.Len);
                    

                    if (s1.Len == minWidth) {

                        PointL p1 = new PointL(), p2 = new PointL();
                        if (pre.Dir is Dir.U or Dir.D) {
                            p1.X = p2.X = pre.MaxX;
                            p1.Y = pre.EndY;
                            p2.Y = post.StartY;
                        } else  {
                            p1.Y = p2.Y = pre.MaxY;
                            p1.X = pre.EndX;
                            p2.X = post.StartX;
                        }

                        if (!CheckForNoCrossovers(ill, p1, p2)) {
                            Console.WriteLine("Skipping - crossovers #2");
                            illCurrent = i1;  
                            goto after;   
                        }

                        var toAdd = minWidth * (s2.Len - 1);
                        areaAdded += toAdd;
                        Console.WriteLine($"Case 2 RLL-S1Smaller Adds {toAdd}");
                        ill.Remove(s1N);
                        ill.Remove(s2N);
                        pre.Len += s2.Len;
                        s3.Len -= s1.Len;
                        illCurrent = postN;
                    } else { // s3 is shorter

                        if (pre.Dir != post.Dir) {

                            PointL p1 = new PointL(), p2 = new PointL();
                            if (pre.Dir is Dir.U or Dir.D) {
                                p1.X = p2.X = post.MaxX;
                                p1.Y = pre.EndY;
                                p2.Y = post.EndY;
                            } else  {
                                p1.Y = p2.Y = post.MaxY;
                                p1.X = pre.EndX;
                                p2.X = post.EndX;
                            }

                            if (!CheckForNoCrossovers(ill, p1, p2)) {
                                Console.WriteLine("Skipping - crossovers #3a");
                                illCurrent = i1;  
                                goto after;   
                            }

                            var toAdd = (minWidth -1) * (s2.Len - 1) + (s2.Len - post.Len - 1);
                            areaAdded += toAdd;
                            Console.WriteLine($"Case 3 RLL-S3Smaller-OppositeDir Adds {toAdd}");
                            ill.Remove(s2N);
                            ill.Remove(s3N);
                            post.Dir = pre.Dir;
                            post.Len = s2.Len - post.Len;
                            s1.Len -= s3.Len;
                            illCurrent = postN;

                        } else {

                            PointL p1 = new PointL(), p2 = new PointL();
                            if (pre.Dir is Dir.U or Dir.D) {
                                p1.X = p2.X = post.MaxX;
                                p1.Y = pre.EndY;
                                p2.Y = post.StartY;
                            } else  {
                                p1.Y = p2.Y = post.MaxY;
                                p1.X = pre.EndX;
                                p2.X = post.StartX;
                            }

                            if (!CheckForNoCrossovers(ill, p1, p2)) {
                                Console.WriteLine("Skipping - crossovers #3b");
                                illCurrent = i1;  
                                goto after;   
                            }
                            var toAdd = minWidth * (s2.Len - 1);
                            areaAdded += toAdd;
                            Console.WriteLine($"Case 4 RLL-S3Smaller-SameDir Adds {toAdd}");
                            ill.Remove(s2N);
                            ill.Remove(s3N);
                            post.Len += s2.Len;
                            s1.Len -= s3.Len;
                            illCurrent = postN;
                        }
                    }
                }

                Console.WriteLine("Now:");
                Render(ill);
after:                
                pre.RenderChar = (char)0;
                s1.RenderChar = (char)0;
                s2.RenderChar = (char)0;
                s3.RenderChar = (char)0;
                post.RenderChar = (char)0;

            } else if (bumpS == "RRL") {
               
                var preN = i0.Previous;
                var s1N = i1.Previous;
                var s2N = i2.Previous;
                var s3N = i3.Previous;
                var postN = i4.Previous;

                 if (preN == null) {
                    Console.WriteLine("Skipping - too soon #1");
                    illCurrent = i1;
                    continue;
                }
                if (s1N == null || s2N == null || s3N == null || postN == null)
                {
                    if (ill.Count == 6) {
                        Console.WriteLine("Near the end");
                        s1N = preN.Next;
                        s2N = s1N.Next ?? ill.First;
                        s3N = s2N.Next ?? ill.First;
                        postN = s3N.Next ?? ill.First;

                    } else {
                        Console.WriteLine("Skipping - too soon #1a");
                        illCurrent = null;
                        continue;
                    }
                }

                var pre = preN.Value;
                var s1 = s1N.Value;
                var s2 = s2N.Value;
                var s3 = s3N.Value;
                var post = postN.Value;

                pre.RenderChar = '0';
                s1.RenderChar = '1';
                s2.RenderChar = '2';
                s3.RenderChar = '3';
                post.RenderChar = '4';

                Render(ill);

                if (s1.Len == s3.Len) {
                    if (pre.Dir != post.Dir) {
                        Console.WriteLine("Skipping - too soon #3");
                        illCurrent = i1;  
                        goto after;      
                    }

                    PointL p1 = new PointL(), p2 = new PointL();
                    if (pre.Dir is Dir.U or Dir.D) {
                        p1.X = p2.X = post.MaxX;
                        p1.Y = pre.EndY;
                        p2.Y = post.StartY;
                    } else  {
                        p1.Y = p2.Y = post.MaxY;
                        p1.X = pre.EndX;
                        p2.X = post.StartX;
                    }

                    if (!CheckForNoCrossovers(ill, p1, p2)) {
                        Console.WriteLine("Skipping - crossovers #4");
                        illCurrent = i1;  
                        goto after;   
                    }


                    var toAdd = -1 * s1.Len * (s2.Len + 1);
                    Console.WriteLine($"Case 5 RRL-Equal Adds {toAdd}");
                    areaAdded += toAdd;
                    ill.Remove(s1N);
                    ill.Remove(s2N);
                    ill.Remove(s3N);
                    ill.Remove(postN);
                    pre.Len += s2.Len + post.Len;
                    illCurrent = preN.Next;
                } else {
                    var minWidth = Math.Min(s1.Len, s3.Len);
                    

                    if (s1.Len == minWidth) {
                        if (pre.Dir == post.Dir) {

                            PointL p1 = new PointL(), p2 = new PointL();
                            if (pre.Dir is Dir.U or Dir.D) {
                                p1.X = p2.X = post.MaxX;
                                p1.Y = pre.EndY;
                                p2.Y = post.StartY;
                            } else  {
                                p1.Y = p2.Y = post.MaxY;
                                p1.X = pre.EndX;
                                p2.X = post.StartX;
                            }

                            if (!CheckForNoCrossovers(ill, p1, p2)) {
                                Console.WriteLine("Skipping - crossovers #4");
                                illCurrent = i1;  
                                goto after;   
                            }

                            var toAdd = -1 * minWidth * (s2.Len + 1);
                            areaAdded += toAdd;
                            Console.WriteLine($"Case 6 RRL-S1Less-SameDir Adds {toAdd}");
                            ill.Remove(s1N);
                            ill.Remove(s2N);
                            pre.Len += s2.Len;
                            s3.Len -= s1.Len;
                            illCurrent = postN;
                        } else {


                            PointL p1 = new PointL(), p2 = new PointL();
                            if (pre.Dir is Dir.U or Dir.D) {
                                p1.X = p2.X = pre.MaxX;
                                p1.Y = pre.StartY;
                                p2.Y = post.StartY;
                            } else  {
                                p1.Y = p2.Y = pre.MaxY;
                                p1.X = pre.StartX;
                                p2.X = post.StartX;
                            }

                            if (!CheckForNoCrossovers(ill, p1, p2)) {
                                Console.WriteLine("Skipping - crossovers #4");
                                illCurrent = i1;  
                                goto after;   
                            }

                            var toAdd = -1 * (minWidth * (s2.Len + 1) + pre.Len);
                            areaAdded += toAdd;
                            Console.WriteLine($"Case 7 RRL-S1Less-OppositeDir Adds {toAdd}");
                            ill.Remove(s1N);
                            ill.Remove(s2N);
                            pre.Dir = post.Dir;
                            pre.Len = s2.Len - pre.Len;
                            s3.Len -= s1.Len;
                            illCurrent = postN;
                        }
                    } else { // s3 is shorter
                        if (pre.Dir != post.Dir) {
                            PointL p1 = new PointL(), p2 = new PointL();
                            if (pre.Dir is Dir.U or Dir.D) {
                                p1.X = p2.X = post.MaxX;
                                p1.Y = pre.EndY;
                                p2.Y = post.StartY;
                            } else  {
                                p1.Y = p2.Y = post.MaxY;
                                p1.X = pre.EndX;
                                p2.X = post.StartX;
                            }

                            if (!CheckForNoCrossovers(ill, p1, p2)) {
                                Console.WriteLine("Skipping - crossovers #4");
                                illCurrent = i1;  
                                goto after;   
                            }

                            var toAdd = -1 * minWidth * (s2.Len + 1);
                            areaAdded += toAdd;
                            Console.WriteLine($"Case 8 RRL-S3Less-OppositeDir Adds {toAdd}");
                            ill.Remove(s2N);
                            ill.Remove(s3N);
                            post.Len += s2.Len;
                            s1.Len -= s3.Len;
                            illCurrent = postN;
                        } else {

                            PointL p1 = new PointL(), p2 = new PointL();
                            if (pre.Dir is Dir.U or Dir.D) {
                                p1.X = p2.X = post.MaxX;
                                p1.Y = pre.EndY;
                                p2.Y = post.StartY;
                            } else  {
                                p1.Y = p2.Y = post.MaxY;
                                p1.X = pre.EndX;
                                p2.X = post.StartX;
                            }

                            if (!CheckForNoCrossovers(ill, p1, p2)) {
                                Console.WriteLine("Skipping - crossovers #4");
                                illCurrent = i1;  
                                goto after;   
                            }

                            var toAdd = -1 * minWidth * (s2.Len + 1);
                            areaAdded += toAdd;
                            Console.WriteLine($"Case 9 RRL-S3Less-SameDir Adds {toAdd}");
                            ill.Remove(s2N);
                            ill.Remove(s3N);
                            post.Len += s2.Len;
                            s1.Len -= s3.Len;
                            illCurrent = postN;
                        }
                    }
                }

                Console.WriteLine("Now:");
                Render(ill);
after:
                pre.RenderChar = (char)0;
                s1.RenderChar = (char)0;
                s2.RenderChar = (char)0;
                s3.RenderChar = (char)0;
                post.RenderChar = (char)0;

            } else {
                illCurrent = illCurrent.Next;
            }
        }
    }

    var i0F = ill.First;
    var i1F = i0F?.Next;
    var i2F = i1F?.Next;
    var i3F = i2F?.Next;

    if (i0F.Value.Len != i2F.Value.Len || i1F.Value.Len != i3F.Value.Len) {
        throw new Exception("Didn't find a rectangle");
    }

    var area = (i0F.Value.Len + 1) * (i1F.Value.Len + 1);

    area -= areaAdded;

    Console.Out.WriteLine($"Area: {area}");
}


void CalcInstructionBounds(LinkedList<Instruction> ill) {
    var currentX = 0L;
    var currentY = 0L;
    var illNode = ill.First;

    while (illNode != null) {
        var instruction = illNode.Value;

        if (instruction.Dir == Dir.D) {
            var nextY = currentY + instruction.Len;
            instruction.MinX = currentX;
            instruction.MaxX = currentX;
            instruction.MinY = currentY;
            instruction.MaxY = nextY;

            instruction.StartX = currentX;
            instruction.StartY = currentY;
            instruction.EndX = currentX;
            instruction.EndY = nextY;

            currentY = nextY;
        } else if (instruction.Dir == Dir.U) {
            var nextY = currentY - instruction.Len;
            instruction.MinX = instruction.MaxX = currentX;
            instruction.MinY = nextY;
            instruction.MaxY = currentY;

            instruction.StartX = currentX;
            instruction.StartY = currentY;
            instruction.EndX = currentX;
            instruction.EndY = nextY;

            currentY = nextY;

        } else if (instruction.Dir == Dir.R) {
            var nextX = currentX + instruction.Len;
            instruction.MinY = instruction.MaxY = currentY;
            instruction.MinX = currentX;
            instruction.MaxX = nextX;

            instruction.StartX = currentX;
            instruction.StartY = currentY;
            instruction.EndX = nextX;
            instruction.EndY = currentY;

            currentX = nextX;
        } else if (instruction.Dir == Dir.L) {
            var nextX = currentX - instruction.Len;
            instruction.MinY = instruction.MaxY = currentY;
            instruction.MinX = nextX;
            instruction.MaxX = currentX;

            instruction.StartX = currentX;
            instruction.StartY = currentY;
            instruction.EndX = nextX;
            instruction.EndY = currentY;

            currentX = nextX;
        }

        illNode = illNode.Next;
    }

    if (currentX != 0 || currentY != 0) {
        throw new Exception("Invalid state in CalcInstructionBounds");
    }
}

bool CheckForNoCrossovers(LinkedList<Instruction> ill, PointL p1, PointL p2) {
    bool horizontal = p1.Y == p2.Y;
    bool vertical = !horizontal;
    var minX = Math.Min(p1.X, p2.X);
    var maxX = Math.Max(p1.X, p2.X);
    var minY = Math.Min(p1.Y, p2.Y);
    var maxY = Math.Max(p1.Y, p2.Y);

    var illNode = ill.First;
    while (illNode != null) {
        var instruction = illNode.Value;

        if (instruction.Dir is Dir.D or Dir.U && horizontal) {
            if (instruction.MinX > minX && instruction.MinX < maxX && instruction.MinY < minY && instruction.MaxY > minY) {
                return false;
            }
        } else if (instruction.Dir is Dir.R or Dir.L && vertical) {
            if (instruction.MinY > minY && instruction.MinY < maxY && instruction.MinX < minX && instruction.MaxX > minX) {
                return false;
            }
        }

        illNode = illNode.Next;
    }

    return true;
}

void Part2(string[] lines)
{


}



void Render(IEnumerable<Instruction> instructions, bool big = false) {

    if (RenderCount++ < RenderSkip) {
        return;
    }

    if (RenderCount > RenderThrow) {
        throw new Exception("Render Throw for debugging");
    }

    int boardWidth = 400;
    int boardHeight = 250;

    int originWidth = 110;
    int originHeight = 200;

    if (big) {
        boardWidth = 600;
        boardHeight = 600;
        originHeight = 300;
        originWidth = 300;
    }

    if (sample)
    {
        boardWidth = 30;
        boardHeight = 30;
        originHeight = 15;
        originWidth = 15;
    }
    var map = new string[boardHeight, boardWidth];
    long curRow = originHeight;
    long curCol = originWidth;
    try {
        
        
        foreach(var instruction in instructions) {

            var c = instruction.RenderChar == 0 ? "#" : $"{RED}{instruction.RenderChar}{NORMAL}";
            if (instruction.Dir == Dir.D) {
                for (int deltaR = 0; deltaR < instruction.Len; deltaR++) {
                    map[curRow + deltaR, curCol] = c;
                }
                curRow += instruction.Len;
            } else if (instruction.Dir == Dir.U) {
                for (int deltaR = 0; deltaR < instruction.Len; deltaR++) {
                    map[curRow - deltaR, curCol] = c;
                }
                curRow -= instruction.Len;
            } else if (instruction.Dir == Dir.R) {
                for (int deltaC = 0; deltaC < instruction.Len; deltaC++) {
                    map[curRow, curCol  + deltaC] = c;
                }
                curCol += instruction.Len;
            } else if (instruction.Dir == Dir.L) {
                for (int deltaC = 0; deltaC < instruction.Len; deltaC++) {
                    map[curRow, curCol - deltaC] = c;
                }
                curCol -= instruction.Len;
            }

        }
    } catch (Exception) {
        if (!big) {
            Render(instructions, true);
            return;
        }
        else {
            throw;
        }
    }

    StringBuilder sb = new();
    sb.AppendLine();
    for (int row = 0; row < boardHeight; row++) {
        for (int col = 0; col < boardWidth; col++) {

            bool origin = row == originHeight && col == originWidth;
            if (origin) {
                sb.Append(GREEN);
            }
            var ch = map[row,col];
            sb.Append(ch == null ? ' ' : ch );
            if (origin) {
                sb.Append(NORMAL);
            }
        }
        sb.AppendLine();
    }
    sb.AppendLine();
    sb.AppendLine("</Render>");

    Console.Write(sb.ToString());
    if (curCol != originWidth || curRow != originHeight) {
        throw new Exception("Invalid bump removal detected");
    }

}


IEnumerable<Turn> GetTurnList(List<Instruction> instructions) {
    return instructions.Pairwise((a,b) => GetTurn(a.Dir,b.Dir));
}



Turn GetTurn(Dir d1, Dir d2) {

    return (d1, d2) switch {
        (Dir.U, Dir.L) => Turn.L,
        (Dir.U, Dir.R) => Turn.R,
        (Dir.D, Dir.L) => Turn.R,
        (Dir.D, Dir.R) => Turn.L,
        (Dir.L, Dir.U) => Turn.R,
        (Dir.L, Dir.D) => Turn.L,
        (Dir.R, Dir.U) => Turn.L,
        (Dir.R, Dir.D) => Turn.R,
    };
 }


record Instruction {
    public Dir Dir;
    public long Len;
    public char RenderChar;

    public long MinX, MinY, MaxX, MaxY;
    public long StartX, StartY, EndX, EndY;
}

enum Dir {
    U,D,L,R
}

enum Turn {
    R, L
}

struct PointL {
    public long X;
    public long Y;
}
