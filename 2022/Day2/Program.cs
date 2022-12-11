#pragma warning disable CS8524
#pragma warning disable CS8509
using MoreLinq;

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

var game = lines
    .Select(l => new EncryptedRound() {
        Theirs = l[0],
        Ours = l[2]});

Part1(game);
Part2(game);

static void Part1(IEnumerable<EncryptedRound> encryptedGame) {
   
    var decryptedGame = encryptedGame.Select(Round1.Decrypt);
    var totalScore = decryptedGame.Sum(g => g.Score);

    Console.Out.WriteLine($"Part 1 Total Score: {totalScore}");
}


static void Part2(IEnumerable<EncryptedRound> encryptedGame) {
   
   var decryptedGame = encryptedGame.Select(Round2.Decrypt).Select(round => round.Solve);
    var totalScore = decryptedGame.Sum(g => g.Score);

    Console.Out.WriteLine($"Part 2 Total Score: {totalScore}");
}

public static class Utils {
    public static readonly Dictionary<Shape, Shape> Beats = new() {
        {Shape.Rock, Shape.Scissors},
        {Shape.Scissors, Shape.Paper},
        {Shape.Paper, Shape.Rock},
    };
}

public record EncryptedRound {
    public char Theirs;
    public char Ours;
}

public enum Shape {
    Rock,
    Paper,
    Scissors
}

public enum Result {
    Lose,
    Draw,
    Win
}

public record Round1 {
    public Shape Theirs;
    public Shape Ours;

    public static Round1 Decrypt(EncryptedRound encrypedRound) {
        return new Round1() {
            Theirs = encrypedRound.Theirs switch {
                'A' => Shape.Rock,
                'B' => Shape.Paper,
                'C' => Shape.Scissors,
            },
            Ours = encrypedRound.Ours switch {
                'X' => Shape.Rock,
                'Y' => Shape.Paper,
                'Z' => Shape.Scissors,
            }
        };
    }
    public Result Outcome => 
        Theirs == Ours ? Result.Draw : Utils.Beats[Ours] == Theirs ? Result.Win : Result.Lose;

    public int OutcomeScore =>
        Outcome switch {
            Result.Draw => 3,
            Result.Win => 6,
            Result.Lose => 0,
        };

    public int ShapeScore =>
        Ours switch {
            Shape.Rock => 1,
            Shape.Paper => 2,
            Shape.Scissors => 3,
        };

    public int Score => ShapeScore + OutcomeScore;
}


public record Round2 {
    public Shape Theirs;
    public Result Goal;

    public static Round2 Decrypt(EncryptedRound encrypedRound) {
        return new Round2() {
            Theirs = encrypedRound.Theirs switch {
                'A' => Shape.Rock,
                'B' => Shape.Paper,
                'C' => Shape.Scissors,
            },
            Goal = encrypedRound.Ours switch {
                'X' => Result.Lose,
                'Y' => Result.Draw,
                'Z' => Result.Win,
            }
        };
    }
    public Round1 Solve =>
        new Round1 {
            Theirs = Theirs,
            Ours = Goal switch {
                Result.Draw => Theirs,
                Result.Win => Utils.Beats.Single(kvp => kvp.Value == Theirs).Key,
                Result.Lose => Utils.Beats[Theirs],
            }
        };
}
