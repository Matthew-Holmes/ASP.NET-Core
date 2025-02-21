var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Welcome to fruit world!");

app.MapGet("/fruit", () => Fruit.All);
var getFruit = (string id) => Fruit.All[id];
app.MapGet("/fruit/{id}", getFruit);

Handlers handler = new();
Handlers.AddFruit("1", new Fruit("mango", 7));
Handlers.AddFruit("2", new Fruit("pear", 12));

app.MapPut("/fruit/{id}", handler.ReplaceFruit);

app.MapDelete("/fruit/{id}", DeleteFruit);


app.Run();

// Warning! this app is simple, isn't thread safe, and doesn't handle edge cases

void DeleteFruit(string id)
{
    Fruit.All.Remove(id);
}

record Fruit(string Name, int Stock)
{
    public static readonly Dictionary<string, Fruit> All = new();
}

class Handlers
{
    public void ReplaceFruit(string id, Fruit fruit)
    {
        Fruit.All[id] = fruit;
    }

    public static void AddFruit(string id, Fruit fruit)
    {
        Fruit.All.Add(id, fruit);
    }
}

