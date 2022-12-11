//using MoreLinq;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Numerics;

public class Day11 {

    public static void Main() {

        string[] lines = File.ReadAllLines("input.txt"); 
        //string[] lines = File.ReadAllLines("sample.txt"); 
        Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");
        var sw = Stopwatch.StartNew();

        var edges = lines.Select(l => (l.Split('-')[0], l.Split('-')[1]));


        
        Console.Out.WriteLine($"Parse time: {sw.ElapsedMilliseconds}");
        //Part1(edges);
        Part2c(edges, sw);

        Console.Out.WriteLine($"Total time {sw.ElapsedMilliseconds}");
    }

    static void Part1(IEnumerable<(string, string)> edges) {
       
        var nodes = edges
            .SelectMany(e => new[] {e.Item1, e.Item2})
            .Distinct()
            .Select(n => new Node(n))
            .ToDictionary(n => n.Name);

        foreach (var edge in edges) {
            nodes[edge.Item1].Edges.Add(nodes[edge.Item2]);
            nodes[edge.Item2].Edges.Add(nodes[edge.Item1]);
        }

        List<List<Node>> paths = new List<List<Node>>();
        var stack = new Stack<List<Node>>();
        var startNode = nodes["start"];
        var endNode = nodes["end"];

        stack.Push(new List<Node> {startNode});
        
        while (stack.Any()) {
            var path = stack.Pop();
            var leaf = path.Last();
            if (leaf == endNode) {
                // Path reached end
                paths.Add(path);
            } else if (leaf.IsSmall && path.Count(l => l == leaf) >= 2) {
                // Not a legal path
            } else {
                // Create new path for each edges
                var newPaths = leaf.Edges.Select(e => new List<Node>(path.Concat(new [] {e})));;
                foreach(var newPath in newPaths) {
                    stack.Push(newPath);
                }
            }
        }

        Console.Out.WriteLine($"Paths: {paths.Count()}");
    }

    static void Part2(IEnumerable<(string, string)> edges, Stopwatch sw) {
       var nodes = edges
            .SelectMany(e => new[] {e.Item1, e.Item2})
            .Distinct()
            .Select(n => new Node(n))
            .ToDictionary(n => n.Name);

        foreach (var edge in edges) {
            if (edge.Item2 != "start") {
                nodes[edge.Item1].Edges.Add(nodes[edge.Item2]);
            }
            if (edge.Item1 != "start") {
                nodes[edge.Item2].Edges.Add(nodes[edge.Item1]);
            }
        }

        var pathsCount = 0;
        var stack = new Stack<List<Node>>();
        var startNode = nodes["start"];
        var endNode = nodes["end"];

        stack.Push(new List<Node> {startNode});
        
        Console.Out.WriteLine($"Starting search: {sw.ElapsedMilliseconds}");
        while (stack.Any()) {
            var path = stack.Pop();
            var current = path.Last();
            
            // Create new path for each edges
            var adjacents = current.Edges;
            foreach(var adjacent in adjacents) {
               
                if (adjacent == endNode) {
                    pathsCount++;
                    continue;
                }
                var newPath = new List<Node>(path);
                newPath.Add(adjacent);
                if (adjacent.IsSmall) {
                    var grp = newPath.Where(n => n.IsSmall).GroupBy(n => n.Name);
                    if (grp.Count(g => g.Count() == 2) == 2 || grp.Any(g => g.Count() > 2)) {
                        // Not a legal path
                        continue;
                    }
                }
                stack.Push(newPath);
            }
        }

        Console.Out.WriteLine($"Paths: {pathsCount}");
    }

    static void Part2a(IEnumerable<(string, string)> edges, Stopwatch sw) {
       var nodes = edges
            .SelectMany(e => new[] {e.Item1, e.Item2})
            .Distinct()
            .Select(n => new Node(n))
            .ToDictionary(n => n.Name);

        foreach (var edge in edges) {
            if (edge.Item2 != "start") {
                nodes[edge.Item1].Edges.Add(nodes[edge.Item2]);
            }
            if (edge.Item1 != "start") {
                nodes[edge.Item2].Edges.Add(nodes[edge.Item1]);
            }
        }

        var pathsCount = 0;
        var stack = new Stack<NodeList>();
        var startNode = nodes["start"];
        var endNode = nodes["end"];

        var startingNodeList = new NodeList {Node = startNode, Prev = null};

        stack.Push(startingNodeList);
        
        Console.Out.WriteLine($"Starting search: {sw.ElapsedMilliseconds}");
        while (stack.Any()) {
            var path = stack.Pop();
            var current = path.Node;
            
            // Create new path for each edges
            var adjacents = current.Edges;
            foreach(var adjacent in adjacents) {
               
                if (adjacent == endNode) {
                    pathsCount++;
                    continue;
                }
                var newPath = new NodeList() {Node = adjacent, Prev = path};
                if (adjacent.IsSmall) {
                    var newPathAsList = new List<Node>();
                    var n = newPath;
                    while (n != null) {
                        newPathAsList.Add(n.Node);
                        n = n.Prev;
                    }
                    var grp = newPathAsList.Where(n => n.IsSmall).GroupBy(n => n.Name);
                    if (grp.Count(g => g.Count() == 2) == 2 || grp.Any(g => g.Count() > 2)) {
                        // Not a legal path
                        continue;
                    }
                }
                stack.Push(newPath);
            }
        }

        Console.Out.WriteLine($"Paths: {pathsCount}");
    }

    static void Part2b(IEnumerable<(string, string)> edges, Stopwatch sw) {
       var nodes = edges
            .SelectMany(e => new[] {e.Item1, e.Item2})
            .Distinct()
            .Select(n => new Node(n))
            .ToDictionary(n => n.Name);

        foreach (var edge in edges) {
            if (edge.Item2 != "start") {
                nodes[edge.Item1].Edges.Add(nodes[edge.Item2]);
            }
            if (edge.Item1 != "start") {
                nodes[edge.Item2].Edges.Add(nodes[edge.Item1]);
            }
        }

        var pathsCount = 0;
        var stack = new Stack<NodeList>();
        var startNode = nodes["start"];
        var endNode = nodes["end"];

        var startingNodeList = new NodeList {Node = startNode, Prev = null};

        stack.Push(startingNodeList);
        
        Console.Out.WriteLine($"Starting search: {sw.ElapsedMilliseconds}");
        while (stack.Any()) {
            var path = stack.Pop();
            var current = path.Node;
            
            // Create new path for each edges
            var adjacents = current.Edges;
            foreach(var adjacent in adjacents) {
               
                if (adjacent == endNode) {
                    pathsCount++;
                    continue;
                }
                var newPath = new NodeList() {Node = adjacent, Prev = path};
                if (adjacent.IsSmall) {

                    var freq = new Dictionary<string, int>();
                    var n = newPath;
                    bool notLegal = false;
                    while (n != null) {
                        if (n.Node.IsSmall) {
                            if (freq.TryGetValue(n.Node.Name, out var old)) {
                                freq[n.Node.Name] = old + 1;
                                if (old + 1 == 3) {
                                    notLegal = true;
                                    break;
                                }
                            } else {
                                freq[n.Node.Name] = 1;
                            }
                        }
                        n = n.Prev;
                    }
                    if (notLegal || freq.Values.Count(v => v == 2) == 2) {
                        continue;
                    }
                }
                stack.Push(newPath);
            }
        }

        Console.Out.WriteLine($"Paths: {pathsCount}");
    }


    static void Part2c(IEnumerable<(string, string)> edges, Stopwatch sw) {
        int c = 0;
       var nodes = edges
            .SelectMany(e => new[] {e.Item1, e.Item2})
            .Distinct()
            .Select(n => new Node(n) {Index = c++})
            .ToDictionary(n => n.Name);

        foreach (var edge in edges) {
            if (edge.Item2 != "start") {
                nodes[edge.Item1].Edges.Add(nodes[edge.Item2]);
            }
            if (edge.Item1 != "start") {
                nodes[edge.Item2].Edges.Add(nodes[edge.Item1]);
            }
        }

        var pathsCount = 0;
        var stack = new Stack<NodeList>();
        var startNode = nodes["start"];
        var endNode = nodes["end"];

        var startingNodeList = new NodeList {Node = startNode, Prev = null, Freq = new int[nodes.Count()]};

        stack.Push(startingNodeList);
        
        Console.Out.WriteLine($"Starting search: {sw.ElapsedMilliseconds}");
        while (stack.Any()) {
            var path = stack.Pop();
            var current = path.Node;
            
            // Create new path for each edges
            var adjacents = current.Edges;
            foreach(var adjacent in adjacents) {
               
                if (adjacent == endNode) {
                    pathsCount++;
                    continue;
                }
                var newPath = new NodeList() {Node = adjacent, Prev = path, Freq = (int[])path.Freq.Clone()};

                if (adjacent.IsSmall) {
                    var freq = newPath.Freq;
                    var newFreq = ++freq[adjacent.Index];
                    if (newFreq == 3) {
                        goto skip;
                    };
                    if (newFreq == 2) {
                        for (int ii = 0; ii < freq.Length; ii++) {
                            if (ii != adjacent.Index && freq[ii] == 2) {
                                goto skip;
                            }
                        }
                    }
                }
                stack.Push(newPath);
                skip:;
            }
        }

        Console.Out.WriteLine($"Paths: {pathsCount}");
    }

    
    public class Node {
        public string Name;
        public List<Node> Edges = new List<Node>();
        public bool IsSmall;

        public int Index;

        public Node(string name)
        {
            Name = name;
            IsSmall = char.IsLower(Name[0]);
        }        
    }

    public class NodeList {
        public Node Node;
        public NodeList Prev;

        public int[] Freq;
        
    }
}

















