using DruidsCornerAPI.Models.DiyDog;

namespace DruidsCornerAPI.DatabaseHandlers
{
    public interface IDatabaseHandler
    {
        public Task<Recipe?> GetRecipe(uint number, bool noCaching = false);
        
        public Task<Recipe?> GetRecipe(string name, bool noCaching = false);

        public Task<List<Recipe>> GetAllRecipeAsync(bool noCaching = false);
    
    }
}
