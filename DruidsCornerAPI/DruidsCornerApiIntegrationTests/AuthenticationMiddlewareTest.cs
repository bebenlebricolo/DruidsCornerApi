using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using DruidsCornerAPI;
using DruidsCornerAPI.Models.Config;
using Microsoft.Extensions.Configuration;

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

    public IConfiguration GetConfiguration()
    {
        var appSettings = @"{
                ""DeployedDatabaseConfig"":{
                    ""RootFolderPath"": ""${DRUIDSCORNERAPI_DIR}/diydog-db"",
                    ""ImagesFolderName"": ""images"",
                    ""PdfPagesFolderName"": ""pdf_pages"",
                    ""RecipesFolderName"" :  ""recipes"",
                    ""IndexedDbFolderName"" :  ""dbanalysis""
                  }
            }";
        var builder = new ConfigurationBuilder();
        builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));

        var configuration = builder.Build();
        return configuration;
    }

    [Test]
    public async Task TestAudiencesValidation()
    {
        // First, we need to check that the local DB is accessible
        Assert.That(System.Environment.GetEnvironmentVariable("DRUIDSCORNERAPI_DIR"), Is.Not.Null);

        // Check that we can open the local database
        var client = _factory.CreateClient();
        var dbConfig = new DeployedDatabaseConfig();
        Assert.That(dbConfig.FromConfig(GetConfiguration()), Is.True);
        
        // Then try to get all recipes from local database
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/recipe/all");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",
                                                                      "eyJhbGciOiJSUzI1NiIsImtpZCI6IjE5MGFkMTE4YTk0MGFkYzlmMmY1Mzc2YjM1MjkyZmVkZThjMmQwZWUiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vZHJ1aWRzLWNvcm5lci1jbG91ZCIsImF1ZCI6ImRydWlkcy1jb3JuZXItY2xvdWQiLCJhdXRoX3RpbWUiOjE2OTQ1NzIzNTIsInVzZXJfaWQiOiJxajI1ZWg3aEpPV3NhUlRPZEJCZkxOZzNmck4yIiwic3ViIjoicWoyNWVoN2hKT1dzYVJUT2RCQmZMTmczZnJOMiIsImlhdCI6MTY5NDU3MjM1MiwiZXhwIjoxNjk0NTc1OTUyLCJlbWFpbCI6ImJlbm9pdHRhcnJhZGVAaG90bWFpbC5mciIsImVtYWlsX3ZlcmlmaWVkIjpmYWxzZSwiZmlyZWJhc2UiOnsiaWRlbnRpdGllcyI6eyJlbWFpbCI6WyJiZW5vaXR0YXJyYWRlQGhvdG1haWwuZnIiXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.BN_YKE3Vmp3W-ngTnrhBARA_-h59hhs-NXRltdv_rGEfiFMTB46LhUfBxkIznpEoxv_Wr77dIPo8kEPoj91o8bTPMd0UiSDcaWr9gtki5yfMI7catHJNGjj7IZvp_U2RY_Plz_-aRuLD8--kqiqibG7jmwClWihAdifv64CvDikwM4NqDpnNQ1gT32b7H_q7JxZY3fYFOG_hswDkwAQ3NJF8gbIeMUEQI3j1N9h8jLc4Ji0OVriym1wvkcSTdsjlY5_hOJb4Ulw9WYw4eWsr6gSJiA_f9C1HOHxFjHVuqhOm8QAO9Gj3BvT8u6G3OrA2Ohe3tp7vdRNRdZHrFtJ-NQ");

        var response = await client.SendAsync(request);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}