#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
//using MoreLinq;

bool sample = false;
bool debug = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt"); 
if (debug) Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw;

sw = Stopwatch.StartNew();

if (debug) {
    var tests = new [] {
        1,
        2,
        3,
        4,
        5,
        6,
        7 ,
        8,
        9,
        10,
        15,
        20,
        2022,
        12345,
    314159265,
    };

    foreach (var i in tests) {
        Console.WriteLine($"{i} => {DecimalToSnafu(i)}");
    }
}
Part1(lines);
//Part2(board);

Console.Out.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");

void Part1(string[] lines) {

    var result = DecimalToSnafu(lines.Select(SnafuToDecimal).Sum());

   Console.WriteLine($"Part 1 Result: {result}");
}

long SnafuToDecimal(string s) {

    return s.Reverse().Select((c, i) => (ch: c, index: i)).Aggregate(0L, (acc, c) => 
        acc + (
            c.ch switch {
                '2' => 2,
                '1' => 1,
                '0' => 0,
                '-' => -1,
                '=' => -2
            } * (long)(Math.Pow(5, c.index))));
}

string DecimalToSnafu(long d) {
    
    string base5result = "";
    int digits = (int)Double.Floor(Math.Log(d, 5)) + 1;

    for (int digit = digits -1; digit >= 0; digit--) {
        var unitPlaceValue = (long)Math.Pow(5, digit);
        var units = d / unitPlaceValue;
        base5result += units.ToString();
        d -= units * unitPlaceValue;
    }

    string snafuResult = "";
    bool carry = false;
    foreach (var cc in base5result.Reverse().Append('0')) {

        char c;
        if (carry) {
            c = cc switch {
                '0' => '1',
                '1' => '2',
                '2' => '3',
                '3' => '4',
                '4' => '5',
            };
            carry = false;
        } else {
            c = cc;
        }

        if (c == '0' || c == '1' || c == '2') {
            snafuResult += c;
            carry = false;
        }
        else if (c == '3') {
            snafuResult += '=';
            carry = true;
        }
        else if (c == '4') {
            snafuResult += '-';
            carry = true;
        } else if (c == '5') {
            snafuResult += '0';
            carry = true;
        }
    }
    var result = new string(snafuResult.Reverse().ToArray());
    if (result[0] == '0') {
        return result[1..];
    }
    return result;
}




