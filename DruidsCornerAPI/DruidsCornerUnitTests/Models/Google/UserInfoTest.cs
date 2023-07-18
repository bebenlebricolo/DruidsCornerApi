using DruidsCornerAPI.Models.Google;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using System.Text.Json;

namespace DruidsCornerUnitTests.Models.Google
{
    public class UserInfoTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CheckJsonSymetry()
        {
            var userInfo = new UserInfo()
            {
                email = "fake email",
                familyName = "fake family name",
                gender = "fake gender",
                givenName = "fake given name",
                hd = "fake hd",
                id = "fake id",
                link = "fake link",
                name = "fake name",
                picture = "fake picture",
                verifiedEmail = true
            };

            var serialized = JsonSerializer.Serialize(userInfo);
            var deserialized = JsonSerializer.Deserialize<UserInfo>(serialized);   

            Assert.That(userInfo, Is.EqualTo(deserialized));
        }
    }
}