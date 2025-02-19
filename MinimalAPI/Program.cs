using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);
#region logging setup
builder.Services.AddHttpLogging(opts =>
    opts.LoggingFields = HttpLoggingFields.RequestProperties);

builder.Logging.AddFilter(
    "Microsoft.AspNetCore.HttpLogging", LogLevel.Information);
#endregion
var app = builder.Build();

var people = new List<Person>
{
    new("Matthew Holmes", "Matthew"),
    new("Thomas Hanks", "Tom"),
    new("Denzel Washington", "Denzel"),
    new("Leonardo DiCaprio", "Leo")
};


app.MapGet("/person/{name}", (string name) =>
    people.Where(p => p.PreferredName == name));


app.Run();


public record Person(string FullName, string PreferredName);