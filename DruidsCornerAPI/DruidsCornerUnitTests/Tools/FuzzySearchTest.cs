using DruidsCornerAPI.Models;
using DruidsCornerAPI.Tools;
using NUnit.Framework.Internal;
using System.Collections.Generic;

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

            var test1Result = FuzzySearch.SearchPartialRatio("test 1", data, elem => elem.Name);
            Assert.That(test1Result.Item1, Is.EqualTo(100));
            Assert.That(test1Result.Item2, Is.EqualTo(data[0]));

            var test2Result = FuzzySearch.SearchPartialRatio("test 2", data, elem => elem.Name);
            Assert.That(test2Result.Item2, Is.EqualTo(data[1]));

            var test3Result = FuzzySearch.SearchPartialRatio("test 3", data, elem => elem.Name);
            Assert.That(test3Result.Item2, Is.EqualTo(data[2]));

            var test4Result = FuzzySearch.SearchPartialRatio("another 1", data, elem => elem.Name);
            Assert.That(test4Result.Item2, Is.EqualTo(data[3]));

            var test5Result = FuzzySearch.SearchPartialRatio("yet another", data, elem => elem.Name);
            Assert.That(test5Result.Item2, Is.EqualTo(data[4]));

            var test6Result = FuzzySearch.SearchPartialRatio("1 te", data, elem => elem.Name);
            Assert.That(test6Result.Item2, Is.EqualTo(data[5]));

            var test7Result = FuzzySearch.SearchPartialRatio("1 te mis", data, elem => elem.Name);
            Assert.That(test7Result.Item2, Is.EqualTo(data[6]));
        }


        [Test]
        /// <summary>
        /// Not a very informative test, but it helps to lock down a stable base for this search algorithm (non-regression test)
        /// It's a bit of a poor man's test..
        /// </summary>
        public void CheckFuzzySearchAlgo_2()
        {
            var data = GenerateFakeData();

            var orderedList = FuzzySearch.SearchPartialRatioCompleteList("test 1", data, elem => elem.Name);
            
            // Lock descending order
            Assert.That(orderedList[0].Item1, Is.GreaterThan(orderedList[1].Item1));
            var expectedOrder = new List<Tuple<int, TestDataContainer>> {
                new Tuple<int, TestDataContainer>(100, data[0]),
                new Tuple<int, TestDataContainer>(67, data[5]),
                new Tuple<int, TestDataContainer>(60, data[3]),
                new Tuple<int, TestDataContainer>(50, data[4]),
                new Tuple<int, TestDataContainer>(43, data[6]),
                new Tuple<int, TestDataContainer>(37, data[1]),
                new Tuple<int, TestDataContainer>(37, data[2]),
            };
        }
    }
}