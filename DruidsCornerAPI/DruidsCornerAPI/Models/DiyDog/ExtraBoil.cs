namespace DruidsCornerAPI.Models.DiyDog
{
    /// <summary>
    /// Extra boil / fermentation ingredient which is not a Hop.
    /// Might by anything, from fruits, to coffee beans, etc.
    /// </summary>
    public record ExtraBoil 
    {
        /// <summary>
        /// Element name
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Ingredient amount (in grams) 
        /// </summary>
        public float Amount { get; set; } = 0.0f;

        /// <summary>
        /// When this element is added to the Boil/Fermentation
        /// </summary>
        public string When { get; set; } = "";

        /// <summary>
        /// Attribute of this ingredient. Usually encodes the kind of profile this ingredient probides, such as "Bittering, Flavour, Aroma, etc."
        /// </summary>
        public string Attribute { get; set; } = "";
    }
}
