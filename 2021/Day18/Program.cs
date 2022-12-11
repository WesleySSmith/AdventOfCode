//using MoreLinq;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Numerics;

public class Day18 {

    public static void Main() {

        //var file = File.ReadAllText("sample.txt");
        //var file = File.ReadAllText("gavin.txt");
        var file = File.ReadAllText("input.txt");
        //string[] lines = File.ReadAllLines("input.txt"); 
        //string[] lines = File.ReadAllLines("sample.txt"); 
        //Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");
        var sw = Stopwatch.StartNew();
     
        var scanners = file.Split("\r\n\r\n").Select(s => s.Split("\r\n").Skip(1).Select(l => new XYZ(l.Split(',').Select(int.Parse))).ToArray()).ToArray();

        Console.Out.WriteLine($"Parse time: {sw.ElapsedMilliseconds}");

        List<Scanner> scannerList = new();
        int n = 0;
        foreach (var scanner in scanners) {
            var s = new Scanner() {Number = n++};
            scannerList.Add(s);
            var orientations = scanner.Select(p => Orientations(p)).ToArray();
        
            for (var ii = 0 ; ii < 24; ii++) {
                s.Orientations[ii] = new Beacons();
                s.Orientations[ii].BeaconPositions = new XYZ[orientations.Length];
                for (var jj = 0; jj < orientations.Length; jj++) {
                    s.Orientations[ii].BeaconPositions[jj] = orientations[jj][ii];
                }
            }
        }

        //foreach (var s in scannerList) {
        //    Console.Out.WriteLine(s.ToString());
        //}

        // var overlaps = Overlap(scannerList[0], scannerList[1]);
        // if (overlaps != null) {
        //     Console.Out.WriteLine($"Overlaps: {overlaps.Count} ({string.Join(" ; ", overlaps.Select(o => o.ToString()))})");
        // } else {
        //     Console.Out.WriteLine("No overlaps");
        // }


        //Console.Out.WriteLine(string.Join("\n", Orientations(test).Select(o => o.ToString())));

        //Part1(scannerList);
        Part2(scannerList);
        Console.Out.WriteLine($"Total time {sw.ElapsedMilliseconds}");
    }

    class Scanner {
        public int Number;
        public int? GlobalOrientation;
        public List<(Scanner scanner, XYZ translation)> Neighbors = new();
        public Beacons[] Orientations = new Beacons[24];
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Scanner {Number}: \n");
            int c = 1;
            foreach (var b in Orientations) {
                sb.Append($"\n\tOrientation {c++}: \n\t\t");
                sb.Append(string.Join("\n\t\t", b.BeaconPositions.Select(o => o.ToString())));
            }
            return sb.ToString();
        }
    }

    class Beacons {
        public XYZ[] BeaconPositions;
    }

    static void Part1(List<Scanner> scanners) {
        var scanner0 = scanners[0];
        scanner0.GlobalOrientation = 0;
        scanners.Remove(scanner0);
        FindNeighbors(scanner0, scanners);
        var points = MergePoints(scanner0, new XYZ(0,0,0));
        var pointHash = points.Distinct();
        Console.Out.WriteLine($"Num overlaps: {pointHash.Count()}");
        foreach(var point in pointHash.OrderBy(p => p.X).ThenBy(p => p.Y).ThenBy(p => p.Z)) {
            //Console.Out.WriteLine(point.ToString());
        }
    }

    static void Part2(List<Scanner> scanners) {
         var scanner0 = scanners[0];
        scanner0.GlobalOrientation = 0;
        scanners.Remove(scanner0);
        FindNeighbors(scanner0, scanners);

        List<XYZ> translations = GraphToList(scanner0, new XYZ(0,0,0));
        int maxManhattan = 0;
        foreach (var tA in translations) {
            foreach (var tB in translations) {
                var manhattan = Math.Abs(tA.X - tB.X) + Math.Abs(tA.Y - tB.Y) + Math.Abs(tA.Z - tB.Z); 
                maxManhattan = Math.Max(manhattan, maxManhattan);
            }
        }

        Console.Out.WriteLine($"Max Manhattan: {maxManhattan}");
    }

    static List<XYZ> GraphToList(Scanner s, XYZ globalTranslation) {
        var list = new List<XYZ> {globalTranslation};

        foreach (var neighborData in s.Neighbors) {
            list.AddRange(GraphToList(neighborData.scanner, neighborData.translation.Add(globalTranslation)));
        }
        return list;
    }

    static void FindNeighbors(Scanner s, List<Scanner> possibleNeighbors) {
         foreach(var scanner in possibleNeighbors) {
            var overlap = Overlap(s, scanner);
            if (overlap.translation != null) {
                scanner.GlobalOrientation = overlap.orientation;
                s.Neighbors.Add((scanner, overlap.translation));
            }
        }
        possibleNeighbors.RemoveAll(n => s.Neighbors.Select(nn=> nn.scanner).Contains(n));
        foreach (var neighbor in s.Neighbors) {
            FindNeighbors(neighbor.scanner, possibleNeighbors);
        }
    }

    static List<XYZ> MergePoints(Scanner s, XYZ globalTranslation) {
        var list = new List<XYZ>();
        list.AddRange(s.Orientations[s.GlobalOrientation.Value].BeaconPositions.Select(p => p.Add(globalTranslation)));

        foreach (var neighborData in s.Neighbors) {
            list.AddRange(MergePoints(neighborData.scanner, neighborData.translation.Add(globalTranslation)));
        }
        return list;
    }

    

    static (XYZ translation, int orientation) Overlap(Scanner a, Scanner b) {
        for (var oB = 0; oB < 24; oB++) {
            var overlap = Overlap(a.Orientations[a.GlobalOrientation.Value], b.Orientations[oB]);
            if (overlap != null) {
                return (overlap, oB);
            }
        }
        return (null, 0);
    }

    static XYZ Overlap(Beacons a, Beacons b) {
        foreach (var pA in a.BeaconPositions) {
            foreach (var pB in b.BeaconPositions) {
                var matchesCount = 0;
                List<(XYZ, XYZ)> matches = new();
                var diff = pA.Difference(pB);
                
                foreach (var pA2 in a.BeaconPositions) {
                    foreach (var pB2 in b.BeaconPositions) {
                        if (pB2.SameAsTranslated(pA2, diff)) {
                            matchesCount++;
                            matches.Add((pA2, pB2));
                            if (matchesCount >= 12) {
                                return diff;
                            }
                        }
                    }
                }
            }
        }
        return null;
    }

    static XYZ[] Orientations (XYZ o) {
        var x = o.X;
        var y = o.Y;
        var z = o.Z;
        return new [] {
            new XYZ(x, y, z),
            new XYZ(x, z, -y),
            new XYZ(x, -y, -z),
            new XYZ(x, -z, y),

            new XYZ(-x, -y, z),
            new XYZ(-x, z, y),
            new XYZ(-x, y, -z),
            new XYZ(-x, -z, -y),

            new XYZ(y,-x, z),
            new XYZ(y,-z, -x),
            new XYZ(y, x, -z),
            new XYZ(y, z, x),

            new XYZ(-y, z, -x),
            new XYZ(-y, x, z),
            new XYZ(-y, -z, x),
            new XYZ(-y, -x, -z),

            new XYZ(z, -y, x),
            new XYZ(z, -x, -y),
            new XYZ(z, y, -x),
            new XYZ(z, x, y),

            new XYZ(-z, y, x),
            new XYZ(-z, -x, y),
            new XYZ(-z, -y, -x),
            new XYZ(-z, x, -y),

        };

    }

    class XYZ {
        public int X;
        public int Y;
        public int Z;

        public XYZ(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public XYZ(IEnumerable<int> e) {
            X = e.ElementAt(0);
            Y = e.ElementAt(1);
            Z = e.ElementAt(2);
        }

        public override string ToString()
        {
            return $"{X},{Y},{Z}";
        }

        public XYZ Difference(XYZ other) {
            return new XYZ(X - other.X, Y - other.Y, Z - other.Z);
        }

        public bool SameAsTranslated(XYZ other, XYZ offset) {
            return this.X + offset.X == other.X && this.Y + offset.Y == other.Y && this.Z + offset.Z == other.Z;
        }

        public XYZ Add(XYZ other) {
            return new XYZ(X + other.X, Y + other.Y, Z + other.Z);
        }

        public override bool Equals(object obj)
        {
            var other = (XYZ)obj;
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
        }
    }

    

   

    
   
}
