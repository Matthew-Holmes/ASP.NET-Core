using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Collections.Concurrent;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer(); // endpoint discovery features in Swashbuckle requires
builder.Services.AddSwaggerGen(opts =>      /* Swashbuckle service required for generating OpenApi documents */
{
    // enable xml comments for the OpenAPI descriptions
    var file = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    opts.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, file));

    opts.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "fruitworld",
        Description = "network mediated human fruit interaction",
        Version = "1.0"
    });
});

var app = builder.Build();

var _fruit = new ConcurrentDictionary<String, Fruit>();

_fruit["mango"] = new Fruit("mango", 7);
_fruit["apple"] = new Fruit("apple", 5);

var handler = new FruitHandler(_fruit);

app.UseSwagger();   // middleware to expose OpenAPI document for the app
app.UseSwaggerUI(); // middleware that serves Swagger UI

app.MapGet("/", () => "welcome to fruit world");


// now most of the metadata is xml comment-annotations in the handler
app.MapGet("/fruit/{id}", handler.GetFruit)
    .WithName("GetFruit"); /* still need some metadata here */

app.MapPost("/fruit/{id}", (string id, Fruit fruit) =>
    _fruit.TryAdd(id, fruit)
        ? TypedResults.Created($"/fruit/{id}", fruit)
        : Results.ValidationProblem(new Dictionary<string, string[]>
        {
            {"id", new[] {"A fruit with this id already exists"} }
        }))
    .WithName("PostFruit")
    .WithTags("fruit")
    .Produces<Fruit>(201) /* also produces fruit but with 201 response */
    .ProducesValidationProblem(/* 400 */)
    .WithSummary("adds a new fruit")
    .WithDescription("adds a new fruit by id, or returns 400"
        + " if a fruit with that id already exists")
    .WithOpenApi();


app.Run();

record Fruit(String Name, int Stock);

internal class FruitHandler
{
    private readonly ConcurrentDictionary<string, Fruit> _fruit;

    public FruitHandler(ConcurrentDictionary<string, Fruit> fruit)
    {
        _fruit = fruit;
    }


    /// <summary>
    /// Fetches a fruit by id, or returns 404 if it does not exist
    /// </summary>
    /// <param name="id">The id of the fruit to fetch</param>
    /// <response code="200">Returns the fruit if it does exist</response>
    /// <response code="404">If the fruit doesn't exist</response>
    [ProducesResponseType(typeof(Fruit), 200)]
    [ProducesResponseType(typeof(HttpValidationProblemDetails), 400, "application/problem+json")] // declare response format
    [Tags("fruit")]
    public IResult GetFruit(string id) => _fruit.TryGetValue(id, out var fruit)
        ? TypedResults.Ok(fruit)
        : Results.Problem(statusCode: 404);
}
