using DruidsCornerAPI.Models.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using System.Text.Json;

namespace DruidsCornerAPI.Controllers
{
    [ApiController]
    [Authorize("OAuth2")]
    [Route("gdata")]
    public class GoogleDataController : ControllerBase
    {
        private readonly ILogger<RecipeController> _logger;

        public GoogleDataController(ILogger<RecipeController> logger)
        {
            _logger = logger;
        }

        private string GetAccessToken(HttpRequest request)
        {
            StringValues authHeaderContent;
            var accessToken = "Not found";

            // Token has already been validated at this point normally so just retrieve it.
            if (request.Headers.TryGetValue("Authorization", out authHeaderContent))
            {
                accessToken = authHeaderContent.ToString().Split(' ')[1];
            }
            return accessToken;
        }

        [HttpGet(Name = "Check user profile")]
        [ProducesResponseType(500)]
        [ProducesResponseType(typeof(UserInfo), 200)]
        public async Task<IActionResult> UserProfile()
        {
            var accessToken = GetAccessToken(Request);
            var client = new HttpClient();
            var url = "https://www.googleapis.com/oauth2/v1/userinfo";
            var queryStrings = new Dictionary<string, string?>() {{"access_token", accessToken}};

            var newUrl = QueryHelpers.AddQueryString(url, queryStrings);

            var response = await client.GetAsync(newUrl);
            response.EnsureSuccessStatusCode();

            try
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonData = JsonSerializer.Deserialize<UserInfo>(responseContent);
                return Ok(jsonData);
            }
            catch (Exception ex) 
            {
                Debug.Print(ex.Message);
                _logger.LogError($"Caught deserialization error while reading Google's user profile response : {ex}");
                return StatusCode(500, "Internal server Error.");
            }
        }

    }
}