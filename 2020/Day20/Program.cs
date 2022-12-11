using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MoreLinq;

public static class Program {
    public static void Main() {
        string[] lines = File.ReadAllLines("input.txt"); 
        //string[] lines = File.ReadAllLines("sample2.txt"); 
        //Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

        var tiles = lines.Batch(12).Select(b => new Tile(b.ToArray())).ToList();

        List<int> sides = new List<int>();
        var singles = tiles.Select(t => t.Sides).SelectMany(t => t).GroupBy(s => s).Where(g => g.Count() == 1).Select(g => g.Key);
        Console.Out.WriteLine($"Singles {singles.Count()} {singles.ToDelimitedString(",")}");

        var sideDictionary = new Dictionary<int, List<OrientedTile>>();

        var regularAndFlipped = tiles.Select(t => new OrientedTile(t, 0) {Flipped = false}).Concat(tiles.Select(t => new OrientedTile(t,0) {Flipped = true}));

        foreach (var tile in regularAndFlipped) {
            foreach (var side in tile.Sides) {
                List<OrientedTile> existingList;
                bool found = sideDictionary.TryGetValue(side.Item1, out existingList);
                if (!found) {
                    existingList = new List<OrientedTile>();
                    sideDictionary.Add(side.Item1, existingList);
                }
                existingList.Add(tile);
            }
        }

        Console.Out.WriteLine($"All tiles");
        foreach (var tile in tiles) {
            Console.Out.WriteLine(tile);
        }

        Console.Out.WriteLine();
        Console.Out.WriteLine($"Potential corners");

        var corners = tiles.Where(
            t => t.Sides.Count(s => sideDictionary[s.Item1].Count() == 1) == 2 || t.Opposites.Count(s => sideDictionary[s.Item1].Count() == 1) == 2);

        long product = 1;            
        foreach (var tile in corners) {
            Console.Out.WriteLine(tile);
            product *= tile.Id;
        }
        Console.Out.WriteLine($"Product of corners: {product}");

        var cornerTL = corners.ElementAt(0);
        var orientedTileTL = new OrientedTile(cornerTL, 0);
        var foundOrientation = false;
        for (byte orientation = 0 ; orientation <= 3; orientation++) {
            orientedTileTL.Orientation = orientation;
            var uniqueSideIndexes = new List<int>();
            for (var ii = 0; ii < 4; ii++) {
                var side = orientedTileTL.Sides[ii];
                if (sideDictionary[side.Item1].Count() == 1) {
                    uniqueSideIndexes.Add(ii);
                }
            }
            if (uniqueSideIndexes.Count() != 2) {
                throw new Exception("E2");
            }

            if (uniqueSideIndexes.Contains(Top) && uniqueSideIndexes.Contains(Left)) {
                foundOrientation = true;
                break;
            }
        }

        if (!foundOrientation) {
            throw new Exception("E4");
        }
        //Console.Out.WriteLine($"TopLeft: {orientedTileTL.ToString()}");
        //foreach (var tile in corners) {
        //    Console.Out.WriteLine(tile);
        //    product *= tile.Id;
        //}

        // var debugTile = new OrientedTile(cornerTL, 0);
        // for (byte flipped = 0; flipped <= 1; flipped++) {
        //     debugTile.Flipped = flipped == 1;
        //     for (byte orientation = 0 ; orientation <= 3; orientation++) {
        //         debugTile.Orientation = orientation;
        //         Console.Out.WriteLine(debugTile);
        //     }
        // }
        // return;

        var dim = (int)Math.Sqrt(tiles.Count());
        Console.Out.WriteLine($"Looking for a {dim}x{dim} array");

        var theSolution = new OrientedTile[dim,dim];
        theSolution[0,0] = orientedTileTL;
        
        //var multiTiles = tiles.Where(t => t.Sides.Where(s=> sideDictionary[s.Item1].Count() > 2).Any()).Count();
        //Console.Out.WriteLine($"multiTiles {multiTiles}");

        SimpleSolve(theSolution, sideDictionary);

        PrintSolution(theSolution);

        var finalPicture = new bool[dim*8, dim*8];

        for (var r = 0; r < theSolution.GetLength(0); r++) {
            for (var c = 0; c < theSolution.GetLength(0); c++) {
                var ot = theSolution[r,c];
                CopyInto(finalPicture, RotateAndFlip(RemoveBorder(ot.Tile.Data), ot.Orientation, ot.Flipped), r*8, c*8);
            }
         }

        int monsters = LookForMonstersInAllDirections(finalPicture);
        
        Console.Out.WriteLine($"Found {monsters} monsters");

        var totalTrue = finalPicture.Cast<bool>().Select(b => b ? 1 : 0).Sum();

        Console.Out.WriteLine($"Roughness {totalTrue - monsters * 15}");

         //PrintPicture(finalPicture);
    }

    public static int LookForMonstersInAllDirections(bool[,] picture) {
        
        for (int flip = 0; flip <= 1; flip++) {
            var next = picture;
            if (flip == 1) {
                next = FlipArray(next);
            }
            for (int orientation = 0; orientation < 4; orientation++) {
                var found = LookForMonsters(next);
                if (found > 0) {
                    return found;
                }
                next = RotateArrayClockwise(next);
            }
        }
        return -1;
    }

public static string monsterStr = @"
                  # 
#    ##    ##    ###
 #  #  #  #  #  #   ";

    public static int LookCount = 0;
    public static int LookForMonsters(bool[,] picture) {
        LookCount++;
        Console.Out.WriteLine($"--------Start Looking {LookCount}");
        PrintPicture(picture);
        Console.Out.WriteLine($"--------End Looking {LookCount}");
        Console.Out.WriteLine();

        var monster = monsterStr.Split("\r\n").Skip(1).Select(l => l.Select(c => c == '#').ToArray()).ToArray();
        var found = 0;
        for (var r = 0; r < picture.GetLength(0) - monster.Length; r++) {
            for (var c = 0; c < picture.GetLength(1) - monster[0].Length; c++) {
                if (LookForMonster(r, c, picture, monster)) {
                    found++;
                }
            }
        }
        return found;
    }

    public static bool LookForMonster(int startR, int startC, bool[,] picture, bool[][] monster) {
         for (var r = 0; r < monster.Length; r++) {
            for (var c = 0; c < monster[0].Length; c++) {
                if (monster[r][c] && !picture[startR + r, startC + c]) {
                    return false;
                }
            }
         }
         return true;
    }

    public static void PrintPicture(bool[,] picture) {
        for (var r = 0; r < picture.GetLength(0); r++) {
            for (var c = 0; c < picture.GetLength(1); c++) {
                Console.Out.Write(picture[r,c] ? '#' : '.');
            }
            Console.Out.WriteLine();
        }
    }

    public static void PrintSolution(OrientedTile[,] solution) {
        for (var r = 0; r < solution.GetLength(0); r++) {
            for (var c = 0; c < solution.GetLength(1); c++) {
                Console.Out.WriteLine($"[{r},{c}]: {solution[r,c]}");
            }
         }
    }
    public static int FromBools(this IEnumerable<bool> array) {
        return array.Aggregate(0, (a, b) => (a << 1) + (b ? 1 : 0));
    } 

    public static IEnumerable<bool>[] RotateRight(this IEnumerable<bool>[] orig) {
        return new IEnumerable<bool>[] {
            orig[3].Reverse(), orig[0], orig[1].Reverse(), orig[2]
        };
    }

    public static IEnumerable<bool>[] FlipH(this IEnumerable<bool>[] orig) {
        return new IEnumerable<bool>[] {
            orig[0].Reverse(), orig[3], orig[2].Reverse(), orig[1]
        };
    }

    public static IEnumerable<bool>[] FlipV(this IEnumerable<bool>[] orig) {
        return new IEnumerable<bool>[] {
            orig[2], orig[1].Reverse(), orig[0], orig[3].Reverse()
        };
    }

    static int Top = 0;
    static int Right = 1;
    static int Bottom = 2;
    static int Left = 3;

    static int NumOrientations = 12;


    public static void SimpleSolve(OrientedTile[,] solution, Dictionary<int, List<OrientedTile>> sideDictionary)  {
        for (var r = 0; r < solution.GetLength(0); r++) {
            for (var c = 0; c < solution.GetLength(0); c++) {
                OrientedTile matcher;
                if (c == 0) {
                    if (r == 0) {
                        continue;
                    }
                    var matchee = solution[r-1,c];
                    var matcheeSide = matchee.Sides[Bottom].Item2;
                    matcher = sideDictionary[matcheeSide].Where(ot => ot.Tile != matchee.Tile).Single();
                    var foundOrientation = false;
                    for (byte orientation = 0 ; orientation <= 3; orientation++) {
                        matcher.Orientation = orientation;
                        if (matcher.Sides[Top].Item1 == matcheeSide) {
                            foundOrientation = true;
                            break;
                        }
                    }
                    if (!foundOrientation) {
                        throw new Exception("E5");
                    }
                } else {
                    var matchee = solution[r, c-1];
                    var matcheeSide = matchee.Sides[Right].Item2;
                    matcher = sideDictionary[matcheeSide].Where(ot => ot.Tile != matchee.Tile).Single();
                    var foundOrientation = false;
                    for (byte orientation = 0 ; orientation <= 3; orientation++) {
                        matcher.Orientation = orientation;
                        if (matcher.Sides[Left].Item1 == matcheeSide) {
                            foundOrientation = true;
                            break;
                        }
                    }
                    if (!foundOrientation) {
                        throw new Exception("E5");
                    }
                }
                solution[r,c] = matcher;
                //Console.Out.WriteLine("------------");
                //PrintSolution(solution);
                //Console.Out.WriteLine("------------");
                //Console.Out.WriteLine();
            }
        }
    }

   
private static void CopyInto(bool[,] dst, bool[,] src, int rOffset, int cOffset) {
    for (int r = 0 ;r < src.GetLength(0); r++) {
        for (int c = 0; c < src.GetLength(1); c++) {
            dst[r+rOffset, c+cOffset] = src[r,c];
        }
    }
}

private static bool[,] RemoveBorder(bool[,] src) {
    var dst = new bool[src.GetLength(0)-2, src.GetLength(1)-2];

    for (int r = 1; r < src.GetLength(0) - 1; r++) {
        for (int c = 1; c < src.GetLength(1) - 1; c++) {
            dst[r-1, c-1] = src[r,c];
        }
    }
    return dst;
}

private static bool[,] RotateAndFlip(bool[,] src, short orientation, bool flipped) {
    return Rotate(flipped ? FlipArray(src) : src, orientation);
}

private static bool[,] Rotate(bool[,] src, short orientation) {
    var next = src;
    for (int ii = 0; ii < orientation; ii++) {
        next = RotateArrayClockwise(next);
    }
    return next;
}
private static bool[,] RotateArrayClockwise(bool[,] src)
{
  int width;
  int height;
  bool[,] dst;

  width = src.GetUpperBound(0) + 1;
  height = src.GetUpperBound(1) + 1;
  dst = new bool[height, width];

  for (int row = 0; row < width; row++)
  {
    for (int col = 0; col < height; col++)
    {
        int newRow;
        int newCol;

        newCol = width - (row + 1);
        newRow = col;

        dst[newRow, newCol] = src[row, col];
    }
  }

  return dst;
}

private static bool[,] FlipArray(bool[,] src)
{
  int width;
  int height;
  bool[,] dst;

  width = src.GetLength(0);
  height = src.GetLength(1);
  dst = new bool[width, height];

  for (int row = 0; row < height; row++)
  {
    for (int col = 0; col < width; col++)
    {
        int newRow;
        int newCol;

        newRow = row;
        newCol = width - (col + 1);

        dst[newRow, newCol] = src[row, col];
    }
  }

  return dst;
}

    public class OrientedTile {
        public Tile Tile;
        public byte Orientation;
        public bool Flipped;

        override public string ToString() {
            return $"{Sides.ToDelimitedString(",")}: ({(Flipped ? "Flipped" : "NotFlipped")}) ({Orientation})  Id: {Tile.Id}";
        }

        public (int, int)[] Sides {
            get {
                var actualSides = Flipped ? Tile.Opposites : Tile.Sides;
                var matcherSides = Flipped ? Tile.Sides : Tile.Opposites;
                if (Orientation == 0) {
                    return actualSides;
                }
                if (Orientation == 1) {
                    return new [] {
                        actualSides.ElementAt(3),
                        actualSides.ElementAt(0),
                        actualSides.ElementAt(1),
                        actualSides.ElementAt(2)
                    };
                }
                if (Orientation == 2) {
                    return new [] {
                        actualSides.ElementAt(2),
                        actualSides.ElementAt(3),
                        actualSides.ElementAt(0),
                        actualSides.ElementAt(1)
                    };
                }
                if (Orientation == 3) {
                    return new [] {
                        actualSides.ElementAt(1),
                        actualSides.ElementAt(2),
                        actualSides.ElementAt(3),
                        actualSides.ElementAt(0)
                    };
                }
                throw new Exception("E3");
            }
        }

        public OrientedTile(Tile tile, byte orientation) {
            this.Tile = tile;
            this.Orientation = orientation;
        }
    }

    public class Tile {

        override public string ToString() {
            return $"{Id}: {Sides.ToDelimitedString(",")} -> {Opposites.ToDelimitedString(",")}";
        }
        public short Id;
        public bool[,] Data = new bool[10,10];

        public (int, int)[] Sides = new (int, int)[4];
        public (int, int)[] Opposites = new (int, int)[4];

        public Tile (string[] lines) {
            Id = short.Parse(lines.First()[5..^1]);
            int r = 0;
            int c = 0;
            for (r = 0; r < 10; r++) {
                var line = lines[r+1];
                for (c = 0; c < 10; c++) {
                    Data[r,c] = line[c] == '#';
                }
            }
            var originalSides = new [] {
                Enumerable.Range(0, 10).Select(c => Data[0, c]),
                Enumerable.Range(0, 10).Select(r => Data[r, 9]),
                Enumerable.Range(0, 10).Select(c => Data[9, c]).Reverse(),
                Enumerable.Range(0, 10).Select(r => Data[r, 0]).Reverse(),
            };
            
            Sides = originalSides.Select(s => (s.FromBools(), s.Reverse().FromBools())).ToArray();
            Opposites = new [] {
                originalSides.ElementAt(0),
                originalSides.ElementAt(3),
                originalSides.ElementAt(2),
                originalSides.ElementAt(1)
            }.Select(s => (s.Reverse().FromBools(), s.FromBools())).ToArray();
        }
    }
}

  