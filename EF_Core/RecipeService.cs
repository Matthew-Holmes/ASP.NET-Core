using Microsoft.EntityFrameworkCore;

namespace EF_Core
{
    // use Create_XXX_Command pattern to mitigate Overposting security vulnerability

    public record CreateIngredientCommand(String Name, decimal Quantity, String Unit);

    public record CreateRecipeCommand(
        String Name,
        int    TimeToCookHrs,
        int    TimeToCookMins,
        String Method,
        bool   IsVegetarian,
        bool   IsVegan,
        IEnumerable<CreateIngredientCommand> Ingredients
        );

    public record RecipeSummaryViewModel
    {
        public int    Id { get; init; }
        public String Name { get; init; }
        public String TimeToCook { get; init; }
    }

    public record RecipeDetailViewModel 
    {
        public record Item
        {
            public String Name     { get; init; }
            public String Quantity { get; init; }
        }
        public int    Id     { get; init; }
        public String Name   { get; init; }
        public String Method { get; init; }
        public IEnumerable<Item> Ingredients { get; init; }
    }



    public class RecipeService
    {
        readonly AppDbContext _context;
        public RecipeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateRecipe(CreateRecipeCommand cmd)
        {
            var recipe = new Recipe
            {
                Name         = cmd.Name,
                Method       = cmd.Method,
                IsVegitarian = cmd.IsVegetarian,
                IsVegan      = cmd.IsVegan,

                Ingredients  = cmd.Ingredients.Select(i =>
                    new Ingredient
                    {
                        Name     = i.Name,
                        Quantity = i.Quantity,
                        Unit     = i.Unit
                    }).ToList(),

                TimeToCook = new TimeSpan(
                    cmd.TimeToCookHrs, cmd.TimeToCookMins, 0),
            };
            _context.Add(recipe);   // tell ef core to track the new entities

            await _context.SaveChangesAsync();

            return recipe.RecipeId; // ef core populated this field when it saved the recipe
        }

        public async Task<ICollection<RecipeSummaryViewModel>> GetRecipes()
        {
            return await _context.Recipes
                .Where( r => !r.IsDeleted)
                .Select(r => new RecipeSummaryViewModel
                {
                    Id         = r.RecipeId,
                    Name       = r.Name,
                    TimeToCook = $"{r.TimeToCook.TotalMinutes}mins"
                })
                .ToListAsync();
        }

        public async Task<RecipeDetailViewModel?> GetRecipeDetail(int id)
        {
            return await _context.Recipes
                .Where(x => x.RecipeId == id)
                .Select(x => new RecipeDetailViewModel
                {
                    Id          = x.RecipeId,
                    Name        = x.Name,
                    Method      = x.Method,
                    Ingredients = x.Ingredients
                        .Select(item => new RecipeDetailViewModel.Item
                        {
                            Name     = item.Name,
                            Quantity = $"{item.Quantity} {item.Unit}"
                        })
                })
                .SingleOrDefaultAsync();
        }

    }
}
