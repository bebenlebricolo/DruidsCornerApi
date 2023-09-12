using System.Net;
using System.Text.Encodings.Web;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace DruidsCornerAPI.AuthenticationHandlers;

/// <summary>
/// Inspired from https://dev.to/sardarmudassaralikhan/custom-jwt-handler-in-aspnet-core-7-web-api-183l
/// </summary>
public class CustomJwtHandler : JwtBearerHandler
{
    private readonly HttpClient _httpClient;

    public CustomJwtHandler(HttpClient httpClient,
                            IOptionsMonitor<JwtBearerOptions> options,
                            ILoggerFactory logger,
                            UrlEncoder encoder,
                            ISystemClock clock
    ) : base(options, logger, encoder, clock)
    {
        _httpClient = httpClient;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        const string bearerStr = "Bearer";
        // Get the token from the Authorization header
        if (!Context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorizationHeaderValues))
        {
            return AuthenticateResult.Fail("Authorization header not found.");
        }

        var authorizationHeader = authorizationHeaderValues.FirstOrDefault();
        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith(bearerStr + " "))
        {
            return AuthenticateResult.Fail("Bearer token not found in Authorization header.");
        }

        var token = authorizationHeader.Substring((bearerStr + " ").Length).Trim();
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token);
        
        // 0 Check expiration date first
        // 1 Check audiences -> Must match client ids of this application
        // 2 Check issuers -> Must match with authorized issuers (depends on the identity services)
        // 3 Check token kid -> use this with the issuers data to retrieve the identity provider signin Key
        //   3.1 Fetch keys that need to be refreshed
        //   3.2 In the fetched keys, retrieve the one that has the same kid
        //   3.3 Use this to validate the signature of the token
        
        // Google : https://developers.google.com/identity/sign-in/web/backend-auth?hl=en
        // Firebase : https://firebase.google.com/docs/auth/admin/verify-id-tokens#c
        
        var claimsPrincipal = GetClaims(token);
        return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, "CustomJwtBearer"));
    }
    
    private ClaimsPrincipal GetClaims(string Token)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadToken(Token) as JwtSecurityToken;

        var claimsIdentity = new ClaimsIdentity(token.Claims, "Token");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        return claimsPrincipal;
    }
}