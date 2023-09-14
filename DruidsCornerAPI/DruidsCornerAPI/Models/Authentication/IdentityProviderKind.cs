namespace DruidsCornerAPI.Models.Authentication;

/// <summary>
/// Available Identity providers that can be used to provide JWT tokens to this api
/// </summary>
public enum IdentityProviderKind
{
    /// <summary>
    /// Regular firebase user, when no 3rd party identity provider is used, identity is provided by
    /// Firebase' database (username/identifier + password scheme) 
    /// </summary>
    Firebase,

    /// <summary>
    /// Google Identity provider (using Google Identity services, e.g. through the Google Sign In options)
    /// </summary>
    Google,

    /// <summary>
    /// Github Identity provider (when account is linked with a Github account)
    /// Not implemented yet
    /// </summary>
    Github,

    /// <summary>
    /// Facebook Identity provider (when account is linked with a Facebook account)
    /// Not implemented yet
    /// </summary>
    Facebook,

    /// <summary>
    /// Default use case, when we cannot guess what's the Identity Provider (error case)
    /// </summary>
    Unknown
}
