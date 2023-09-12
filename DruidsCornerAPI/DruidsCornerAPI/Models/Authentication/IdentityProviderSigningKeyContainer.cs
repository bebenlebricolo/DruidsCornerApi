using System.Runtime.InteropServices.JavaScript;

namespace DruidsCornerAPI.Models.Authentication;

/// <summary>
/// Used to store IdentityProvider public JWT Token Signing keys  
/// </summary>
public record IdentityProviderSigningKeyContainer
{
    /// <summary>
    /// Records the provider kind for this data element
    /// </summary>
    public IdentityProviderKind Kind { get; set; }
    
    /// <summary>
    /// Gives the expiration date for those keys.
    /// This data is retrieved from the Identity Providers endpoints using the Cache-Control header if available
    /// Keys are regularly rotated and may need to be refreshed from time to time.
    /// </summary>
    public DateTime ExpirationDate { get; set; }
    
    /// <summary>
    /// Actual keys content, with their kid and public RSA key counterpart
    /// </summary>
    public Dictionary<string, string> KeysDictionary { get; set; }
}