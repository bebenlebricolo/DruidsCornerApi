using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using DruidsCornerAPI;
using DruidsCornerAPI.Models.Config;
using DruidsCornerAPI.Models.DiyDog.RecipeDb;
using Microsoft.Extensions.Configuration;

namespace DruidsCornerApiIntegrationTests;

public class AuthMiddlewareTest
{
    private readonly WebApplicationFactory<Program> _factory = new();
    private const string FirebaseAccessTokenEnvVarName = "FIREBASE_ACCESS_TOKEN";
    private const string AndroidGuestTokenEnvVarName = "ANDROID_GUEST_ACCESS_TOKEN";
    private const string GoogleAccessTokenEnvVarName = "GOOGLE_ACCESS_TOKEN";
    private const string DeployedDbEnvVarName = "DRUIDSCORNERAPI_DIR";
    private const string BearerConstant = "Bearer";

    [SetUp]
    public void Setup()
    {
        // First, we need to check that the local DB is accessible
        Assert.That(System.Environment.GetEnvironmentVariable(DeployedDbEnvVarName), Is.Not.Null);
        var dbConfig = new DeployedDatabaseConfig();
        Assert.That(dbConfig.FromConfig(GetConfiguration()), Is.True);
    }

    public IConfiguration GetConfiguration()
    {
        var appSettings = new
        {
            DeployedDatabaseConfig = new
            {
                RootFolderPath = "${" + DeployedDbEnvVarName + "}/diydog-db",
                ImagesFolderName = "images",
                PdfPagesFolderName = "pdf_pages",
                RecipesFolderName = "recipes",
                IndexedDbFolderName = "dbanalysis"
            }
        };
        string json = JsonSerializer.Serialize(appSettings);
        var builder = new ConfigurationBuilder();
        builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(json)));

        var configuration = builder.Build();
        return configuration;
    }

    private async Task PerformTokenValidation_Real(string token)
    {
        // Check that we can open the local database
        var client = _factory.CreateClient();

        // Then try to get all recipes from local database
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/recipe/all");
        request.Headers.Authorization = new AuthenticationHeaderValue(BearerConstant, token);

        var response = await client.SendAsync(request);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var recipes = await JsonSerializer.DeserializeAsync<List<Recipe>>(await response.Content.ReadAsStreamAsync());
        Assert.That(recipes, Is.Not.Null);
        Assert.That(recipes!.Count, Is.GreaterThan(0));
    }
    
    [Test]
    public async Task TestFirebaseTokenValidation_Real()
    {
        var token = System.Environment.GetEnvironmentVariable(FirebaseAccessTokenEnvVarName);
        Assert.That(token, Is.Not.Null);

        await PerformTokenValidation_Real(token!);
    }
    
    [Test]
    public async Task TestGoogleTokenValidation_Real()
    {
        var token = System.Environment.GetEnvironmentVariable(GoogleAccessTokenEnvVarName);
        Assert.That(token, Is.Not.Null);

        await PerformTokenValidation_Real(token!);
    }
    
    [Test]
    public async Task TestAndroidGuestTokenValidation_Real()
    {
        var token = System.Environment.GetEnvironmentVariable(AndroidGuestTokenEnvVarName);
        Assert.That(token, Is.Not.Null);

        await PerformTokenValidation_Real(token!);
    }
}