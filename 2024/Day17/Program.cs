#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using MoreLinq;

bool sample = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
//Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();


string NORMAL_BG      = Console.IsOutputRedirected ? "" : "\x1b[49m";
string RED_BG        = Console.IsOutputRedirected ? "" : "\x1b[41m";
string GREEN_BG      = Console.IsOutputRedirected ? "" : "\x1b[42m";
string YELLOW_BG      = Console.IsOutputRedirected ? "" : "\x1b[43m";
string X1_BG      = Console.IsOutputRedirected ? "" : "\x1b[44m";
string X2_BG      = Console.IsOutputRedirected ? "" : "\x1b[45m";
int Part2Iterations = 0;

var regA = int.Parse(lines[0][12..]);
var regB = int.Parse(lines[1][12..]);
var regC = int.Parse(lines[2][12..]);
var program = lines[4][9..].Split(',').Select(byte.Parse).ToArray();

var machine = new Machine() {A = regA, B = regB, C = regC, IP = 0, Program = program};


//Part1(machine);
Part2(machine);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");

void Part1(Machine m) {
    var output = Part1Impl(m);
    Console.Out.WriteLine(string.Join(",", output));
}

List<byte> Part1Impl(Machine m)
{
    List<byte> output = new();
    while (m.IP < m.Program.Length) {

        switch (m.Opcode) {
            case 0: // adv
                m.A = m.A / (int)Math.Pow(2, m.ComboOperand);
                m.IP +=2;
                break;
            case 1: // bxl
                m.B = m.B ^ m.Operand;
                m.IP +=2;
                break;
            case 2: // bst
                m.B = m.ComboOperand % 8;
                m.IP +=2;
                break;
            case 3: // jnz
                if (m.A != 0) {
                    m.IP = m.Operand;
                } else {
                    m.IP +=2;
                }
                break;
            case 4: // bxc
                m.B = m.B ^ m.C;
                m.IP +=2;
                break;
            case 5: // out
                output.Add((byte)(m.ComboOperand % 8));
                m.IP +=2;
                break;
            case 6: // bdv
                m.B = m.A / (int)Math.Pow(2, m.ComboOperand);
                m.IP +=2;
                break;
            case 7: // cdv
                m.C = m.A / (int)Math.Pow(2, m.ComboOperand);
                m.IP +=2;
                break;
        }
    }
    return output;
}

void Part2(Machine m) {
    Part2Impl(m, 0, 0);
}


void Part2Impl(Machine m, long a, int depth) {
    Part2Iterations++;
    if (depth == m.Program.Length) {
        var finalOut = Part1Impl(m with {A = a, IP = 0});

        Console.Out.WriteLine($"{a} => {string.Join(",", finalOut)}");
        Console.Out.WriteLine("Iterations: " + Part2Iterations);
        Environment.Exit(0);
    }

    for (int j = 0; j < 8; j++) {
        var newA = (a << 3) + j;
        var newM = m with {A = newA, IP = 0};
        var output = Part1Impl(newM);
        if (output[0] == m.Program[m.Program.Length -1 - depth]) {
            Part2Impl(m, newA, depth+1);
        }
    }
}

void Part2x(Machine m) {

    List<byte> output = new();
    for (long aa = 0; aa < 8; aa++) {
        //var a = 202_341_106_335_503;
        //var a = (Convert.ToInt64("572235373775", 8) << 3) + aa;
        var a = (Convert.ToInt64("532", 8) << 3) + aa;
        //var a = ((long)aa << 22) + 3_161_103;
        //if (a % 1000 == 0) {
            //Console.Out.WriteLine(a);
        //}
        m.A = a;
        m.B = 0;
        m.C = 0;
        m.IP = 0;
        output.Clear();

        while (m.IP < m.Program.Length) {

                switch (m.Opcode) {
                    case 0: // adv
                        var denom = (long)Math.Pow(2, m.ComboOperand);
                        if (denom == 0) {
                            goto nextA;
                        }
                        m.A = m.A / denom;
                        m.IP +=2;
                        break;
                    case 1: // bxl
                        m.B = m.B ^ m.Operand;
                        m.IP +=2;
                        break;
                    case 2: // bst
                        m.B = m.ComboOperand % 8;
                        m.IP +=2;
                        break;
                    case 3: // jnz
                        if (m.A != 0) {
                            m.IP = m.Operand;
                        } else {
                            m.IP +=2;
                        }
                        break;
                    case 4: // bxc
                        m.B = m.B ^ m.C;
                        m.IP +=2;
                        break;
                    case 5: // out
                        output.Add((byte)(m.ComboOperand % 8));
                        //if (!m.Program.StartsWith(output)) {
                          //  goto nextA;
                        //} else {
                            //Console.Out.WriteLine($"Using a of {a}, output matches so far: {string.Join(',', output)}");
                        //}
                        m.IP +=2;
                        break;
                    case 6: // bdv
                        denom = (long)Math.Pow(2, m.ComboOperand);
                        if (denom == 0) {
                            goto nextA;
                        }
                        m.B = m.A / denom;
                        m.IP +=2;
                        break;
                    case 7: // cdv
                        denom = (long)Math.Pow(2, m.ComboOperand);
                        if (denom == 0) {
                            goto nextA;
                        }
                        m.C = m.A / denom;
                        m.IP +=2;
                        break;
                }
            }

            //if (output.Count == m.Program.Length && output.SequenceEqual(m.Program)) {
            //    Console.Out.WriteLine($"FOUND A: {a}");
            //    break;
            //}
            nextA:

            var numMatches = output.Zip(m.Program).TakeWhile(e => e.First == e.Second).Count();
            Console.Out.WriteLine($"Using a of {a} {Convert.ToString(a, 8).PadLeft(8, '0')}, output matches so far: {string.Join(',', output)}");
    }
    Console.Out.WriteLine($"HALT");
}

record Machine {
    public long A;
    public long B;
    public long C;
    public long IP;
    public byte[] Program;

    public long Opcode => Program[IP];
    public long Operand => Program[IP+1];

    public long ComboOperand =>
        Operand switch {
            0 => 0,
            1 => 1,
            2 => 2,
            3 => 3,
            4 => A,
            5 => B,
            6 => C,
            7 => throw new Exception("Ilegal operand"),
            _ => throw new Exception("Ilegal operand")
        };
};

    


