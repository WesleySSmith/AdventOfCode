#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using MoreLinq;
//using MoreLinq;

bool sample = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
//Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

//Part1(lines);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(string[] lines)
{
    var line = lines[0];
    var lineInts = line.Select(x=> x - '0').ToArray();
    var totalSpace = lineInts.Sum();
    var disk = new int[totalSpace];
    var ptr = 0;
    for (int i = 0; i <= lineInts.Length / 2; i++) {
        var idNumber = i;
        var fileBlocks = lineInts[i*2];
        var spaceBlocks = i*2 + 1 < lineInts.Length ? lineInts[i*2+1] : 0;
        for (int j = 0; j < fileBlocks; j++) {
            disk[ptr++] = idNumber;
        }
        for (int j = 0; j < spaceBlocks; j++) {
            disk[ptr++] = -1;
        }
    }

    // for (int i = 0; i < disk.Length; i++) {
    //     Console.WriteLine($"{i.ToString().PadLeft(3)}: {disk[i]}");
    // }

    var leftMostFreeSpace = 0;
    var rightMostBlock = disk.Length - 1;
    while (true) {
        
        while (disk[leftMostFreeSpace] != -1) {
            leftMostFreeSpace++;
        }
        while (disk[rightMostBlock] == -1) {
            rightMostBlock--;
        }
        if (leftMostFreeSpace >= rightMostBlock) {
            break;
        }
        disk[leftMostFreeSpace] = disk[rightMostBlock];
        disk[rightMostBlock] = -1;
    }

    var checksum = disk.Aggregate<int, (int idx, long checksum)>((0, 0L),(agg, fileId) => {
        return (agg.idx + 1, agg.checksum + agg.idx*(fileId == -1 ? 0 : fileId ));
    }).checksum;

    Console.Out.WriteLine($"Part 1: {checksum}");
}


void Part2(string[] lines) {
    var line = lines[0];
    var lineInts = line.Select(x=> x - '0').ToArray();
    var totalSpace = lineInts.Sum();
    var disk = new int[totalSpace];
    var ptr = 0;
    for (int i = 0; i <= lineInts.Length / 2; i++) {
        var idNumber = i;
        var fileBlocks = lineInts[i*2];
        var spaceBlocks = i*2 + 1 < lineInts.Length ? lineInts[i*2+1] : 0;
        for (int j = 0; j < fileBlocks; j++) {
            disk[ptr++] = idNumber;
        }
        for (int j = 0; j < spaceBlocks; j++) {
            disk[ptr++] = -1;
        }
    }

    // for (int i = 0; i < disk.Length; i++) {
    //     Console.WriteLine($"{i.ToString().PadLeft(3)}: {disk[i]}");
    // }

    var rPos = disk.Length -1;
    for (int fileId = lineInts.Length / 2; fileId >= 0; fileId--) {
        while (disk[rPos] > fileId || disk[rPos] == -1) {
            rPos--;
        }
        var lPos = rPos-1;
        while (lPos > 0 && disk[lPos] == fileId) {
            lPos--;
        }
        lPos++;
        var fileSize = rPos - lPos + 1;

        var availableSlot = StartingIndex(disk, Enumerable.Repeat(-1, fileSize).ToArray());
        if (availableSlot != -1 && availableSlot < lPos) {
            for (var i = 0; i < fileSize; i++) {
                disk[availableSlot+i] = disk[lPos+i];
                disk[lPos+i] = -1;
            }
        }
        
        // Console.Out.WriteLine($"FileId: {fileId}");
        // for (int i = 0; i < disk.Length; i++) {
        //     Console.Write($"{(disk[i] >= 0 ? disk[i].ToString() : ".")}");
        // }
        // Console.Out.WriteLine();
    }

    var checksum = disk.Aggregate<int, (int idx, long checksum)>((0, 0L),(agg, fileId) => {
        return (agg.idx + 1, agg.checksum + agg.idx*(fileId == -1 ? 0 : fileId ));
    }).checksum;

    Console.Out.WriteLine($"Part 2: {checksum}");
}

static bool IsSubArrayEqual(int[] x, int[] y, int start) {
    for (int i = 0; i < y.Length; i++) {
        if (x[start++] != y[i]) return false;
    }
    return true;
}
static int StartingIndex(int[] x, int[] y) {
    int max = 1 + x.Length - y.Length;
    for(int i = 0 ; i < max ; i++) {
        if(IsSubArrayEqual(x,y,i)) return i;
    }
    return -1;
}

