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
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJSUzI1NiIsImtpZCI6IjE5MGFkMTE4YTk0MGFkYzlmMmY1Mzc2YjM1MjkyZmVkZThjMmQwZWUiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vZHJ1aWRzLWNvcm5lci1jbG91ZCIsImF1ZCI6ImRydWlkcy1jb3JuZXItY2xvdWQiLCJhdXRoX3RpbWUiOjE2OTQ1MTQxNDQsInVzZXJfaWQiOiJxajI1ZWg3aEpPV3NhUlRPZEJCZkxOZzNmck4yIiwic3ViIjoicWoyNWVoN2hKT1dzYVJUT2RCQmZMTmczZnJOMiIsImlhdCI6MTY5NDUxNDE0NCwiZXhwIjoxNjk0NTE3NzQ0LCJlbWFpbCI6ImJlbm9pdHRhcnJhZGVAaG90bWFpbC5mciIsImVtYWlsX3ZlcmlmaWVkIjpmYWxzZSwiZmlyZWJhc2UiOnsiaWRlbnRpdGllcyI6eyJlbWFpbCI6WyJiZW5vaXR0YXJyYWRlQGhvdG1haWwuZnIiXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.Yy4Zl_Sp66-JPsUJqItstEoeOYSCrFjtChjqODkSdpJR0qQ7qQMy9f9Gz11fJNtiS0qGRd1hrfLsvRJriOGhw76XHgYUtxDIzyFf4wAwQMKEmkl5IXcJZqlZI5adD1kYbxTgujNycxNDXovr6kbeGUcIKE9ptR-Qa3FnJYpLtq5qpvzA0JgV4O5ceNBGjqnf88BprN1mHnHDZn6uCM6shVjJUEB-UlGI5H7dFY8uRde-5Si7LWe6kFKi0jjqvXFpNBu0nlfjbeCbOugbIwwI8ABr7dVBL0FAG-FpVjbB5PjacBDC4MM8voYCkNlZYcsgrna_wlpU8ll_JGAEB5LTlQ");

        var response = await client.SendAsync(request);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}