namespace DruidsCornerAPI.Models.DiyDog
{
    public record AllRecipesCollection
    {
        public List<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}
