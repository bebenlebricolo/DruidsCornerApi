namespace DruidsCornerAPI.Models.DiyDog
{
    /// <summary>
    /// Temperature instruction for Mash
    /// </summary>
    public record Temperature
    {
        /// <summary>
        /// Temperature expressed in Celsius degrees
        /// </summary>
        public float Celsius { get; set; } = 0.0f;

        /// <summary>
        /// Temperature expressed using Fahrenheit degrees
        /// </summary>
        public float Fahrenheit { get; set; } = 0.0f;
    }

    /// <summary>
    /// Mash temperature
    /// </summary>
    public record MashTemp : Temperature
    {
        /// <summary>
        /// Time of the mash
        /// </summary>
        public float Time { get; set; } = 0.0f;
    }

    /// <summary>
    /// Fermentation temperatures instructions
    /// </summary>
    public record Fermentation : Temperature
    {
        /// <summary>
        /// Optional time (used for extended fermentation steps, such as cask-aging)
        /// </summary>
        public float? Time { get; set; } = null;

        /// <summary>
        /// Optional additional tips indicated by the brewer's team
        /// </summary>
        public List<string> Tips { get; set; } = new List<string>();
    }
}
