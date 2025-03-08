using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();   
app.UseSwaggerUI(); 

#region demo data
Ingredient _egg   = new Ingredient("egg",               "eggs");
Ingredient _flour = new Ingredient("plain white flour", "grams");
Ingredient _milk  = new Ingredient("whole milk",        "millilitres");

Recipe _pancakes = new Recipe(
    "pancakes",
    new List<Tuple<Ingredient, decimal>>
    {
        Tuple.Create(_egg, 2.0m),
        Tuple.Create(_flour, 500.0m),
        Tuple.Create(_milk, 100.0m)
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
    "hard boiled egg",
    new List<Tuple<Ingredient, decimal>>
    {
        Tuple.Create(_egg, 2.0m)
    },
    new List<string>
    {
        "boil the egg in a pan of water for 10 minutes",
        "peel and serve"
    });
#endregion

ConcurrentDictionary<int, Ingredient> _ingredients = new ConcurrentDictionary<int, Ingredient>
{
    [0] = _egg,
    [1] = _flour,
    [2] = _milk
};

ConcurrentDictionary<int, Recipe> _recipes = new ConcurrentDictionary<int, Recipe>
{
    [0] = _boiledEgg,
    [1] = _pancakes,
};


app.MapGet("/", () => "Welcome to recipe world!");
app.MapGet("/recipes", () => _recipes);
app.MapGet("/recipes/{id}", (int id) => _recipes.TryGetValue(id, out var rec) 
    ? TypedResults.Ok(rec) : Results.NotFound());

app.MapGet("/ingredients/{id}", (int id) => _ingredients.TryGetValue(id, out var ing)
    ? TypedResults.Ok(ing) : Results.NotFound());


// not idempotent
app.MapPost("/recipes/{id}", (int id, Recipe rec) => _recipes.TryAdd(id, rec)
    ? TypedResults.Created($"/recipes/{id}", rec) : Results.BadRequest(new
    { id = " a recipe with this id already exists" }));

// will definitely add or overwrite (thus idempotent)
app.MapPut("/recipes/{id}", (int id, Recipe rec) =>
    _recipes.AddOrUpdate(id, (i) => rec, (i, _) => rec));

app.Run();

public record Ingredient(String Name, String Unit);
public record Recipe(
    String Name,
    List<Tuple<Ingredient, decimal>> Ingredients,
    List<String> steps);




