using System.Text;
using Microsoft.Extensions.Configuration;
using DruidsCornerAPI.Models.Config;

namespace DruidsCornerUnitTests.Models.Config
{
    public class DeployedDatabaseConfigTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CheckCanReadFromConfig()
        {
            var appSettings = @"{
                ""DeployedDatabaseConfig"":{
                    ""RootFolderPath"" : ""Value1"",
                    ""ImagesFolderName"" : ""Value2"",
                    ""PdfPagesFolderName"" : ""Value3"",
                    ""RecipesFolderName"" : ""Value4"",
                    ""IndexedDbFolderName"" : ""Value5""
                  }
            }";
            var builder = new ConfigurationBuilder();
            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));

            var configuration = builder.Build();

            var deployedDatabaseConfig = new DeployedDatabaseConfig();
            Assert.That(!deployedDatabaseConfig.FromConfig(configuration));
            Assert.That(deployedDatabaseConfig.RootFolderPath, Is.EqualTo("Value1"));
            Assert.That(deployedDatabaseConfig.ImagesFolderName, Is.EqualTo("Value2"));
            Assert.That(deployedDatabaseConfig.PdfPagesFolderName, Is.EqualTo("Value3"));
            Assert.That(deployedDatabaseConfig.RecipesFolderName, Is.EqualTo("Value4"));
            Assert.That(deployedDatabaseConfig.IndexedDbFolderName, Is.EqualTo("Value5"));
        }
    }
}