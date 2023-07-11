namespace DruidsCornerAPI.Models.DiyDog.RecipeDb
{
    /// <summary>
    /// Regular Volume structure
    /// </summary>
    public record Volume
    {
        /// <summary>
        /// Volume specified in Litres
        /// </summary>
        public float Litres { get; set; } = 0.0f;
        
        /// <summary>
        /// Volume specified in Galons
        /// </summary>
        public float Galons { get; set; } = 0.0f;
    }
}
