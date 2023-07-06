using System.Text;
using Microsoft.Extensions.Configuration;
using DruidsCornerAPI.Models.Config;

namespace DruidsCornerUnitTests.Models.Google
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
            Assert.IsFalse(deployedDatabaseConfig.FromConfig(configuration));
            Assert.AreEqual(deployedDatabaseConfig.RootFolderPath, "Value1");
            Assert.AreEqual(deployedDatabaseConfig.ImagesFolderName, "Value2");
            Assert.AreEqual(deployedDatabaseConfig.PdfPagesFolderName, "Value3");
            Assert.AreEqual(deployedDatabaseConfig.RecipesFolderName, "Value4");
            Assert.AreEqual(deployedDatabaseConfig.IndexedDbFolderName, "Value5");
        }
    }
}