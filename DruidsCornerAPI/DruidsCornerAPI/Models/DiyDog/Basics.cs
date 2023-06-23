namespace DruidsCornerAPI.Models.DiyDog
{
    public record Basics 
    {
        public Volume Volume { get; set; } = new Volume();

        public Volume BoilVolume { get; set; } = new Volume();

        public float Abv { get; set; } = 0.0f;

        public float TargetOg { get; set; } = 0.0f;

        public float TargetFg { get; set; } = 0.0f;

        public float Ebc { get; set; } = 0.0f;

        public float Ibu { get; set; } = 0.0f;

        public float Srm { get; set; } = 0.0f;

        public float Ph { get; set; } = 0.0f;

        public float AttenuationLevel { get; set; } = 0.0f;

       //public override bool Equals(object? obj)
       //{
       //    if (obj == null)
       //    {
       //        return false;
       //    }
       //    if (obj is not Basics)
       //    {
       //        return false;
       //    }
       //    return true;
       //}

    }
}
