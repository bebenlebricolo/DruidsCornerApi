namespace DruidsCornerAPI.Models.DiyDog.References
{

    /// <summary>
    /// Encodes availanle known good properties
    /// From DiyDogExtractor known_good_{prop}.json dbs
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
    public class BaseProperty
    {
        /// <summary>
        /// Property name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Url for this base property
        /// </summary>
        public string? Url { get; set; } = null;

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if(obj is not BaseProperty || obj is null)
            {
                return false;
            }

            var other = obj as BaseProperty;
            if(other == null) 
            {
                return false;
            }
            bool identical = true;

            identical &= Name == other.Name;
            identical &= Url == other.Url;
            return identical;
        }

        /// <summary>
        /// Custom equality operator
        /// </summary>
        public static bool operator == (BaseProperty left, BaseProperty right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Custom inequality operator
        /// </summary>
        public static bool operator != (BaseProperty left, BaseProperty right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            var hash = Name.GetHashCode();
            if(Url != null){
                hash *= Url.GetHashCode() * 11;
            }
            return hash;
        }
    }

}