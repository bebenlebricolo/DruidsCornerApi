namespace DruidsCornerAPI.Models.DiyDog.References
{
    /// <summary>
    /// Very simple Known Good Styles list
    /// </summary>
    public record ReferenceStyles
    {
        /// <summary>
        /// List of Style properties
        /// </summary>
        public List<StyleProperty> Styles = new List<StyleProperty>();  
    }
}