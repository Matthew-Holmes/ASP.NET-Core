using SQLitePCL;

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

    }
}
