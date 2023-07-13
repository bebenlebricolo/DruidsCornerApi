namespace DruidsCornerAPI.Models.DiyDog.IndexedDb
{
    /// <summary>
    /// Encodes basic information about BrewDog's beer recipe.
    /// </summary>
    public class IndexedYeastDb : IndexedDb
    {
        /// <summary>
        /// List of properties in a reversed DB construct
        /// </summary>
        public List<ReversedPropMapping> Yeasts {get; set;} = new List<ReversedPropMapping>();  

         /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if(obj is not IndexedYeastDb || obj is null)
            {
                return false;
            }

            var other = obj as IndexedYeastDb;
            if(other == null)
            {
                return false;
            }

            if(Yeasts.Count != other.Yeasts.Count)
            {
                return false;
            }

            bool identical = true;
            
            // We don't care about the ordering here
            int index = 0;
            while(index < Yeasts.Count && identical)
            {
                identical &= other.Yeasts.Contains(Yeasts[index]);
                index++;
            }
            return identical;
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            return Yeasts.GetHashCode() * 3;
        }
    }
}
