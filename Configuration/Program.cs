var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/config", () => app.Configuration.AsEnumerable());

app.Run();
