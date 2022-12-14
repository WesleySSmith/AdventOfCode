#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Diagnostics;
using MoreLinq;

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

var pairs = lines
    .GroupAdjacent(l => l.Length > 0)
    .Where(group => group.Key)
    .Select(l => l.Select(m => Parse2(m)).ToArray()).ToArray();

//Part1(pairs);
Part2(pairs);

Console.Out.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");


static void Part1(ElementList[][] pairs) {

    var inOrderPairs = pairs.Select(pair => InOrder(pair[0], pair[1])).ToList();
    var sumInOrderPairs = inOrderPairs
        .Select((inOrder, i) => (inOrder, i))
        .Where(inOrderWithIndex => inOrderWithIndex.Item1.Value)
        .Select(inOrderWithIndex => inOrderWithIndex.Item2 + 1)
        .Sum();

    Console.WriteLine($"Part 1 In Order Pairs: {sumInOrderPairs}");
}

static void Part2(ElementList[][] pairs) {

    var divider1 = new ElementList();
    var divider1a = new ElementList();
    divider1a.Children.Add(new Literal() {Value = 2});
    divider1.Children.Add(divider1a);

    var divider2 = new ElementList();
    var divider2a = new ElementList();
    divider2a.Children.Add(new Literal() {Value = 6});
    divider2.Children.Add(divider2a);

    var list = pairs.SelectMany(pair => pair).Concat(new [] {divider1, divider2});

    var sorted = list.OrderBy(l => l, Comparer<ElementList>.Create((a,b) => InOrder(a,b).Value ? -1 : 1)).ToList();

    var decoder = (sorted.IndexOf(divider1) + 1) * (sorted.IndexOf(divider2) + 1);

    Console.WriteLine($"Part 2 Decoder: {decoder}");
}

static bool? InOrder(Element l, Element r) {


    if (l is Literal ll && r is Literal rl) {
        if (ll.Value < rl.Value) {
            return true;
        }
        if (ll.Value > rl.Value) {
            return false;
        }
        return null;
    }

    if (l is ElementList lell && r is ElementList rell) {
        for (var child = 0; child < lell.Children.Count; child++) {
            if (rell.Children.Count < child + 1 )
            {
                return false;
            }

            var lel = lell.Children[child];
            var rel = rell.Children[child];
            var inOrder = InOrder(lel, rel);
            if (inOrder.HasValue) {
                return inOrder.Value;
            }
        }
        if (rell.Children.Count > lell.Children.Count) {
            return true;
        }
        return null;
    }

    if (l is Literal ll2) {
        var replacementList = new ElementList();
        replacementList.Children.Add(ll2);
        return InOrder(replacementList, r);
    }

    if (r is Literal rl2) {
        var replacementList = new ElementList();
        replacementList.Children.Add(rl2);
        return InOrder(l, replacementList);
    }
    return null;
}


static ElementList Parse2(string s) {
    return Parse(s, 0).Item1;
}
static (ElementList, int) Parse(string s, int pos) {

    var root = new ElementList();

    while (pos < s.Length) {
        if (s[pos] == '[') {

            (var list, pos) = Parse(s, pos+1);
            root.Children.Add(list);
        }
        else if (s[pos] == ']') {
            return (root, pos + 1);
        } else if (s[pos] >= '0' && s[pos] <= '9') {
            var digitCount = 1;
            while (s[pos+digitCount] >= '0' && s[pos+digitCount] <= '9') {
                digitCount++;
            }
            root.Children.Add(new Literal() {Value = int.Parse(s[pos..(pos+digitCount)])});
            pos += digitCount;
        }  else if (s[pos] == ',') {
            pos++;
        }
    }
    return (root, pos);
}

public class Element {
}

public class ElementList : Element {
    public List<Element> Children {get;} = new List<Element>();
}

public class Literal : Element {
    public int Value {get; init;}
}