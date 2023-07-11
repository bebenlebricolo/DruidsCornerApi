using System.Text.Json.Serialization;
using DruidsCornerAPI.Models.DiyDog.RecipeDb;

namespace DruidsCornerAPI.Models.Search
{
    /// <summary>
    /// Encapsulates a Recipe search result 
    /// </summary>
    public record MultipleRecipeResult
    {
        /// <summary>
        /// List of recipes that match input query
        /// <see cref="Queries"/> 
        /// <see cref="Recipe"/> 
        /// </summary>
        public List<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}