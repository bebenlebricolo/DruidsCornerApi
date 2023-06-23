namespace DruidsCornerAPI.Models.DiyDog
{
    public record MethodTimings
    {
        public List<MashTemp> MashTemps { get; set; } = new List<MashTemp>();

        public List<string> MashTips { get; set; } = new List<string>();

        public Fermentation Fermentation { get; set; } = new Fermentation();

        public List<Twist> Twists { get; set; } = new List<Twist>();
    }
}
