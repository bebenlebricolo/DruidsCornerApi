using DruidsCornerAPI.Models;
using DruidsCornerAPI.Models.DiyDog;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DruidsCornerUnitTests.Models.Google
{
    public class DataRecordTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestFileRecordCustomJsonConverter()
        {
            var fileRecord = new FileRecord { Path = "Somewhere" };


            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new DataRecordPolymorphicConverter()
                }
            };

            var serialized = JsonSerializer.Serialize(fileRecord, options);
            var deserialized = JsonSerializer.Deserialize<FileRecord>(serialized, options);   

            Assert.That(fileRecord, Is.EqualTo(deserialized));
        }
    }
}