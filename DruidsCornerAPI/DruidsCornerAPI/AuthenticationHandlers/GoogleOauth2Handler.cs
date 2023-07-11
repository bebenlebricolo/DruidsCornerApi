using DruidsCornerAPI.Models.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace DruidsCornerAPI.AuthenticationHandlers
{
    /// <summary>
    /// Basic options
    /// </summary>
    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
        // Nothing to do here
    }

    /// <summary>
    /// Custom Google authenticator
    /// </summary>
    /// <see aref="https://dotnetcorecentral.com/blog/authentication-handler-in-asp-net-core/"/> 
    public class GoogleOauth2AuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        private ILogger<GoogleOauth2AuthenticationHandler> _logger;

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="options"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="encoder"></param>
        /// <param name="clock"></param>
        /// <param name="logger"></param>
        public GoogleOauth2AuthenticationHandler(IOptionsMonitor<BasicAuthenticationOptions> options,
                                                 ILoggerFactory loggerFactory,
                                                 UrlEncoder encoder,
                                                 ISystemClock clock,
                                                 ILogger<GoogleOauth2AuthenticationHandler> logger) : base(options, loggerFactory, encoder, clock)
        {
            _logger = logger;
        }

        /// <summary>
        /// Provides authentication to Google's services
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Authorization Header is absent, this should not pass !
            if (!Request.Headers.Any(h => h.Key == "Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authentication header");
            }

            var authHeaderContent = Request.Headers["Authorization"].ToString();

            // Reject if Bearer is not there as well
            if (!authHeaderContent.Contains(JwtBearerDefaults.AuthenticationScheme))
            {
                return AuthenticateResult.Fail("Unprocessable token");
            }

            var rawToken = authHeaderContent.Split(' ')[1];
            if (rawToken == null)
            {
                return AuthenticateResult.Fail("Unprocessable token");
            }

            var googleCheckTokenBaseUrl = "https://www.googleapis.com/oauth2/v1/tokeninfo";
            var url = QueryHelpers.AddQueryString(googleCheckTokenBaseUrl, new Dictionary<string, string?>() { {"access_token", rawToken } });
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);

            int statusCode = (int)response.StatusCode;
            if(statusCode >= 300 && statusCode != 400)
            {
                return AuthenticateResult.Fail($"Authentication errors, status code was {statusCode}");
            }

            try
            {
                var accessToken = await response.Content.ReadFromJsonAsync<OAuthAccessToken>();
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, accessToken!.email)
                };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new System.Security.Principal.GenericPrincipal(identity, null);
                var ticket = new AuthenticationTicket(principal, "Custom Auth scheme");
                return AuthenticateResult.Success(ticket);
            }
            catch(Exception ex) 
            {
                _logger.LogError($"Caught exception while deserializing Google Access Token : {ex}");
                return AuthenticateResult.Fail("Bad token");
            }
        }
    }
}
