#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
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
    var instructions = lines[0];

    var nodeDict = lines[2..]
    .Select(l => (Key: l[0..3], Value: new Node {Left = l[7..10], Right = l[12..15]}))
    .ToDictionary(v => v.Key, v => v.Value);

    int instructionPtr = 0;
    string current = "AAA";
    int count = 1;
    while(true) {
        var node = nodeDict[current];
        current = instructions[instructionPtr] switch {'R' => node.Right, 'L' => node.Left};
        if (current == "ZZZ") {
            break;
        }
        instructionPtr = (instructionPtr + 1) % instructions.Length;
        count++;
    }

    Console.Out.WriteLine($"Count is {count}");

}

void Part2(string[] lines)
{
    var instructions = lines[0];

    var nodeDict = lines[2..]
    .Select(l => (Key: l[0..3], Value: new Node {Left = l[7..10], Right = l[12..15]}))
    .ToDictionary(v => v.Key, v => v.Value);
    
    var currents = nodeDict.Keys.Where(k => k.EndsWith('A')).ToArray();

    Console.WriteLine($"currents.Length {currents.Length}");
    var loopLengths = new List<long>();
    for(int ii = 0; ii < currents.Length; ii++) {
        int instructionPtr = 0;
        var current = currents[ii]; 
        int count = 1;
        while(true) {
            var node = nodeDict[current];
            current = instructions[instructionPtr] switch {'R' => node.Right, 'L' => node.Left};

            if (current.EndsWith('Z')) {
                Console.WriteLine($"Loop {ii} is at {current} at instruction {instructionPtr} at count {count}");
                loopLengths.Add(count);
                break;
            }

            instructionPtr = (instructionPtr + 1) % instructions.Length;
            count++;
        }
    }
    
    long lcm = LCM(loopLengths.ToArray());

    Console.Out.WriteLine($"LCM is {lcm}");
   
 }

 static long LCM(long[] numbers)
{
    return numbers.Aggregate(lcm);
}
static long lcm(long a, long b)
{
    return Math.Abs(a * b) / GCD(a, b);
}
static long GCD(long a, long b)
{
    return b == 0 ? a : GCD(b, a % b);
}

 record Node {
    public string Left;
    public string Right;
 }


 

