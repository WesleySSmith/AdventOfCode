#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Diagnostics;
using MoreLinq;


bool sample = false;


string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();


Part1(lines);
Part2(lines);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(string[] lines)
{
    var games = lines.Select(l => ParseGame(l));

    // 12 red cubes, 13 green cubes, and 14 blue cubes


   var score =  games.Select(g => (g.Id, g.Draws.Aggregate(new Draw(), (accum, draw) => {
        return new Draw {
            Red = int.Max(accum.Red, draw.Red),
            Green = int.Max(accum.Green, draw.Green),
            Blue = int.Max(accum.Blue, draw.Blue),
        };
    })))
    .Where(g => g.Item2.Red <= 12 && g.Item2.Green <= 13 && g.Item2.Blue <= 14)
    .Sum(i => i.Id);

    Console.Out.WriteLine($"Score is {score}");

}

void Part2(string[] lines)
{
    
    var games = lines.Select(l => ParseGame(l));

    // For each game, find minimum number of colors of each cube that could have been in bag
    // Compute "power": R*G*B
    // Sum powers for all games

    var sumPowers = games.Select(g => g.Draws.Aggregate(new Draw(), (accum, draw) => {
        return new Draw {
            Red = int.Max(accum.Red, draw.Red),
            Green = int.Max(accum.Green, draw.Green),
            Blue = int.Max(accum.Blue, draw.Blue),
        };
    }))
    .Select(g => g.Red * g.Green * g.Blue)
    .Sum();

    Console.Out.WriteLine($"Sum of powers is {sumPowers}");




}


Game ParseGame(string line)
{
   // Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green

   var parts = line.Split(":");
   var gameId = int.Parse(parts[0].Split(" ")[1]);

   var draws = parts[1].Split(";");
   
   var parsedDraws = draws.Select(draw =>  {
        var parts2 = draw.Split(",").Select(p => {
            var parts3 = p.Trim().Split(" ");

            return new {Color= parts3[1], Count= int.Parse(parts3[0])};
        });

        var red = parts2.SingleOrDefault(p => p.Color == "red")?.Count ?? 0;
        var blue = parts2.SingleOrDefault(p => p.Color == "blue")?.Count ?? 0;
        var green = parts2.SingleOrDefault(p => p.Color == "green")?.Count ?? 0;

        return new Draw {
            Red = red,
            Blue = blue,
            Green = green
        };
    });

    return new Game {Id = gameId, Draws = parsedDraws.ToList()};
}

record Game {
    public int Id;
    public List<Draw> Draws;

}

record Draw {
    public int Red;
    public int Blue;
    public int Green;
}
