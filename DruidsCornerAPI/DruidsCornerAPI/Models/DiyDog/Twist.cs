namespace DruidsCornerAPI.Models.DiyDog
{
    public record Twist
    {
        public string Name { get; set; } = "";

        public float? Amount { get; set; } = null;

        public string? When { get; set; } = null;
    }
}
