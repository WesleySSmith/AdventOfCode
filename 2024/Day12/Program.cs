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
    var rows = lines.Length;
    var cols = lines[0].Length;

    var map = new (char Plant, bool Visited)[rows+2,cols+2];

    for (int r = 0; r < rows +2; r++) {
        for (int c = 0; c < cols +2; c++) {
            if (r == 0 || c == 0 || r == rows+1 || c == cols+1) {
                map[r,c] = ((char)0, false);
            }
            else {
                map[r,c] = (lines[r-1][c-1], false);
            }
        }
    } 

    List<(int Area, int Perim)> bigList = new ();
    for (int i = 1; i < rows + 1; i++) {
        for (int j = 1; j < cols +1; j++) {

            if (map[i,j].Plant == 0 || map[i,j].Visited) {
                continue;
            }

            int area = 0;
            int perim = 0;
            Stack<RC> stack = new();
            var current = new  RC(i, j);
            area++;
            stack.Push(current);
            var plant = map[current.Row, current.Col].Plant;
            map[current.Row,current.Col].Visited = true;
            while (stack.TryPop(out current)) {
                var left = current with {Row = current.Row -1};
                if (map[left.Row,left.Col].Plant == plant) {
                    if (!map[left.Row,left.Col].Visited) {
                        stack.Push(left);
                        area++;
                        map[left.Row,left.Col].Visited = true;
                    }
                } else {
                    perim++;
                }
                var right = current with {Row = current.Row +1};
                if (map[right.Row,right.Col].Plant == plant) {
                    if (!map[right.Row,right.Col].Visited) {
                        stack.Push(right);
                        area++;
                        map[right.Row,right.Col].Visited = true;
                    }
                } else {
                    perim++;
                }
                var up = current with {Col = current.Col -1};
                if (map[up.Row,up.Col].Plant == plant) {
                    if (!map[up.Row,up.Col].Visited) {
                        stack.Push(up);
                        area++;
                        map[up.Row,up.Col].Visited = true;
                    }
                }
                else {
                    perim++;
                }
                var down = current with {Col = current.Col +1};
                if (map[down.Row,down.Col] .Plant == plant) {
                    if (!map[down.Row,down.Col].Visited) {
                        stack.Push(down);
                        area++;
                        map[down.Row,down.Col].Visited = true;
                    }
                } else {
                    perim++;
                }
            }
            bigList.Add((area, perim));
        }
    }


    var score = bigList.Sum(l => l.Area * l.Perim);

    Console.Out.WriteLine(score);


    //Console.Out.WriteLine($"Part 1: {stones.Count()}");
}


void Part2(string[] lines) {
    var rows = lines.Length;
    var cols = lines[0].Length;

    var map = new (char Plant, bool Visited)[rows+2,cols+2];

    for (int r = 0; r < rows +2; r++) {
        for (int c = 0; c < cols +2; c++) {
            if (r == 0 || c == 0 || r == rows+1 || c == cols+1) {
                map[r,c] = ((char)0, false);
            }
            else {
                map[r,c] = (lines[r-1][c-1], false);
            }
        }
    } 
    List<Lelem> bigList = new ();
    Dictionary<RC, Lelem> bigMap = new();
    for (int i = 1; i < rows + 1; i++) {
        for (int j = 1; j < cols +1; j++) {

            if (map[i,j].Plant == 0 || map[i,j].Visited) {
                continue;
            }

            Stack<RC> stack = new();
            var current = new  RC(i, j);
            stack.Push(current);
            var plant = map[current.Row, current.Col].Plant;
            map[current.Row,current.Col].Visited = true;
            Lelem lelem = new Lelem{ Area = 1, Plant = plant};
            bigMap[current] = lelem;

            while (stack.TryPop(out current)) {
                var left = current with {Row = current.Row -1};
                if (map[left.Row,left.Col].Plant == plant) {
                    if (!map[left.Row,left.Col].Visited) {
                        stack.Push(left);
                        lelem.Area++;
                        map[left.Row,left.Col].Visited = true;
                        bigMap[left] = lelem;
                    }
                } else {
                    lelem.Perimeter++;
                }
                var right = current with {Row = current.Row +1};
                if (map[right.Row,right.Col].Plant == plant) {
                    if (!map[right.Row,right.Col].Visited) {
                        stack.Push(right);
                        lelem.Area++;
                        map[right.Row,right.Col].Visited = true;
                        bigMap[right] = lelem;
                    }
                } else {
                    lelem.Perimeter++;
                }
                var up = current with {Col = current.Col -1};
                if (map[up.Row,up.Col].Plant == plant) {
                    if (!map[up.Row,up.Col].Visited) {
                        stack.Push(up);
                        lelem.Area++;
                        map[up.Row,up.Col].Visited = true;
                        bigMap[up] = lelem;
                    }
                }
                else {
                    lelem.Perimeter++;
                }
                var down = current with {Col = current.Col +1};
                if (map[down.Row,down.Col] .Plant == plant) {
                    if (!map[down.Row,down.Col].Visited) {
                        stack.Push(down);
                        lelem.Area++;
                        map[down.Row,down.Col].Visited = true;
                        bigMap[down] = lelem;
                    }
                } else {
                    lelem.Perimeter++;
                }
            }
            bigList.Add(lelem);
        }
    }

    for (int i = 1; i < rows + 1; i++) {
        for (int j = 1; j < cols +1; j++) {

            var current = new  RC(i, j);
            var plant = map[current.Row, current.Col].Plant;


            var up = current with {Row = current.Row - 1};
            var down = current with {Row = current.Row + 1};
            var right = current with {Col = current.Col + 1};
            var upright = current with {Row = current.Row - 1, Col = current.Col + 1};
            var downright = current with {Row = current.Row + 1, Col = current.Col + 1};
                
            if (map[up.Row,up.Col].Plant != plant) {
                // has a fence on the top
                if (map[right.Row,right.Col].Plant != plant || map[upright.Row, upright.Col].Plant == plant) {
                    // The top fence is ending, so count it
                    bigMap[current].Sides++;
                }
            }

            if (map[down.Row,down.Col] .Plant != plant) {
                // has a fence on the bottom
                if (map[right.Row,right.Col].Plant != plant || map[downright.Row, downright.Col].Plant == plant) {
                    // The bottom fence is ending, so count it
                    bigMap[current].Sides++;
                }
            }
        }
    }

    for (int j = 1; j < cols +1; j++) {
        for (int i = 1; i < rows + 1; i++) {

            var current = new  RC(i, j);
            var plant = map[current.Row, current.Col].Plant;

            var up = current with {Row = current.Row - 1};
            var down = current with {Row = current.Row + 1};
            var right = current with {Col = current.Col + 1};
            var left = current with {Col = current.Col - 1};
            var upright = current with {Row = current.Row - 1, Col = current.Col + 1};
            var downright = current with {Row = current.Row + 1, Col = current.Col + 1};
            var downleft = current with {Row = current.Row +1, Col = current.Col - 1};
                
            if (map[right.Row,right.Col].Plant != plant) {
                // has a fence on the right
                if (map[down.Row,down.Col].Plant != plant || map[downright.Row, downright.Col].Plant == plant) {
                    // The right fence is ending, so count it
                    bigMap[current].Sides++;
                }
            }

            if (map[left.Row,left.Col] .Plant != plant) {
                // has a fence on the left
                if (map[down.Row,down.Col].Plant != plant || map[downleft.Row, downleft.Col].Plant == plant) {
                    // The left fence is ending, so count it
                    bigMap[current].Sides++;
                }
            }
        }
    }



    var score = bigList.Sum(l => l.Area * l.Sides);

    Console.Out.WriteLine(score);


   
}

record RC(int Row, int Col);
record Lelem() {
    public int Area {get; set;}
    public char Plant {get; init;}
    public int Perimeter {get; set;}

    public int Sides {get; set;}
}

