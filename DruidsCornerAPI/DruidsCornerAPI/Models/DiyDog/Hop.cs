namespace DruidsCornerAPI.Models.DiyDog
{
    public record Hop 
    {
        public string Name { get; set; } = "";

        public float Amount { get; set; } = 0.0f;

        public string When { get; set; } = "";

        public string Attribute { get; set; } = "";
    }
}
