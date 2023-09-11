using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using DruidsCornerAPI;
namespace DruidsCornerApiIntegrationTests;

public class AuthMiddlewareTest
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthMiddlewareTest()
    {
        _factory = new WebApplicationFactory<Program>();
    }
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task TestAudiencesValidation()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/recipe/all");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJSUzI1NiIsImtpZCI6ImZkNDhhNzUxMzhkOWQ0OGYwYWE2MzVlZjU2OWM0ZTE5NmY3YWU4ZDYiLCJ0eXAiOiJKV1QifQ.eyJhdWQiOiI3NTgxNjQ4NjI1MzAtZGJuMmhxdjNwOGZjdmo2MGFwZXU4dG9nYWplZWtmZ20uYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJhenAiOiJkcnVpZHNjb3JuZXJhcHAtYW5kcm9pZHNhQGRydWlkcy1jb3JuZXItY2xvdWQuaWFtLmdzZXJ2aWNlYWNjb3VudC5jb20iLCJlbWFpbCI6ImRydWlkc2Nvcm5lcmFwcC1hbmRyb2lkc2FAZHJ1aWRzLWNvcm5lci1jbG91ZC5pYW0uZ3NlcnZpY2VhY2NvdW50LmNvbSIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJleHAiOjE2OTExNTAzMjIsImlhdCI6MTY5MTE0NjcyMiwiaXNzIjoiaHR0cHM6Ly9hY2NvdW50cy5nb29nbGUuY29tIiwic3ViIjoiMTA3ODQyMDg5MjU4Nzk2ODU0MTI2In0.JcLLHrNWSnZ7A0ptkA9TTgOfMznH0PAN4v4G41N4MH8zYmIWWjXgSVeKB6OstcSqUsiCpEFJaTF40GmpMn8178iicgTMRyIYnMF3HCRiU2AKfpOK4-uekgpHiN71y130uR1GXxhEMcmEgBOVvv13N4UzHgFcZzn90NrF1MFfq6Nzhn2Hho_8vCrkceXY1Jm5FrBT8BqxMWCq80Dg6ObXpUZBx8499paPrrs9H6fHyL5uz7aaK7qneK56PmySlVrK4bEnUZCt61w4b3T56PdSJxVz3MpyXh9-N1jB1NYlGKFLsFlqThdthhrBXCbovzxScnM00VVeUFmBvbSzf7tn7w");

        var response = await client.SendAsync(request);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}