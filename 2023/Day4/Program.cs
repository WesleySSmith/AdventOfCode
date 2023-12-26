#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
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
    var cards = lines.Select(ParseCard);
    var countWins = cards.Select(CountWinningNumbers);
    var scores = countWins.Select(countWinningNumbers => countWinningNumbers == 0 ? 0 : Math.Pow(2, countWinningNumbers-1));
    var score = scores.Sum();
    Console.Out.WriteLine($"Score is {score}");

}

void Part2(string[] lines)
{
    var cards = lines.Select(ParseCard);

    cards = cards.Select(c =>  c with {Points = CountWinningNumbers(c)});

    var cardObjs = cards.ToArray();

    Queue<int> q = new Queue<int>();
    for (int ii = 1; ii <= cardObjs.Length; ii++) {
        q.Enqueue(ii);
    }

    long processed = 0;

    while(q.TryDequeue(out int cardNum)) {
        processed++;
        for (int ii = 0; ii < cardObjs[cardNum-1].Points; ii++) {
            q.Enqueue(cardNum + ii + 1);
        }
    }

    Console.Out.WriteLine($"Num cards is {processed}");




}

int CountWinningNumbers(Card card) {
    return card.WeHave.Where(ours => card.Winning.Contains(ours)).Count();
}

Card ParseCard(string line)
{
   // Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53

   var parts = line.Split(":")[1].Split("|");
   
    return new Card {
        Winning = ParseNums(parts[0]),
        WeHave = ParseNums(parts[1])
    };
}

List<int> ParseNums(string nums) {
    return nums.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
}

record Card {
    public List<int> Winning;
    public List<int> WeHave;
    public int Points;

}
