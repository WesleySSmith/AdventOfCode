using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MoreLinq;

namespace Day24
{
    class Program
    {

        static bool White = false;
        static bool Black = true;

        static int dim = 200;
        static int offset = dim / 2;

        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("input.txt"); 
            //string[] lines = File.ReadAllLines("sample.txt"); 
            //Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

            var paths = lines.Select(StringToDirectionList);

            var tiles = new Dictionary<(int x, int y), bool>();

            foreach (var path in paths) {

                int x = 0;
                int y = 0;
                foreach (var direction in path) {
                    switch (direction) {
                        case Direction.e:
                            x++;
                            break;
                        case Direction.ne:
                            y++;
                            break;
                        case  Direction.w:
                            x--;
                            break;
                        case Direction.nw:
                            x--;
                            y++;
                            break;
                        case Direction.se:
                            x++;
                            y--;
                            break;
                        case Direction.sw:
                            y--;
                            break;
                    }
                }
                var current = White; 
                var found = tiles.TryGetValue((x,y), out current);
                tiles[(x,y)] = !current;
            }

            var answer = tiles.Values.Count(t => t == Black);
            Console.Out.WriteLine($"{answer} black tiles");

            
            var tileA = new bool[dim,dim];
            foreach (var tile in tiles) {
                tileA[tile.Key.x + offset, tile.Key.y + offset] = tile.Value;
            }

            var tileACopy = (bool[,])tileA.Clone();

            for (int ii = 0; ii < 100; ii++) {

                for(int r = 2; r < dim-2; r++) {
                    for(int c = 2; c < dim-2; c++) {
                        var currentColor = tileA[r,c];
                        bool newColor = currentColor;
                        var blackNeighbors = CountBlackNeighbors(r, c, tileA);
                        if (currentColor == Black && (blackNeighbors == 0 || blackNeighbors > 2)) {
                                newColor = White;
                        } else if (currentColor == White && blackNeighbors == 2) {
                            newColor = Black;
                        }
                        tileACopy[r,c] = newColor;
                    }
                }
                var temp = tileA;
                tileA = tileACopy;
                tileACopy = temp;
                //PrintBlackTiles(tileA, ii + 1);
            }
            PrintBlackTiles(tileA, 100);

        }
        
        static void PrintBlackTiles(bool[,] tiles, int day) {
            var numBlackTiles = tiles.Cast<bool>().Count(b => b == Black);
            Console.Out.WriteLine($"Day {day}: {numBlackTiles}");
        }

        static int CountBlackNeighbors(int x, int y, bool[,] tiles) {
            int accum = 0;
            for (int ii = 0; ii < 6; ii++) {
                var neighborPos = FindNeighbor(x, y, (Direction)ii);
                var neighborColor = tiles[neighborPos.x, neighborPos.y];
                accum += neighborColor == Black ? 1 : 0; 
            }
            return accum;
        }

        static (int x, int y) FindNeighbor(int x, int y, Direction direction) {

            switch (direction) {
                case Direction.e:
                    x++;
                    break;
                case Direction.ne:
                    y++;
                    break;
                case  Direction.w:
                    x--;
                    break;
                case Direction.nw:
                    x--;
                    y++;
                    break;
                case Direction.se:
                    x++;
                    y--;
                    break;
                case Direction.sw:
                    y--;
                    break;
            }
            return (x,y);
        }

        static List<Direction> StringToDirectionList(string s) {
            List<Direction> dl = new List<Direction>();
            int startpos = 0;
            while (startpos < s.Length) {
                string token = s[startpos..(startpos+1)];
                if (token == "e" || token == "w") {
                    dl.Add(token == "w" ? Direction.w : Direction.e);
                    startpos += 1;
                }
                else {
                    token = s[startpos..(startpos+2)];
                    dl.Add(token switch {
                        "se" => Direction.se,
                        "sw" => Direction.sw,
                        "ne" => Direction.ne,
                        "nw" => Direction.nw,
                        _ => throw new Exception("?")});
                    startpos += 2;
                }
            }
            return dl;
        }
    }

    enum Direction {
        e,
        se,
        sw,
        w,
        nw,
        ne
    }
}
