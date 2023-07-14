using DruidsCornerAPI.Tools;
using NUnit.Framework.Internal;

namespace DruidsCornerUnitTests.Tools
{

    public class StatisticsTest
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public void TestGeometricMean()
        {
           var data = new List<double>(){
                12, 15, 18, 28
           };

           var result = Statistics.GeometricMean(data);
           Assert.AreEqual(result, 17.35, 0.1);
        }

        [Test]
        public void TestGeometricMeanEmptyArray()
        {
           var data = new List<double>(){
           };

           var result = Statistics.GeometricMean(data);
           Assert.AreEqual(result, 0.0, 0.01);
        }

        [Test]
        public void TestGeometricMeanWithZeros()
        {
           var data = new List<double>(){
                0,3,45,6,25,3
           };

            // Zeros are ignored as values, because this geometric mean is used as a selective filter, 
            // but as zeros in a product erases all other values, it's far too selective for our needs (if any string has a 0 matching score,
            // it takes everything down even though the other values might have been very close to 100!) 
           var result = Statistics.GeometricMean(data);
           Assert.AreEqual(result, 9.05, 0.01);
        }
    }
}