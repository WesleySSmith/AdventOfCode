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
//using MoreLinq.Extensions;

string NORMAL      = Console.IsOutputRedirected ? "" : "\x1b[39m";
string RED         = Console.IsOutputRedirected ? "" : "\x1b[91m";
string GREEN       = Console.IsOutputRedirected ? "" : "\x1b[92m";


int RenderCount = 0;
int RenderSkip = 450;
int RenderThrow = RenderSkip + 5;

bool sample = false;

Console.WriteLine($"Testing {RED}red{NORMAL} and {GREEN}green{NORMAL}");
string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

var instructions = lines.Select(l => new Instruction { Dir = Enum.Parse<Dir>(l[0].ToString()), Len = int.Parse(l[2..].Split(' ')[0])}).ToList();

Render(instructions);
Part1(instructions);
//Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(List<Instruction> instructionsX)
{
    
    
    long areaAdded = 0;

    int mainLoopCount = 0;

    var ill = new LinkedList<Instruction>(instructionsX);
        
    while (ill.Count > 4) {

        Console.WriteLine($"Looper {mainLoopCount}");

        if (mainLoopCount++ > 50) {
            throw new Exception("Too many main loops");
        }
        var illCurrent = ill.First;
        while (illCurrent != null) {

            var i0 = illCurrent;
            var i1 = i0?.Next;
            var i2 = i1?.Next;
            var i3 = i2?.Next;
            var i4 = i3?.Next;

            if (i4 == null) {
                break;
            }

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
                    var toAdd = (s2.Len - 1) * s1.Len;
                    Console.WriteLine($"Case RLL-1 Adds {toAdd}");
                    areaAdded += toAdd;
                    ill.Remove(s1N);
                    ill.Remove(s2N);
                    ill.Remove(s3N);
                    ill.Remove(postN);
                    pre.Len += s2.Len + post.Len;
                    illCurrent = preN.Next;
                } else {
                    var minWidth = Math.Min(s1.Len, s3.Len);
                    var toAdd = minWidth * (s2.Len - 1);
                    areaAdded += toAdd;

                    if (pre.Dir != post.Dir) {
                        Console.WriteLine("Skipping - too soon #2");
                        illCurrent = i1;  
                        goto after;      
                    }

                    if (s1.Len == minWidth) {
                        Console.WriteLine($"Case RLL-2 Adds {toAdd}");
                        ill.Remove(s1N);
                        ill.Remove(s2N);
                        pre.Len += s2.Len;
                        s3.Len -= s1.Len;
                        illCurrent = postN;
                    } else {
                        Console.WriteLine($"Case RLL-3 Adds {toAdd}");
                        ill.Remove(s2N);
                        ill.Remove(s3N);
                        post.Len += s2.Len;
                        s1.Len -= s3.Len;
                        illCurrent = postN;
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
                    var toAdd = -1 * s1.Len * (s2.Len + 1);
                    Console.WriteLine($"Case RRL-1 Adds {toAdd}");
                    areaAdded += toAdd;
                    ill.Remove(s1N);
                    ill.Remove(s2N);
                    ill.Remove(s3N);
                    ill.Remove(postN);
                    pre.Len += s2.Len + post.Len;
                    illCurrent = preN.Next;
                } else {
                    var minWidth = Math.Min(s1.Len, s3.Len);
                    var toAdd = -1 * minWidth * (s2.Len + 1);
                    areaAdded += toAdd;

                    if (s1.Len == minWidth) {
                        if (pre.Dir == post.Dir) {
                            Console.WriteLine($"Case RRL-2a Adds {toAdd}");
                            ill.Remove(s1N);
                            ill.Remove(s2N);
                            pre.Len += s2.Len;
                            s3.Len -= s1.Len;
                            illCurrent = postN;
                        } else {
                            Console.WriteLine($"Case RRL-2b Adds {toAdd}");
                            ill.Remove(s1N);
                            ill.Remove(s2N);
                            pre.Dir = post.Dir;
                            pre.Len = s2.Len - pre.Len;
                            s3.Len -= s1.Len;
                            illCurrent = postN;
                        }
                    } else {
                        if (pre.Dir != post.Dir) {
                            Console.WriteLine("Skipping - too soon #4");
                            illCurrent = i1;  
                            goto after;      
                        }
                        Console.WriteLine($"Case RRL-3 Adds {toAdd}");
                        ill.Remove(s2N);
                        ill.Remove(s3N);
                        post.Len += s2.Len;
                        s1.Len -= s3.Len;
                        illCurrent = postN;
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
        boardWidth = 15;
        boardHeight = 15;
        originHeight = 0;
        originWidth = 0;
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
}

enum Dir {
    U,D,L,R
}

enum Turn {
    R, L
}
