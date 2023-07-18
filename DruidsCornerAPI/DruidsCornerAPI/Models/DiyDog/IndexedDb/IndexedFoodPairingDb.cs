using System.Reflection.Metadata.Ecma335;
using DruidsCornerAPI.Tools;

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
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if(obj is not IndexedFoodPairingDb)
            {
                return false;
            }
            return Equals(obj as IndexedFoodPairingDb);
        }

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IndexedFoodPairingDb? other)
        {
            bool identical = other  is not null;
            if(!identical) return false;
            
            identical &= Language.CompareEquivalentLists(FoodPairing, other!.FoodPairing);
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
