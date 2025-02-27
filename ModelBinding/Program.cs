using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/paged-products/{id}/paged",
    ([FromRoute] int id,
     [FromQuery] int page,
     [FromHeader(Name = "PageSize")] int pageSize)
    => $"received id {id}, page {page}, pageSize {pageSize}");

app.MapGet("/product/{id}", (ProductId id) => $"recieved {id}");

// warning - never run any files uploaded!
app.MapPost("/upload", (IFormFile file) => $"recieved file of size {file.Length}")
    .DisableAntiforgery(); // don't do this

app.Run();

readonly record struct ProductId(int id)
{
    public static bool TryParse(string? s, out ProductId result)
    {
        if (s is not null
            && s.StartsWith('p')
            /* efficiently slice by treating the string as a ReadOnlySpan */
            && int.TryParse(s.AsSpan().Slice(1), out int id))
        {
            result = new ProductId(id);
            return true;
        }
        result = default;
        return false;
    }

}

