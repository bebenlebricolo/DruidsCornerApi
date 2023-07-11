namespace DruidsCornerAPI.Models.DiyDog.References
{
    /// <summary>
    /// Very simple Known Good Hops list
    /// </summary>
    public record ReferenceHops
    {
        /// <summary>
        /// List of Hop properties
        /// </summary>
        public List<HopProperty> Hops = new List<HopProperty>();  
    }
}