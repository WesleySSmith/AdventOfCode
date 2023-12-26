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
   
    var seeds = ParseSeeds(lines[0]);
    var maps = new List<Map>(7);
    var index = 2;
    for (int ii = 0; ii < 7; ii++) {
        var map = Map.FromStrings(lines, ref index);
        maps.Add(map);
    }
    
    var vals = seeds.Select(seed => {
        long value = seed;
        foreach (Map map in maps) {
            value = map.MapSourceToDest(value);
        }
        return value;
    })
    .ToList();
    var minVal = vals.Min();
    
    Console.Out.WriteLine($"Lowest location is {minVal}");

}

void Part2(string[] lines)
{
    
    var seedRanges = ParseSeeds(lines[0]).Batch(2).Select(pair => new SeedRange {RangeStart = pair.ElementAt(0), RangeLen = pair.ElementAt(1)});
   

   var maps = new List<Map>(7);
   var reversedMaps = new List<Map>(7);
    var index = 2;
    for (int ii = 0; ii < 7; ii++) {
        var map = Map.FromStrings(lines, ref index);
        maps.Add(map);
        reversedMaps.Add(map);
    }
    
    reversedMaps.Reverse();
    for (long ii = 0; ii < long.MaxValue; ii++) {
        //Console.WriteLine($"Reversing from {ii}");
        long value = ii;
         foreach (Map map in reversedMaps) {
            value = map.MapDestToSource(value);
            //Console.WriteLine(value);
        }

// Validation
/*
        Console.WriteLine($"Forward from {value}");
        long valueX = value;
        foreach (Map map in maps) {
            valueX = map.MapSourceToDest(valueX);
            Console.WriteLine(valueX);

        }

        if (valueX != ii) {
            throw new Exception($"{valueX} != {ii}");
        }
*/
        if (seedRanges.Any(sr => value >= sr.RangeStart && value < sr.RangeStart + sr.RangeLen)) {
            Console.Out.WriteLine($"Best location is {ii}");
            break;
        }
    }

}

List<long> ParseSeeds(string line) {
    return line[7..].Split(" ").Select(long.Parse).ToList();
}
public record Map {
    public string Name;
    public List<MapSegment> Segments;

    public long MapSourceToDest(long source) {
        foreach (var segment in Segments) {
            var dest = segment.MapSourceToDest(source);
            if (dest.HasValue) {
                return dest.Value;
            }
        }
        return source;
    }

    public long MapDestToSource(long dest) {
        foreach (var segment in Segments) {
            var source = segment.MapDestToSource(dest);
            if (source.HasValue) {
                return source.Value;
            }
        }
        return dest;
    }

    public static Map FromStrings(string[] lines, ref int index) {
        var name = lines[index];
        index++;
        var segments = new List<MapSegment>();
        while (index < lines.Length && !lines[index].Contains(":")) {
            var line = lines[index];

            if (line.Length > 0) {
                segments.Add(MapSegment.FromString(line));
            }
            index++;
        }
        return new Map {
            Name = name,
            Segments = segments
        };
    }
}


public record MapSegment {
    public long DestRangeStart;
    public long SourceRangeStart;
    public long RangeLen;

    public long? MapSourceToDest(long source) {
        if (source >= SourceRangeStart && source < SourceRangeStart + RangeLen) {
            return (source - SourceRangeStart) + DestRangeStart;
        }
        return null;
    }


    public long? MapDestToSource(long dest) {
        if (dest >= DestRangeStart && dest < DestRangeStart + RangeLen) {
            return (dest - DestRangeStart) + SourceRangeStart;
        }
        return null;
    }

    public static MapSegment FromString(string s) {
        var nums = s.Split(" ").Select(long.Parse).ToArray();
        return new MapSegment {
            DestRangeStart = nums[0],
            SourceRangeStart = nums[1],
            RangeLen = nums[2]
        };
    }
}

public record SeedRange {
    public long RangeStart;
    public long RangeLen;
}
