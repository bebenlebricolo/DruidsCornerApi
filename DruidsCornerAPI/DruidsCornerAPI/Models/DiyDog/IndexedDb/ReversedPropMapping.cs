namespace DruidsCornerAPI.Models.DiyDog.IndexedDb
{

    /// <summary>
    /// Encodes a simple reversed property mapping as found in <prop>_rv_db.json provided by 
    /// DiyDogExtract databases
    /// </summary>
    public record ReversedPropMapping
    {
        /// <summary>
        /// Property name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// List of recipes ordered by their number / id in which this property was found.
        /// </summary>
        public List<uint> FoundInBeers { get; set; } = new List<uint>();
    }

}