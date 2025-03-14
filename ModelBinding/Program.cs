using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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

app.MapGet("/category/{id}",
    ([AsParameters] SearchModel model) => $"recieved {model}");

app.MapPost("/users", (UserModel user) => user.ToString()).WithParameterValidation();

app.MapPost("/complex-users", (CreateUserModel user) =>
{
    var validationResults = new List<ValidationResult>();
    var validationContext = new ValidationContext(user);

    // Manually validate IValidatableObject logic
    bool isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

    if (!isValid)
    {
        return Results.ValidationProblem(validationResults.ToDictionary(
            v => v.MemberNames.FirstOrDefault() ?? string.Empty,
            v => new[] { v.ErrorMessage }
        ));
    }

    return Results.Ok(new { message = "User data is valid", data = user });

}).WithParameterValidation(); // Still keeps DataAnnotations validation

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

// AsParameters binding
record struct SearchModel(
    int id,
    int page,
    [FromHeader(Name = "sort")] bool?  sortAsc,
    [FromQuery(Name = "q")]     string search);

// DataAnnotations
public record UserModel
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Your name")]
    public string FirstName { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Last name")]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Phone]
    [Display(Name = "Phone number")]
    public string PhoneNumber { get; set; }
}

public record CreateUserModel : IValidatableObject
{
    [EmailAddress]
    public string Email { get; set; }

    [Phone]
    public string PhoneNumber { get; set; }


    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(Email)
         && string.IsNullOrEmpty(PhoneNumber))
        {
            yield return new ValidationResult(
                "you must provide an Email or a PhoneNumber",
                new[] { nameof(Email), nameof(PhoneNumber) });
        }
    }
}

