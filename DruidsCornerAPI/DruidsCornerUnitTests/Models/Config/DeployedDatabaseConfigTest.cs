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
                    ""ImagesFolderPath"" : ""Value2"",
                    ""PdfPagesFolderPath"" : ""Value3"",
                    ""RecipesFolderPath"" : ""Value4""
                  }
            }";
            var builder = new ConfigurationBuilder();
            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));

            var configuration = builder.Build();

            var deployedDatabaseConfig = new DeployedDatabaseConfig();
            Assert.IsFalse(deployedDatabaseConfig.FromConfig(configuration));
            Assert.AreEqual(deployedDatabaseConfig.RootFolderPath, "Value1");
            Assert.AreEqual(deployedDatabaseConfig.ImagesFolderPath, "Value2");
            Assert.AreEqual(deployedDatabaseConfig.PdfPagesFolderPath, "Value3");
            Assert.AreEqual(deployedDatabaseConfig.RecipesFolderPath, "Value4");
        }
    }
}