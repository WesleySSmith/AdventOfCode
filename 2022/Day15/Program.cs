#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
//using MoreLinq;

bool sample = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

var sensorDatas = lines.Select(SensorData.Parse).ToArray();

Part1(sensorDatas);
Part2(sensorDatas);

Console.Out.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");


void Part1(SensorData[] sensorDatas) {

   int row = sample ? 10 : 2_000_000;

    List<Range> ranges = FindOverlaps(row, sensorDatas);
    var mergedRanges = MergeRanges(ranges);

    var rangeContainsCount = mergedRanges.Aggregate(0, (count, r) => r.Right - r.Left + 1);
    var beaconsInLine = sensorDatas.DistinctBy(data => data.Beacon).Count(sensorData => sensorData.Beacon.Y == row);
    var beaconsNotPresent = rangeContainsCount - beaconsInLine;

    Console.WriteLine($"Part 1 Count: {beaconsNotPresent}");
}


void Part2(SensorData[] sensorDatas) {

    for (int row = 0; row < (sample ? 20 : 4_000_000); row++) {

        List<Range> ranges = FindOverlaps(row, sensorDatas);
    
        var mergedRanges = MergeRanges(ranges);

        if (mergedRanges.Count == 2) {
            var gapY = row;
            var gapX = mergedRanges[0].Right + 1;
            var frequency = 4_000_000L * gapX + gapY;

            Console.WriteLine($"Part 2 Frequency: {frequency}");
            return;
        }
    }
}

List<Range> FindOverlaps(int row, SensorData[] sensorDatas) {
    List<Range> ranges = new List<Range>();
    foreach(var sensorData in sensorDatas) {

        var overlap = sensorData.Distance - Math.Abs(sensorData.Sensor.Y - row);
        if (overlap > 0) {
            ranges.Add(new Range {
                Left = sensorData.Sensor.X - overlap,
                Right = sensorData.Sensor.X + overlap});
        }
    }
    return ranges;
}

List<Range> MergeRanges(List<Range> ranges) {
        return 
            ranges
            .OrderBy(r => r.Left)
            .Aggregate(new List<Range>(),(rList, r) => {
                if (rList.Count == 0) {
                    rList.Add(r);
                } else {
                    var lastRange = rList.Last();
                    if (r.Left <= lastRange.Right) {
                        if (r.Right > lastRange.Right) {
                            // extend range
                            lastRange.Right = r.Right;
                        }
                    } else {
                        // new range
                        rList.Add(r);
                    }
                }
                return rList;
            });
    }

public record Point(int X, int Y) {
    public static Point Parse(string s) {
        var coords = s.Split(",");
        return new Point(int.Parse(coords[0].Split("=")[1]), int.Parse(coords[1].Split("=")[1]));
    }
}

public record Range() {
    public int Left {get; set;}
    public int Right {get; set;}
}

public record SensorData(Point Sensor, Point Beacon, int Distance) {

    public static SensorData Parse(string l) {
        var parts = l.Split(":").Select(Point.Parse).ToArray();
        return new SensorData(
            parts[0],
            parts[1],
            Math.Abs(parts[0].X - parts[1].X) + Math.Abs(parts[0].Y - parts[1].Y));
    }
}
