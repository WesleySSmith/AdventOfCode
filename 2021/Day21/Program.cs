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
     
        var p1StartPos = int.Parse(lines[0].Split(": ")[1]);
        var p2StartPos = int.Parse(lines[1].Split(": ")[1]);


        Console.Out.WriteLine($"Parse time: {sw.ElapsedMilliseconds}");

        //Part1(p1StartPos, p2StartPos);
        Part2(p1StartPos, p2StartPos);

        Console.Out.WriteLine($"Total time {sw.ElapsedMilliseconds}");
    }

    static void Part1(int p1Pos, int p2Pos) {
        
        int p1Score = 0, p2Score = 0;
        DeterministicDice dice = new();
        var winner = 0;
        while (true) {
            var p1Roll = dice.Roll() + dice.Roll() + dice.Roll();
            p1Pos += p1Roll;
            p1Pos = (p1Pos - 1) % 10 + 1;
            p1Score += p1Pos;
            if (p1Score >= 1000) {
                winner = 1;
                break;
                
            }
            var p2Roll = dice.Roll() + dice.Roll() + dice.Roll();
            p2Pos += p2Roll;
            p2Pos = (p2Pos - 1) % 10 + 1;
            p2Score += p2Pos;
            if (p2Score >= 1000) {
                winner = 2;
                break;
            }
        }

        var losingScore = winner switch {1 => p2Score, 2 => p1Score, _ => -1};
        
        Console.Out.WriteLine($"Result: {losingScore} * {dice.TotalRolls} = {losingScore * dice.TotalRolls}");
      
    }

    public class DeterministicDice {
        private int Current = 1;
        public int TotalRolls { get; private set;} = 0;

        public int Roll() {
            TotalRolls++;
            var result = Current;
            Current++;
            if (Current == 101) {
                Current = 1;
            }
            return result;
        }
    }

    static void Part2(int p1InitialPos, int p2InitialPos) {

        var rollFreq = new int[10];
        for (int i = 1; i <= 3; i++){
            for (int j = 1; j <= 3; j++) {
                for (int k = 1; k <= 3; k++) {
                    rollFreq[i+j+k]++;
                }
            }
        }
// 0: p1Pos
// 1: p1Score
// 2: p2Pos
// 3: p2Score
        var freq = new long[11, 21, 11, 21];

        freq[p1InitialPos, 0, p2InitialPos, 0] = 1;
        long p1Wins = 0;
        long p2Wins = 0;
        bool stillRunning = true;
        while (stillRunning) {
            stillRunning = false;
            var nextFreq = new long[11, 21, 11, 21];
            for (var p1Pos = 1; p1Pos <= 10; p1Pos++)  {
                for (var p2Pos = 1; p2Pos <= 10; p2Pos++)  {
                    for (var p1Score = 0; p1Score < 21; p1Score++) {
                        for (var p2Score = 0; p2Score < 21; p2Score++) {
                            var universeFrequency = freq[p1Pos, p1Score, p2Pos, p2Score];
                            if (universeFrequency == 0) {
                                continue;
                            }
                            stillRunning = true;

                            // Player 1 rolls
                            for(int p1Roll = 3; p1Roll <= 9; p1Roll++) {
                                var p1RollFrequency = rollFreq[p1Roll];
                                var p1NewPos = (p1Pos + p1Roll - 1) % 10 + 1;
                                var p1NewScore = p1Score + p1NewPos;
                                if (p1NewScore >= 21) {
                                    p1Wins += universeFrequency * p1RollFrequency;
                                    continue;
                                } 
                                
                                // Player 2 rolls
                                for(int p2Roll = 3; p2Roll <= 9; p2Roll++) {
                                    var p2RollFrequency = rollFreq[p2Roll];
                                    var p2NewPos = (p2Pos + p2Roll - 1) % 10 + 1;
                                    var p2NewScore = p2Score + p2NewPos;
                                    if (p2NewScore >= 21) {
                                        p2Wins += universeFrequency * p2RollFrequency * p1RollFrequency;
                                        continue;
                                    } 
                                    nextFreq[p1NewPos, p1NewScore, p2NewPos, p2NewScore] += universeFrequency * p1RollFrequency * p2RollFrequency;
                                }
                            }
                        }
                    }
                }
            }
            freq = nextFreq;
        }

        Console.Out.WriteLine($"Result: {Math.Max(p1Wins, p2Wins)}");
      
    }

    class Universe {
        public int p1Pos;
        public int p1Score;
        public int p2Pos;
        public int p2Score;
        public int Frequency;
    }

    
   
}
