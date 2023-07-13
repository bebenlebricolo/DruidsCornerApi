namespace DruidsCornerAPI.Models.DiyDog.IndexedDb
{
    /// <summary>
    /// Encodes basic information about BrewDog's beer recipe.
    /// </summary>
    public class IndexedHopDb : IndexedDb
    {
        /// <summary>
        /// List of properties in a reversed DB construct
        /// </summary>
        public List<ReversedPropMapping> Hops {get; set;} = new List<ReversedPropMapping>();  

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if(obj is not IndexedHopDb || obj is null)
            {
                return false;
            }

            var other = obj as IndexedHopDb;
            if(other == null)
            {
                return false;
            }

            if(Hops.Count != other.Hops.Count)
            {
                return false;
            }

            bool identical = true;
            
            // We don't care about the ordering here
            int index = 0;
            while(index < Hops.Count && identical)
            {
                identical &= other.Hops.Contains(Hops[index]);
                index++;
            }
            return identical;
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            return Hops.GetHashCode() * 3;
        }
    }
}
