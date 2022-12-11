using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;


List<Direction> dirList = new List<Direction>();

 for (short w = -1; w <= 1; w++) {
    for (short x = -1; x <= 1; x++) {
        for (short y = -1; y <= 1; y++) {
            for (short z = -1; z <= 1; z++) {
                if (!(w == 0 && x == 0 && y == 0 && z == 0)) {
                    dirList.Add(new Direction(w, x, y, z));
                }
            }
        }
    }
 }
var directions = dirList.ToArray();


//string[] lines = File.ReadAllLines("input.txt"); 
string[] lines = File.ReadAllLines("sample.txt"); 
//Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

var offset = 10;
var extent = lines.Length;
var paddedExtent = lines.Length + 2 * offset;

var state = new bool[paddedExtent, paddedExtent, paddedExtent, paddedExtent];
var nextState = new bool[paddedExtent, paddedExtent, paddedExtent, paddedExtent];

for (int row = 0; row < extent; row++) {
    var line = lines[row];
    for (var col = 0; col < extent; col++) {
        state[row + offset, col + offset, 0 + offset, 0 + offset] = line[col] == '#';
    }
}
//PrintState(state);
Console.Out.WriteLine($"Total alive: {state.Cast<bool>().Where(s => s).Count()}");
for (var cycle = 0; cycle < 6; cycle++) {
    ComputeNextGen1(state, nextState);
    var tmp = state;
    state = nextState;
    nextState = tmp;

    //PrintState(state);
    Console.Out.WriteLine($"[after {cycle}] Total alive: {state.Cast<bool>().Where(s => s).Count()}");
}



Console.Out.WriteLine($"Total alive: {state.Cast<bool>().Where(s => s).Count()}");


/* void PrintState(bool[,,,] stateToPrint) {
    Console.Out.WriteLine("================");
    for (int level = 0; level < paddedExtent; level++) {
        Console.Out.WriteLine($"----- {level - offset} -----");
        StringBuilder sb = new StringBuilder(paddedExtent);
        for (int row = 0; row < paddedExtent; row++) {
            for (var col = 0; col < paddedExtent; col++) {
                sb.Append(stateToPrint[row, col, level] ? '#' : '.');
            }
            Console.Out.WriteLine(sb.ToString());
            sb.Clear();
        }
        Console.Out.WriteLine("-----");
    }
    Console.Out.WriteLine("================");
    Console.Out.WriteLine();
} */

void ComputeNextGen1(bool[,,,] gen1, bool[,,,] gen2) {
    for (int w = 1; w < paddedExtent - 1; w++) {
        for (int x = 1; x < paddedExtent - 1; x++) {
            for (var y = 1; y < paddedExtent - 1; y++) {
                 for (var z = 1; z < paddedExtent - 1; z++) {

                    bool currentState = gen1[w, x, y, z];

                    int occupiedAdj = 0;
                    foreach(var direction in directions) {
                        var w2 = w + direction.W;
                        var x2 = x + direction.X;
                        var y2 = y + direction.Y;
                        var z2 = z + direction.Z;
                        var adjState = gen1[w2, x2, y2, z2];
                        if (adjState) {
                            occupiedAdj++;
                        }
                    }

                    if (currentState) {
                        // currenty active
                        gen2[w, x, y, z] = occupiedAdj == 2 || occupiedAdj == 3;
                    } else {
                        // current inactive
                        gen2[w, x, y, z] = occupiedAdj == 3;
                    }
                }
            }
        }
    }
}

public struct Direction {
    public Direction(short x, short y, short z, short w) {
        this.W = w;
        this.Y = x;
        this.X = y;
        this.Z = z;
    }
    public short X;
    public short Y;
    public short Z;
    public short W;
}
