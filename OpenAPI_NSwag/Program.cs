﻿using Fruit;


IFruitClient fc = new FruitClient(
    new HttpClient()
    {
        BaseAddress = new Uri("http://localhost:5000")
    });


Fruit.Fruit created = await fc.PostFruitAsync("123",
    new Fruit.Fruit { Name = "Banana", Stock = 100 });
Console.WriteLine($"created {created.Name}");

Fruit.Fruit fetched = await fc.GetFruitAsync("123");
Console.WriteLine($"fetched {fetched.Name}");

