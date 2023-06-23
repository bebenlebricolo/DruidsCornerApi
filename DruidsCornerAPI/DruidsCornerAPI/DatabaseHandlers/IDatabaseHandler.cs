using DruidsCornerAPI.Models.DiyDog;

namespace DruidsCornerAPI.DatabaseHandlers
{
    public interface IDatabaseHandler
    {
        public Task<Recipe?> GetRecipeByNumberAsync(uint number, bool noCaching = false);
        
        public Task<Recipe?> GetRecipeByNameAsync(string name, bool noCaching = false);

        public Task<List<Recipe>> GetAllRecipesAsync(bool noCaching = false);

        public Task<Stream?> GetRecipeImageAsync(uint number, bool noCaching = false);
        
        public Task<Stream?> GetRecipePdfPageAsync(uint number, bool noCaching = false);

    }
}
