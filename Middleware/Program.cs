var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//app.UseWelcomePage();
app.UseDeveloperExceptionPage();
app.UseStaticFiles();
app.UseRouting();

app.MapGet("/", () => "Hello World!");
app.MapGet("/async", async () =>
{
    await Task.Delay(100);
    throw new Exception("my custom async exception");
});

app.Run();
