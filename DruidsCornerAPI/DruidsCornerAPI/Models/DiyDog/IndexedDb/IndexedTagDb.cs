namespace DruidsCornerAPI.Models.DiyDog.IndexedDb
{
    /// <summary>
    /// Encodes basic information about BrewDog's beer recipe.
    /// </summary>
    public record IndexedTagDb : IndexedDb
    {
        /// <summary>
        /// List of properties in a reversed DB construct
        /// </summary>
        public List<ReversedPropMapping> Tags {get; set;} = new List<ReversedPropMapping>();  
    }
}
