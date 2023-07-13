namespace DruidsCornerAPI.Models.DiyDog.References
{
    /// <summary>
    /// Very simple Known Good Yeasts list
    /// </summary>
    public class ReferenceYeasts
    {
        /// <summary>
        /// List of yeast properties
        /// </summary>
        public List<YeastProperty> Yeasts {get; set; } = new List<YeastProperty>();  

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            var other = obj as ReferenceYeasts;
            bool identical = other != null;
            identical &= Yeasts.Count == other!.Yeasts.Count;
            if(!identical) return false;
            
            // We don't care about the order here, we are looking for the same numbers and nothing more.
            int index = 0;
            while(index < Yeasts.Count && identical)
            {
                identical &= other.Yeasts!.Any(item => item == Yeasts[index]);
                index++;
            }
            return identical;
        }

        /// <summary>
        /// Custom equality operator
        /// </summary>
        public static bool operator == (ReferenceYeasts left, ReferenceYeasts right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Custom inequality operator
        /// </summary>
        public static bool operator != (ReferenceYeasts left, ReferenceYeasts right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            var hash = Yeasts.GetHashCode() * 3;
            return hash;
        }
    }
}