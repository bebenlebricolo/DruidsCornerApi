using DruidsCornerAPI.Models.DiyDog.RecipeDb;
using DruidsCornerAPI.Models.DiyDog.IndexedDb;
using DruidsCornerAPI.Models.DiyDog.References;
using DruidsCornerAPI.Models.Search;

namespace DruidsCornerAPI.DatabaseHandlers
{
    /// <summary>
    /// Abstracts a database handler, and allows to polymorphically switch the current database handler used.
    /// </summary>
    public interface IDatabaseHandler
    {
        /// <summary>
        /// Retrieves a recipe using it's number/id 
        /// </summary>
        /// <param name="number">Recipe's unique number/id</param>
        /// <param name="noCaching">Optional noCaching parameter. If set to true, it'll prevent the database handler to use 
        /// internal caching and save up some memory. Otherwise, it'll cache data and speed up subsequent requests. </param>
        /// <returns>Found recipe, or null in case none was found with the requested number</returns>
        public Task<Recipe?> GetRecipeByNumberAsync(uint number, bool noCaching = false);
        
        /// <summary>
        /// Retrieves a recipe using it's name. This method might use FuzzySearch algorithms in order to find a recipe that has
        /// the highest match score.
        /// </summary>
        /// <param name="name">Recipe's name / query string</param>
        /// <param name="noCaching">Optional noCaching parameter. If set to true, it'll prevent the database handler to use 
        /// internal caching and save up some memory. Otherwise, it'll cache data and speed up subsequent requests. </param>
        /// <returns>Found recipe alongside it's hit rating</returns>
        public Task<RecipeResult> GetRecipeByNameAsync(string name, bool noCaching = false);

        /// <summary>
        /// Retrieves all available recipes from database store.
        /// </summary>
        /// <param name="noCaching">Optional noCaching parameter. If set to true, it'll prevent the database handler to use 
        /// internal caching and save up some memory. Otherwise, it'll cache data and speed up subsequent requests. </param>
        /// <returns>List of available recipes</returns>
        public Task<List<Recipe>> GetAllRecipesAsync(bool noCaching = false);

        /// <summary>
        /// Retrieves a recipe's image using it's number/id as a search parameter.
        /// </summary>
        /// <param name="number">Recipe's number / id</param>
        /// <param name="noCaching">Optional noCaching parameter. If set to true, it'll prevent the database handler to use 
        /// internal caching and save up some memory. Otherwise, it'll cache data and speed up subsequent requests. </param>
        /// <returns>Recipe's image stream, or null in case none was found with the requested number</returns>
        public Task<Stream?> GetRecipeImageAsync(uint number, bool noCaching = false);
        
        /// <summary>
        /// Retrieves a recipe pdf page using it's number as a search parameter.
        /// </summary>
        /// <param name="number">Recipe's number / id.</param>
        /// <param name="noCaching">Optional noCaching parameter. If set to true, it'll prevent the database handler to use 
        /// internal caching and save up some memory. Otherwise, it'll cache data and speed up subsequent requests. </param>
        /// <returns>Found recipe's pdf page stream, or null in case none was found with the requested number</returns>
        public Task<Stream?> GetRecipePdfPageAsync(uint number, bool noCaching = false);


        /// #################################################################################################
        /// ############################# Known good databases accessors ####################################
        /// #################################################################################################

        /// <summary>
        /// Retrieves a known good hops list from current dbs
        /// </summary>
        /// <returns>Hops reference object or null</returns>
        public Task<ReferenceHops?> GetReferenceHopsAsync();
        
        /// <summary>
        /// Retrieves a known good malts list from current dbs
        /// </summary>
        /// <returns>Malts reference object or null</returns>
        public Task<ReferenceMalts?> GetReferenceMaltsAsync();
         
        /// <summary>
        /// Retrieves a known good yeasts list from current dbs
        /// </summary>
        /// <returns>Yeasts reference object or null</returns>
        public Task<ReferenceYeasts?> GetReferenceYeastsAsync();
         
        /// <summary>
        /// Retrieves a known good styles list from current dbs
        /// </summary>
        /// <returns>Styles reference object or null</returns>
        public Task<ReferenceStyles?> GetReferenceStylesAsync();

        
        /// #################################################################################################
        /// ############################### Indexed databases accessors #####################################
        /// #################################################################################################

        /// <summary>
        /// Retrieves an indexed Database (reversed indexing) using the kind parameter as the query parameter.
        /// </summary>
        /// <returns>IndexedDb or null</returns>
        public Task<IndexedDb?> GetIndexedDbAsync(IndexedDbPropKind kind);
    }
}
