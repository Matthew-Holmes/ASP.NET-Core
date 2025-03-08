var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Welcome to recipe world!");

app.Run();

public record Ingredient(int Id, String Name, String Unit);
public record Recipe(
    int Id,
    String Name,
    List<Dictionary<Ingredient, decimal>> Ingredients,
    List<String> steps);




