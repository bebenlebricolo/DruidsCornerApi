namespace DruidsCornerAPI.Models.DiyDog
{
    public record Ingredients 
    {
        public List<Malt> Malts { get; set; } = new List<Malt>();

        public List<Hop> Hops { get; set; } = new List<Hop>();

        public List<Yeast> Yeasts { get; set; } = new List<Yeast>();

        public string? AlternativeDescription { get; set; } = null;

    }
}
