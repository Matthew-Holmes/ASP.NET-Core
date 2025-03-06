using Microsoft.OpenApi.Models;
using System.Collections.Concurrent;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer(); // endpoint discovery features in Swashbuckle requires
builder.Services.AddSwaggerGen(x =>         /* Swashbuckle service required for generating OpenApi documents */
    x.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title       = "fruitworld",
        Description = "network mediated human fruit interaction",
        Version     = "1.0"
    }));

var app = builder.Build();

var _fruit = new ConcurrentDictionary<String, Fruit>();

_fruit["mango"] = new Fruit("mango", 7);
_fruit["apple"] = new Fruit("apple", 5);

app.UseSwagger();   // middleware to expose OpenAPI document for the app
app.UseSwaggerUI(); // middleware that serves Swagger UI

app.MapGet("/", () => "welcome to fruit world");

app.MapGet("/fruit/{id}", (string id) =>
    _fruit.TryGetValue(id, out var fruit)
        ? TypedResults.Ok(fruit)
        : Results.Problem(statusCode: 404))
    .WithTags("fruit") /* creates a tag group in Swagger UI */
    .Produces<Fruit>(/* default is 200 */)
    .ProducesProblem(404);

app.MapPost("/fruit/{id}", (string id, Fruit fruit) =>
    _fruit.TryAdd(id, fruit)
        ? TypedResults.Created($"/fruit/{id}", fruit)
        : Results.ValidationProblem(new Dictionary<string, string[]>
        {
            {"id", new[] {"A fruit with this id already exists"} }
        }))
    .WithTags("fruit")
    .Produces<Fruit>(201) /* also produces fruit but with 201 response */
    .ProducesValidationProblem(/* 400 */);


app.Run();

record Fruit(String Name, int Stock);