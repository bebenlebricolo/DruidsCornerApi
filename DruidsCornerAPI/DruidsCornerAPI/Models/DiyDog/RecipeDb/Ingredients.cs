namespace DruidsCornerAPI.Models.DiyDog.RecipeDb
{
    /// <summary>
    /// Ingredients datastructure 
    /// </summary>
    public record Ingredients 
    {
        /// <summary>
        /// List of malts used for a single recipe
        /// </summary>
        public List<Malt> Malts { get; set; } = new List<Malt>();

        /// <summary>
        /// List of hops used for a single recipe
        /// </summary>
        public List<Hop> Hops { get; set; } = new List<Hop>();


        /// <summary>
        /// Optional list of extra boil / fermentation ingredient
        /// </summary>
        public List<ExtraBoil>? extraBoils{ get; set; } = null;
        
        /// <summary>
        /// Optional list of extra mash ingredients
        /// </summary>
        public List<ExtraMash>? extraMashes{ get; set; } = null;


        /// <summary>
        /// List of yeast used for a single recipe
        /// </summary> 
        public List<Yeast> Yeasts { get; set; } = new List<Yeast>();

        /// <summary>
        /// Alternative optional description, sometimes on some recipes an additional description was given instead of ingredients (...)
        /// </summary>
        public string? AlternativeDescription { get; set; } = null;

    }
}
