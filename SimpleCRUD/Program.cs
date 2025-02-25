using Microsoft.AspNetCore.Mvc.Formatters;
using System.Collections.Concurrent;
using System.Net.Mime;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Welcome to fruit world!");

var _fruit = new ConcurrentDictionary<string, Fruit>(); // for API thread safety
_fruit["1"] = new Fruit("mango", 7);
_fruit["2"] = new Fruit("pear", 12);

app.MapGet("/fruit", () => _fruit);

app.MapGet("/fruit/{id}", (string id) =>
    _fruit.TryGetValue(id, out Fruit? fruit)
          ? TypedResults.Ok(fruit) /* 200 */
          : Results.NotFound()     /* 404 */);

// not idempotent therefore second call can complain
app.MapPost("/fruit/{id}", (string id, Fruit fruit) =>
    _fruit.TryAdd(id, fruit)
          ? TypedResults.Created($"/fruit/{id}", fruit) /* 201 */
          : Results.BadRequest(new                      /* 400 */
                { id = "a fruit with this id already exists" }));

app.MapPut("/fruit/{id}", (string id, Fruit fruit) =>
{
    _fruit[id] = fruit;
    return Results.NoContent(); /* 204 */
});

app.MapDelete("/fruit/{id}", (string id) =>
{
    _fruit.TryRemove(id, out _);
    return Results.NoContent(); /* 204 */
});

// fine grained status code control
app.MapGet("/teapot", (HttpResponse response) =>
{
    response.StatusCode = 418;
    response.ContentType = MediaTypeNames.Text.Plain;
    return response.WriteAsync("I'm a teapot!");
});

app.Run();

record Fruit(string Name, int Stock)
{
    public static readonly Dictionary<string, Fruit> All = new();
}


