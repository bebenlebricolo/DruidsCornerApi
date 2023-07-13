namespace DruidsCornerAPI.Models.DiyDog.IndexedDb
{
    /// <summary>
    /// Encodes basic information about BrewDog's beer recipe.
    /// </summary>
    public class IndexedMaltDb : IndexedDb
    {
        /// <summary>
        /// List of properties in a reversed DB construct
        /// </summary>
        public List<ReversedPropMapping> Malts {get; set;} = new List<ReversedPropMapping>();  

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if(obj is not IndexedMaltDb || obj is null)
            {
                return false;
            }

            var other = obj as IndexedMaltDb;
            if(other == null)
            {
                return false;
            }

            if(Malts.Count != other.Malts.Count)
            {
                return false;
            }

            bool identical = true;
            
            // We don't care about the ordering here
            int index = 0;
            while(index < Malts.Count && identical)
            {
                identical &= other.Malts.Contains(Malts[index]);
                index++;
            }
            return identical;
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            return Malts.GetHashCode() * 3;
        }
    }
}
