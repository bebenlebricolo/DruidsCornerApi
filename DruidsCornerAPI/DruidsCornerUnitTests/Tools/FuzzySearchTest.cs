using DruidsCornerAPI.Tools;
using NUnit.Framework.Internal;

namespace DruidsCornerUnitTests.Tools
{

    internal record TestDataContainer
    {
        public TestDataContainer(string name, string origin)
        {
            Name = name;
            Origin = origin;
        }

        public string Name {get; set;}

        public string Origin {get; set;}        
    }

    public class FuzzySearchTests
    {
        [SetUp]
        public void Setup()
        {
        }


        private List<TestDataContainer> GenerateFakeData()
        {
            var data = new List<TestDataContainer>{
                new TestDataContainer("test 1", "some origin"),
                new TestDataContainer("test 2 with some text", "some origin"),
                new TestDataContainer("some text with test 3", "some origin"),
                new TestDataContainer("another test 1", "some origin"),
                new TestDataContainer("yet another test 1", "some origin"),
                new TestDataContainer("1 test", "some origin"),
                new TestDataContainer("1 test misordered", "some origin")
           };

           return data;
        }

        [Test]
        public void CheckFuzzySearchAlgo_1()
        {
           
            var data = GenerateFakeData();

            var test1Result = FuzzySearch.SearchInList("test 1", data, elem => new List<string> {elem.Name});
            Assert.That(test1Result, Is.Not.Null);
            Assert.That(test1Result.Ratio, Is.EqualTo(100));
            Assert.That(test1Result.Prop, Is.EqualTo(data[0]));

            var test2Result = FuzzySearch.SearchInList("test 2", data, elem => new List<string> {elem.Name});
            Assert.That(test2Result, Is.Not.Null);
            Assert.That(test2Result.Prop, Is.EqualTo(data[1]));

            var test3Result = FuzzySearch.SearchInList("test 3", data, elem => new List<string> {elem.Name});
            Assert.That(test3Result, Is.Not.Null);
            Assert.That(test3Result.Prop, Is.EqualTo(data[2]));

            var test4Result = FuzzySearch.SearchInList("another 1", data, elem => new List<string> {elem.Name});
            Assert.That(test4Result, Is.Not.Null);
            Assert.That(test4Result.Prop, Is.EqualTo(data[3]));

            var test5Result = FuzzySearch.SearchInList("yet another", data, elem => new List<string> {elem.Name});
            Assert.That(test5Result, Is.Not.Null);
            Assert.That(test5Result.Prop, Is.EqualTo(data[4]));

            var test6Result = FuzzySearch.SearchInList("1 te", data, elem => new List<string> {elem.Name});
            Assert.That(test6Result, Is.Not.Null);
            Assert.That(test6Result.Prop, Is.EqualTo(data[5]));

            var test7Result = FuzzySearch.SearchInList("1 te mis", data, elem => new List<string> {elem.Name});
            Assert.That(test7Result, Is.Not.Null);
            Assert.That(test7Result.Prop, Is.EqualTo(data[6]));
        }


        [Test]
        /// <summary>
        /// Not a very informative test, but it helps to lock down a stable base for this search algorithm (non-regression test)
        /// It's a bit of a poor man's test..
        /// </summary>
        public void CheckFuzzySearchAlgo_2()
        {
            var data = GenerateFakeData();

            var orderedList = FuzzySearch.SearchInListFullResults("test 1", data, elem => new List<string> {elem.Name}, FuzzMode.Ratio);
            
            // Lock descending order
            Assert.That(orderedList[0].Ratio, Is.GreaterThan(orderedList[1].Ratio));
            var expectedOrder = new List<FuzzySearchResult<TestDataContainer>> {
                new FuzzySearchResult<TestDataContainer>(100, data[0]),
                new FuzzySearchResult<TestDataContainer>(67, data[5]),
                new FuzzySearchResult<TestDataContainer>(60, data[3]),
                new FuzzySearchResult<TestDataContainer>(50, data[4]),
                new FuzzySearchResult<TestDataContainer>(43, data[6]),
                new FuzzySearchResult<TestDataContainer>(37, data[1]),
                new FuzzySearchResult<TestDataContainer>(37, data[2]),
            };

            // Assert order and values are correct
            for(int i = 0; i < orderedList.Count ; i++) 
            {
                Assert.That(orderedList[i], Is.EqualTo(expectedOrder[i]));
            }
        }
    }
}