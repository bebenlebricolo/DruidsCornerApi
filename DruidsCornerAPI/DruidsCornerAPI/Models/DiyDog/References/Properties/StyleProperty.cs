namespace DruidsCornerAPI.Models.DiyDog.References
{
    /// <summary>
    /// Encodes a known good Style property
    /// This is the base for all ReferenceProperties (aka "known good" properties in the DiyDogExtractor databases)
    /// </summary>
    public class StyleProperty : BaseProperty
    {
        /// <summary>
        /// Style's known Category in the big Styles family
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            var other = obj as StyleProperty;
            bool identical = other != null;
            if(!identical) return false;
            
            identical &= base.Equals(other as BaseProperty);
            identical &= Category == other!.Category;            
            return identical;
        }

        /// <summary>
        /// Custom equality operator
        /// </summary>
        public static bool operator == (StyleProperty left, StyleProperty right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Custom inequality operator
        /// </summary>
        public static bool operator != (StyleProperty left, StyleProperty right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            var hash = base.GetHashCode() * 4 + Category.GetHashCode() * 3;
            return hash;
        }
    }
}