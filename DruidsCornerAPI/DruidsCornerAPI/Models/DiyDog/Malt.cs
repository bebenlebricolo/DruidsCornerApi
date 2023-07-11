namespace DruidsCornerAPI.Models.DiyDog
{
    /// <summary>
    /// Malt ingredient
    /// </summary>
    public record Malt 
    {
        /// <summary>
        /// Malt's name
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Amount of Malt in kilograms
        /// </summary>
        public float Kgs { get; set; } = 0.0f;

        /// <summary>
        /// Amount of Malt in pounds
        /// </summary>
        public float Lbs { get; set; } = 0.0f;

    }
}
