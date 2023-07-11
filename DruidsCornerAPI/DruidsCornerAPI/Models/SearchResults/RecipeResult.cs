using System.Text.Json.Serialization;
using DruidsCornerAPI.Models.DiyDog;

namespace DruidsCornerAPI.Models.SearchResults
{
    /// <summary>
    /// Encapsulates a Recipe search result 
    /// </summary> <summary>
    public record RecipeResult
    {
        /// <summary>
        /// Recipe's relevance (the higher the Probability - or "Score" - the better)
        /// </summary>
        public int Probability { get; set; } = 0;

        /// <summary>
        /// Found recipe
        /// </summary> <summary>
        public Recipe Recipe { get; set; }

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="probability"></param>
        /// <param name="recipe"></param>
        public RecipeResult(int probability, Recipe recipe)
        {
            Probability = probability;
            Recipe = recipe;
        }
    }
}