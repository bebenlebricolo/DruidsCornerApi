using DruidsCornerAPI.Models.DiyDog;
using DruidsCornerAPI.Models.SearchResults;

namespace DruidsCornerAPI.DatabaseHandlers
{
    public interface IDatabaseHandler
    {
        public Task<Recipe?> GetRecipeByNumberAsync(uint number, bool noCaching = false);
        
        public Task<RecipeResult> GetRecipeByNameAsync(string name, bool noCaching = false);

        public Task<List<Recipe>> GetAllRecipesAsync(bool noCaching = false);

        public Task<Stream?> GetRecipeImageAsync(uint number, bool noCaching = false);
        
        public Task<Stream?> GetRecipePdfPageAsync(uint number, bool noCaching = false);

    }
}
