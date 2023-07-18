namespace DruidsCornerAPI.Models.DiyDog.RecipeDb
{
    /// <summary>
    /// Extra Mash ingredient.
    /// Might be anything, from sugar to milk, lactose, dextrose, etc.
    /// </summary>
    public record ExtraMash 
    {
        /// <summary>
        /// Ingredient name
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Amount of this ingredient in Kilograms 
        /// </summary>
        public float Kgs { get; set; } = 0.0f;

        /// <summary>
        /// Amount of this ingredient in Lbs
        /// </summary>
        public float Lbs { get; set; } = 0.0f;

    }
}
