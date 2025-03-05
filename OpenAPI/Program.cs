using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer(); // endpoint discovery features in Swashbuckle requires
builder.Services.AddSwaggerGen();           // Swashbuckle service required for generating OpenApi documents

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
        : Results.Problem(statusCode: 404));

app.MapPost("/fruit/{id}", (string id, Fruit fruit) =>
    _fruit.TryAdd(id, fruit)
        ? TypedResults.Created($"/fruit/{id}", fruit)
        : Results.ValidationProblem(new Dictionary<string, string[]>
        {
            {"id", new[] {"A fruit with this id already exists"} }
        }));


app.Run();

record Fruit(String Name, int Stock);