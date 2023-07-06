namespace DruidsCornerAPI.Models.DiyDog
{
    /// <summary>
    /// Collection of Recipes.
    /// Used to serialize/deserialize content of all_recipes.json (list of recipes)
    /// </summary>
    /// <value></value>
    public record AllRecipesCollection
    {
        public List<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}
