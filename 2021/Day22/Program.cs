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
     
        var instructions = lines.Select(l => {
            var  parts = l.Split(" ");
            var on = parts[0] == "on";
            var coords = parts[1].Split(",").Select(s => s.Split("=")[1].Split("..").Select(int.Parse).ToArray()).ToArray();
            return new Instruction {
                On = parts[0] == "on",
                minX = coords[0][0],
                maxX = coords[0][1],
                minY = coords[1][0],
                maxY = coords[1][1],
                minZ = coords[2][0],
                maxZ = coords[2][1]};
        }).ToArray();


        Console.Out.WriteLine($"Parse time: {sw.ElapsedMilliseconds}");

        //Part1(instructions);
        Part2(instructions);

        Console.Out.WriteLine($"Total time {sw.ElapsedMilliseconds}");
    }

    static void Part1(Instruction[] instructions) {
        
        for (var xx = 0; xx < instructions.Count(); xx++) {
            var a = new bool[101, 101, 101];
            SafeArray sa = new SafeArray(a, 50, 50, 50);

            foreach (var i in instructions.Take(xx+1)) {
                for (long x = Math.Max(-48, i.minX) ; x <= Math.Min(-32, i.maxX); x++) {
                    for (long y = Math.Max(26, i.minY) ; y <= Math.Min(41, i.maxY); y++) {
                        for (long z = Math.Max(-47, i.minZ) ; z <= Math.Min(37, i.maxZ); z++) {
                            sa.Set(x, y, z, i.On);
                        }
                    }
                }
            }

            long count = 0;
            for (long x = -50 ; x <= 50; x++) {
                for (long y = -50 ; y <= 50; y++) {
                    for (long z = -50 ; z <= 50; z++) {
                        count += sa.Get(x, y, z) ?? false ? 1 : 0;
                    }
                }
            }

            Console.Out.WriteLine($"[{xx+1}] Count: {count}");
        }
    }

   
    static void Part2(Instruction[] instructions) {
        int globalMinX = int.MaxValue;
        int globalMinY = int.MaxValue;
        int globalMinZ = int.MaxValue;
        int globalMaxX = int.MinValue;
        int globalMaxY = int.MinValue;
        int globalMaxZ = int.MinValue;

        foreach (var i in instructions) {
            globalMinX = Math.Min(globalMinX, i.minX);
            globalMinY = Math.Min(globalMinY, i.minY);
            globalMinZ = Math.Min(globalMinZ, i.minZ);
            globalMaxX = Math.Max(globalMaxX, i.maxX);
            globalMaxY = Math.Max(globalMaxY, i.maxY);
            globalMaxZ = Math.Max(globalMaxZ, i.maxZ);
        }

        var globalCuboid = new Cuboid {
            maxX = globalMaxX,
            minX = globalMinX,
            maxY = globalMaxY,
            minY = globalMinY,
            maxZ = globalMaxZ,
            minZ = globalMinZ
            };

        //globalCuboid = instructions[10];

        Console.Out.WriteLine($"Global Cuboid: {globalCuboid}");
        Console.Out.WriteLine($"Instruction Count: {instructions.Count()}");

        var totalOn = HowManyOn(instructions, instructions.Count() - 1,globalCuboid);
        Console.Out.WriteLine($"Total on: {totalOn}");

        //long totalOn = HowManyOn(instructions, instructions.Length -1,globalCuboid);

        //Console.Out.WriteLine($"Total on: {totalOn}");
        
        
//define reactor cuboid as instruction min/maxes
    //const reactorCuboid

    //HowManyOn(Data, reactorCuboid)
      
    }

    static long HowManyOn(IList<Instruction> a, int currentInstruction, Cuboid cuboid) {
        var instr = a[currentInstruction];
        if (currentInstruction == 0) {
            //base case
            if (instr.On) {
                return instr.Intersect(cuboid).Volume();
            } else {
                return 0;
            }
        }
        else {
            var allOn = HowManyOn(a, currentInstruction-1, cuboid);
            var intersection = cuboid.Intersect(instr);
            var intersectionVolume = intersection.Volume();
            if (intersectionVolume == 0) return allOn;
            var on = HowManyOn(a, currentInstruction-1, intersection);

            if (instr.On) {
                return allOn - on /* unaffected*/ + intersectionVolume;
            }
            else {
                return allOn - on;
            }
        }
    }
        


public class Cuboid {
    public bool Zero;
    public int minX;
    public int maxX;
    public int minY;
    public int maxY;
    public int minZ;
    public int maxZ;

    public long Volume() {
        if (Zero) {
            return 0;
        }
        return (maxX - minX + 1L) * (maxY - minY + 1L) * (maxZ - minZ + 1L);
    }

    override public string ToString() {
        return $"x={minX}..{maxX},y={minY}..{maxY},z={minZ}..{maxZ}";
    }

    public Cuboid Intersect(Cuboid c) {
        var potentialC = new Cuboid {
            maxX = Math.Min(maxX, c.maxX),
            minX = Math.Max(minX, c.minX),
            maxY = Math.Min(maxY, c.maxY),
            minY = Math.Max(minY, c.minY),
            maxZ = Math.Min(maxZ, c.maxZ),
            minZ = Math.Max(minZ, c.minZ)
        };
        if (potentialC.maxX < potentialC.minX || potentialC.maxY < potentialC.minY || potentialC.maxZ < potentialC.minZ ) {
            return new Cuboid {Zero = true};
        }
        return potentialC;
    }
}
    public class Instruction : Cuboid {
        public bool On;

        override public string ToString() {
            return $"{(On ? "on" : "off")} {base.ToString()}";
        }

    }

   public class SafeArray {
        private long Height;
        private long Width;
        private long Depth;
        public bool[,,] Array;

        public long HeightOffset;
        public long WidthOffset;
        public long DepthOffset;

        public bool Default;

        public SafeArray(bool[,,] array, long heightOffset, long widthOffset, long depthOffset)
        {
            this.Height = array.GetLength(0);
            this.Width = array.GetLength(1);
            this.Depth = array.GetLength(2);
            this.Array = array;
            this.HeightOffset = heightOffset;
            this.WidthOffset = widthOffset;
            this.DepthOffset = depthOffset;
        }

        public bool? Get(long r, long c, long d){
            r += HeightOffset;
            c += WidthOffset;
            d += DepthOffset;
            if (r >= 0 && r <= Height -1 && c >= 0 && c <= Width -1 && d >= 0 && d <= Depth -1) {
                return this.Array[r, c, d];
            } return null;
        }

        public void Set(long r, long c, long d, bool value){
            r += HeightOffset;
            c += WidthOffset;
            d += DepthOffset;
            if (r >= 0 && r <= Height -1 && c >= 0 && c <= Width -1 && d >= 0 && d <= Depth -1) {
                this.Array[r, c,d] = value;
            }
        }
    }
   
}
