using System.Net;
using System.Text.Encodings.Web;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using DruidsCornerAPI.Models.Authentication;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DruidsCornerAPI.AuthenticationHandlers;

/// <summary>
/// Inspired from https://dev.to/sardarmudassaralikhan/custom-jwt-handler-in-aspnet-core-7-web-api-183l
/// </summary>
public class CustomJwtHandler : JwtBearerHandler
{
    private readonly HttpClient _httpClient;
    private readonly IdentityProviderHandler _identityProviderHandler;

    /// <summary>
    /// Custom JWT token handler used to validate several kinds of JWT coming from different Identity Providers
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="identityProviderHandler"></param>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    /// <param name="encoder"></param>
    /// <param name="clock"></param>
    public CustomJwtHandler(HttpClient httpClient,
                            IdentityProviderHandler identityProviderHandler,
                            IOptionsMonitor<JwtBearerOptions> options,
                            ILoggerFactory logger,
                            UrlEncoder encoder,
                            ISystemClock clock
    ) : base(options, logger, encoder, clock)
    {
        _httpClient = httpClient;
        _identityProviderHandler = identityProviderHandler;
    }

    /// <summary>
    /// Handles the authentication part (checks JWT expiration date, issuers, audience, and signature)
    /// </summary>
    /// <returns></returns>
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
        var jwtToken = (JwtSecurityToken)handler.ReadToken(token);

        // 0 Check expiration date first
        if (jwtToken.ValidTo <= (DateTime.Now.ToUniversalTime() - Options.TokenValidationParameters.ClockSkew))
        {
            return AuthenticateResult.Fail("Token is expired.");
        }

        // 1 Check audiences -> Must match client ids of this application7
        {
            var validAudiences = Options.TokenValidationParameters.ValidAudiences?.ToList() ?? new List<string>();
            var rejectedAudiences = new List<string>();
            foreach (var aud in jwtToken.Audiences)
            {
                if (!validAudiences.Contains(aud))
                {
                    rejectedAudiences.Add(aud);
                }
            }

            // We have rejected some audiences
            if (rejectedAudiences.Count != 0)
            {
                Logger.LogWarning($"Received bad token with wrong audiences. Audiences : {jwtToken.Audiences}");
                return AuthenticateResult.Fail("Invalid token, your client is not authorized.");
            }
        }

        // 2 Check issuers -> Must match with authorized issuers (depends on the identity services)
        {
            var validIssuers = Options.TokenValidationParameters.ValidIssuers;
            if (jwtToken.Issuer == null || !validIssuers.Contains(jwtToken.Issuer))
            {
                Logger.LogWarning($"Received bad token with wrong issuer. Issuer : {jwtToken.Issuer}");
                return AuthenticateResult.Fail("Invalid token.");
            }
        }

        // First refresh provider signing keys.
        // If done regularly, this operation only perform http calls when cached keys are expired, otherwise it returns quickly.
        // TODO : Maybe find a better way to refresh keys, like call this on webserver startup quite early and before it starts listening
        // TODO : because despite the call being quite quick to return, there are still some checks performed internally such as iteration over stored data.
        await _identityProviderHandler.RefreshKeys();

        // 3 Check token kid -> use this with the issuers data to retrieve the identity provider signin Key
        // KID is used to validate the signature from the various IdentityProviders
        var kid = jwtToken.Header.Kid;
        var targetKeysContainer = _identityProviderHandler.FindKeysFromKeyId(kid);
        if (targetKeysContainer == null)
        {
            Logger.LogWarning($"Could not find a suitable Signing Key from token kid : {kid}");
            return AuthenticateResult.Fail("Invalid token signature.");
        }

        // Identity Provider provide their key signatures using various systems, 
        // Firebase and Google provide a similar one (all hosted in google's servers)
        bool validSignature = false;
        switch (targetKeysContainer.Kind)
        {
            case IdentityProviderKind.Firebase:
                validSignature = ValidateFirebaseTokenSignature(handler, targetKeysContainer.KeysDictionary[kid], token);
                break;

            case IdentityProviderKind.Google:
                validSignature = ValidateGoogleTokenSignature(handler, targetKeysContainer.KeysDictionary[kid], token);
                break;

            // Other kinds of providers are not implemented yet
            case IdentityProviderKind.Facebook:
            case IdentityProviderKind.Github:
            case IdentityProviderKind.Unknown:
            default:
                break;
        }


        if (validSignature)
        {
            var claimsPrincipal = GetClaims(jwtToken);
            return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, "CustomJwtBearer"));
        }
        else
        {
            return AuthenticateResult.Fail("Invalid token.");
        }
    }

    private bool ValidateFirebaseTokenSignature(JwtSecurityTokenHandler handler, string rawKey, string token)
    {
        // Firebase : https://firebase.google.com/docs/auth/admin/verify-id-tokens#c
        var cert = X509Certificate2.CreateFromPem(rawKey.ToCharArray());
        var publicKey = cert.GetPublicKey();
        var rsa = RSA.Create();
        rsa.ImportRSAPublicKey(publicKey, out _);
        var securityKey = new RsaSecurityKey(rsa);
        try
        {
            handler.ValidateToken(token,
                                  new TokenValidationParameters()
                                  {
                                      ValidateAudience = false,
                                      ValidateLifetime = false,
                                      ValidateIssuer = false,
                                      IssuerSigningKey = securityKey
                                  },
                                  out _);
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Token signature verification failed");
            return false;
        }

        return true;
    }

    private bool ValidateGoogleTokenSignature(JwtSecurityTokenHandler handler, string rawKey, string token)
    {
        // Google : https://developers.google.com/identity/sign-in/web/backend-auth?hl=en
        var cert = X509Certificate2.CreateFromPem(rawKey.ToCharArray());
        var publicKey = cert.GetPublicKey();
        var rsa = RSA.Create();
        rsa.ImportRSAPublicKey(publicKey, out _);
        var securityKey = new RsaSecurityKey(rsa);
        try
        {
            handler.ValidateToken(token,
                                  new TokenValidationParameters()
                                  {
                                      ValidateAudience = false,
                                      ValidateLifetime = false,
                                      ValidateIssuer = false,
                                      IssuerSigningKey = securityKey
                                  },
                                  out _);
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Token signature verification failed");
            return false;
        }

        return true;
    }

    private ClaimsPrincipal GetClaims(JwtSecurityToken token)
    {
        var claimsIdentity = new ClaimsIdentity(token.Claims, "Token");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        return claimsPrincipal;
    }
}