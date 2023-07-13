namespace DruidsCornerAPI.Models.DiyDog.IndexedDb
{
    /// <summary>
    /// Encodes basic information about BrewDog's beer recipe.
    /// </summary>
    public class IndexedStyleDb : IndexedDb
    {
        /// <summary>
        /// List of properties in a reversed DB construct
        /// </summary>
        public List<ReversedPropMapping> Styles {get; set;} = new List<ReversedPropMapping>();  

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if(obj is not IndexedStyleDb || obj is null)
            {
                return false;
            }

            var other = obj as IndexedStyleDb;
            if(other == null)
            {
                return false;
            }

            if(Styles.Count != other.Styles.Count)
            {
                return false;
            }

            bool identical = true;
            
            // We don't care about the ordering here
            int index = 0;
            while(index < Styles.Count && identical)
            {
                identical &= other.Styles.Contains(Styles[index]);
                index++;
            }
            return identical;
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            return Styles.GetHashCode() * 3;
        }
    }
}
