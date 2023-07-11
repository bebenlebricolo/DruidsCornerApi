namespace DruidsCornerAPI.Models.DiyDog.IndexedDb
{

    /// <summary>
    /// Encodes the various available properties that can be found in the indexed databases
    /// </summary>
    public enum IndexedDbPropKind
    {
        /// <summary>Indexed for yeasts</summary>
        Yeasts,
        /// <summary>Indexed for styles</summary>
        Styles,
        /// <summary>Indexed for malts</summary>
        Malts,
        /// <summary>Indexed for hops</summary>
        Hops,
        /// <summary>Indexed for food pairings</summary>
        FoodPairing,
        /// <summary>Unsupported value</summary>
        Unknown
    }


    /// <summary>
    /// Encodes basic information about BrewDog's beer recipe.
    /// </summary>
    public record IndexedDb
    {
        /// <summary>
        /// Underlying type of indexed database (only available for properties listed in <see cref="IndexedDbPropKind"/> )
        /// </summary>
        public IndexedDbPropKind Kind {get; set;} = IndexedDbPropKind.Unknown;

        /// <summary>
        /// List of properties in a reversed DB construct
        /// </summary>
        public List<ReversedPropMapping> properties = new List<ReversedPropMapping>();  

        /// <summary>
        /// Checks if an IndexDb can be constructed for the given object
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool CanRead(string key) 
        {
            var output = IndexedDbPropKind.Unknown;
            return Enum.TryParse<IndexedDbPropKind>(key, ignoreCase:true, out output);
        }
    }



}
