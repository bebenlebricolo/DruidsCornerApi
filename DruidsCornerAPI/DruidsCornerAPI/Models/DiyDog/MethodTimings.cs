namespace DruidsCornerAPI.Models.DiyDog
{
    /// <summary>
    /// Method Timings characteristics
    /// </summary>
    public record MethodTimings
    {
        /// <summary>
        /// Mashing temperatures
        /// </summary>
        public List<MashTemp> MashTemps { get; set; } = new List<MashTemp>();

        /// <summary>
        /// Mashing tips
        /// </summary>
        public List<string> MashTips { get; set; } = new List<string>();

        /// <summary>
        /// Fermentation instructions
        /// </summary>
        public Fermentation Fermentation { get; set; } = new Fermentation();

        /// <summary>
        /// Recipe twists (custom ingredients)
        /// </summary>
        public List<Twist> Twists { get; set; } = new List<Twist>();
    }
}
