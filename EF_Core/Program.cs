using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

#region demo data
Ingredient _egg   = new Ingredient(0, "egg",               "eggs");
Ingredient _flour = new Ingredient(1, "plain white flour", "grams");
Ingredient _milk  = new Ingredient(2, "whole milk",        "millilitres");

Recipe _pancakes = new Recipe(
    0,
    "pancakes",
    new Dictionary<Ingredient, decimal>
    {
        { _egg, 2.0m },
        { _flour, 500.0m },
        { _milk, 100.0m }
    },
    new List<String> 
    { 
        "crack the eggs into a bowl",
        "stir all ingredients into the bowl with the eggs",
        "heat a pan",
        "oil the pan",
        "fry one pancake at a time for roughly 2 minutes, turning once",
        "serve immediately"
    });

Recipe _boiledEgg = new Recipe(
    1,
    "hard boiled egg",
    new Dictionary<Ingredient, decimal>
    {
        { _egg, 2.0m },
    },
    new List<string>
    {
        "boil the egg in a pan of water for 10 minutes",
        "peel and serve"
    });
#endregion

ConcurrentBag<Ingredient> _ingredients = new ConcurrentBag<Ingredient>
{
    _egg,
    _flour,
    _milk,
};

ConcurrentBag<Recipe> _recipes = new ConcurrentBag<Recipe>
{
    _boiledEgg,
    _pancakes,
};


app.MapGet("/", () => "Welcome to recipe world!");

app.Run();

public record Ingredient(int Id, String Name, String Unit);
public record Recipe(
    int Id,
    String Name,
    Dictionary<Ingredient, decimal> Ingredients,
    List<String> steps);




