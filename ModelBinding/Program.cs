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
app.MapPost("/size", (SizeDetails size) => $"recieved {size}");

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

// custom binding
public record SizeDetails(double height, double width)
{
    public static async ValueTask<SizeDetails?> BindAsync(
        HttpContext context)
    {
        using var sr = new StreamReader(context.Request.Body);

        string? line1 = await sr.ReadLineAsync(context.RequestAborted);
        if (line1 is null) { return null; }

        string? line2 = await sr.ReadLineAsync(context.RequestAborted);
        if (line2 is null) { return null; }

        return double.TryParse(line1, out double height)
            && double.TryParse(line2, out double width)
            ? new SizeDetails(height, width) : null;
    }
}


