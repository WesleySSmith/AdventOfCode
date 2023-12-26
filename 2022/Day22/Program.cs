#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
//using MoreLinq;

bool sample = false;
bool debug = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt"); 
if (debug) Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

var map = lines[0..^2];
var instructions = lines[(lines.Length-1)];

Stopwatch sw;

sw = Stopwatch.StartNew();
//Part1(map, instructions);
Part2(map, instructions);

Console.Out.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");

void Part1(string[] mapDescription, string instructions) {
   

   var maxWidth = mapDescription.MaxBy(m => m.Length).Length;
   var maxHeight = mapDescription.Length;

   Cell[,] map = new Cell[maxHeight, maxWidth];
   
    for(var row = 0; row < maxHeight; row++) {
        var line = mapDescription[row];
        for (var col = 0; col < maxWidth; col++) {
           
            var c = col < line.Length ? line[col] : ' ';
            if (c == '.') {
                map[row, col] = new Cell {Type = CellType.Open, Col = col, Row = row};
            } else if (c == '#'){
                map[row, col] = new Cell {Type = CellType.Wall, Col = col, Row = row};
            }
        }
    }

    for(var row = 0; row < maxHeight; row++) {
        for (var col = 0; col < maxWidth; col++) {
            Cell cell = map[row, col];
            if (cell != null) {
                if (cell.Type == CellType.Open) {

                    {
                        var col2 = col;
                        do {
                            col2--;
                            if (col2 < 0) {
                                col2 = maxWidth - 1;
                            }
                        } while (map[row, col2] == null);

                        cell.Left = map[row, col2];
                    }

                    {
                        var col2 = col;
                        do {
                            col2++;
                            if (col2 > maxWidth - 1) {
                                col2 = 0;
                            }
                        } while (map[row, col2] == null);

                        cell.Right = map[row, col2];
                    }

                    {
                        var row2 = row;
                        do {
                            row2--;
                            if (row2 < 0) {
                                row2 = maxHeight -1;
                            }
                        } while (map[row2, col] == null);

                        cell.Up = map[row2, col];

                    }

                     {
                        var row2 = row;
                        do {
                            row2++;
                            if (row2 > maxHeight - 1) {
                                row2 = 0;
                            }
                        } while (map[row2, col] == null);

                        cell.Down = map[row2, col];

                    }

                }
            }
        }
    }


    Regex regex = new Regex("R|L|[0-9]+");
    MatchCollection matches = regex.Matches(instructions);

    var path = matches.Select(m => 
    {
        var isDirection = m.Value == "R" || m.Value == "L";
        if (isDirection) {
            return new Segment {Right = m.Value == "R"};
        }
        return new Segment {Steps = int.Parse(m.Value)};
    }).ToArray();

    
    Cell position = null;
    Direction direction = Direction.Right;
    {
        for (int ii = 0; ii < maxWidth; ii++) {
            if (map[0, ii]?.Type == CellType.Open) {
                position = map[0, ii];
                break;
            }
        }
    }
   
   foreach (Segment segment in path) {

        if (segment.Steps.HasValue) {
            for (int ii = 0; ii < segment.Steps; ii++) {

                var nextPosition = direction switch {
                    Direction.Up => position.Up,
                    Direction.Down => position.Down,
                    Direction.Right => position.Right,
                    Direction.Left => position.Left
                };

                if (nextPosition.Type == CellType.Wall) {
                    break;
                }
                position = nextPosition;
            }
        } else {
            direction = segment.Right switch {
                    true => direction switch {
                        Direction.Down => Direction.Left,
                        Direction.Left => Direction.Up,
                        Direction.Up => Direction.Right,
                        Direction.Right => Direction.Down
                    },
                    false => direction switch {
                        Direction.Down => Direction.Right,
                        Direction.Right => Direction.Up,
                        Direction.Up => Direction.Left,
                        Direction.Left => Direction.Down
                    }
            };
        }
   }


    var password = (1000 * (position.Row + 1)) + (4 * (position.Col + 1)) + direction switch {
        Direction.Right => 0,
        Direction.Down => 1,
        Direction.Left => 2,
        Direction.Up => 3
    };

    Console.WriteLine($"Part 1 Password: {password}");
}



void Part2(string[] mapDescription, string instructions) {
   

    Face[] faces;
    
    if (sample) {
        faces = new[] {
            null, /* TO make array indexes match */
            new Face {Name=1, GlobalTop = 0, GlobalBottom= 3, GlobalLeft = 8, GlobalRight = 11, Moves = new FaceMove[] {
                new FaceMove{Source = 1, Dest = 1},
                new FaceMove{Source = 1, Dest = 2, Valid = true, SourceDirection = Direction.Up, DestDirection = Direction.Down},
                new FaceMove{Source = 1, Dest = 3, Valid = true, SourceDirection = Direction.Left, DestDirection = Direction.Down},
                new FaceMove{Source = 1, Dest = 4, Valid = true, SourceDirection = Direction.Down, DestDirection = Direction.Down},
                new FaceMove{Source = 1, Dest = 5},
                new FaceMove{Source = 1, Dest = 6, Valid = true, SourceDirection = Direction.Right, DestDirection = Direction.Left}
            }},
            new Face {Name=2, GlobalTop = 4, GlobalBottom= 7, GlobalLeft = 0, GlobalRight = 3, Moves = new FaceMove[] {
                new FaceMove{Source = 2, Dest = 1, Valid = true, SourceDirection = Direction.Up, DestDirection = Direction.Down},
                new FaceMove{Source = 2, Dest = 2},
                new FaceMove{Source = 2, Dest = 3, Valid = true, SourceDirection = Direction.Right, DestDirection = Direction.Right},
                new FaceMove{Source = 2, Dest = 4},
                new FaceMove{Source = 2, Dest = 5, Valid = true, SourceDirection = Direction.Down, DestDirection = Direction.Up},
                new FaceMove{Source = 2, Dest = 6, Valid = true, SourceDirection = Direction.Left, DestDirection = Direction.Up}
            }},
            new Face {Name=3, GlobalTop = 4, GlobalBottom= 7, GlobalLeft = 4, GlobalRight = 7, Moves = new FaceMove[] {
                new FaceMove{Source = 3, Dest = 1, Valid = true, SourceDirection = Direction.Up, DestDirection = Direction.Right},
                new FaceMove{Source = 3, Dest = 2, Valid = true, SourceDirection = Direction.Left, DestDirection = Direction.Left},
                new FaceMove{Source = 3, Dest = 3},
                new FaceMove{Source = 3, Dest = 4, Valid = true, SourceDirection = Direction.Right, DestDirection = Direction.Right},
                new FaceMove{Source = 3, Dest = 5, Valid = true, SourceDirection = Direction.Down, DestDirection = Direction.Right},
                new FaceMove{Source = 3, Dest = 6}
            }},
            new Face {Name=4, GlobalTop = 4, GlobalBottom= 7, GlobalLeft = 8, GlobalRight = 11, Moves = new FaceMove[] {
                new FaceMove{Source = 4, Dest = 1, Valid = true, SourceDirection = Direction.Up, DestDirection = Direction.Up},
                new FaceMove{Source = 4, Dest = 2},
                new FaceMove{Source = 4, Dest = 3, Valid = true, SourceDirection = Direction.Left, DestDirection = Direction.Left},
                new FaceMove{Source = 4, Dest = 4},
                new FaceMove{Source = 4, Dest = 5, Valid = true, SourceDirection = Direction.Down, DestDirection = Direction.Down},
                new FaceMove{Source = 4, Dest = 6, Valid = true, SourceDirection = Direction.Right, DestDirection = Direction.Down}
            }},
            new Face {Name=5, GlobalTop = 8, GlobalBottom= 11, GlobalLeft = 8, GlobalRight = 11, Moves = new FaceMove[] {
                new FaceMove{Source = 5, Dest = 1},
                new FaceMove{Source = 5, Dest = 2, Valid = true, SourceDirection = Direction.Down, DestDirection = Direction.Up},
                new FaceMove{Source = 5, Dest = 3, Valid = true, SourceDirection = Direction.Left, DestDirection = Direction.Up},
                new FaceMove{Source = 5, Dest = 4, Valid = true, SourceDirection = Direction.Up, DestDirection = Direction.Up},
                new FaceMove{Source = 5, Dest = 5},
                new FaceMove{Source = 5, Dest = 6, Valid = true, SourceDirection = Direction.Right, DestDirection = Direction.Right}
            }},
            new Face {Name=6, GlobalTop = 8, GlobalBottom= 11, GlobalLeft = 12, GlobalRight = 15, Moves = new FaceMove[] {
                new FaceMove{Source = 6, Dest = 1, Valid = true, SourceDirection = Direction.Right, DestDirection = Direction.Left},
                new FaceMove{Source = 6, Dest = 2, Valid = true, SourceDirection = Direction.Down, DestDirection = Direction.Right},
                new FaceMove{Source = 6, Dest = 3},
                new FaceMove{Source = 6, Dest = 4, Valid = true, SourceDirection = Direction.Up, DestDirection = Direction.Right},
                new FaceMove{Source = 6, Dest = 5, Valid = true, SourceDirection = Direction.Left, DestDirection = Direction.Left},
                new FaceMove{Source = 6, Dest = 6}
            }}
        };
    } else {
        faces = new[] {
            null, /* TO make array indexes match */
            new Face {Name=1, GlobalTop = 0, GlobalBottom= 49, GlobalLeft = 50, GlobalRight = 99, Moves = new FaceMove[] {
                new FaceMove{Source = 1, Dest = 1},
                new FaceMove{Source = 1, Dest = 2, Valid = true, SourceDirection = Direction.Up, DestDirection = Direction.Right},
                new FaceMove{Source = 1, Dest = 3, Valid = true, SourceDirection = Direction.Left, DestDirection = Direction.Right},
                new FaceMove{Source = 1, Dest = 4, Valid = true, SourceDirection = Direction.Down, DestDirection = Direction.Down},
                new FaceMove{Source = 1, Dest = 5},
                new FaceMove{Source = 1, Dest = 6, Valid = true, SourceDirection = Direction.Right, DestDirection = Direction.Right}
            }},
            new Face {Name=2, GlobalTop = 150, GlobalBottom= 199, GlobalLeft = 0, GlobalRight = 49, Moves = new FaceMove[] {
                new FaceMove{Source = 2, Dest = 1, Valid = true, SourceDirection = Direction.Left, DestDirection = Direction.Down},
                new FaceMove{Source = 2, Dest = 2},
                new FaceMove{Source = 2, Dest = 3, Valid = true, SourceDirection = Direction.Up, DestDirection = Direction.Up},
                new FaceMove{Source = 2, Dest = 4},
                new FaceMove{Source = 2, Dest = 5, Valid = true, SourceDirection = Direction.Right, DestDirection = Direction.Up},
                new FaceMove{Source = 2, Dest = 6, Valid = true, SourceDirection = Direction.Down, DestDirection = Direction.Down}
            }},
            new Face {Name=3, GlobalTop = 100, GlobalBottom= 149, GlobalLeft = 0, GlobalRight = 49, Moves = new FaceMove[] {
                new FaceMove{Source = 3, Dest = 1, Valid = true, SourceDirection = Direction.Left, DestDirection = Direction.Right},
                new FaceMove{Source = 3, Dest = 2, Valid = true, SourceDirection = Direction.Down, DestDirection = Direction.Down},
                new FaceMove{Source = 3, Dest = 3},
                new FaceMove{Source = 3, Dest = 4, Valid = true, SourceDirection = Direction.Up, DestDirection = Direction.Right},
                new FaceMove{Source = 3, Dest = 5, Valid = true, SourceDirection = Direction.Right, DestDirection = Direction.Right},
                new FaceMove{Source = 3, Dest = 6}
            }},
            new Face {Name=4, GlobalTop = 50, GlobalBottom= 99, GlobalLeft = 50, GlobalRight = 99, Moves = new FaceMove[] {
                new FaceMove{Source = 4, Dest = 1, Valid = true, SourceDirection = Direction.Up, DestDirection = Direction.Up},
                new FaceMove{Source = 4, Dest = 2},
                new FaceMove{Source = 4, Dest = 3, Valid = true, SourceDirection = Direction.Left, DestDirection = Direction.Down},
                new FaceMove{Source = 4, Dest = 4},
                new FaceMove{Source = 4, Dest = 5, Valid = true, SourceDirection = Direction.Down, DestDirection = Direction.Down},
                new FaceMove{Source = 4, Dest = 6, Valid = true, SourceDirection = Direction.Right, DestDirection = Direction.Up}
            }},
            new Face {Name=5, GlobalTop = 100, GlobalBottom= 149, GlobalLeft = 50, GlobalRight = 99, Moves = new FaceMove[] {
                new FaceMove{Source = 5, Dest = 1},
                new FaceMove{Source = 5, Dest = 2, Valid = true, SourceDirection = Direction.Down, DestDirection = Direction.Left},
                new FaceMove{Source = 5, Dest = 3, Valid = true, SourceDirection = Direction.Left, DestDirection = Direction.Left},
                new FaceMove{Source = 5, Dest = 4, Valid = true, SourceDirection = Direction.Up, DestDirection = Direction.Up},
                new FaceMove{Source = 5, Dest = 5},
                new FaceMove{Source = 5, Dest = 6, Valid = true, SourceDirection = Direction.Right, DestDirection = Direction.Left}
            }},
            new Face {Name=6, GlobalTop = 0, GlobalBottom= 49, GlobalLeft = 100, GlobalRight = 149, Moves = new FaceMove[] {
                new FaceMove{Source = 6, Dest = 1, Valid = true, SourceDirection = Direction.Left, DestDirection = Direction.Left},
                new FaceMove{Source = 6, Dest = 2, Valid = true, SourceDirection = Direction.Up, DestDirection = Direction.Up},
                new FaceMove{Source = 6, Dest = 3},
                new FaceMove{Source = 6, Dest = 4, Valid = true, SourceDirection = Direction.Down, DestDirection = Direction.Left},
                new FaceMove{Source = 6, Dest = 5, Valid = true, SourceDirection = Direction.Right, DestDirection = Direction.Left},
                new FaceMove{Source = 6, Dest = 6}
            }}
        };
    }



   var maxWidth = mapDescription.MaxBy(m => m.Length).Length;
   var maxHeight = mapDescription.Length;

   Cell[,] map = new Cell[maxHeight, maxWidth];

   Cell GetCellInDirection(Cell cell, Direction dir) {

        var sourceFaceName = cell.Face;
        var sourceFace = faces[sourceFaceName];
        // Are we on the edge
        var onEdge = dir switch {
            Direction.Left => cell.Col == sourceFace.GlobalLeft,
            Direction.Right => cell.Col == sourceFace.GlobalRight,
            Direction.Up => cell.Row == sourceFace.GlobalTop,
            Direction.Down => cell.Row == sourceFace.GlobalBottom,
        };

        if (!onEdge) { 
            return dir switch {
                Direction.Left => map[cell.Row, cell.Col - 1],
                Direction.Right => map[cell.Row, cell.Col + 1],
                Direction.Up => map[cell.Row - 1, cell.Col],
                Direction.Down => map[cell.Row + 1, cell.Col],
            };
        }

        var move = sourceFace.Moves.Single(m => m.Valid && m.SourceDirection == dir);
        var destFace = faces[move.Dest];
        
        if (move.SourceDirection == Direction.Up) {

            var colOffset = cell.Col - sourceFace.GlobalLeft;

            if (move.DestDirection == Direction.Up) { // 4 -> 1
                return map[destFace.GlobalBottom, colOffset + destFace.GlobalLeft];
            }
            if (move.DestDirection == Direction.Down) { // 1 -> 2
                return map[destFace.GlobalTop, destFace.GlobalRight - colOffset];
            }
            if (move.DestDirection == Direction.Left) {
                throw new NotImplementedException($"{sourceFaceName} => {destFace.Name}");
            }
            if (move.DestDirection == Direction.Right) {
                return map[colOffset + destFace.GlobalTop, destFace.GlobalLeft];
            }
        }

        if (move.SourceDirection == Direction.Down) {
            var colOffset = cell.Col - sourceFace.GlobalLeft;
            if (move.DestDirection == Direction.Up) {
                return map[destFace.GlobalBottom, destFace.GlobalRight - colOffset];
            }
            if (move.DestDirection == Direction.Down) {
                return map[destFace.GlobalTop, colOffset + destFace.GlobalLeft];
            }
            if (move.DestDirection == Direction.Left) {
                return map[destFace.GlobalTop + colOffset, destFace.GlobalRight];
                 throw new NotImplementedException($"{sourceFaceName} => {destFace.Name}");
            }
            if (move.DestDirection == Direction.Right) {
                return map[destFace.GlobalBottom - colOffset, destFace.GlobalLeft];
            }
        }

        if (move.SourceDirection == Direction.Left) {
            var rowOffset = cell.Row - sourceFace.GlobalTop;
            if (move.DestDirection == Direction.Up) {
                return map[destFace.GlobalBottom, destFace.GlobalRight - rowOffset];
            }
            if (move.DestDirection == Direction.Down) {
                return map[destFace.GlobalTop, rowOffset + destFace.GlobalLeft];
            }
            if (move.DestDirection == Direction.Left) {
                 return map[destFace.GlobalTop + rowOffset, destFace.GlobalRight];
            }
            if (move.DestDirection == Direction.Right) {
                return map[destFace.GlobalBottom - rowOffset, destFace.GlobalLeft];
            }
        }

        if (move.SourceDirection == Direction.Right) {
            var rowOffset = cell.Row - sourceFace.GlobalTop;
            if (move.DestDirection == Direction.Up) {
                return map[destFace.GlobalBottom, destFace.GlobalLeft + rowOffset];
            }
            if (move.DestDirection == Direction.Down) {
                return map[destFace.GlobalTop, destFace.GlobalRight - rowOffset];
            }
            if (move.DestDirection == Direction.Left) {
                    return map[destFace.GlobalBottom - rowOffset, destFace.GlobalRight];
            }
            if (move.DestDirection == Direction.Right) {
                return map[destFace.GlobalTop + rowOffset, destFace.GlobalLeft];
            }
        }

        throw new Exception("Aack");
   }
   
    for(var row = 0; row < maxHeight; row++) {
        var line = mapDescription[row];
        for (var col = 0; col < maxWidth; col++) {
           var face = faces.Skip(1).SingleOrDefault(f => f.GlobalLeft <= col && f.GlobalRight >= col && f.GlobalTop <= row && f.GlobalBottom >= row)?.Name;
           if (face.HasValue) {
                var c = line[col];
                if (c == '.') {
                    map[row, col] = new Cell {Type = CellType.Open, Col = col, Row = row, Face = face.Value};
                } else if (c == '#'){
                    map[row, col] = new Cell {Type = CellType.Wall, Col = col, Row = row};
                }
           }
        }
    }

    Regex regex = new Regex("R|L|[0-9]+");
    MatchCollection matches = regex.Matches(instructions);

    var path = matches.Select(m => 
    {
        var isDirection = m.Value == "R" || m.Value == "L";
        if (isDirection) {
            return new Segment {Right = m.Value == "R"};
        }
        return new Segment {Steps = int.Parse(m.Value)};
    }).ToArray();

    
    Cell position = null;
    Direction direction = Direction.Right;
    {
        for (int ii = 0; ii < maxWidth; ii++) {
            if (map[0, ii]?.Type == CellType.Open) {
                position = map[0, ii];
                break;
            }
        }
    }
   
   foreach (Segment segment in path) {

        if (segment.Steps.HasValue) {
            for (int ii = 0; ii < segment.Steps; ii++) {

                var nextCell = GetCellInDirection(position, direction);

                if (nextCell.Type == CellType.Wall) {
                    break;
                }
                var oldDirection = direction;
                if (nextCell.Face != position.Face) {
                    direction = faces[position.Face].Moves.Single(m => m.Valid && m.Dest == nextCell.Face).DestDirection;
                }
                if (debug) {Console.WriteLine($"{oldDirection} {position.Face}:[{position.Row}, {position.Col}] ==> {direction} {nextCell.Face}:[{nextCell.Row}, {nextCell.Col}]");}
                position = nextCell;
                
            }
        } else {
            direction = segment.Right switch {
                    true => direction switch {
                        Direction.Down => Direction.Left,
                        Direction.Left => Direction.Up,
                        Direction.Up => Direction.Right,
                        Direction.Right => Direction.Down
                    },
                    false => direction switch {
                        Direction.Down => Direction.Right,
                        Direction.Right => Direction.Up,
                        Direction.Up => Direction.Left,
                        Direction.Left => Direction.Down
                    }
            };
            if (debug) {Console.WriteLine($"Direction now: {direction}");}
        }
   }


    var password = (1000 * (position.Row + 1)) + (4 * (position.Col + 1)) + direction switch {
        Direction.Right => 0,
        Direction.Down => 1,
        Direction.Left => 2,
        Direction.Up => 3
    };

    Console.WriteLine($"Part 2 Password: {password}");
}

enum Direction {
    Up,
    Down,
    Right,
    Left
}
enum CellType {
    Open,
    Wall
}

record Cell {
    public CellType Type;
    public Cell Up;
    public Cell Down;
    public Cell Right;
    public Cell Left;
    public int Row;
    public int Col;
    public int Face;
}

record Segment {
    public bool? Right;
    public int? Steps;
}

record Face {
    public int Name;
    public int GlobalLeft;
    public int GlobalRight;
    public int GlobalTop;
    public int GlobalBottom;

    public FaceMove[] Moves;
}

record FaceMove {
    public int Source;
    public int Dest;
    public bool Valid;
    public Direction SourceDirection;

    public Direction DestDirection;
}







