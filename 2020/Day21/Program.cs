using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MoreLinq;

namespace Day21
{
    class Program
    {
        
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("input.txt"); 
            //string[] lines = File.ReadAllLines("sample.txt"); 
            //Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

            var foods = new List<Food>();
            int lineCounter = 0;
            foreach (var line in lines) {
                var parts = line.Split(" (contains ");
                foods.Add(new Food { 
                    Num = ++lineCounter,
                    Ingredients = parts[0].Split(' ').ToList(), 
                    Allergens = parts[1][..^1].Split(" ").Select(s => s.Split(',').First()).ToList()
                });
            }
            Console.Out.WriteLine($"Parsed {foods.Count()} foods");

            var allergens = foods.Select(f => f.Allergens).SelectMany(a => a).Distinct().ToDictionary(a => a, a => new List<Food>());
            
            var ingredients = foods.Select(f => f.Ingredients).SelectMany(a => a).Distinct().ToDictionary(a => a, a => new List<Food>());

            foreach (var food in foods) {
                foreach (var allergen in food.Allergens) {
                    allergens[allergen].Add(food);
                }
                foreach (var ingredient in food.Ingredients) {
                    ingredients[ingredient].Add(food);
                }
            }

            var matches = new List<(string i,string a)>();
    
            while (allergens.Any()) {
                Console.Out.WriteLine($"Reducing from {allergens.Count()}");
                var sortedAllergenList = allergens.OrderByDescending(a => a.Value.Count()).ToList();
                bool progress = false;
                foreach (var allergen in sortedAllergenList) {
                    // Look for 1 common ingredient
                    var numFoods = allergen.Value.Count();
                    
                    var commonIngredients =
                    allergen.Value
                    .Select(f => f.Ingredients)
                    .SelectMany(a => a)
                    .GroupBy(i => i)
                    .Where(g => g.Count() == numFoods)
                    .Select(g => g.Key);

                    if (commonIngredients.Count() == 1) {
                        var commonIngredient = commonIngredients.Single();
                        Console.Out.WriteLine($"{commonIngredient} contains {allergen.Key}");
                        matches.Add((commonIngredient, allergen.Key));
                        bool ok;

                        foreach (var food in ingredients[commonIngredient]) {
                            food.Ingredients.Remove(commonIngredient);
                        }
                        ok = ingredients.Remove(commonIngredient);
                        if (!ok) throw new Exception("ingredient wasn't in ingredients?");

                        foreach(var food in allergen.Value) {
                            ok = food.Allergens.Remove(allergen.Key);
                            if (!ok) throw new Exception("allergen wasn't there?");
                        }
                        ok = allergens.Remove(allergen.Key);
                        if (!ok) throw new Exception("allergen wasn't in allergens?");

                        progress = true;
                    }
                }
                if (!progress) {
                    throw new Exception("Making no progress");
                }
            }

            // Count how many ingredients left
            var leftover = foods.Select(f => f.Ingredients.Count()).Sum();
            Console.Out.WriteLine($"{leftover} Leftovers");

            Console.Out.WriteLine($"List: {matches.OrderBy(a => a.a).Select(a=> a.i).ToDelimitedString(",")}");
        }
    }

    class Food {
        public int Num;
        public List<string> Ingredients;
        public List<string> Allergens;
    }
}
