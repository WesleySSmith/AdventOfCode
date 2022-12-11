//using MoreLinq;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Numerics;

public class Day18 {

    public static void Main() {
        string[] lines = File.ReadAllLines("input.txt"); 
        //string[] lines = File.ReadAllLines("sample.txt"); 
        Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");
        var sw = Stopwatch.StartNew();
     
        Console.Out.WriteLine($"Parse time: {sw.ElapsedMilliseconds}");

        Part1(lines);
        Part2(lines);

        Console.Out.WriteLine($"Total time {sw.ElapsedMilliseconds}");
    }

    static void Part1(string[] nodeStrings) {
        var nodes = nodeStrings.Select(ParseString);
        var soFar = nodes.First();
        foreach (var node in nodes.Skip(1)) {
            soFar = Add(soFar, node);
        }
        var mag = Magnitude(soFar);
        Console.Out.WriteLine($"Magnitude: {mag}");
    }

    static void Part2(string[] nodeStrings) {
        int maxMag = 0;
        foreach(var n1s in nodeStrings) {
            foreach (var n2s in nodeStrings) {
                if (n1s != n2s) {
                    var n1 = ParseString(n1s);
                    var n2 = ParseString(n2s);
                    var mag = Magnitude(Add(n1, n2));
                    maxMag = Math.Max(maxMag, mag);
                }
            }
        }
        Console.Out.WriteLine($"Max Magnitude: {maxMag}");
    }

    public static Node ParseString(string s) {

        if (!s.Contains('[')) {
            return new Node(int.Parse(s));
        }

        int depth = 0;
        for (var i = 1; ; i++) {
            switch (s[i]) {
                case '[':
                    depth++;
                    break;
                case ']':
                    depth--;
                    break;
                case ',':
                    if (depth == 0) {
                        return new Node(ParseString(s[1..i]), ParseString(s[(i+1)..^1]));
                    }
                    break;
            }
        }

        throw new Exception("Parse error");
    }

    public static int Magnitude(Node n) {
        if (n.Value.HasValue) {
            return n.Value.Value;
        }
        return 3 * Magnitude(n.L) + 2 * Magnitude(n.R);
    }

    public static string NodeToString(Node n) {
        if (n.Value.HasValue) {
            return n.Value.ToString();
        }
        return $"[{NodeToString(n.L)},{NodeToString(n.R)}]";
    }

    public static Node Add(Node l, Node r) {
        Node n = new(l, r);
        Reduce(n);
        return n;
    }

    public static void Reduce(Node n) {
        bool changed;
        do {
            changed = Explode(n);
            if (!changed) {
                changed = Split(n);
            }
        }
        while (changed);
    }

    public static bool Explode(Node root) {
        var list = FlattenValueNodes(root);
        var explodeIndex = list.FindIndex(n => n.depth == 5);
        if (explodeIndex != -1) {
            var entryToExplode = list[explodeIndex];
            var nodeToExplode = entryToExplode.parent;
            var lval = nodeToExplode.L.Value;
            var rval = nodeToExplode.R.Value;
            if (explodeIndex != 0) {
                list[explodeIndex-1].node.Value += lval;
            }
            if (explodeIndex != list.Count -2) {
                list[explodeIndex+2].node.Value += rval;
            }
            nodeToExplode.Value = 0;
            nodeToExplode.L = null;
            nodeToExplode.R = null;
            return true;
        }
        return false;
    }

     public static bool Split(Node n) {
        var list = FlattenValueNodes(n);
        var splitIndex = list.FindIndex(n => n.node.Value >= 10);
        if (splitIndex != -1) {
            var entryToSplit = list[splitIndex];
            var valueToSplit = entryToSplit.node.Value.Value;
            var lval = valueToSplit / 2;
            var rval = (valueToSplit + 1) / 2;

            entryToSplit.node.L = new Node(lval);
            entryToSplit.node.R = new Node(rval);
            entryToSplit.node.Value = null;
            return true;
        }
        return false;
    }

    public static List<(Node node, int depth, Node parent, bool isLeftChild)> FlattenValueNodes(Node root) {
        List<(Node node, int depth, Node parent, bool isLeftChild)> nodeList = new();
        Stack<(Node node, int depth, Node parent, bool isLeftChild)> stack = new();
        stack.Push((root, 0, null, false));
        while (stack.Any()) {
            var top = stack.Pop();
            if (top.node.Value == null) {
                stack.Push((top.node.R, top.depth+1, top.node, false));
                stack.Push((top.node.L, top.depth+1, top.node, true));
            } else {
                nodeList.Add(top);
            }
        }
        return nodeList;
    }

    public class Node {
        public int? Value;
        public Node L;
        public Node R;

        public Node (int v)
        {
            Value = v;
        }

        public Node (int l, int r)
        {
            L = new Node(l);
            R = new Node(r);
        }

        public Node (Node l, Node r) {
            L = l;
            R = r;
        }
    }
   

    
   
}
