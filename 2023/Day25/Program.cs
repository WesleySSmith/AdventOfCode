#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
//using MoreLinq;


bool sample = false;
bool debug = false;


string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();


Part1(lines);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(string[] lines)
{
    
    var nodesAndNeighbors = lines.Select(line => {
        var splits = line.Split(':');
        var node = splits[0];
        var neighbors = splits[1].Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        return (Node: node, Neighbors: neighbors);
    }).ToList();

    var nodeDict = nodesAndNeighbors.Select(l => l.Node)
        .Concat(nodesAndNeighbors.SelectMany(l => l.Neighbors))
        .Distinct()
        .Select(n => new Node {Name = n})
        .ToDictionary(n => n.Name);

    Console.WriteLine($"Found {nodeDict.Count} nodes");
    foreach (var nodeAndNeighbors in nodesAndNeighbors) {
        foreach (var neighbor in nodeAndNeighbors.Neighbors) {
            nodeDict[nodeAndNeighbors.Node].Neighbors.Add(nodeDict[neighbor], 1);
            nodeDict[neighbor].Neighbors.Add(nodeDict[nodeAndNeighbors.Node], 1);
        }
    }

    var outerGraph = nodeDict;

    var randomNode = nodeDict.Values.First();
    Node lastNodeAdded = null;
    Node penultimateNodeAdded = null;
    var outerLoopCount = 0;
    while (true) {
        outerLoopCount++;
        Console.WriteLine($"Outer loop {outerLoopCount}");
        //Node supernode = new Node {Name = "S" + randomNode.Name};
        lastNodeAdded = null;
        penultimateNodeAdded = null;
        
        HashSet<Node> superset = new();
        superset.Add(randomNode);
        while (true) {
            if (debug) Console.WriteLine($"Inner loop with superset count {superset.Count}");


            var allSupersetNeighbors = superset.SelectMany(n => n.Neighbors)
                .Where(neighbor => !superset.Contains(neighbor.Key));
            if (!allSupersetNeighbors.Any()) {
                break;
            }
            var largestWeightNeighbor = allSupersetNeighbors
                .GroupBy(n => n.Key)
                .MaxBy(g => g.Sum(n => n.Value))
                .Key;

            superset.Add(largestWeightNeighbor); 
            if (debug) Console.WriteLine($"Adding to superset: {largestWeightNeighbor.Name}");   

            penultimateNodeAdded = lastNodeAdded;
            lastNodeAdded = largestWeightNeighbor;
        }

        if (lastNodeAdded.Neighbors.Sum(n => n.Value) == 3) {

            superset.Remove(lastNodeAdded);

              

            var selectedNodeCount = superset.Sum(n => n.OriginalNodeCount);
            var product = selectedNodeCount * (nodeDict.Count - selectedNodeCount);
            Console.Out.WriteLine($"selectedNodeCount: {selectedNodeCount} -  num total nodes: {nodeDict.Count} -  product is {product}");

            Console.WriteLine($"Count: {superset.Count}, Sum: {superset.Sum(n => n.OriginalNodeCount)}");
            if (superset.Count != superset.Sum(n => n.OriginalNodeCount)) {
                Console.WriteLine("***WARNING");
            }  


            break;
        } else {
            if (debug) Console.WriteLine($"Merging {lastNodeAdded.Name} with {penultimateNodeAdded.Name}");
            MergeNodes(penultimateNodeAdded, lastNodeAdded);
        }

        // while (supernode.Neighbors.Count > 1) {
        //     // Add the node with the highest weight to the supernode
        //     var maxWeightNeighbor = supernode.Neighbors.MaxBy(n => n.Value).Key;

        //     penultimateNodeAdded = lastNodeAdded;
        //     lastNodeAdded = maxWeightNeighbor;
        //     MergeNodes(supernode, maxWeightNeighbor);
        // }
    }

    //var selectedNodeCount = lastNodeAdded.Neighbors.Keys.Sum(n => n.OriginalNodeCount);
   

}

void MergeNodes(Node a, Node b) {
    //a.Name += "-" + b.Name;
    a.OriginalNodeCount += b.OriginalNodeCount;
    a.Neighbors.Remove(b);
    b.Neighbors.Remove(a);

    foreach (var bNeighbor in b.Neighbors) {
        if (bNeighbor.Key.Neighbors.TryGetValue(a, out var existingWeight))  {
            bNeighbor.Key.Neighbors[a] = existingWeight + bNeighbor.Value;
            a.Neighbors[bNeighbor.Key] = existingWeight + bNeighbor.Value;
        } else {
            bNeighbor.Key.Neighbors.Add(a, bNeighbor.Value);
            a.Neighbors.Add(bNeighbor.Key, bNeighbor.Value);
        }
        bNeighbor.Key.Neighbors.Remove(b);
    }
}

void Part2(string[] lines)
{


}


class Node {
    public string Name;
    public int OriginalNodeCount = 1;
    public Dictionary<Node,int> Neighbors = new();
}