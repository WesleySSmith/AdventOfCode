using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MoreLinq;

namespace Day25
{
    class Program
    {
        static void Main(string[] args)
        {
            //long cardPk = 5764801;
            //long doorPk = 17807724;

            long cardPk = 5099500;
            long doorPk = 7648211;

            var cardLoop = FindLoopSize(7, cardPk);
            Console.Out.WriteLine($"Card Loop: {cardLoop}");
            var doorLoop = FindLoopSize(7, doorPk);
            Console.Out.WriteLine($"Door Loop: {doorLoop}");
            var key = Transform(doorPk, cardLoop);
            Console.Out.WriteLine($"Encryption key: {key}");
        }

        static long Transform(long subjectNumber, long loopSize) {
            var value = 1L;
            for (int ii = 0; ii < loopSize; ii++) {
                value *= subjectNumber;
                value %= 20201227;
            }
            return value;
        }
        
        static long FindLoopSize(int subjectNumber, long target) {  

            var value = 1L;
            for (int ii = 1; ; ii++) {
                value *= subjectNumber;
                value %= 20201227;
                if (value == target) {
                    return ii;
                }
            }
        }
    }
}
