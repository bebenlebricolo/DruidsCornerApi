using System.Text;
using DruidsCornerAPI.AuthenticationHandlers;
using DruidsCornerAPI.Models.Authentication;
using DruidsCornerAPI.Models.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace DruidsCornerUnitTests.AuthenticationHandlers
{
    public class IdentityProviderHandlerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task TestCanReadGoogleSigningKeys_RealCalls()
        {
            var mockLogger = new Mock<ILogger<IdentityProviderHandler>>();
            var httpClient = new HttpClient();

            var handler = new IdentityProviderHandler(mockLogger.Object, httpClient);
            var keys = await handler.GetKeys(IdentityProviderKind.Google);
            Assert.That(keys, Is.Not.Null);
            Assert.That(keys!.KeysDictionary.Count, Is.EqualTo(2));
            Assert.That(keys!.ExpirationDate, Is.GreaterThanOrEqualTo(DateTime.Now));
        }
        
        [Test]
        public async Task TestCanReadFirebaseSigningKeys_RealCalls()
        {
            var mockLogger = new Mock<ILogger<IdentityProviderHandler>>();
            var httpClient = new HttpClient();

            var handler = new IdentityProviderHandler(mockLogger.Object, httpClient);
            var keys = await handler.GetKeys(IdentityProviderKind.Firebase);
            Assert.That(keys, Is.Not.Null);
            Assert.That(keys!.KeysDictionary.Count, Is.EqualTo(2));
            Assert.That(keys!.ExpirationDate, Is.GreaterThanOrEqualTo(DateTime.Now));
        }
    }
}