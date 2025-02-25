var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddRazorPages();

var app = builder.Build();


app.MapGet("/", () => "Hello World!"); // minimal API endpoint
app.MapHealthChecks("/healthz");       // register a health-check endpoint 
app.MapRazorPages();                   // register all razor pages as endpoints

app.Run();
