var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<RouteOptions>(o =>
{
    o.LowercaseUrls = true;
    o.AppendTrailingSlash = true;
    o.LowercaseQueryStrings = false;
});


builder.Services.AddHealthChecks();
builder.Services.AddRazorPages();

var app = builder.Build();


app.MapGet("/", () => "Hello World!").WithName("hello"); // minimal API endpoint
app.MapHealthChecks("/healthz").WithName("healthcheck"); // register a health-check endpoint 
app.MapRazorPages();                                     // register all razor pages as endpoints

// link generator (note endpoint name metadata ARE case sensitive)
app.MapGet("/product/{name}/", (string name) => $"the product is {name}")
        .WithName("product"); // metadata

app.MapGet("/links", (LinkGenerator links) =>
new []
{
    links.GetPathByName("healthcheck"),
    links.GetPathByName("product",
        new { name = "big-widget", Q = "Test"}),
});

// redirects
app.MapGet("/redirect-me", () => Results.RedirectToRoute("hello")); // generates response that redirects to the "hello" endpoint
app.Run();
