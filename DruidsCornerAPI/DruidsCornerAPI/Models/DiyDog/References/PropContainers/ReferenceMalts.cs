namespace DruidsCornerAPI.Models.DiyDog.References
{
    /// <summary>
    /// Very simple Known Good Malts list
    /// </summary>
    public record ReferenceMalts
    {
        /// <summary>
        /// List of Malt properties
        /// </summary>
        public List<MaltProperty> Malts = new List<MaltProperty>();  
    }
}