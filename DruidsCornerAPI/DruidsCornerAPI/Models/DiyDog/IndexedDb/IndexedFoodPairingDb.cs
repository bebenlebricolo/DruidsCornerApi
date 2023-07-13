namespace DruidsCornerAPI.Models.DiyDog.IndexedDb
{
    /// <summary>
    /// Encodes basic information about BrewDog's beer recipe.
    /// </summary>
    public class IndexedFoodPairingDb : IndexedDb
    {
        /// <summary>
        /// List of properties in a reversed DB construct
        /// </summary>
        public List<ReversedPropMapping> FoodPairing  {get; set;} = new List<ReversedPropMapping>();  

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if(obj is not IndexedFoodPairingDb || obj is null)
            {
                return false;
            }

            var other = obj as IndexedFoodPairingDb;
            if(other == null)
            {
                return false;
            }

            if(FoodPairing.Count != other.FoodPairing.Count)
            {
                return false;
            }

            bool identical = true;
            
            // We don't care about the ordering here
            int index = 0;
            while(index < FoodPairing.Count && identical)
            {
                identical &= other.FoodPairing.Contains(FoodPairing[index]);
                index++;
            }
            return identical;
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            return FoodPairing.GetHashCode() * 3;
        }
    }
}
