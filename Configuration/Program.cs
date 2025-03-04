var builder = WebApplication.CreateBuilder(args);

builder.Configuration.Sources.Clear();
builder.Configuration.AddJsonFile("appsettings.json", optional: true);

var zoomlevel = builder.Configuration["MapSettings:DefaultZoomLevel"];
var lat1 = builder.Configuration["MapSettings:DefaultLocation:Latitude"];
var lat2 = builder.Configuration.GetSection("MapSettings")["DefaultLocation:Latitude"];



var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/config", () => app.Configuration.AsEnumerable());
// can also access via dependency injection
app.MapGet("/config/di", (IConfiguration config) => config.AsEnumerable());

app.Run();
