#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Text;
using MoreLinq;
using EmnumerableEx;
//using MoreLinq.Extensions;

string NORMAL      = Console.IsOutputRedirected ? "" : "\x1b[39m";
string RED         = Console.IsOutputRedirected ? "" : "\x1b[91m";
string GREEN       = Console.IsOutputRedirected ? "" : "\x1b[92m";

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


void Part1(List<Instruction> instructions)
{
    
    long areaAdded = 0;

    while (instructions.Count > 4) {

        

         var bump = 
             GetTurnList(instructions)
            .Tripplewise((c1, c2, c3) => c1.ToString() + c2.ToString() + c3.ToString())
            .Index(0)
            .FirstOrDefault(kvp => kvp.Key > 0 && kvp.Value is "RLL" or "RRL", new KeyValuePair<int, string>(-1, ""));
            
        if (bump.Key >= 0) {
            
            Console.Write($"Found {bump.Value} Bump at index {bump.Key} ");

            var bumpIndex = bump.Key;

           

            if (bump.Value == "RLL") {
                
                // Example: If bump = 1, then bump is instructions 2,3,4 and 1 and 5 are pre and post segments.
                
                var pre = instructions[bumpIndex];
                var s1 = instructions[bumpIndex+1];
                var s2 = instructions[bumpIndex+2];
                var s3 = instructions[bumpIndex+3];
                var post = instructions[bumpIndex+4];

                if (s1.Len == s3.Len) {
                    var toAdd = (s2.Len - 1) * s1.Len;
                    Console.WriteLine($"Case RLL-1 Adds {toAdd}");
                    areaAdded += toAdd;
                    instructions.RemoveRange(bumpIndex + 1, 4);
                    pre.Len += s2.Len + post.Len;
                } else {
                    var minWidth = Math.Min(s1.Len, s3.Len);
                    var toAdd = minWidth * (s2.Len - 1);
                    areaAdded += toAdd;

                    if (s1.Len == minWidth) {
                        Console.WriteLine($"Case RLL-2 Adds {toAdd}");
                        instructions.RemoveRange(bumpIndex + 1, 2);
                        pre.Len += s2.Len;
                        s3.Len -= s1.Len;
                    } else {
                        Console.WriteLine($"Case RLL-3 Adds {toAdd}");
                        instructions.RemoveRange(bumpIndex + 2, 2);
                        post.Len += s2.Len;
                        s1.Len -= s3.Len;
                    }
                }
            } else if (bump.Value == "RRL") {

                bumpIndex -= 1;
                var pre = instructions[bumpIndex];
                var s1 = instructions[bumpIndex+1];
                var s2 = instructions[bumpIndex+2];
                var s3 = instructions[bumpIndex+3];
                var post = instructions[bumpIndex+4];
                if (s1.Len == s3.Len) {
                    if (pre.Dir != post.Dir) {
                        throw new Exception("Unhandled 1");
                    }
                    var toAdd = -1 * s1.Len * (s2.Len + 1);
                    Console.WriteLine($"Case RRL-1 Adds {toAdd}");
                    areaAdded += toAdd;
                    instructions.RemoveRange(bumpIndex + 1, 4);
                    pre.Len += s2.Len + post.Len;
                } else {
                    var minWidth = Math.Min(s1.Len, s3.Len);
                    var toAdd = -1 * minWidth * (s2.Len + 1);
                    areaAdded += toAdd;

                    if (s1.Len == minWidth) {
                        Console.WriteLine($"Case RRL-2 Adds {toAdd}");
                        instructions.RemoveRange(bumpIndex + 1, 2);
                        pre.Len += s2.Len;
                        s3.Len -= s1.Len;
                    } else {
                        Console.WriteLine($"Case RRL-3 Adds {toAdd}");
                        instructions.RemoveRange(bumpIndex + 2, 2);
                        post.Len += s2.Len;
                        s1.Len -= s3.Len;
                    }
                }
            }

        } else {
            throw new Exception("Didn't find a bump");
        }

        Render(instructions);
/*
        bump = turns.IndexOf("LRR");
        if (bump >= 0) {

            // Example: If bump = 1, then bump is instructions 2,3,4 and 1 and 5 are pre and post segments.
            var pre = instructions[bump];
            var width1 = instructions[bump+1];
            var height = instructions[bump+2];
            var width2 = instructions[bump+3];
            var post = instructions[bump+4];

            if (width1 == width2) {
                areaAdded -= width1.Len * (height.Len - 1);
                instructions.RemoveRange(bump + 1, 4);
                pre.Len += height.Len + post.Len;
            } else {
                var minWidth = Math.Min(width1.Len, width2.Len);
                areaAdded -= minWidth * (height.Len -1);

                if (width1.Len == minWidth) {
                    instructions.RemoveRange(bump + 1, 2);
                    pre.Len += height.Len;
                    width2.Len -= (width2.Len - width1.Len);
                } else {
                    instructions.RemoveRange(bump + 2, 2);
                    post.Len += height.Len;
                    width1.Len -= (width1.Len - width2.Len);
                }
            }

            continue;
        }
*/


    }

    if (instructions[0].Len != instructions[2].Len || instructions[1].Len != instructions[3].Len) {
        throw new Exception("Didn't find a rectangle");
    }

    var area = (instructions[0].Len + 1) * (instructions[1].Len + 1);

    area -= areaAdded;



    
    Console.Out.WriteLine($"Area: {area}");


}

void Part2(string[] lines)
{


}

void Render(List<Instruction> instructions) {

    int boardWidth = 500;//400;
    int boardHeight = 500;// 250;

    int originWidth = 200;//110;
    int originHeight = 300;//200;
    var map = new char[boardHeight, boardWidth];

    long curRow = originHeight;
    long curCol = originWidth;
    char c = '#';
    foreach(var instruction in instructions) {
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

        //c++;
    }

    StringBuilder sb = new();
    for (int row = 0; row < boardHeight; row++) {
        for (int col = 0; col < boardWidth; col++) {

            bool origin = row == originHeight && col == originWidth;
            if (origin) {
                sb.Append(RED);
            }
            var ch = map[row,col];
            sb.Append(ch == 0 ? ' ' : ch );
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
}

enum Dir {
    U,D,L,R
}

enum Turn {
    R, L
}
