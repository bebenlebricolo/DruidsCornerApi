namespace DruidsCornerAPI.Models.DiyDog
{
    public record Temperature
    {
        public float Celsius { get; set; } = 0.0f;

        public float Fahrenheit { get; set; } = 0.0f;
    }

    public record MashTemp : Temperature
    {
        public float Time { get; set; } = 0.0f;
    }

    public record Fermentation : Temperature
    {
        public float? Time { get; set; } = null;

        public List<string> Tips { get; set; } = new List<string>();
    }
}
