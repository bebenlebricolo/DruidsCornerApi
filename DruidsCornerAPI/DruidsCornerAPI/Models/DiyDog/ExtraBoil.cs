namespace DruidsCornerAPI.Models.DiyDog
{
    /// <summary>
    /// Extra boil / fermentation ingredient which is not a Hop.
    /// Might by anything, from fruits, to coffee beans, etc.
    /// </summary>
    public record ExtraBoil 
    {
        public string Name { get; set; } = "";

        public float Amount { get; set; } = 0.0f;

        public string When { get; set; } = "";

        public string Attribute { get; set; } = "";
    }
}
