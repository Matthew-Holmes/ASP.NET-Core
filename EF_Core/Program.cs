using System.Collections.Concurrent;
using System.Drawing;
using EF_Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<RecipeService>();

var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlite(connString));

var app = builder.Build();
app.UseSwagger();   
app.UseSwaggerUI(); 


app.MapGet("/", () => "Welcome to recipe world!");


RouteGroupBuilder recipeApi = app.MapGroup("/recipe");
recipeApi.MapGet("/", (RecipeService rs) => rs.GetRecipes())
    .WithTags("recipe")
    .Produces<ICollection<RecipeSummaryViewModel>>(StatusCodes.Status200OK);

recipeApi.MapGet("/{id}", (RecipeService rs, int id)
    => rs.GetRecipeDetail(id) is { /* matches any non-null */ } recipe
            ? TypedResults.Ok(recipe)
            : Results.NotFound())
    .WithTags("recipe")
    .Produces<RecipeDetailViewModel>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status404NotFound);

// not idempotent
recipeApi.MapPost("/", async (RecipeService rs, CreateRecipeCommand cmd) =>
{
    if (cmd == null)
        return Results.BadRequest(new { message = "Recipe data is required." });

    int id = await rs.CreateRecipe(cmd);

    return TypedResults.Created($"/recipes/{id}", new RecipeResponse(id));
})
    .WithTags("recipe")
    .Produces<RecipeResponse>(StatusCodes.Status201Created) 
    .ProducesProblem(StatusCodes.Status400BadRequest);  


app.Run();


public record RecipeResponse(int Id);




