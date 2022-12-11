using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MoreLinq;

namespace Day23
{
    class Program
    {
        static int NumCups = 1_000_000;
        static int NumMoves = 10_000_000;
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("input.txt"); 
            //string[] lines = File.ReadAllLines("sample.txt"); 
            //Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

            var cups = lines[0].Select(c => c - '0');
            var cupsA = new LinkedListNode<int>[NumCups+1];
            var cupsll = new LinkedList<int>();
            foreach (var cup in cups) {
                cupsA[cup] = cupsll.AddLast(cup);
            }
            
            for (int ii = 10; ii <= NumCups; ii++) {
                cupsA[ii] = cupsll.AddLast(ii);
            }
            
            var current = cupsll.First;

            //PrintCups(1, cupsll, current);
            var allToRemove = new List<LinkedListNode<int>>();
            for (int ii = 2; ii < (NumMoves + 2); ii++) {
                allToRemove.Clear();
                var firstToRemove = current;
                for (int jj = 0; jj < 3; jj++) {
                   firstToRemove = firstToRemove.Next;
                    if (firstToRemove == null) {
                        firstToRemove = cupsll.First;
                    }
                    allToRemove.Add(firstToRemove);
                }

                foreach (var toRemove in allToRemove) {
                    cupsll.Remove(toRemove);
                }
              
                var currentVal = current.Value;
                var destinationVal = currentVal -1;
                LinkedListNode<int> dest = null;
                while (true) {
                    if (destinationVal == 0) {
                        destinationVal = NumCups;
                    }
                    if (allToRemove.Any(n => n.Value == destinationVal)) {
                        destinationVal--;
                        continue;
                    }

                    dest = cupsA[destinationVal--];
                    break;
                }

                var addAfter = dest;
                foreach (var toAdd in allToRemove) {
                    cupsll.AddAfter(addAfter, toAdd);
                    addAfter = toAdd;
                }

                current = current.Next;
                if (current == null) {
                    current = cupsll.First;
                }
                
                if (ii % 1000 == 0) {
                    Console.Out.WriteLine(ii);
                    //PrintCups(ii, cupsll, current);
                }

            }

            var oneNode = cupsA[1];
            var answer = 1L * oneNode.Next.Value * oneNode.Next.Next.Value;

            Console.Out.WriteLine($"Answer: {answer}");

        }

        static void PrintCups(int move, LinkedList<int> cupsll, LinkedListNode<int> current) {
            var node = cupsll.First;
            StringBuilder sb = new StringBuilder($"{move} - cups: ");
            while (node != null) {
                var isCurrent = node == current;
                sb.Append($"{(isCurrent ? "(" : "")}{node.Value}{(isCurrent ? ")" : "")} ");
                node = node.Next;
            }
            Console.Out.WriteLine(sb.ToString());
        }
    }
}
