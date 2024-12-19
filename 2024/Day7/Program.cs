#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
//using MoreLinq;

bool sample = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
//Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

//Part1(lines);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(string[] lines)
{
    long totalResult = 0;
    foreach (var equation in lines) {
        var parts = equation.Split(':');
        var testValue = long.Parse(parts[0]);
        var numbers = parts[1][1..].Split(' ').Select(int.Parse).ToArray();

        var optionCount = (int)Math.Pow(2, numbers.Length - 1);
        
        
        for (int option = 0; option < optionCount; option++) {
            long acc = numbers[0];
            for(int operatorPos = 0; operatorPos < numbers.Length - 1; operatorPos++) {
                var number = numbers[operatorPos+1];
                if (((option >> operatorPos) & 1) == 1) {
                    acc += number;
                } else {
                    acc *= number;
                }
            }

            if (acc == testValue) {
                totalResult += acc;
                break;
            }
        }
    }

    Console.Out.WriteLine($"Part 1: {totalResult}");
}


void Part2(string[] lines) {
     long totalResult = 0;
    foreach (var equation in lines) {
        var parts = equation.Split(':');
        var testValue = long.Parse(parts[0]);
        var numbers = parts[1][1..].Split(' ').Select(int.Parse).ToArray();

        var optionCount = (int)Math.Pow(3, numbers.Length - 1);
        
        
        for (int option = 0; option < optionCount; option++) {
            var option2 = option;
            long acc = numbers[0];
            for(int operatorPos = 0; operatorPos < numbers.Length - 1; operatorPos++) {
                var number = numbers[operatorPos+1];

                var op = option2 % 3;
                if (op == 0) {
                    acc += number;
                } else if (op == 1) {
                    acc *= number;
                } else {
                    acc = acc * (long)Math.Pow(10, (int)Math.Log10(number)+1) + number;
                }
                option2 = (int)Math.Floor((decimal)option2 / 3);
            }

            if (acc == testValue) {
                totalResult += acc;
                break;
            }
        }
    }

    Console.Out.WriteLine($"Part 2: {totalResult}");
}

