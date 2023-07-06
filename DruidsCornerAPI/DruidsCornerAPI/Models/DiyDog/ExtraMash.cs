namespace DruidsCornerAPI.Models.DiyDog
{
    /// <summary>
    /// Extra Mash ingredient.
    /// Might be anything, from sugar to milk, lactose, dextrose, etc.
    /// </summary>
    public record ExtraMash 
    {
        public string Name { get; set; } = "";

        public float Kgs { get; set; } = 0.0f;

        public float Lbs { get; set; } = 0.0f;

    }
}
