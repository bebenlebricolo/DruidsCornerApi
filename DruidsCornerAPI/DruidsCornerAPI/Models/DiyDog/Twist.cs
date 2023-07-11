namespace DruidsCornerAPI.Models.DiyDog
{
    /// <summary>
    /// Additional so-called "twist" ingredients.
    /// This groups ingredients that are not really usual in beers, such as coffee, extra grains, etc.
    /// </summary>
    public record Twist
    {
        /// <summary>
        /// Twist's naming
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Twist's amount (in grams)
        /// </summary>
        public float? Amount { get; set; } = null;

        /// <summary>
        /// When the ingredient might be added (start, middle, end, fermentation, anytime...)
        /// </summary>
        public string? When { get; set; } = null;
    }
}
