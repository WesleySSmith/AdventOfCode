using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MoreLinq;

namespace Day22
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("input.txt"); 
            //string[] lines = File.ReadAllLines("sample.txt"); 
            //Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

            var l1 = lines.Skip(1).TakeWhile(l => l.Length > 0).Select(int.Parse);
            var l2 = lines.Skip(l1.Count() + 3).Select(int.Parse);

            var ll1 = new LinkedList<int>(l1);
            var ll2 = new LinkedList<int>(l2);

            Console.Out.WriteLine($"Player 1: {ll1.ToDelimitedString(",")}");
            Console.Out.WriteLine($"Player 2: {ll2.ToDelimitedString(",")}");

            var winnerCards = Game(ll1, ll2) == 1 ? ll1 : ll2;

            Console.Out.WriteLine($"Player 1: {ll1.ToDelimitedString(",")}");
            Console.Out.WriteLine($"Player 2: {ll2.ToDelimitedString(",")}");

            var score = winnerCards.Reverse().Aggregate((index: 1, sum: 0), (a,b) => (a.index+1, a.sum + b*a.index)).sum;
            Console.Out.WriteLine($"Score: {score}");
        }

        static string Fingerprint(LinkedList<int> ll1) {
            return ll1.ToDelimitedString(",");
        }

        static string Fingerprint(LinkedList<int> ll1, LinkedList<int> ll2) {
            return Fingerprint(ll1) + "X" + Fingerprint(ll2);
        }

        static byte Game(LinkedList<int> ll1, LinkedList<int> ll2) { 

            var fingerprints = new HashSet<string>();

             while (ll1.Any() && ll2.Any()) {
                 var fingerprint = Fingerprint(ll1, ll2);
                 if (fingerprints.Contains(fingerprint)) {
                     return 1;
                 }
                 Round(ll1, ll2);
                 fingerprints.Add(fingerprint);
             }
             return ll1.Any() ? 1 : 2;
        }

        static byte Round(LinkedList<int> ll1, LinkedList<int> ll2) {
            var c1 = ll1.First;
            ll1.RemoveFirst();

            var c2 = ll2.First;
            ll2.RemoveFirst();

            byte winner;
            if (ll1.Count >= c1.Value && ll2.Count >= c2.Value) {
                var ll1c = new LinkedList<int>(ll1.Take(c1.Value));
                var ll2c = new LinkedList<int>(ll2.Take(c2.Value));
                winner = Game(ll1c, ll2c);
            } else {
                winner = c1.Value > c2.Value ? 1 : 2;
            }

             if (winner == 1) {
                    ll1.AddLast(c1);
                    ll1.AddLast(c2);
                } else {
                    ll2.AddLast(c2);
                    ll2.AddLast(c1);
                }
            return winner;
        }
    }
}
