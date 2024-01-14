// 31991 Too high
#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Numerics;
using MoreLinq;


bool sample = false;
bool debug = true;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

var hailstones = lines.Select(Hailstone.Parse).ToList();

//Part1(hailstones);
Part2(hailstones);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(List<Hailstone> hailstones)
{

    long rangeLow;
    long rangeHigh;
    if (sample) {
        rangeLow = 7;
        rangeHigh = 27;
    } else {
        rangeLow = 200000000000000;
        rangeHigh = 400000000000000;
    }

    int collisions = 0;
    for (int ii = 0; ii < hailstones.Count; ii++) {
        for (int jj = ii + 1; jj < hailstones.Count; jj++) {
            var h1 = hailstones[ii];
            var h2 = hailstones[jj];
            var intersectResult = Intersect(h1, h2);

            if (debug) Console.Write($"{h1} + {h2}: ");

            if (intersectResult.Point != null) {

                if (debug) Console.Write($"Collide at {intersectResult} which is ");

                if (intersectResult.Future1 && intersectResult.Future2) {
                    if (intersectResult.Point.X >= rangeLow && intersectResult.Point.X <= rangeHigh 
                    && intersectResult.Point.Y >= rangeLow && intersectResult.Point.Y <= rangeHigh) {
                        if (debug) Console.WriteLine("inside");
                        collisions++;
                    } else {
                        if (debug) Console.WriteLine("outside");
                    }
                } else {
                    if (!intersectResult.Future1 && !intersectResult.Future2) {
                        if (debug) Console.WriteLine("in past for both");
                    } else if (!intersectResult.Future1) {
                        if (debug) Console.WriteLine("in past for A");
                    } else {
                        if (debug) Console.WriteLine("in past for B");
                    }
                }
                
            } else {
                if (debug) Console.WriteLine("Parallel");
            }
        }
    }
    
    Console.Out.WriteLine($"Collisions {collisions}");


}



void Part2(List<Hailstone> hailstones)
{

    var w = new long[3, 3] {
        {1,2,3},
        {7,8,10},
        {4,5,6}
    };
    var wD  = Determinant(w);


    long[,] m1;
    long[] m2;

    var vx0 = hailstones[0].Vx;
    var vy0 = hailstones[0].Vy;
    var vz0 = hailstones[0].Vz;
    var px0 = hailstones[0].Px;
    var py0 = hailstones[0].Py;
    var pz0 = hailstones[0].Pz;

    var vx1 = hailstones[1].Vx;
    var vy1 = hailstones[1].Vy;
    var vz1 = hailstones[1].Vz;
    var px1 = hailstones[1].Px;
    var py1 = hailstones[1].Py;
    var pz1 = hailstones[1].Pz;

    var vx2 = hailstones[2].Vx;
    var vy2 = hailstones[2].Vy;
    var vz2 = hailstones[2].Vz;
    var px2 = hailstones[2].Px;
    var py2 = hailstones[2].Py;
    var pz2 = hailstones[2].Pz;

    var vx3 = hailstones[3].Vx;
    var vy3 = hailstones[3].Vy;
    var vz3 = hailstones[3].Vz;
    var px3 = hailstones[3].Px;
    var py3 = hailstones[3].Py;
    var pz3 = hailstones[3].Pz;

m1 = new long[6,6] {
{vy0 - vy1,   vx1 - vx0,   0        ,    py1 - py0,   px0 - px1,   0,       },
{vz0 - vz1,   0        ,   vx1 - vx0,    pz1 - pz0,   0        ,   px0 - px1},
{vy0 - vy2,   vx2 - vx0,   0        ,    py2 - py0,   px0 - px2,   0        },
{vz0 - vz2,   0        ,   vx2 - vx0,    pz2 - pz0,   0        ,   px0 - px2},
{vy0 - vy3,   vx3 - vx0,   0        ,    py3 - py0,   px0 - px3,   0        },
{vz0 - vz3,   0        ,   vx3 - vx0,    pz3 - pz0,   0        ,   px0 - px3}
};

// m2[0] = 
// m2[1] = 
// m2[2] = 
// m2[3] = 
// m2[4] = 
// m2[5] = 



m2 = [
    px0*vy0 - py0*vx0 - px1*vy1 + py1*vx1,
    px0*vz0 - pz0*vx0 - px1*vz1 + pz1*vx1,
    px0*vy0 - py0*vx0 - px2*vy2 + py2*vx2,
    px0*vz0 - pz0*vx0 - px2*vz2 + pz2*vx2,
    px0*vy0 - py0*vx0 - px3*vy3 + py3*vx3,
    px0*vz0 - pz0*vx0 - px3*vz3 + pz3*vx3];


var c1 = CramerMatrix(m1, m2, 0);
var c2 = CramerMatrix(m1, m2, 1);
var c3 = CramerMatrix(m1, m2, 2);

var det = Determinant(m1);
var detC1 = Determinant(c1);
var detC2 = Determinant(c2);
var detC3 = Determinant(c3);


var px = detC1 / det;
var py = detC2 / det;
var pz = detC3 / det;

Console.WriteLine($"{px},{py},{pz}");

}

long[,] CramerMatrix(long[,] m, long[] v, int index) {
    var c1 = m.Clone() as long[,];

    for (int ii = 0; ii < m.GetLength(0); ii++) {
        c1[ii,index] = v[ii];
    }

    return c1;
}

long Determinant(long[,] m) {

    var n = m.GetLength(0);
    if (n == 2) {
        return m[0,0] * m[1,1] - m[0,1] * m[1,0];
    }

    long result = 0;
    for(var ii = 0; ii < n; ii++) {

        // Construct new matrix, leaving out row 0 and column i
        var m2 = new long[n-1,n-1];

        for (var jj = 0; jj < n-1; jj++) {
            for (var kk = 0; kk < n-1; kk++) {
                m2[jj,kk] = m[jj+1, kk + (kk >= ii ? 1 : 0)];
            }
        }

        var det = Determinant(m2);
        if (ii % 2 == 1) {
            det *= -1;
        }
        result += det * m[0,ii];
    }

    return result;

}

(PointF2 Point, bool Future1, bool Future2) Intersect(Hailstone h1, Hailstone h2) {


    float GetB(Hailstone h) {
        return h.Py - ((float)h.Vy/h.Vx)*h.Px;
    }
    float GetM(Hailstone h) {
        return (float)h.Vy/h.Vx;
    }

    bool IntersectsInFuture(Hailstone h, PointF2 intersect) {
        var dx = intersect.X - h.Px;
        var dy = intersect.Y - h.Py;

        return dx <= 0 && h.Vx <= 0 || dx >= 0 && h.Vx >= 0;


        
    }

    var b1 = GetB(h1);
    var b2 = GetB(h2);
    var m1 = GetM(h1);
    var m3 = GetM(h2);

    if (m1 ==m3) {
        return (null, false, false);
    }

    var x = (b2 - b1)/(m1 -m3);
    var y = m1*x + b1;

    var intersect = new PointF2(x,y);

    return (intersect, IntersectsInFuture(h1, intersect), IntersectsInFuture(h2, intersect));
}

record Hailstone(long Px, long Py, long Pz, long Vx, long Vy, long Vz) {

    public static Hailstone Parse(string s) {
        var splits = s.Split('@');
        var pSplit = splits[0].Split(',').Select(long.Parse).ToArray();
        var vSplit = splits[1].Split(',').Select(long.Parse).ToArray();;
        return new Hailstone(pSplit[0], pSplit[1], pSplit[2], vSplit[0], vSplit[1], vSplit[2]);
    }
}

record Point2(long X, long Y);
record PointF2(float X, float Y);
