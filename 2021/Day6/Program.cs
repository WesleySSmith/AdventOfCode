using MoreLinq;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Numerics;


string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
//Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");
var sw =Stopwatch.StartNew();

var fish = lines[0].Split(',').Select(int.Parse).ToArray();
//Part1(fish);
Part2(fish);

Console.Out.WriteLine($"Total time {sw.ElapsedMilliseconds}");


static void Part1(IEnumerable<int> fish) {
   
   var thisStep = new int[9];
   foreach (var f in fish) {
       thisStep[f]++;
   }

    for (int ii = 0; ii < 80; ii++) {
    var nextStep = (int[])thisStep.Clone();

    nextStep[8] = thisStep[0];
    nextStep[7] = thisStep[8];
    nextStep[6] = thisStep[7] + thisStep[0];
    nextStep[5] = thisStep[6];
    nextStep[4] = thisStep[5];
    nextStep[3] = thisStep[4];
    nextStep[2] = thisStep[3];
    nextStep[1] = thisStep[2];
    nextStep[0] = thisStep[1];

        thisStep = nextStep;
    }

    Console.Out.WriteLine($"Population: {thisStep.Sum()}");
}

static void Part2(IEnumerable<int> fish) {
    var thisStep = new BigInteger[9];
    foreach (var f in fish) {
        thisStep[f]++;
    }

    for (int ii = 0; ii < 256; ii++) {
        var temp = thisStep[0];
        thisStep[0] = thisStep[1];
        thisStep[1] = thisStep[2];
        thisStep[2] = thisStep[3];
        thisStep[3] = thisStep[4];
        thisStep[4] = thisStep[5];
        thisStep[5] = thisStep[6];
        thisStep[6] = thisStep[7] + temp;
        thisStep[7] = thisStep[8];
        thisStep[8] = temp;
    }

    Console.Out.WriteLine($"Population: {thisStep.Aggregate(BigInteger.Zero, (acc, v) => acc + v)}");
}









