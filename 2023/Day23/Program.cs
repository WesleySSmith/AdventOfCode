// 6387 too high
#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Data;
using System.Diagnostics;
using MoreLinq;


bool sample = false;


string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

var map = lines.Select(l=> l.ToArray()).ToArray();
int MaxPath2Count = 0;
//Part1(map);
Part2(map);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");



void Part1(char[][] map)
{
    var row = 0;
    var col = 1;

    var destRow = map.Length -1;
    var destCol = map[0].Length -2;

    var goal = new Node();

    var root = ExploreMap(map, row, col, Dir.S, destRow, destCol, goal);


    var maxPath = MaxPath(root, goal) -1;
    Console.Out.WriteLine($"MaxPath: {maxPath}");

}


void Part2(char[][] map)
{
    var row = 0;
    var col = 1;

    var destRow = map.Length -1;
    var destCol = map[0].Length -2;

    

    for (int ii = 0; ii < map.Length; ii++) {
        for (int jj = 0; jj < map[0].Length; jj++) {
            if (map[ii][jj] is '>' or '<' or '^' or 'v') {
                map[ii][jj] = '.';
            }
        }
    }

    
    var goalPoint = new Point(destRow, destCol);
    var startPoint = new Point(row, col);

    var goal = new Node2() {Point = goalPoint};
    var start = new Node2() {Point = startPoint};

    var nodeDict = new Dictionary<Point, Node2>();

    nodeDict.Add(goalPoint, goal);
    nodeDict.Add(startPoint, start);
    ExploreMap2(map, row, col, Dir.S, start, goal, nodeDict);

    PrintNodeList(start, goal, nodeDict);

    var maxPath = MaxPath2(start, goal, new HashSet<Node2>());
    var maxLen = maxPath.MaxPath -1;
    Console.Out.WriteLine($"MaxPath: {maxLen}");
    Console.Out.WriteLine(string.Join("\n", maxPath.VisitedNodeList.AsEnumerable().Reverse().Select(n => n.Node.Point + " => " + n.Len + " = " + n.Total )));

}

void PrintNodeList(Node2 root, Node2 goal, Dictionary<Point, Node2> nodeDict) {
    var nodes = nodeDict.Values.OrderBy(n => n.Point.R).ThenBy(n => n.Point.C).ToList();
    foreach (var node in nodes) {
        Console.WriteLine($"{(node == root ? "^" : node == goal ? "$" : " " )} {node.Point}:");
        Console.WriteLine("\t" + string.Join("\n\t", node.Neighbors.Select(n => n.Weight.ToString() + " => " + n.Node.Point)));
    }
}


int MaxPath(Node root, Node goal) {
    if (root == goal) {
        return 0;
    } 

    var longestChild = root.Next.Select(n => MaxPath(n, goal)).Max();
    return root.Len + longestChild;
}

(int MaxPath, List<(Node2 Node, int Len, int Total)> VisitedNodeList) MaxPath2(Node2 root, Node2 goal, HashSet<Node2> visitedNodes) {

    
    if (root == goal) {
        return (0, new List<(Node2 Node, int Len, int Total)>());
    } 

    var visitedNodesCopy = new HashSet<Node2>(visitedNodes);
    visitedNodesCopy.Add(root);
    int maxLength = -1;

    List<(Node2 Node, int Len, int Total)> bestPath = null;
    int bestWeight = -1;

    foreach (var neighbor in root.Neighbors) {
        if (!visitedNodes.Contains(neighbor.Node)) {
            var result = MaxPath2(neighbor.Node, goal, visitedNodesCopy);
            if (result.MaxPath < 0) {
                // Didn't get to goal
                continue;
            }
            var possibleLength = result.MaxPath + neighbor.Weight;
           
            if (possibleLength > maxLength) {
                maxLength = possibleLength;
                bestPath = result.VisitedNodeList;
                bestWeight = neighbor.Weight;
            }
        }
    }

   

    if (MaxPath2Count++ % 1_000_000 == 0) {
        Console.WriteLine($"{MaxPath2Count}: {maxLength}");
    }


    if (maxLength == -1) {
        // Dead end
        return (-1, null);
    }

    bestPath.Add((root, bestWeight, maxLength));
    return (maxLength, bestPath);
}

Node ExploreMap(char[][] map, int row, int col, Dir dir, int destRow, int destCol, Node goal) {
    int len = 1;

    while (true)
    {
        bool northPossible = dir != Dir.S && map[row-1][col] is '.' or '^';
        bool southPossible = dir != Dir.N && map[row+1][col] is '.' or 'v';
        bool eastPossible = dir != Dir.W && map[row][col+1] is '.' or '>';
        bool westPossible = dir != Dir.E && map[row][col-1] is '.' or '<';

         var possibleCount = (northPossible ? 1 : 0) + (southPossible ? 1 : 0) + (eastPossible ? 1 : 0) + (westPossible ? 1: 0);

        if (possibleCount == 0) {
            // Dead end
            return null;
        }

        if (possibleCount == 1) {
            // move
            len++;
            if (northPossible) {
                row--;
                dir = Dir.N;
            } else if (southPossible) {
                row++;
                dir = Dir.S;
            } else if (eastPossible) {
                col++;
                dir = Dir.E;
            } else {
                col--;
                dir = Dir.W;
            }

            if (row == destRow && col == destCol) {
                var result = new Node() {Len = len};
                result.Next.Add(goal);
                return result;
            }
        } else {
            // choice!
            var node = new Node() {Len = len};
            if (northPossible) {
                node.Next.Add(ExploreMap(map, row-1, col, Dir.N, destRow, destCol, goal));
            }
            if (southPossible) {
                node.Next.Add(ExploreMap(map, row+1, col, Dir.S, destRow, destCol, goal));
            }
            if (eastPossible) {
                node.Next.Add(ExploreMap(map, row, col+1, Dir.E, destRow, destCol, goal));
            }
            if (westPossible) {
                node.Next.Add(ExploreMap(map, row, col-1, Dir.W, destRow, destCol, goal));
            }
            return node;
        }
    }
}


void ExploreMap2(char[][] map, int row, int col, Dir dir, Node2 parent, Node2 goal, Dictionary<Point, Node2> nodeIndex) {
    int len = 1;

    while (true)
    {
        bool northPossible = dir != Dir.S && map[row-1][col] is '.';
        bool southPossible = dir != Dir.N && map[row+1][col] is '.';
        bool eastPossible = dir != Dir.W && map[row][col+1] is '.';
        bool westPossible = dir != Dir.E && map[row][col-1] is '.';

        var possibleCount = (northPossible ? 1 : 0) + (southPossible ? 1 : 0) + (eastPossible ? 1 : 0) + (westPossible ? 1: 0);

        if (possibleCount == 0) {
            // Dead end
            // Don't add any edges
            return;
        }

        if (possibleCount == 1) {
            // move
            len++;
            if (northPossible) {
                row--;
                dir = Dir.N;
            } else if (southPossible) {
                row++;
                dir = Dir.S;
            } else if (eastPossible) {
                col++;
                dir = Dir.E;
            } else {
                col--;
                dir = Dir.W;
            }

            if (row == goal.Point.R && col == goal.Point.C) {
                // Reached the goal
                parent.Neighbors.Add((goal, len));
                // No need to add a back link;
                return;
            }
        } else {
            // choice!
            
            // See if the current position is already known as a node
            // and make a new node if not
            var p = new Point(row, col);
            var found = nodeIndex.TryGetValue(p, out var existingNode);
            Node2 node;
            if (found) {
                node = existingNode;
            } else {
                node = new Node2() {Point = p};
                nodeIndex.Add(p, node);
            }
            parent.Neighbors.Add((node, len));
            node.Neighbors.Add((parent, len));

            if (!found) {
            
                if (northPossible) {
                    ExploreMap2(map, row-1, col, Dir.N, node, goal, nodeIndex);
                }
                if (southPossible) {
                    ExploreMap2(map, row+1, col, Dir.S, node, goal, nodeIndex);
                }
                if (eastPossible) {
                    ExploreMap2(map, row, col+1, Dir.E, node, goal, nodeIndex);
                }
                if (westPossible) {
                    ExploreMap2(map, row, col-1, Dir.W, node, goal, nodeIndex);
                }
            }

            return;
        }
    }
}

record Node {
    public int Len;
    public List<Node> Next = new();
}

record Node2 {
    public Point Point;
    public HashSet<(Node2 Node, int Weight)> Neighbors = [];
}


enum Dir {
    N,S,E,W
}

record struct Point(int R, int C);

