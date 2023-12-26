#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using MoreLinq;


bool sample = false;


string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();


//Part1(lines);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(string[] lines)
{
    var patterns = lines.Split("").Select(s => s.Select(s2 => s2.ToArray()).ToArray()).ToArray();

     var score = 0L;

    foreach (var pattern in patterns)
    {
        var scores = NewMethod2(pattern);
        score += scores.RowScore * 100 +  scores.ColScore;
    }

    Console.Out.WriteLine($"Total Score is {score}.");
}

void Part2(string[] lines)
{
    var patterns = lines.Split("").Select(s => s.Select(s2 => s2.ToArray()).ToArray()).ToArray();

    var score = 0L;
    foreach (var pattern in patterns)
    {

        var originalScores = NewMethod2(pattern);
        for (int ii = 0; ii < pattern.Length; ii++) { 
            for (int jj = 0; jj < pattern[0].Length; jj++)
            {
                var alteredPattern = new char[pattern.Length][];
                for (int iii = 0; iii < pattern.Length; iii++) { 
                    alteredPattern[iii] = new char[pattern[0].Length];
                    for (int jjj = 0; jjj < pattern[0].Length; jjj++) {
                        alteredPattern[iii][jjj] = pattern[iii][jjj];
                    }
                }
                alteredPattern[ii][jj] = alteredPattern[ii][jj] == '.' ? '#' : '.';

                var subScores = NewMethod2(alteredPattern, originalScores);
                if (subScores.RowScore > 0 || subScores.ColScore > 0) {
                    score += subScores.RowScore * 100 +  subScores.ColScore;
                    goto nextPattern;
                }
            }
        }
        throw new Exception("Didn't find it");
        nextPattern:
        ;
    }



    Console.Out.WriteLine($"Total Score is {score}.");

}


static (long RowScore,long ColScore) NewMethod2(char[][] pattern, (long RowScore,long ColScore)? ignores = null )
{
    Console.WriteLine("Checking for pattern");
    // Look by rows:
    //Console.WriteLine("Checking rows");
    var rowScore = NewMethod(pattern, ignores?.RowScore);

    var colScore = 0L;
    if (rowScore == 0) {
        //Console.WriteLine($"Score: {score}");
        // Look by columns:
        var patternInvert = new char[pattern[0].Length][];
        for (int jj = 0; jj < pattern[0].Length; jj++)
        {
            patternInvert[jj] = new char[pattern.Length];
            for (int ii = 0; ii < pattern.Length; ii++)
            {
                patternInvert[jj][ii] = pattern[ii][jj];
            }
        }
        //Console.WriteLine("Checking cols");
        colScore = NewMethod(patternInvert, ignores?.ColScore);
    } 
    //Console.WriteLine($"Score: {score}");
    return (rowScore, colScore);
}

static long NewMethod(IEnumerable<char[]> pattern, long? ignore)
{
    var rows = pattern.Select(line => Convert.ToInt64(string.Join("", line.Select(c => c == '#' ? '1' : '0')), 2)).ToArray();

    var last = rows[0];
    for (int row = 1; row < rows.Length; row++)
    {
        if (row == ignore) {
            continue;
        }
        var next = rows[row];
        if (next == last)
        {
            // check for reflection
            // potential line is between (row - 1) and row
            var reflection = true;
            for (var ii = 0; ii < row && ii < rows.Length - row; ii++)
            {
                var up = rows[row - 1 - ii];
                var down = rows[row + ii];
                if (up != down)
                {
                    reflection = false;
                    break;
                }
            }
            if (reflection)
            {
                //Console.WriteLine($"Found Row Reflection between row {row} and {row + 1}");
                return row;
            }
        }
        last = next;
    }

    return 0;
}