using Microsoft.AspNetCore.Mvc.Formatters;
using System.Collections.Concurrent;
using System.Net.Mime;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails(); // adds the IProblemDetails implementation

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(); // no path, so uses IProblemDetailsService
}

app.UseStatusCodePages();

//app.MapGet("/", void () => throw new Exception());
app.MapGet("/", () => "Welcome to fruit world!");

var _fruit = new ConcurrentDictionary<string, Fruit>(); // for API thread safety
_fruit["f1"] = new Fruit("mango", 7);
_fruit["f2"] = new Fruit("pear", 12);

app.MapGet("/fruit", () => _fruit);

app.MapGet("/fruit/{id}", (string id) =>
    _fruit.TryGetValue(id, out Fruit? fruit)
          ? TypedResults.Ok(fruit) /* 200 */
          : Results.Problem(statusCode: 404))
    .AddEndpointFilter(ValidationHelper.ValidateID) /* warning - can short-circut and omit logging! */
    .AddEndpointFilter(async (context, next) =>
    {
        app.Logger.LogInformation("Executing filter...");
        object? result = await next(context);
        app.Logger.LogInformation($"Handler results: {result}");
        return result;
    });

// not idempotent therefore second call can complain
app.MapPost("/fruit/{id}", (string id, Fruit fruit) =>
    _fruit.TryAdd(id, fruit)
          ? TypedResults.Created($"/fruit/{id}", fruit) /* 201 */
          : Results.ValidationProblem(new Dictionary<string, string[]>
            {
                { "id", new [] { "a fruit with this id already exists"} }
            }));


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

class ValidationHelper
{
    internal static async ValueTask<object?> ValidateID(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var id = context.GetArgument<string>(0);
        if (string.IsNullOrEmpty(id) || !id.StartsWith('f'))
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    {"id", new[]{"invalid format, id must start with 'f'"}}
                });
        }
        return await next(context);
    }
}

record Fruit(string Name, int Stock)
{
    public static readonly Dictionary<string, Fruit> All = new();
}


