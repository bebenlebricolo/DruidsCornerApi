using DruidsCornerAPI.Models.Google;
using System.Text.Json;

namespace DruidsCornerUnitTests.Models.Google
{
    public class OAuthAccessTokenTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CheckJsonSymetry()
        {
            var accessToken = new OAuthAccessToken()
            {
                email = "fake email",
                verifiedEmail = true,
                accessType = "online",
                audience = "fake audience",
                expiresIn = 3600,
                issuedTo = "no one",
                scope = "scope1 scope2 scope3",
                userId = "123456789",

            };

            var serialized = JsonSerializer.Serialize(accessToken);
            var deserialized = JsonSerializer.Deserialize<OAuthAccessToken>(serialized);   

            Assert.AreEqual(accessToken, deserialized);
        }
    }
}