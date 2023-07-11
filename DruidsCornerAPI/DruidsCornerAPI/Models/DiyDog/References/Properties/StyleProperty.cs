namespace DruidsCornerAPI.Models.DiyDog.References
{
    /// <summary>
    /// Encodes a known good Style property
    /// This is the base for all ReferenceProperties (aka "known good" properties in the DiyDogExtractor databases)
    /// </summary>
    public record StyleProperty : BaseProperty
    {
        /// <summary>
        /// Style's known Category in the big Styles family
        /// </summary>
        public string Category { get; set; } = string.Empty;
    }
}