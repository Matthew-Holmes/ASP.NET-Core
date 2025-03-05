using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// later additions can overwrite ealier
builder.Configuration.Sources.Clear();
builder.Configuration.AddJsonFile("appsettings.json", optional: true);
builder.Configuration.AddJsonFile("sharedSettings.json", optional: true);
builder.Configuration.AddEnvironmentVariables();
//builder.Configuration.AddEnvironmentVariables("SomePrefix"); // only variables like SomePrefix__MyVar

var zoomlevel = builder.Configuration["MapSettings:DefaultZoomLevel"];
var lat1 = builder.Configuration["MapSettings:DefaultLocation:Latitude"];
var lat2 = builder.Configuration.GetSection("MapSettings")["DefaultLocation:Latitude"];


var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/config", () => app.Configuration.AsEnumerable());
// can also access via dependency injection
app.MapGet("/config/di", (IConfiguration config) => config.AsEnumerable());
app.MapGet("/display-settings",
    (IOptions<AppDisplaySettings> options) =>
{
    AppDisplaySettings settings = options.Value;
    string title = settings.Title;
    bool showCopyright = settings.ShowCopyright;

    return new { title, showCopyright };
});

app.Run();


public class AppDisplaySettings
{
    public string Title { get; set; } = string.Empty;
    public bool ShowCopyright { get; set; } = false;
}
