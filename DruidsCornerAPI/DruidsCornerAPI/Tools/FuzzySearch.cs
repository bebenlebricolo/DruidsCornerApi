using FuzzySharp;
namespace DruidsCornerAPI.Tools
{

    /// <summary>
    /// Template delegate accessor definition to be used in conjunction with FuzzySearch static class
    /// <see cref="FuzzySearch"/>
    /// </summary>
    /// <param name="elem">Any object</param>
    /// <returns>string</returns>
    public delegate List<string>? GetProp<T>(T elem); 

    /// <summary>
    /// Encapsulate the a FuzzySearch result for a given object type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FuzzySearchResult<T> where T : class
    {
        /// <summary>
        /// Fuzzy Search "Ratio" (aka "Score")
        /// </summary>
        public int Ratio { get; set; } = 0;

        /// <summary>
        /// Matching property
        /// </summary>
        public T? Prop { get; set; } = null;

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="ratio"></param>
        /// <param name="prop"></param>
        public FuzzySearchResult(int ratio, T prop)
        {
            Ratio = ratio;
            Prop = prop;
        }

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if(obj is not FuzzySearchResult<T> || obj is null)
            {
                return false;
            }

            var other = obj as FuzzySearchResult<T>;
            return other!.Ratio == Ratio 
                && other.Prop == Prop ;
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            return Ratio.GetHashCode() * 32;
        }
    }


    /// <summary>
    /// The Fuzz algorithms Enum mapping
    /// </summary>
    public enum FuzzMode
    {
        /// <summary>Quick ratio mode from The Fuzz</summary>
        Ratio,
        
        /// <summary>Token set ratio mode from The Fuzz</summary>
        TokenSetRatio,
        
        /// <summary>Token sort ratio mode from The Fuzz</summary>
        TokenSortRatio,

        /// <summary>Token Abbreviation ratio mode from The Fuzz</summary>
        TokenAbbreviationRatio,
        
        /// <summary>Token Difference ratio mode from The Fuzz</summary>
        TokenDifferenceRatio,
        
        /// <summary>Token Initialism ratio mode from The Fuzz</summary>
        TokenInitialismRatio,

        /// <summary>Weighted ratio mode from The Fuzz</summary>
        WeightedRatio,
        
        /// <summary>Partial ratio mode from The Fuzz  </summary>
        PartialRatio,
        
        /// <summary>Partial Token Sort ratio mode from The Fuzz</summary>
        PartialTokenSortRatio,

        /// <summary>Partial Token Set ratio mode from The Fuzz</summary>
        PartialTokenSetRatio,

        /// <summary>Partial Token Initialism ratio mode from The Fuzz</summary>
        PartialTokenInitialismRatio,
        
        /// <summary>Partial Token Abbreviation ratio mode from The Fuzz </summary>
        PartialTokenAbbreviationRatio,
        
        /// <summary>Partial Token Difference ratio mode from The Fuzz </summary>
        PartialTokenDifferenceRatio,
    }


    /// <summary>
    /// FuzzySearch toolset 
    /// </summary>
    public static class FuzzySearch
    {
        /// <summary>
        /// Maps The Fuzz functions with the enum parameter
        /// </summary>
        /// <param name="mode">Enum value that selects the underlying FuzzySearch method used</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Func<string, string, int> GetMappedFuzzFunction(FuzzMode mode)
        {
            return mode switch
            {
                FuzzMode.Ratio => Fuzz.Ratio,
                FuzzMode.TokenSetRatio => Fuzz.TokenSetRatio,
                FuzzMode.TokenSortRatio => Fuzz.TokenSortRatio,
                FuzzMode.TokenAbbreviationRatio => Fuzz.TokenAbbreviationRatio,
                FuzzMode.TokenInitialismRatio => Fuzz.TokenInitialismRatio,
                FuzzMode.TokenDifferenceRatio => Fuzz.TokenDifferenceRatio,
                FuzzMode.WeightedRatio => Fuzz.WeightedRatio,
                FuzzMode.PartialRatio => Fuzz.PartialRatio,
                FuzzMode.PartialTokenSetRatio => Fuzz.PartialTokenSetRatio,
                FuzzMode.PartialTokenSortRatio => Fuzz.PartialTokenSortRatio,
                FuzzMode.PartialTokenAbbreviationRatio => Fuzz.PartialTokenAbbreviationRatio,
                FuzzMode.PartialTokenDifferenceRatio => Fuzz.PartialTokenDifferenceRatio,
                FuzzMode.PartialTokenInitialismRatio => Fuzz.PartialTokenInitialismRatio,
                _ => throw new ArgumentException("Could not infer type based on input"),
            };
        }
        

        /// <summary>
        /// Handles fuzzy search on a single subject, and uses the potential list of properties with GeometricMean in order to discriminate elements
        /// </summary>
        /// <param name="property">Queried property</param>
        /// <param name="subject">Input subject that'll be tested against</param>
        /// <param name="propAccessor">Delegate method that accesses the inner field of the Subject object</param>
        /// <param name="fuzzMode">Optional fuzzy search mode selector</param>
        /// <returns>FuzzySearchResult for this element</returns>
        public static FuzzySearchResult<T> SearchSingleSubject<T>(string property, T subject, GetProp<T> propAccessor, FuzzMode fuzzMode = FuzzMode.PartialRatio) where T : class 
        {
            // Stores multiple ratios
            List<string>? objectProps = propAccessor.Invoke(subject);
            if(objectProps == null)
            {
                return new FuzzySearchResult<T>(0, subject);
            }

            // Iterate over all props and compute ratios accordingly
            var partialRatios = new List<double>();
            bool foundExactFlag = false;
            foreach(var prop in objectProps)
            {
                var method = GetMappedFuzzFunction(fuzzMode);
                int ratio =  method.Invoke(property, prop);
                partialRatios.Add(Convert.ToDouble(ratio));

                // Found an exact match, exiting early
                if(ratio == 100)
                {
                    foundExactFlag = true;
                    break;
                }
            }

            // Stores the result of the accumulated ratio (works for single target property and 
            // multiple properties, such as aliases, etc.)
            int globalRatio;

            // Bump globalRatio to max because we are certain to having found an exact match in the list
            if(foundExactFlag)
            {
                globalRatio = 100;
            }
            else
            {
                globalRatio = Convert.ToInt32(Statistics.GeometricMean(partialRatios));
            }

            return new FuzzySearchResult<T>(globalRatio, subject);
        }

        /// <summary>
        /// Searches for a match with words individual matching using Fuzzy search algorithms
        /// </summary>
        /// <param name="property">Input property string we'll try to match against a known reference list.</param>
        /// <param name="list">List of input objects against which we'll compare input property.</param>
        /// <param name="propAccessor">A delegate function that allows this method to access to targeted property (such as T.Name). <see cref="GetProp{T}"/></param>
        /// <param name="fuzzMode">Optional fuzzy search mode selector</param>
        /// <returns>Tuple of MaxRatio (0-100) and the reference object that has the highest probability</returns>
        /// <throws>NullReferenceException if input list is empty</throws>
        public static FuzzySearchResult<T>? SearchInList<T>(string property, List<T> list, GetProp<T> propAccessor, FuzzMode fuzzMode = FuzzMode.PartialRatio) where T : class
        {
            int maxRatio = 0;
            if(list.Count == 0)
            {
                return null;
            }

            // Will throw a null reference exception if list does not contain anything
            T? mostProbableElem = list.FirstOrDefault();
            foreach (var element in list) 
            {
                var result = SearchSingleSubject<T>(property, element, propAccessor, fuzzMode);
                if(result.Ratio >= 95)
                {
                    return result;
                }

                // Only get the max element out of here
                if(result.Ratio > maxRatio)
                {
                    maxRatio = result.Ratio;
                    mostProbableElem = element;
                }
            }

            return new FuzzySearchResult<T>(maxRatio, mostProbableElem!);
        }
    
        /// <summary>
        /// Variation of SearchExactRatio <see cref="SearchInList"/> that returns a full list of ratios, reversed order 
        /// with Highest probability element as first element and least probable one as last element
        /// </summary>
        /// <param name="property"></param>
        /// <param name="list"></param>
        /// <param name="propAccessor"></param>
        /// <param name="fuzzMode">Optional fuzzy search mode selector</param>
        /// <returns></returns>
        public static List<FuzzySearchResult<T>> SearchInListFullResults<T>(string property, List<T> list, GetProp<T> propAccessor, FuzzMode fuzzMode = FuzzMode.PartialRatio) where T : class
        {
            var outList = new List<FuzzySearchResult<T>>();
            foreach (var element in list) 
            {
                var result = SearchSingleSubject<T>(property, element, propAccessor, fuzzMode);
                outList.Add(result);
            }

            outList.Sort((elem1, elem2) => elem2.Ratio.CompareTo(elem1.Ratio));
            return outList;
        }
    
    
    }
}
