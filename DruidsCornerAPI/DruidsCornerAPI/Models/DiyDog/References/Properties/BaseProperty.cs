namespace DruidsCornerAPI.Models.DiyDog.References
{

    /// <summary>
    /// Encodes availanle known good properties
    /// From DiyDogExtractor known_good_<prop>.json dbs
    /// </summary>
    public enum ReferencePropKind
    {
        /// <summary>Yeast property</summary>
        Yeast, 
        /// <summary>Tag property</summary>
        Tag, 
        /// <summary>Style property</summary>
        Style,
        /// <summary>Hop property</summary>
        Hop,
        /// <summary>Malt property</summary>
        Malt,
        /// <summary>Unsupported type</summary>
        Unknown
    }

    /// <summary>
    /// Encodes a base property (either yeast, hop, malts or style)
    /// This is the base for all ReferenceProperties (aka "known good" properties in the DiyDogExtractor databases)
    /// </summary>
    public record BaseProperty
    {
        /// <summary>
        /// Property name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Url for this base property
        /// </summary>
        public string? Url { get; set; } = null;
    }

}