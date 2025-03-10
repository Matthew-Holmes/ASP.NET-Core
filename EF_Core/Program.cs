using System.Collections.Concurrent;
using EF_Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlite(connString));

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

app.MapGet("/ingredients/{id}", (int id) => _ingredients.TryGetValue(id, out var ing)
    ? TypedResults.Ok(ing) : Results.NotFound());

RouteGroupBuilder recipeApi = app.MapGroup("/recipe");
recipeApi.MapGet("/", () => _recipes)
    .WithTags("recipe")
    .Produces<IEnumerable<Recipe>>(StatusCodes.Status200OK);

recipeApi.MapGet("/{id}", (int id) => _recipes.TryGetValue(id, out var rec)
    ? TypedResults.Ok(rec) : Results.NotFound())
    .WithTags("recipe")
    .Produces<Recipe>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status404NotFound);

// not idempotent
recipeApi.MapPost("/", (Recipe rec) => {
    if (rec == null)
        return Results.BadRequest(new { message = "Recipe data is required." });

    int id = _recipes.Keys.DefaultIfEmpty(0).Max() + 1;

    if (!_recipes.TryAdd(id, rec))
        return Results.BadRequest(new { message = "Failed to add recipe." });

    return TypedResults.Created($"/recipes/{id}", new RecipeResponse(id, rec));
})
    .WithTags("recipe")
    .Produces<RecipeResponse>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest);

// will definitely add or overwrite (thus idempotent)
recipeApi.MapPut("/{id}", (int id, Recipe rec) => {
    var isUpdate = _recipes.ContainsKey(id);
    _recipes[id] = rec;
    return isUpdate ? Results.Ok(rec) : TypedResults.Created($"/recipe/{id}", rec);
})
    .WithTags("recipe")
    .Produces<Recipe>(StatusCodes.Status200OK)
    .Produces<Recipe>(StatusCodes.Status201Created);

recipeApi.MapDelete("/{id}", (int id) =>
    _recipes.TryRemove(id, out var _) ? Results.NoContent() : Results.NotFound())
    .WithTags("recipe")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status404NotFound);

app.Run();

public record Ingredient(String Name, String Unit);
public record Recipe(
    String Name,
    List<Tuple<Ingredient, decimal>> Ingredients,
    List<String> steps);

public record RecipeResponse(int Id, Recipe Recipe);





