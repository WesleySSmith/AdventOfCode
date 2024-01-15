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

    BigInteger[,] m1;
    BigInteger[] m2;

    BigInteger vx0 = hailstones[0].Vx;
    BigInteger vy0 = hailstones[0].Vy;
    BigInteger vz0 = hailstones[0].Vz;
    BigInteger px0 = hailstones[0].Px;
    BigInteger py0 = hailstones[0].Py;
    BigInteger pz0 = hailstones[0].Pz;

    BigInteger vx1 = hailstones[1].Vx;
    BigInteger vy1 = hailstones[1].Vy;
    BigInteger vz1 = hailstones[1].Vz;
    BigInteger px1 = hailstones[1].Px;
    BigInteger py1 = hailstones[1].Py;
    BigInteger pz1 = hailstones[1].Pz;

    BigInteger vx2 = hailstones[2].Vx;
    BigInteger vy2 = hailstones[2].Vy;
    BigInteger vz2 = hailstones[2].Vz;
    BigInteger px2 = hailstones[2].Px;
    BigInteger py2 = hailstones[2].Py;
    BigInteger pz2 = hailstones[2].Pz;

    BigInteger vx3 = hailstones[3].Vx;
    BigInteger vy3 = hailstones[3].Vy;
    BigInteger vz3 = hailstones[3].Vz;
    BigInteger px3 = hailstones[3].Px;
    BigInteger py3 = hailstones[3].Py;
    BigInteger pz3 = hailstones[3].Pz;

m1 = new BigInteger[6,6] {
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

var det = DeterminantBi(m1);
var detC1 = DeterminantBi(c1);
var detC2 = DeterminantBi(c2);
var detC3 = DeterminantBi(c3);


var px = detC1 / det;
var py = detC2 / det;
var pz = detC3 / det;

Console.WriteLine($"{px},{py},{pz} : {px+py+pz}");

}

BigInteger[,] CramerMatrix(BigInteger[,] m, BigInteger[] v, int index) {
    var c1 = m.Clone() as BigInteger[,];

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


BigInteger DeterminantBi(BigInteger[,] m) {

    var n = m.GetLength(0);
    if (n == 2) {
        return m[0,0] * m[1,1] - m[0,1] * m[1,0];
    }

    BigInteger result = 0;
    for(var ii = 0; ii < n; ii++) {

        // Construct new matrix, leaving out row 0 and column i
        var m2 = new BigInteger[n-1,n-1];

        for (var jj = 0; jj < n-1; jj++) {
            for (var kk = 0; kk < n-1; kk++) {
                m2[jj,kk] = m[jj+1, kk + (kk >= ii ? 1 : 0)];
            }
        }

        var det = DeterminantBi(m2);
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
