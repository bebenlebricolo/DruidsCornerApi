namespace DruidsCornerAPI.Models.DiyDog.RecipeDb
{
    /// <summary>
    /// Encodes basic information about BrewDog's beer recipe.
    /// </summary>
    /// <value></value>
    public record Basics 
    {
        /// <summary>
        /// Final beer volume (target volume)
        /// </summary>
        /// <returns></returns>
        public Volume Volume { get; set; } = new Volume();

        /// <summary>
        /// Boil starting volume
        /// </summary>
        /// <returns></returns>
        public Volume BoilVolume { get; set; } = new Volume();

        /// <summary>
        /// Beer's average alcoholic value (percent)
        /// </summary>
        /// <value></value>
        public float Abv { get; set; } = 0.0f;

        /// <summary>
        /// Target Original Gravity
        /// </summary>
        /// <value></value>
        public float TargetOg { get; set; } = 0.0f;

        /// <summary>
        /// Target Final gravity
        /// </summary>
        /// <value></value>
        public float TargetFg { get; set; } = 0.0f;

        /// <summary>
        /// Beer's color (EBC scale)
        /// </summary>
        /// <value></value>
        public float Ebc { get; set; } = 0.0f;

        /// <summary>
        /// Beer's bitterness rating
        /// </summary>
        /// <value></value>
        public float Ibu { get; set; } = 0.0f;

        /// <summary>
        /// Beer's Standard Color rating (up to 40, but can get even bigger depending on how extreme the beer can be. )
        /// -> Unit derived from the exact measure of how much light (specific wavelength) comes through a sample of liquid.
        /// </summary>
        /// <value></value>
        public float Srm { get; set; } = 0.0f;

        /// <summary>
        /// Beer's target PH
        /// </summary>
        /// <value></value>
        public float Ph { get; set; } = 0.0f;

        /// <summary>
        /// Expected attenuation level (calculated using Original and Final gravities)
        /// </summary>
        /// <value></value>
        public float AttenuationLevel { get; set; } = 0.0f;
    }
}
