#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Text;
//using MoreLinq;

bool sample = false;
bool debug = true;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt"); 
if (debug) Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw;

var list = lines.Select(int.Parse);


sw = Stopwatch.StartNew();
//Part1(list);
//Console.Out.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");
sw = Stopwatch.StartNew();
Part2(list);
Console.Out.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");

void Part1(IEnumerable<int> list) {
    var ll = new LinkedList<int>();

    List<LinkedListNode<int>> originalOrder = new();

    foreach (int i in list) {
        originalOrder.Add(ll.AddLast(i));
    }

    //Console.WriteLine(string.Join(", ", ll));
    var index = 0;
    
    foreach (var lln in originalOrder) {
        var c = 0;
        var v = lln.Value;
        var p = lln;


        //var startIndex = ll.Select((vv,i) => (vv,i)).First(e => e.vv == v);


        if (v > 0) {
            c++;
            p = p.Next;
            if (p == null) {
                p = ll.First;
            }
            ll.Remove(lln);
            for (int ii = 1; ii < v; ii++) {
                c++;
                p = p.Next;
                if (p == null) {
                    p = ll.First;
                }
            }
            if (p == ll.Last) {
                ll.AddFirst(v);
            } else {
                ll.AddAfter(p, v);
            }
            
            
        } else if (v < 0) {
            c++;
            p = p.Previous;
            if (p == null) {
                p = ll.Last;
            }
            ll.Remove(lln);
            for (int ii = 1; ii < 0 - v; ii++) {
                c++;
                p = p.Previous;
                if (p == null) {
                    p = ll.Last;
                }
            }
            if (p == ll.First) {
                ll.AddLast(v);
            } else {
                ll.AddBefore(p, v);
            }
        }

        //Console.WriteLine($"[{index}] Loops: {c}");

        //var endIndex = ll.Select((vv,i) => (vv,i)).First(e => e.vv == v);


        index++;
        //if (index < 6) {
        //    Console.WriteLine(string.Join("\n", ll.Chunk(30).Select(chunk => string.Join(", ", chunk))));
        //    Console.WriteLine();
        //}
    }


    var zeroIndex = ll.Select((v,i) => (v,i)).Single(e => e.v == 0).i;
    var n = ll.Count;


    var v1000 = ll.ElementAt((zeroIndex + 1000) % n);
    var v2000 = ll.ElementAt((zeroIndex + 2000) % n);
    var v3000 = ll.ElementAt((zeroIndex + 3000) % n);

    Console.WriteLine($"Part 1 Sum: {v1000 + v2000 + v3000}");
}


void Part2(IEnumerable<int> list) {
    var ll = new LinkedList<long>();

    List<LinkedListNode<long>> originalOrder = new();

    foreach (int i in list) {
        originalOrder.Add(ll.AddLast((long)i * 811589153L));
    }

    //Console.WriteLine(string.Join(", ", ll));
    var index = 0;
    
    for (var loop = 0; loop < 10; loop++) {

        foreach (var lln in originalOrder) {
            var c = 0;
            var v = lln.Value;
            var p = lln;


            //var startIndex = ll.Select((vv,i) => (vv,i)).First(e => e.vv == v);

            var vv = (short)(Math.Abs(v) < ll.Count() ? v : ((v < 0 ? -1 : 1) * Math.Abs(v) % (ll.Count() - 1)));
            if (vv > 0) {
                c++;
                p = p.Next;
                if (p == null) {
                    p = ll.First;
                }
                ll.Remove(lln);
                for (short ii = 1; ii < vv; ii++) {
                    c++;
                    p = p.Next;
                    if (p == null) {
                        p = ll.First;
                    }
                }
                if (p == ll.Last) {
                    ll.AddFirst(lln);
                } else {
                    ll.AddAfter(p, lln);
                }
                
                
            } else if (vv < 0) {
                c++;
                p = p.Previous;
                if (p == null) {
                    p = ll.Last;
                }
                ll.Remove(lln);
                for (short ii = 1; ii < 0 - vv; ii++) {
                    c++;
                    p = p.Previous;
                    if (p == null) {
                        p = ll.Last;
                    }
                }
                if (p == ll.First) {
                    ll.AddLast(lln);
                } else {
                    ll.AddBefore(p, lln);
                }
            }

            //var endIndex = ll.Select((vv,i) => (vv,i)).First(e => e.vv == v);

            //Console.WriteLine($"[{index}] Loops: {c}");

            index++;
            //if (index < 6) {
            //    Console.WriteLine(string.Join("\n", ll.Chunk(30).Select(chunk => string.Join(", ", chunk))));
            //    Console.WriteLine();
            //}
        }
    }


    var zeroIndex = ll.Select((v,i) => (v,i)).Single(e => e.v == 0).i;
    var n = ll.Count;


    var v1000 = ll.ElementAt((zeroIndex + 1000) % n);
    var v2000 = ll.ElementAt((zeroIndex + 2000) % n);
    var v3000 = ll.ElementAt((zeroIndex + 3000) % n);

    Console.WriteLine($"Part 2 Sum: {v1000 + v2000 + v3000}");
}



