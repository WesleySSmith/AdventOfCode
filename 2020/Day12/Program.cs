using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

var instructions = lines.Select(l => 
    new Instruction {
        Action = (l[0])switch {
            'N' => Action.N,
            'S' => Action.S,
            'E' => Action.E,
            'W' => Action.W,
            'L' => Action.L,
            'R' => Action.R,
            
            'F' => Action.F,
            _ => throw new Exception("Ack!")
        },
        Value = int.Parse(l[1..])
        });
/*
(int x, int y) shipPos = (0, 0);
Direction direction = Direction.E;

foreach (var instruction in instructions) {
    switch (instruction.Action) {
        case Action.N:
            UpdatePosition(Direction.N, instruction.Value);
            break;
        case Action.S:
            UpdatePosition(Direction.S, instruction.Value);
            break;
        case Action.E:
            UpdatePosition(Direction.E, instruction.Value);
            break;
        case Action.W:
            UpdatePosition(Direction.W, instruction.Value);
            break;
        case Action.L:
            UpdateDirection(instruction.Value);
            break;
        case Action.R:
            UpdateDirection(-instruction.Value);
            break;
        case Action.F:
            UpdatePosition(direction, instruction.Value);
            break;
    }
    Console.Out.WriteLine($"Location: {shipPos.x},{shipPos.y}, Direction: {direction}"); 
}

Console.Out.WriteLine($"Location: {shipPos.x},{shipPos.y}  Distance: {Math.Abs(shipPos.x)+ Math.Abs(shipPos.y)}");

void UpdateDirection(int degrees) {
    direction = (Direction)(((byte)direction + (degrees / 90) + 4) % 4 );
}

void UpdatePosition(Direction direction, int value) {
    switch (direction) {
        case Direction.N:
            shipPos.y += value;
            break;
        case Direction.S:
            shipPos.y -= value;
            break;
        case Direction.E:
            shipPos.x += value;
            break;
        case Direction.W:
            shipPos.x -= value;
            break;
    }
}

*/

(int x, int y) shipPos = (0, 0);
(int x, int y) waypointOffset = (10, 1);

foreach (var instruction in instructions) {
    switch (instruction.Action) {
        case Action.N:
            UpdateWaypointPosition(Direction.N, instruction.Value);
            break;
        case Action.S:
            UpdateWaypointPosition(Direction.S, instruction.Value);
            break;
        case Action.E:
            UpdateWaypointPosition(Direction.E, instruction.Value);
            break;
        case Action.W:
            UpdateWaypointPosition(Direction.W, instruction.Value);
            break;
        case Action.L:
            RotateWaypoint(instruction.Value);
            break;
        case Action.R:
            RotateWaypoint(-instruction.Value);
            break;
        case Action.F:
            MoveShip(instruction.Value);
            break;
    }
    Console.Out.WriteLine($"Location: {shipPos.x},{shipPos.y}, Waypoint Offset: {waypointOffset.x},{waypointOffset.y}"); 
}

Console.Out.WriteLine($"Location: {shipPos.x},{shipPos.y}  Distance: {Math.Abs(shipPos.x)+ Math.Abs(shipPos.y)}");


void UpdateWaypointPosition(Direction direction, int value) {
    switch (direction) {
        case Direction.N:
            waypointOffset.y += value;
            break;
        case Direction.S:
            waypointOffset.y -= value;
            break;
        case Direction.E:
            waypointOffset.x += value;
            break;
        case Direction.W:
            waypointOffset.x -= value;
            break;
    }
}

void RotateWaypoint(int degrees) {
    if (degrees == 90 || degrees == -270)
        waypointOffset = (-waypointOffset.y, waypointOffset.x);
    else if (degrees == 180 || degrees == -180)
        waypointOffset = (-waypointOffset.x, -waypointOffset.y);
    else if (degrees == -90 || degrees == 270)
        waypointOffset = (waypointOffset.y, -waypointOffset.x);
    else 
        throw new ArgumentException($"What? {degrees}");
}

void MoveShip(int value) {
    shipPos = (shipPos.x + (waypointOffset.x * value), shipPos.y + (waypointOffset.y * value));
}

enum Direction : byte {
    E,
    N,
    W,
    S
}

enum Action {
    N,
    S,
    E,
    W,
    L,
    R,
    F
}
readonly struct Instruction {
    public Action Action {get; init;}
    public int Value {get; init;}

}


       
