var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddRazorPages();

var app = builder.Build();


app.MapGet("/", () => "Hello World!").WithName("hello"); // minimal API endpoint
app.MapHealthChecks("/healthz");                         // register a health-check endpoint 
app.MapRazorPages();                                     // register all razor pages as endpoints

// link generator (note endpoint name metadata ARE case sensitive)
app.MapGet("/product/{name}", (string name) => $"the product is {name}")
        .WithName("product"); // metadata

app.MapGet("/links", (LinkGenerator links) =>
{
    /* creates a lik using the route name */
    string link = links.GetPathByName("product",
        new { name = "big-widget" });
    return $"view the product at {link}";
});

// redirects
app.MapGet("/redirect-me", () => Results.RedirectToRoute("hello")); // generates response that redirects to the "hello" endpoint
app.Run();
