using DruidsCornerAPI.DatabaseHandlers;
using DruidsCornerAPI.Models.Search;
using DruidsCornerAPI.Models.DiyDog.RecipeDb;
using DruidsCornerAPI.Tools;
using DruidsCornerAPI.Models.DiyDog.IndexedDb;

namespace DruidsCornerAPI.Services
{

    /// <summary>
    /// Search Service class, allows to dig in some database content and use custom filters
    /// To retrieve Recipes from it.
    /// </summary>
    public class SearchService
    {
        private readonly IConfiguration _configuration;
        private ILogger<SearchService> _logger;

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public SearchService(IConfiguration configuration, ILogger<SearchService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }


#region Filters
        /// <summary>
        /// Filters out TargetOg value on a single recipe
        /// </summary>
        /// <param name="recipeSubject">Subject recipe on which we'll apply the query filter</param>
        /// <param name="testSubject">Subject value of the recipe on which we'll apply the query filter</param>
        /// <param name="range">Subject range used to perform the filter</param>
        /// <returns>Selected recipe (matching the search criterion) or null if subject is filtered out by the search query</returns>
        public Recipe? FilterOutRangeValueDiscrete(Recipe recipeSubject, float testSubject, Range<float>? range)
        {
            if(range is null)
            {
                // No filter applied -> All recipes selected
                return recipeSubject;
            }
            
            if(range.InRange(testSubject))
            {
                return recipeSubject;
            }
            return null;
        }



        /// <summary>
        /// Filters out ABV value on a single recipe
        /// </summary>
        /// <param name="subject">Subject recipe on which we'll apply the query filter</param>
        /// <param name="queries">Queries object which is passed along to carry the filter data</param>
        /// <returns>Selected recipe (matching the search criterion) or null if subject is filtered out by the search query</returns>
        public Recipe? FilterOutAbvDiscrete(Recipe subject, Queries queries)
        {
            return FilterOutRangeValueDiscrete(subject, subject.Basics.Abv, queries.Abv);
        }

        /// <summary>
        /// Filters out IBU value on a single recipe
        /// </summary>
        /// <param name="subject">Subject recipe on which we'll apply the query filter</param>
        /// <param name="queries">Queries object which is passed along to carry the filter data</param>
        /// <returns>Selected recipe (matching the search criterion) or null if subject is filtered out by the search query</returns>
        public Recipe? FilterOutIbuDiscrete(Recipe subject, Queries queries)
        {
            return FilterOutRangeValueDiscrete(subject, subject.Basics.Ibu, queries.Ibu);
        }

        /// <summary>
        /// Filters out EBC value on a single recipe
        /// </summary>
        /// <param name="subject">Subject recipe on which we'll apply the query filter</param>
        /// <param name="queries">Queries object which is passed along to carry the filter data</param>
        /// <returns>Selected recipe (matching the search criterion) or null if subject is filtered out by the search query</returns>
        public Recipe? FilterOutEbcDiscrete(Recipe subject, Queries queries)
        {            
            return FilterOutRangeValueDiscrete(subject, subject.Basics.Ebc, queries.Ebc);
        }
    
        /// <summary>
        /// Filters out TargetOg value on a single recipe
        /// </summary>
        /// <param name="subject">Subject recipe on which we'll apply the query filter</param>
        /// <param name="queries">Queries object which is passed along to carry the filter data</param>
        /// <returns>Selected recipe (matching the search criterion) or null if subject is filtered out by the search query</returns>
        public Recipe? FilterOutTargetOgDiscrete(Recipe subject, Queries queries)
        {
            return FilterOutRangeValueDiscrete(subject, subject.Basics.TargetOg, queries.TargetOg);
        }


        /// <summary>
        /// Filters out TargetFg value on a single recipe
        /// </summary>
        /// <param name="subject">Subject recipe on which we'll apply the query filter</param>
        /// <param name="queries">Queries object which is passed along to carry the filter data</param>
        /// <returns>Selected recipe (matching the search criterion) or null if subject is filtered out by the search query</returns>
        public Recipe? FilterOutTargetFgDiscrete(Recipe subject, Queries queries)
        {
            return FilterOutRangeValueDiscrete(subject, subject.Basics.TargetFg, queries.TargetFg);
        }

        /// <summary>
        /// Filters out Ph value on a single recipe
        /// </summary>
        /// <param name="subject">Subject recipe on which we'll apply the query filter</param>
        /// <param name="queries">Queries object which is passed along to carry the filter data</param>
        /// <returns>Selected recipe (matching the search criterion) or null if subject is filtered out by the search query</returns>
        public Recipe? FilterOutPhDiscrete(Recipe subject, Queries queries)
        {
            return FilterOutRangeValueDiscrete(subject, subject.Basics.Ph, queries.Ph);
        }

        /// <summary>
        /// Filters out AttenuationLevel value on a single recipe
        /// </summary>
        /// <param name="subject">Subject recipe on which we'll apply the query filter</param>
        /// <param name="queries">Queries object which is passed along to carry the filter data</param>
        /// <returns>Selected recipe (matching the search criterion) or null if subject is filtered out by the search query</returns>
        public Recipe? FilterOutAttenuationLevelDiscrete(Recipe subject, Queries queries)
        {
            return FilterOutRangeValueDiscrete(subject, subject.Basics.AttenuationLevel, queries.AttenuationLevel);
        }


        /// <summary>
        /// Filters out MashTemps value on a single recipe (Logical OR).
        /// If any MashTemp of the targeted recipe match the requested criteria, the recipe will be accepted as a valid one for this filter.
        /// </summary>
        /// <param name="subject">Subject recipe on which we'll apply the query filter</param>
        /// <param name="queries">Queries object which is passed along to carry the filter data</param>
        /// <returns>Selected recipe (matching the search criterion) or null if subject is filtered out by the search query</returns>
        public Recipe? FilterOutMashTempsDiscrete(Recipe subject, Queries queries)
        {
            Recipe? output = null;
            foreach(var mashTemp in subject.MethodTimings.MashTemps)
            {
                output ??= FilterOutRangeValueDiscrete(subject, mashTemp.Celsius, queries.MashTemps);
            }
            return output;
        }

        /// <summary>
        /// Filters out FermentationTemps value on a single recipe.
        /// </summary>
        /// <param name="subject">Subject recipe on which we'll apply the query filter</param>
        /// <param name="queries">Queries object which is passed along to carry the filter data</param>
        /// <returns>Selected recipe (matching the search criterion) or null if subject is filtered out by the search query</returns>
        public Recipe? FilterOutFermentationTempsDiscrete(Recipe subject, Queries queries)
        {
            return FilterOutRangeValueDiscrete(subject, subject.MethodTimings.Fermentation.Celsius, queries.FermentationTemps);
        }

#endregion Filters

#region FuzzySearchFilters
        /// <summary>
        /// Performs a fuzzy search on property mapping in the given propMapping list (coming from an indexed DB)
        /// </summary>
        /// <param name="propMapping">List of ReversedPropMapping from an indexed Db</param>
        /// <param name="propName">Property name serving as the search parameter</param>
        /// <returns></returns>
        public FuzzySearchResult<ReversedPropMapping> FuzzySearchInIndexedDb(List<ReversedPropMapping> propMapping, string propName)
        {
            return FuzzySearch.SearchInList(propName, propMapping, elem => new List<string>{elem.Name});
        }

        /// <summary>
        /// Performs multiple fuzzy search on several queries of the same kind and returns the merged lists as a result
        /// </summary>
        /// <param name="propMapping">List of reference properties mapping</param>
        /// <param name="queries">List of input queries serving as search parameters</param>
        /// <returns></returns>
        public List<FuzzySearchResult<ReversedPropMapping>> FuzzySearchInIndexedDbMultipleQueries(List<ReversedPropMapping> propMapping, List<string> queries)
        {
            List<FuzzySearchResult<ReversedPropMapping>> candidateProps = new List<FuzzySearchResult<ReversedPropMapping>>();
            foreach(var query in queries)  
            {
                var candidate = FuzzySearchInIndexedDb(propMapping, query);
                if(!candidateProps.Any(elem => elem.Prop!.Name == candidate.Prop!.Name))
                {
                    candidateProps.Add(candidate);
                }
            }
            return  candidateProps;
        }

        /// <summary>
        /// Performs a fuzzy search on a list of recipes, using a list of queries and custom field accessor
        /// </summary>
        /// <param name="candidates">Candidate recipes list</param>
        /// <param name="queries">List of string queries</param>
        /// <param name="accessor">Delegate function that allows to access the appropriate field of Recipe object</param>
        /// <returns></returns>
        public List<FuzzySearchResult<Recipe>> FuzzySearchInRecipesMultipleQueries(List<Recipe> candidates, List<string> queries, GetProp<Recipe> accessor )
        {
            List<FuzzySearchResult<Recipe>> candidatesResults = new List<FuzzySearchResult<Recipe>>();
            foreach(var query in queries)
            {
                var candidateRecipe = FuzzySearch.SearchInList<Recipe>(query, candidates, accessor);
                candidatesResults.Add(candidateRecipe);
            }

            return candidatesResults;
        }

#endregion FuzzySearchFilters        
        
        /// <summary>
        /// Searches for a single recipe using the given queries and database handler.
        /// </summary>
        /// <param name="queries">Input queries used for searching</param>
        /// <param name="dbHandler">Database handler used in current context</param>
        /// <returns></returns>
        public async Task<List<RecipeResult>> SearchRecipe(Queries queries, IDatabaseHandler dbHandler)
        {
            var allRecipes = await dbHandler.GetAllRecipesAsync();
            
            // Perform filtering on all recipes and skip as early as possible
            List<Recipe> candidates = new List<Recipe>();
            foreach(var recipe in allRecipes)
            {
                Recipe? candidate = recipe;
                candidate = FilterOutAbvDiscrete(candidate, queries);
                if(candidate == null) break;

                candidate = FilterOutEbcDiscrete(candidate, queries);
                if(candidate == null) break;

                candidate = FilterOutIbuDiscrete(candidate, queries);
                if(candidate == null) break;

                candidate = FilterOutTargetOgDiscrete(candidate, queries);
                if(candidate == null) break;

                candidate = FilterOutTargetFgDiscrete(candidate, queries);
                if(candidate == null) break;

                candidate = FilterOutAttenuationLevelDiscrete(candidate, queries);
                if(candidate == null) break;

                candidate = FilterOutMashTempsDiscrete(candidate, queries);
                if(candidate == null) break;
                
                candidate = FilterOutFermentationTempsDiscrete(candidate, queries);

                // Then and only then we can add this candidate to the list !
                if(candidate != null)
                {
                    candidates.Add(candidate);
                }
            }

            var indexedMalts = await dbHandler.GetIndexedMaltDbAsync();
            var indexedHops = await dbHandler.GetIndexedHopDbAsync();
            var indexedStyles = await dbHandler.GetIndexedStyleDbAsync();
            var indexedYeasts = await dbHandler.GetIndexedYeastDbAsync();
            var indexedTags = await dbHandler.GetIndexedTagDbAsync();
            var indexedFoodPairings = await dbHandler.GetIndexedFoodPairingDbAsync();
            
            // Fuzzy Search for names
            if(queries.NameList != null)
            {
                var nameCandidatesResults = FuzzySearchInRecipesMultipleQueries(candidates, queries.NameList, c => new List<string>{c.Name});
            }

            // Fuzzy Search for names
            if(queries.ExtraMashList != null)
            {
                var extraMashCandidatesResults = FuzzySearchInRecipesMultipleQueries(candidates, queries.ExtraMashList, c => {
                    // Null values are handled locally in the algorithm
                    return c.Ingredients.extraMashes!.Select(x => x.Name).ToList();
                });
            }

            // Fuzzy Search for names
            if(queries.ExtraBoilList != null)
            {
                var extraMashCandidatesResults = FuzzySearchInRecipesMultipleQueries(candidates, queries.ExtraBoilList, c => {
                    // Null values are handled locally in the algorithm
                    return c.Ingredients.extraBoils!.Select(x => x.Name).ToList();
                });
            }


            List<uint> candidatesIndicesList = new List<uint>();
            if(queries.MaltList != null && indexedMalts != null)
            {
                var maltsCandidates = FuzzySearchInIndexedDbMultipleQueries(indexedMalts.Malts, queries.MaltList);
            }

            if(queries.StyleList != null && indexedStyles != null)
            {
                var stylesCandidates = FuzzySearchInIndexedDbMultipleQueries(indexedStyles.Styles, queries.StyleList);
            }

            if(queries.HopList != null && indexedHops != null)
            {
                var hopsCandidates = FuzzySearchInIndexedDbMultipleQueries(indexedHops.Hops, queries.HopList);
            }

            if(queries.YeastList != null && indexedYeasts != null)
            {
                var yeastsCandidates = FuzzySearchInIndexedDbMultipleQueries(indexedYeasts.Yeasts, queries.YeastList);
            }

            if(queries.TagList != null && indexedTags != null)
            {
                var tagsCandidates = FuzzySearchInIndexedDbMultipleQueries(indexedTags.Tags, queries.TagList);
            }

            if(queries.FoodPairingList != null && indexedFoodPairings != null)
            {
                var foodPairingsCandidates = FuzzySearchInIndexedDbMultipleQueries(indexedFoodPairings.FoodPairing, queries.FoodPairingList);
            }


            List<Recipe> filteredCandidates = new List<Recipe>();
            // Iterate over remaining candidates and filter out the list even more
            // Using the indices found earlier
            foreach(var recipe in candidates)
            {
                if(candidatesIndicesList.Contains(recipe.Number))
                {
                    filteredCandidates.Add(recipe);
                }
            }


            List<RecipeResult> filteredResults = new List<RecipeResult>();

            return filteredResults;
        }
    }
}
