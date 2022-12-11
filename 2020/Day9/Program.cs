using System;
using System.IO;
using System.Linq;

namespace Day9
{
    class Program
    {
        static void Main(string[] args)
        {
           //string[] lines = File.ReadAllLines("input.txt"); var window = 25;
           string[] lines = File.ReadAllLines("sample.txt"); var window = 5;
            Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

            var numbers = lines.Select(long.Parse).ToArray();

            long incorrect = 0;

            for (int ii = window; ii < lines.Length; ii++) {
                int windowStart = ii - window;
                int windowEnd = ii - 1;
                var target = numbers[ii];
                //Console.Out.WriteLine($"{target}:");
                for (int jj = windowStart; jj < windowEnd; jj++) {
                    var num1 = numbers[jj];
                    for (int kk = jj + 1; kk <= windowEnd; kk++) {
                        var num2 = numbers[kk];
                        var sum = num1 + num2;
                        //Console.Out.Write($"\t{num1} + {num2} = {sum} ?= {target}");
                        if (sum == target) {
                            goto outer;
                        }
                        //Console.Out.WriteLine("");
                    }
                }
                incorrect = target;
                Console.Out.WriteLine($"Couldn't find 2 numbers that sum to {target}");
                break;
outer:
                ;
            }

            for (int ii = 0; ii < lines.Length; ii++) {
                long acc = 0;
                for (var range = 0; acc < incorrect; range++) {
                    acc += numbers[ii+range];
                    if (acc == incorrect) {

                        Console.Out.WriteLine($"Found range: {ii} ({numbers[ii]}) - {ii+range} ({numbers[ii+range]})");

                        long min = numbers[ii..(ii+range)].Min();
                        long max = numbers[ii..(ii+range)].Max();

                        Console.Out.WriteLine($"Min: {min}  Max: {max}  Sum: {min+max}");
                        return;
                    }
                }
            }
            Console.Out.WriteLine("Didn't find a range :(");


        }
    }
}
