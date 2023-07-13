namespace DruidsCornerAPI.Models.DiyDog.IndexedDb
{
    /// <summary>
    /// Encodes basic information about BrewDog's beer recipe.
    /// </summary>
    public class IndexedTagDb : IndexedDb
    {
        /// <summary>
        /// List of properties in a reversed DB construct
        /// </summary>
        public List<ReversedPropMapping> Tags {get; set;} = new List<ReversedPropMapping>();  
        
        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if(obj is not IndexedTagDb || obj is null)
            {
                return false;
            }

            var other = obj as IndexedTagDb;
            if(other == null)
            {
                return false;
            }

            if(Tags.Count != other.Tags.Count)
            {
                return false;
            }

            bool identical = true;
            
            // We don't care about the ordering here
            int index = 0;
            while(index < Tags.Count && identical)
            {
                identical &= other.Tags.Contains(Tags[index]);
                index++;
            }
            return identical;
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            return Tags.GetHashCode() * 3;
        }
    }
}
