#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Data;
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
    
    var sum = lines[0].Split(',').Select(i => hashString(i)).Sum();
    Console.Out.WriteLine($"Sum is {sum}.");
}

void Part2(string[] lines)
{
    var boxes = new Box[256];
    for(int ii = 0; ii < 256; ii++) {
        boxes[ii] = new Box();
    }
    var instuctions = lines[0].Split(',');
    foreach (var instuction in instuctions) {
        if (instuction.EndsWith('-')) {
            var label = instuction[0..^1];
            var boxId = hashString(label);
            var box = boxes[boxId];
            box.Lenses = box.Lenses.Where(l => l.Label != label).ToList();
        } else {
            var parts = instuction.Split('=');
            var label = parts[0];
            var focalLength = int.Parse(parts[1]);
            var boxId = hashString(label);
            var box = boxes[boxId];
            var existing = box.Lenses.SingleOrDefault(l => l.Label == label);
            if (existing != null) {
                existing.FocalLength = focalLength;
            }
            else {
                box.Lenses.Add(new Lens {Label = label, FocalLength = focalLength});
            }
        }
    }


    var power = boxes.Aggregate(
        (Total: 0, BoxIndex: 1),
        (acc, box) => 
            (
                acc.Total + box.Lenses.Aggregate(
                    (LensTotal: 0, LensIndex: 1),
                    (lensAcc, lens) => 
                        ( 
                            lensAcc.LensTotal + (acc.BoxIndex * lensAcc.LensIndex * lens.FocalLength),
                            lensAcc.LensIndex + 1)
                ).LensTotal,

                acc.BoxIndex + 1
            )
        ).Total;

        Console.Out.WriteLine($"Power is {power}.");

}

int hashString(string s) {
    return s.Aggregate(0, (acc, c) =>  (acc + c) * 17 % 256);
}

public class Box {
    public List<Lens> Lenses = new();
}

public record Lens {
    public string Label;
    public int FocalLength;
}