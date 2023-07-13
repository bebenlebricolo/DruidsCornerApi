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
        /// <summary>Indexed for tags</summary>
        Tags,
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
    public interface IndexedDb
    {
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
