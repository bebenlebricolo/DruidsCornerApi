namespace DruidsCornerAPI.Models.DiyDog.References
{
    /// <summary>
    /// Very simple Known Good Yeasts list
    /// </summary>
    public record ReferenceYeasts
    {
        /// <summary>
        /// List of yeast properties
        /// </summary>
        public List<YeastProperty> Yeasts = new List<YeastProperty>();  
    }
}