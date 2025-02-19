var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//app.UseWelcomePage();
//app.UseDeveloperExceptionPage(); // automatic if in dev environment
app.UseStaticFiles();
app.UseRouting();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}

app.MapGet("/", () => "Hello World!");
app.MapGet("/async", async () =>
{
    await Task.Delay(100);
    throw new Exception("my custom async exception");
});

app.MapGet("/error", () => "Sorry, an error occurred");


app.Run();
