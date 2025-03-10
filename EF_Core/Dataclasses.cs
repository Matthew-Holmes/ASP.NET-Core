namespace EF_Core
{
    public class Recipe
    {
        public int RecipeId { get; set; }
        public required string Name { get; set; }
        public TimeSpan TimeToCook { get; set; }
        public bool IsDeleted { get; set; }
        public required string Method { get; set; }
    }

    public class Ingredient
    {
        public int IngredientID { get; set; }
        public int RecipeID { get; set; }
        public required string Name { get; set; }
        public decimal Quantity { get; set; }
        public required string Unit { get; set; }
}
