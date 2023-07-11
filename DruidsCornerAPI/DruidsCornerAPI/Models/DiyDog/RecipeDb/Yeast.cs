namespace DruidsCornerAPI.Models.DiyDog.RecipeDb
{
    /// <summary>
    /// Yeast used in the fermentation(s) step(s)
    /// </summary>
    public record Yeast
    {

        /// <summary>
        /// Yeast name
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Optional Manufacturer Link, if we happen to find it
        /// </summary>
        public string? ManufacturerLink { get; set; } = null;
    }
}
