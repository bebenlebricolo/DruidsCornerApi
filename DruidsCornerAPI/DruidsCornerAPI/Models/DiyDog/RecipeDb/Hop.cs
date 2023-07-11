namespace DruidsCornerAPI.Models.DiyDog.RecipeDb
{
    /// <summary>
    /// Hop ingredient 
    /// </summary>
    public record Hop 
    {
        /// <summary>
        /// Hop name 
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Amount of Hop in grams
        /// </summary>
        public float Amount { get; set; } = 0.0f;

        /// <summary>
        /// When the hop is added to the boil (usually encoded in Minutes, but sometimes it's marked as "Start", "Middle", "End", "First fermentation", etc.)
        /// </summary>
        public string When { get; set; } = "";

        /// <summary>
        /// Hop attribute. Usually goes to "Bittering, Aroma, Flavour"
        /// </summary>
        public string Attribute { get; set; } = "";
    }
}
