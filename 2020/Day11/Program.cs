using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

var directions = new [] {
    new Direction(-1, +0),
    new Direction(-1, +1),
    new Direction(-1, -1),
    new Direction(+1, +0),
    new Direction(+1, +1),
    new Direction(+1, -1),
    new Direction(+0, -1),
    new Direction(+0, +1),
};

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
//Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

var rows = lines.Length;
var cols = lines.First().Length;

var state = new State[rows, cols];
var nextState = new State[rows, cols];

for (int row = 0; row < rows; row++) {
    var line = lines[row];
    for (var col = 0; col < cols; col++) {
        state[row, col] = line[col] switch {'.' => State.Floor, 'L' => State.Empty, '#' => State.Occupied, _ => throw new Exception("Invalid char") };
    }
}
//PrintState(state);

var round = 0;
while (true) {
    round++;
    ComputeNextGen2(state, nextState);
    //PrintState(nextState);
    if (state.Cast<State>().SequenceEqual(nextState.Cast<State>())) {
        break;
    }
    var tmp = state;
    state = nextState;
    nextState = tmp;
}
var numOccupied = state.Cast<State>().Count(s => s == State.Occupied);
Console.Out.Write($"Stabilized at: {round} with {numOccupied} occupied");



void ComputeNextGen1(State[,] gen1, State[,] gen2) {
    var rows = state.GetLength(0);
    var cols = state.GetLength(1);
    for (int row = 0; row < rows; row++) {
        for (var col = 0; col < cols; col++) {

            State currentState = gen1[row, col];
            if (currentState == State.Floor) {
                continue;
            }

            int occupiedAdj = 0;
            foreach(var direction in directions) {
                var rowX = row;
                var colX = col;
                rowX += direction.RowOffset;
                colX += direction.ColOffset;
                if (!InBounds(rowX, colX)) {
                    continue;
                }
                var adjState = gen1[rowX, colX];
                if (adjState == State.Occupied) {
                    occupiedAdj++;
                }
            }

            if (currentState == State.Empty && occupiedAdj == 0) {
                gen2[row, col] = State.Occupied;
            } else if (currentState == State.Occupied && occupiedAdj >= 4) {
                gen2[row, col] = State.Empty;
            } else {
              gen2[row, col] = currentState;
            }
        }
    }
}


bool InBounds(int row, int col) {
    return row >= 0 && row < rows && col >= 0 && col < cols;
}
void ComputeNextGen2(State[,] gen1, State[,] gen2) {


    for (int row = 0; row < rows; row++) {
        for (var col = 0; col < cols; col++) {

            State currentState = gen1[row, col];
            if (currentState == State.Floor) {
                continue;
            }

            int occupiedAdj = 0;
            foreach(var direction in directions) {
                var rowX = row;
                var colX = col;
                while (true) {
                    rowX += direction.RowOffset;
                    colX += direction.ColOffset;
                    if (!InBounds(rowX, colX)) {
                        break;
                    }
                    var state = gen1[rowX, colX];

                    if (state == State.Floor) {
                        continue;
                    } else if (state == State.Empty) {
                        break;
                    } else {
                        occupiedAdj++;
                        break;
                    }
                }
            }

            if (currentState == State.Empty && occupiedAdj == 0) {
                gen2[row, col] = State.Occupied;
            } else if (currentState == State.Occupied && occupiedAdj >= 5) {
                gen2[row, col] = State.Empty;
            } else {
              gen2[row, col] = currentState;
            }
        }
    }
}

void PrintState(State[,] stateToPrint) {
    Console.Out.WriteLine("-----");
    var rows = stateToPrint.GetLength(0);
    var cols = stateToPrint.GetLength(1);
    StringBuilder sb = new StringBuilder(cols);
    for (int row = 0; row < rows; row++) {
        for (var col = 0; col < cols; col++) {
            sb.Append(stateToPrint[row, col] switch {
                State.Floor => '.', 
                State.Empty => 'L', 
                State.Occupied => '#',
                 _ => '?'});
        }
        Console.Out.WriteLine(sb.ToString());
        sb.Clear();
    }
    Console.Out.WriteLine("-----");
    Console.Out.WriteLine();
}

public enum State {
    Floor,
    Occupied,
    Empty
}

public struct Direction {
    public Direction(short RowOffset, short ColOffset) {
        this.RowOffset = RowOffset;
        this.ColOffset = ColOffset;
    }
    public short ColOffset;
    public short RowOffset;
}