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
            
            // Sometimes we can get as much as 3 keys, when they are rotating the keys some may continue to exist for a little
            // while before they are revoked and removed from the publication url.
            Assert.That(keys!.KeysDictionary.Count, Is.GreaterThanOrEqualTo(2));

            // Same remark here, for the keys that are just rotated, some discrepancy is observed in their expiration date
            // It might be a little bit off when testing, so provisioning a little more time might do the trick.
            Assert.That(keys!.ExpirationDate, Is.GreaterThanOrEqualTo(DateTime.Now - TimeSpan.FromSeconds(5)));
        }
        
        [Test]
        public async Task TestCanReadFirebaseSigningKeys_RealCalls()
        {
            var mockLogger = new Mock<ILogger<IdentityProviderHandler>>();
            var httpClient = new HttpClient();

            var handler = new IdentityProviderHandler(mockLogger.Object, httpClient);
            var keys = await handler.GetKeys(IdentityProviderKind.Firebase);
            Assert.That(keys, Is.Not.Null);

            // Sometimes we can get as much as 3 keys, when they are rotating the keys some may continue to exist for a little
            // while before they are revoked and removed from the publication url.
            Assert.That(keys!.KeysDictionary.Count, Is.GreaterThanOrEqualTo(2));

            // Same remark here, for the keys that are just rotated, some discrepancy is observed in their expiration date
            // It might be a little bit off when testing, so provisioning a little more time might do the trick.
            Assert.That(keys!.ExpirationDate, Is.GreaterThanOrEqualTo(DateTime.Now - TimeSpan.FromSeconds(5)));
        }
    }
}