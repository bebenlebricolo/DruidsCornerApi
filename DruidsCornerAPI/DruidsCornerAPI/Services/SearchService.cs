using DruidsCornerAPI.Tools;
using DruidsCornerAPI.Models.Search;
using DruidsCornerAPI.DatabaseHandlers;
using DruidsCornerAPI.Models.DiyDog.RecipeDb;
using DruidsCornerAPI.Models.DiyDog.IndexedDb;
using System.Security.Cryptography;
using DruidsCornerAPI.Models.DiyDog.References;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DruidsCornerAPI.Controllers;

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


#region RangeFilters
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
#endregion RangeFilters

#region FuzzySearchFilters

#region GenericFilters
        /// <summary>
        /// Performs a fuzzy search on property mapping in the given propMapping list (coming from an indexed DB)
        /// </summary>
        /// <param name="propMapping">List of ReversedPropMapping from an indexed Db</param>
        /// <param name="propName">Property name serving as the search parameter</param>
        /// <returns></returns>
        public FuzzySearchResult<ReversedPropMapping>? FuzzySearchInIndexedDb(List<ReversedPropMapping> propMapping, string propName)
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
                if(candidate == null)
                {
                    continue;
                }

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
            if(candidates.Count == 0)
            {
                return new List<FuzzySearchResult<Recipe>>();
            }
            
            List<FuzzySearchResult<Recipe>> candidatesResults = new List<FuzzySearchResult<Recipe>>();
            foreach(var query in queries)
            {
                var candidateRecipe = FuzzySearch.SearchInList<Recipe>(query, candidates, accessor);
                if(candidateRecipe == null)
                {
                    continue;
                }
                candidatesResults.Add(candidateRecipe);
            }

            return candidatesResults;
        }

        /// <summary>
        /// Performs a fuzzy search on a list of recipes, using a list of queries and custom field accessor
        /// </summary>
        /// <param name="propList">Property list used as a search target</param>
        /// <param name="queries">List of string queries</param>
        /// <param name="accessor">Custom accessor expression that returns a List of string from input T property</param>
        /// <returns></returns>
        public List<FuzzySearchResult<T>> FuzzySearchInReferenceProperties<T>(List<T> propList, List<string> queries, Func<T, List<string>> accessor) where T : BaseProperty
        {
            var candidatesResults = new List<FuzzySearchResult<T>>();
            foreach(var query in queries)
            {
                // Generate a list containing names and aliases all together.
                // This might need dedicated handling if we need a stronger weight on Name property, but this should work as well, with even weights.
                var candidates = FuzzySearch.SearchInListFullResults<T>(query, propList, item => accessor.Invoke(item));
                foreach(var elem in candidates)
                {
                    candidatesResults.Add(elem);
                }
            }

            candidatesResults.Sort((elem1, elem2) => elem2.Ratio.CompareTo(elem1.Ratio));
            return candidatesResults;
        }

#endregion GenericFilters

        /// <summary>
        /// Small wrapper to get matching recipe using a Name only query
        /// </summary>
        /// <param name="candidates"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public List<Recipe> GetMatchingRecipeByNames( List<Recipe> candidates, List<string>? names)
        {
            // Fuzzy Search for names
            if(names != null)
            {
                var nameCandidatesResults = FuzzySearchInRecipesMultipleQueries(candidates, names, c => new List<string>{c.Name});;
                candidates = KeepSimilar(candidates, nameCandidatesResults);
            }
            return candidates;
        }

        /// <summary>
        /// Small wrapper around <see cref="FuzzySearchInIndexedDbMultipleQueries"/> for the specified type (used for abstraction and testing)
        /// </summary>
        /// <param name="candidates"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public List<Recipe> GetMatchingRecipeByExtraMashIngredients( List<Recipe> candidates, List<string>? names)
        {
            // Fuzzy Search for names
            if(names != null)
            {
                var extraMashCandidatesResults = FuzzySearchInRecipesMultipleQueries(candidates, names, c => {
                    if(c.Ingredients.ExtraMash == null)
                    {
                        return new List<string>();
                    }
                    // Null values are handled locally in the algorithm
                    return c.Ingredients.ExtraMash.Select(x => x.Name).ToList();
                });
                candidates = KeepSimilar(candidates, extraMashCandidatesResults);
            }
            
            return  candidates;
        }

        /// <summary>
        /// Small wrapper around <see cref="FuzzySearchInIndexedDbMultipleQueries"/> for the specified type (used for abstraction and testing)
        /// </summary>
        /// <param name="candidates"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public List<Recipe> GetMatchingRecipeByExtraBoilIngredients( List<Recipe> candidates, List<string>? names)
        {
            // Fuzzy Search for names
            if(names != null)
            {
                var extraBoilCandidatesResults = FuzzySearchInRecipesMultipleQueries(candidates, names, c => {
                    if(c.Ingredients.ExtraBoil == null)
                    {
                        return new List<string>();
                    }
                    // Null values are handled locally in the algorithm
                    return c.Ingredients.ExtraBoil.Select(x => x.Name).ToList();
                });
                candidates = KeepSimilar(candidates, extraBoilCandidatesResults);
            }
            return  candidates;
        }

        /// <summary>
        /// Find matching recipes in candidates list and returns a new list containing only recipes that uses the queried items
        /// </summary>
        /// <param name="candidates">List of candidate recipes</param>
        /// <param name="indexedMalts">Indexed Malts database</param>
        /// <param name="names">List of malt names</param>
        public List<Recipe> GetMatchingRecipeByMalts(List<Recipe> candidates, IndexedMaltDb? indexedMalts, List<string>? names)
        {
            if(names != null && indexedMalts != null)
            {
                var maltsCandidates = FuzzySearchInIndexedDbMultipleQueries(indexedMalts.Malts, names);
                candidates = KeepSimilarIndices(candidates, maltsCandidates);
            }
            return candidates;
        }

        /// <summary>
        /// Small wrapper around <see cref="FuzzySearchInIndexedDbMultipleQueries"/> for the specified type (used for abstraction and testing)
        /// </summary>
        /// <param name="candidates"></param>
        /// <param name="indexedStyle"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public List<Recipe> GetMatchingRecipeByStyles(List<Recipe> candidates, IndexedStyleDb? indexedStyle, List<string>? names)
        {
            if(names != null && indexedStyle != null)
            {
                var stylesCandidates = FuzzySearchInIndexedDbMultipleQueries(indexedStyle.Styles, names);
                candidates = KeepSimilarIndices(candidates, stylesCandidates);
            }
            return candidates;
        }


        
        /// <summary>
        /// Small wrapper around <see cref="FuzzySearchInIndexedDbMultipleQueries"/> for the specified type (used for abstraction and testing)
        /// </summary>
        /// <param name="candidates"></param>
        /// <param name="indexedHops"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public List<Recipe> GetMatchingRecipeByHops(List<Recipe> candidates, IndexedHopDb? indexedHops, List<string>? names)
        {
            if(names != null && indexedHops != null)
            {
                var hopsCandidates = FuzzySearchInIndexedDbMultipleQueries(indexedHops.Hops, names);
                candidates = KeepSimilarIndices(candidates, hopsCandidates);
            }
            return candidates;
        }

        /// <summary>
        /// Small wrapper around <see cref="FuzzySearchInIndexedDbMultipleQueries"/> for the specified type (used for abstraction and testing)
        /// </summary>
        /// <param name="candidates"></param>
        /// <param name="indexedYeasts"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public List<Recipe> GetMatchingRecipeByYeasts(List<Recipe> candidates, IndexedYeastDb? indexedYeasts, List<string>? names)
        {
            if(names != null && indexedYeasts != null)
            {
                var yeastsCandidates = FuzzySearchInIndexedDbMultipleQueries(indexedYeasts.Yeasts, names);
                candidates = KeepSimilarIndices(candidates, yeastsCandidates);
            }
            return candidates;
        }

        
        /// <summary>
        /// Small wrapper around <see cref="FuzzySearchInIndexedDbMultipleQueries"/> for the specified type (used for abstraction and testing)
        /// </summary>
        /// <param name="candidates"></param>
        /// <param name="indexedTags"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public List<Recipe> GetMatchingRecipeByTags(List<Recipe> candidates, IndexedTagDb? indexedTags, List<string>? names)        
        {
            if(names != null && indexedTags != null)
            {
                var tagsCandidates = FuzzySearchInIndexedDbMultipleQueries(indexedTags.Tags, names);
                candidates = KeepSimilarIndices(candidates, tagsCandidates);
            }
            return candidates;
        }


        /// <summary>
        /// Small wrapper around <see cref="FuzzySearchInIndexedDbMultipleQueries"/> for the specified type (used for abstraction and testing)
        /// </summary>
        /// <param name="candidates"></param>
        /// <param name="indexedFoodPairings"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public List<Recipe> GetMatchingRecipeByFoodPairings(List<Recipe> candidates, IndexedFoodPairingDb? indexedFoodPairings, List<string>? names)        
        {
            if(names != null && indexedFoodPairings != null)
            {
                var foodPairingsCandidates = FuzzySearchInIndexedDbMultipleQueries(indexedFoodPairings.FoodPairing, names);
                candidates = KeepSimilarIndices(candidates, foodPairingsCandidates);
            }
            return candidates;
        }

        /// <summary>
        /// Filters out discrete properties from a single recipe
        /// </summary>
        /// <param name="recipe">Subject recipe on which we'll apply the query filter</param>
        /// <param name="subjectPropList">Recipe targeted properties listed as string</param>
        /// <param name="queries">Queries object which is passed along to carry the filter data</param>
        /// <returns>Selected recipe (matching the search criterion) or null if subject is filtered out by the search query</returns>
        public Recipe? FilterOutPropDiscrete(Recipe recipe, List<string>? subjectPropList, List<string>? queries)
        {
            // No filter applied, return the recipe as there is not restrictions applied on it.
            if(queries == null)
            {
                return recipe;
            }

            // Reject recipes that don't have any twists.
            // If query is specified with some twists, it means we need to rule out 
            // All recipes that don't have any.
            if(subjectPropList == null || subjectPropList.Count == 0 )
            {
                return null;
            }

            Recipe? output = null;
            var fuzzResult = FuzzySearch.SearchSingleSubject(queries, recipe, subjectPropList);
            if(fuzzResult.Ratio >= 35)
            {
                output = fuzzResult.Prop;
            }

            return output;
        }

        /// <summary>
        /// Filters out Twist values on a single recipe (Logical OR).
        /// If any Twist of the targeted recipe match the requested criteria, the recipe will be accepted as a valid one for this filter.
        /// </summary>
        /// <param name="recipe">recipe recipe on which we'll apply the query filter</param>
        /// <param name="queries">Queries object which is passed along to carry the filter data</param>
        /// <returns>Selected recipe (matching the search criterion) or null if recipe is filtered out by the search query</returns>
        public Recipe? FilterOutTwistsDiscrete(Recipe recipe, Queries queries)
        {
            List<string>? propList = null;
            if(recipe.MethodTimings.Twists != null)
            {
                propList = recipe.MethodTimings.Twists.Select((twist) => twist.Name).ToList();
            }
            return FilterOutPropDiscrete(recipe, propList, queries.TwistList);
        }

        /// <summary>
        /// Filters out MashTemps value on a single recipe (Logical OR).
        /// If any MashTemp of the targeted recipe match the requested criteria, the recipe will be accepted as a valid one for this filter.
        /// </summary>
        /// <param name="recipe">Subject recipe on which we'll apply the query filter</param>
        /// <param name="queries">Queries object which is passed along to carry the filter data</param>
        /// <returns>Selected recipe (matching the search criterion) or null if subject is filtered out by the search query</returns>
        public Recipe? FilterOutExtraMashDiscrete(Recipe recipe, Queries queries)
        {
            List<string>? propList = null;
            if(recipe.Ingredients.ExtraMash != null)
            {
                propList = recipe.Ingredients.ExtraMash.Select((extra) => extra.Name).ToList();
            }
            return FilterOutPropDiscrete(recipe, propList, queries.ExtraMashList);
        }

        /// <summary>
        /// Filters out MashTemps value on a single recipe (Logical OR).
        /// If any MashTemp of the targeted recipe match the requested criteria, the recipe will be accepted as a valid one for this filter.
        /// </summary>
        /// <param name="recipe">Subject recipe on which we'll apply the query filter</param>
        /// <param name="queries">Queries object which is passed along to carry the filter data</param>
        /// <returns>Selected recipe (matching the search criterion) or null if subject is filtered out by the search query</returns>
        public Recipe? FilterOutExtraBoilDiscrete(Recipe recipe, Queries queries)
        {
            List<string>? propList = null;
            if(recipe.Ingredients.ExtraBoil != null)
            {
                propList = recipe.Ingredients.ExtraBoil.Select((extra) => extra.Name).ToList();
            }
            return FilterOutPropDiscrete(recipe, propList, queries.ExtraBoilList);
        }




#endregion FuzzySearchFilters        
        
        /// <summary>
        /// Removes duplicated recipes in input list
        /// </summary>
        /// <param name="recipes"></param>
        /// <returns></returns>
        public List<Recipe> RemoveDoubles(List<Recipe> recipes)
        {
            var output = new List<Recipe>();
            foreach(var recipe in recipes) 
            {
                if(!output.Any(r => r.Number == recipe.Number))
                {
                    output.Add(recipe);
                }
            }
            return output;
        }

        /// <summary>
        /// Only keep objects that are both in candidates list and filterOutputList
        /// (performs a logical AND on both lists)
        /// </summary>
        /// <param name="candidates">Recipe candidates list </param>
        /// <param name="filterOutputList">Previous filtering stage output result list</param>
        /// <returns>New list containing only elements that are common to both input lists</returns>
        public List<Recipe> KeepSimilar(List<Recipe> candidates, List<FuzzySearchResult<Recipe>> filterOutputList)
        {
            var outputList = new List<Recipe>();
            foreach(var filterOutput in filterOutputList)
            {
                if(candidates.Any(elem => elem.Number == filterOutput.Prop!.Number))
                {
                    outputList.Add(filterOutput.Prop!);
                }
            }
            outputList = RemoveDoubles(outputList);
            return outputList;
        }

       /// <summary>
        /// Variant of the <see cref="KeepSimilar"/> method above, but works with indices provided by reverse property mappings 
        /// (performs a logical AND on both lists)
        /// </summary>
        /// <param name="candidates">Recipe candidates list </param>
        /// <param name="filterOutputList">Previous filtering stage output result list</param>
        /// <returns>New list containing only elements that are common to both input lists</returns>
        public List<Recipe> KeepSimilarIndices(List<Recipe> candidates, List<FuzzySearchResult<ReversedPropMapping>> filterOutputList)
        {
            var outputList = new List<Recipe>();
            foreach(var filterOutput in filterOutputList)
            {
                foreach(var index in filterOutput.Prop!.FoundInBeers)
                {
                    var candidate = candidates.FirstOrDefault(c => c.Number == index);
                    if(candidate != null)
                    {
                        outputList.Add(candidate);
                    }
                }
            }

            outputList = RemoveDoubles(outputList);
            return outputList;
        }

        /// <summary>
        /// Searches for a single recipe using the given queries and database handler.
        /// </summary>
        /// <param name="queries">Input queries used for searching</param>
        /// <param name="dbHandler">Database handler used in current context</param>
        /// <returns></returns>
        public async Task<List<Recipe>> SearchRecipeAsync(Queries queries, IDatabaseHandler dbHandler)
        {
            var allRecipes = await dbHandler.GetAllRecipesAsync();
            if(allRecipes == null)
            {
                return new List<Recipe>();
            }
            
            // Perform filtering on all recipes and skip as early as possible
            List<Recipe> candidates = new List<Recipe>();
            foreach(var recipe in allRecipes)
            {
                Recipe? candidate = recipe;
                candidate = FilterOutAbvDiscrete(candidate, queries);
                if(candidate == null) continue;

                candidate = FilterOutEbcDiscrete(candidate, queries);
                if(candidate == null) continue;

                candidate = FilterOutIbuDiscrete(candidate, queries);
                if(candidate == null) continue;

                candidate = FilterOutTargetOgDiscrete(candidate, queries);
                if(candidate == null) continue;

                candidate = FilterOutTargetFgDiscrete(candidate, queries);
                if(candidate == null) continue;

                candidate = FilterOutPhDiscrete(candidate, queries);
                if(candidate == null) continue;

                candidate = FilterOutAttenuationLevelDiscrete(candidate, queries);
                if(candidate == null) continue;

                candidate = FilterOutMashTempsDiscrete(candidate, queries);
                if(candidate == null) continue;
                
                candidate = FilterOutTwistsDiscrete(candidate, queries);
                if(candidate == null) continue;

                candidate = FilterOutExtraBoilDiscrete(candidate, queries);
                if(candidate == null) continue;

                candidate = FilterOutExtraMashDiscrete(candidate, queries);
                if(candidate == null) continue;
                
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
            candidates = GetMatchingRecipeByNames(candidates, queries.NameList);
            if(candidates.Count == 0) return candidates;

            candidates = GetMatchingRecipeByExtraMashIngredients(candidates, queries.ExtraMashList);
            if(candidates.Count == 0) return candidates;

            candidates = GetMatchingRecipeByExtraBoilIngredients(candidates, queries.ExtraBoilList);
            if(candidates.Count == 0) return candidates;

            // Perform the same kind of analysis with Reversed properties (and indices)
            candidates = GetMatchingRecipeByMalts(candidates, indexedMalts, queries.MaltList);
            if(candidates.Count == 0) return candidates;

            candidates = GetMatchingRecipeByStyles(candidates, indexedStyles, queries.StyleList);
            if(candidates.Count == 0) return candidates;

            candidates = GetMatchingRecipeByHops(candidates, indexedHops, queries.HopList);
            if(candidates.Count == 0) return candidates;

            candidates = GetMatchingRecipeByYeasts(candidates, indexedYeasts, queries.YeastList);
            if(candidates.Count == 0) return candidates;

            candidates = GetMatchingRecipeByTags(candidates, indexedTags, queries.TagList);
            if(candidates.Count == 0) return candidates;

            candidates = GetMatchingRecipeByFoodPairings(candidates, indexedFoodPairings, queries.FoodPairingList);

            return candidates;
        }
    
        /// <summary>
        /// Uses fuzzy search to list available hops using input query
        /// </summary>
        /// <param name="names">List of tokens for hop selection</param>
        /// <param name="dbHandler"></param>
        /// <param name="minimumMatchScore"></param>
        /// <returns></returns>
        public async Task<List<HopProperty>> SearchHopsWithQuery(List<string> names, IDatabaseHandler dbHandler, uint minimumMatchScore = 50)
        {
            var refList = await dbHandler.GetReferenceHopsAsync();
            var hopsCandidates =  FuzzySearchInReferenceProperties(refList!.Hops, names, item => {
                var list = new List<string>{item.Name};
                if(item.Aliases != null)
                {
                    list.Concat(item.Aliases);
                }
                return list;
            });

            // Removing doubles : https://stackoverflow.com/a/4158364
            var filteredList = hopsCandidates.GroupBy(item => item.Prop!.Name).Select(group => group.First());
            hopsCandidates = filteredList.ToList();

            var outputList = new List<HopProperty>();
            foreach(var elem in hopsCandidates)
            {
                if(elem.Ratio >= minimumMatchScore)
                {
                    outputList.Add(elem.Prop!);
                }
            }

            return outputList;
        }

        /// <summary>
        /// Uses fuzzy search to list available malts using input query
        /// </summary>
        /// <param name="names">List of tokens for hop selection</param>
        /// <param name="dbHandler"></param>
        /// <param name="minimumMatchScore"></param>
        /// <returns></returns>
        public async Task<List<MaltProperty>> SearchMaltsWithQuery(List<string> names, IDatabaseHandler dbHandler, uint minimumMatchScore = 50)
        {
            var refList = await dbHandler.GetReferenceMaltsAsync();
            var candidates =  FuzzySearchInReferenceProperties(refList!.Malts, names, item => {
                var list = new List<string>{item.Name};
                if(item.Aliases != null)
                {
                    list.Concat(item.Aliases);
                }
                return list;
            });

            // Removing doubles : https://stackoverflow.com/a/4158364
            var filteredList = candidates.GroupBy(item => item.Prop!.Name).Select(group => group.First());
            candidates = filteredList.ToList();

            var outputList = new List<MaltProperty>();
            foreach(var elem in candidates)
            {
                if(elem.Ratio >= minimumMatchScore)
                {
                    outputList.Add(elem.Prop!);
                }
            }

            return outputList;
        }

        /// <summary>
        /// Uses fuzzy search to list available yeasts using input query
        /// </summary>
        /// <param name="names">List of tokens for hop selection</param>
        /// <param name="dbHandler"></param>
        /// <param name="minimumMatchScore"></param>
        /// <returns></returns>
        public async Task<List<YeastProperty>> SearchYeastsWithQuery(List<string> names, IDatabaseHandler dbHandler, uint minimumMatchScore = 50)
        {
            var refList = await dbHandler.GetReferenceYeastsAsync();
            var candidates =  FuzzySearchInReferenceProperties(refList!.Yeasts, names, item => {
                var list = new List<string>{item.Name};
                if(item.Aliases != null)
                {
                    list.Concat(item.Aliases);
                }
                return list;
            });

            // Removing doubles : https://stackoverflow.com/a/4158364
            var filteredList = candidates.GroupBy(item => item.Prop!.Name).Select(group => group.First());
            candidates = filteredList.ToList();

            var outputList = new List<YeastProperty>();
            foreach(var elem in candidates)
            {
                if(elem.Ratio >= minimumMatchScore)
                {
                    outputList.Add(elem.Prop!);
                }
            }

            return outputList;
        }
        
        /// <summary>
        /// Uses fuzzy search to list available yeasts using input query
        /// </summary>
        /// <param name="names">List of tokens for hop selection</param>
        /// <param name="dbHandler"></param>
        /// <param name="minimumMatchScore"></param>
        /// <returns></returns>
        public async Task<List<StyleProperty>> SearchStylesWithQuery(List<string> names, IDatabaseHandler dbHandler, uint minimumMatchScore = 50)
        {
            var refList = await dbHandler.GetReferenceStylesAsync();
            var candidates =  FuzzySearchInReferenceProperties(refList!.Styles, names, item => {
                var list = new List<string>{item.Name, item.Category};
                return list;
            });

            // Removing doubles : https://stackoverflow.com/a/4158364
            var filteredList = candidates.GroupBy(item => item.Prop!.Name).Select(group => group.First());
            candidates = filteredList.ToList();

            var outputList = new List<StyleProperty>();
            foreach(var elem in candidates)
            {
                if(elem.Ratio >= minimumMatchScore)
                {
                    outputList.Add(elem.Prop!);
                }
            }

            return outputList;
        }
        
    }
}
