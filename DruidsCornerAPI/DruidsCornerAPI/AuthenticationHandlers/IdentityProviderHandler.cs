using System.Text.Json;
using DruidsCornerAPI.Models.Authentication;

namespace DruidsCornerAPI.AuthenticationHandlers;

/// <summary>
/// This class handles Identity Provider Signing Keys retrieval and caching.
/// It is used to retrieve the public keys exposed by the registered Identity Providers, and handle proper keys retrieval upon expiration
/// Details : Usually, this renewal operation is performed when a service of this class is called: an http request to the selected Identity Provider endpoint
/// is sent and the Cache-Control returned header is used to register next keys rotation period. When a method of this class is called from client
/// code, it'll first check if the cached keys are still valid, otherwise it'll trigger a new keys retrieval operation and caching for next calls).
/// </summary>
public class IdentityProviderHandler
{
    private readonly ILogger<IdentityProviderHandler> _logger;
    private readonly List<IdentityProviderKind> _availableProviders;
    private readonly HttpClient _httpClient;

    private const string FirebaseKeysUrl = "https://www.googleapis.com/robot/v1/metadata/x509/securetoken@system.gserviceaccount.com";
    private const string GoogleKeysUrl = "https://www.googleapis.com/oauth2/v1/certs";

    private Dictionary<IdentityProviderKind, IdentityProviderSigningKeyContainer> _storedKeys;


    /// <summary>
    /// IdentityProviderKindHandler constructor
    /// </summary>
    /// <param name="httpClient">Http client which will be used to fetch the keys</param>
    /// <param name="availableProviders"></param>
    public IdentityProviderHandler(ILogger<IdentityProviderHandler> logger,
                                   HttpClient httpClient,
                                   List<IdentityProviderKind>? availableProviders = null
    )
    {
        _logger = logger;
        _httpClient = httpClient;
        _storedKeys = new Dictionary<IdentityProviderKind, IdentityProviderSigningKeyContainer>();

        if (availableProviders == null)
        {
            _availableProviders = new List<IdentityProviderKind>()
            {
                IdentityProviderKind.Google,
                IdentityProviderKind.Firebase
            };
        }
        else
        {
            var filtered = availableProviders.Distinct().ToList();
            _availableProviders = filtered;
        }
    }

    /// <summary>
    /// Registers a new IdentityProviderKind to the list
    /// </summary>
    /// <param name="provider"></param>
    public void RegisterProvider(IdentityProviderKind provider)
    {
        if (_availableProviders.Contains(provider))
        {
            _availableProviders.Add(provider);
        }
    }

    /// <summary>
    /// Unregisters a single provider
    /// </summary>
    /// <param name="provider"></param>
    public void UnregisterProvider(IdentityProviderKind provider)
    {
        if (_availableProviders.Contains(provider))
        {
            _availableProviders.Remove(provider);
        }
    }

    /// <summary>
    /// Used to trigger a specific Identity provider to refresh its keys
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="forceRefresh">Forces keys refresh (bypasses internal checks)</param>
    /// <returns></returns>
    private async Task<bool> RefreshKeysSingleProvider(IdentityProviderKind provider, bool forceRefresh = false)
    {
        if (!_availableProviders.Contains(provider))
        {
            return false;
        }

        // Reroute to the appropriate Keys provider
        switch (provider)
        {
            case IdentityProviderKind.Google:
                return await RefreshGoogleKeys(forceRefresh);

            case IdentityProviderKind.Firebase:
                return await RefreshFirebaseKeys(forceRefresh);

            // Default use cases - unsupported operations
            case IdentityProviderKind.Facebook:
            case IdentityProviderKind.Github:
            case IdentityProviderKind.Unknown:
            default:
                return false;
        }
    }


    private async Task<IdentityProviderSigningKeyContainer?> HandleGoogleFirebaseResponseData(
        HttpResponseMessage response,
        IdentityProviderKind identityProviderKind
    )
    {
        // Probably ok
        if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 400)
        {
            // Deserialize keys content
            try
            {
                var keysDictionary = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(await response.Content.ReadAsStreamAsync());
                if (response.Headers.CacheControl != null
                    && response.Headers.CacheControl.MaxAge != null)
                {
                    var maxAge = response.Headers.CacheControl.MaxAge;
                    // We need to deduce the caching operated by other layers between IP's servers and this webserver, as
                    // per recommended by https://developer.mozilla.org/fr/docs/Web/HTTP/Headers/Cache-Control
                    var age = response.Headers.Age;
                    var expirationDate = DateTime.Now + maxAge - age;

                    var storedKey = new IdentityProviderSigningKeyContainer
                    {
                        Kind = identityProviderKind,
                        ExpirationDate = expirationDate ?? DateTime.Now,
                        KeysDictionary = keysDictionary!
                    };
                    return storedKey;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not fetch Signing Keys data from Identity Provider {identityProviderKind.ToString()} : {ex.Message}");
            }
        }
        // Response status indicates a failure
        else
        {
            _logger.LogError($"Cannot reach endpoint for {identityProviderKind.ToString()}. Response status code : {response.StatusCode}, Response message : {response.ReasonPhrase}, Response body : {response.ToString()}");
        }

        return null;
    }

    /// <summary>
    /// Calls Firebase Identity provider and loads the new keys in memory
    /// Only refresh data if this is the first time this method is called, or data is expired.
    /// The force refresh mode bypasses all checks and trigger the refreshing unilaterally. 
    /// </summary>
    /// <param name="forceRefresh">Bypasses all internal checks and forces keys to be fetched once again.</param>
    /// <returns></returns>
    private async Task<bool> RefreshFirebaseKeys(bool forceRefresh = false)
    {
        const IdentityProviderKind identityProviderKind = IdentityProviderKind.Firebase;
        bool willRefresh = forceRefresh;
        willRefresh |= !_storedKeys.ContainsKey(identityProviderKind);
        willRefresh |= _storedKeys.ContainsKey(identityProviderKind) &&
                       _storedKeys[identityProviderKind].ExpirationDate <= DateTime.Now;

        if (willRefresh)
        {
            var response = await _httpClient.GetAsync(FirebaseKeysUrl);
            var storedKey = await HandleGoogleFirebaseResponseData(response, identityProviderKind);

            if (storedKey != null)
            {
                // Flushes previous keys (needed when keys are effectively rotated for good)
                _storedKeys.Clear();
                // Add the new keys to the stored keys container
                _storedKeys[identityProviderKind] = storedKey;
                return true;
            }

            return false;
        }

        return true;
    }

    /// <summary>
    /// Calls Google Identity provider and loads the new keys in memory
    /// Only refresh data if this is the first time this method is called, or data is expired.
    /// The force refresh mode bypasses all checks and trigger the refreshing unilaterally. 
    /// </summary>
    /// <param name="forceRefresh">Bypasses all internal checks and forces keys to be fetched once again.</param>
    /// <returns></returns>
    private async Task<bool> RefreshGoogleKeys(bool forceRefresh = false)
    {
        const IdentityProviderKind identityProviderKind = IdentityProviderKind.Google;
        bool willRefresh = forceRefresh;
        willRefresh |= !_storedKeys.ContainsKey(identityProviderKind);
        willRefresh |= _storedKeys.ContainsKey(identityProviderKind) &&
                       _storedKeys[identityProviderKind].ExpirationDate <= DateTime.Now;

        if (willRefresh)
        {
            var response = await _httpClient.GetAsync(GoogleKeysUrl);
            var storedKey = await HandleGoogleFirebaseResponseData(response, identityProviderKind);

            if (storedKey != null)
            {
                // Flushes previous keys (needed when keys are effectively rotated for good)
                _storedKeys.Clear();
                // Add the new keys to the stored keys container
                _storedKeys[identityProviderKind] = storedKey;
                return true;
            }

            return false;
        }

        return true;
    }

    /// <summary>
    /// Refresh all keys at once.
    /// Each existing key is first audited to check if it needs to be refreshed, otherwise the cached version is used.
    /// The forceRefresh parameter bypasses all checks and forces retrieval of new keys
    /// </summary>
    /// <param name="forceRefresh">Forces all keys to be refreshed, even if the expiration date is not reached yet</param>
    /// <returns></returns>
    public async Task<bool> RefreshKeys(bool forceRefresh = false)
    {
        bool result = true;
        foreach (var provider in _availableProviders)
        {
            switch (provider)
            {
                case IdentityProviderKind.Firebase:
                    result &= await RefreshFirebaseKeys(forceRefresh);
                    break;

                case IdentityProviderKind.Google:
                    result &= await RefreshGoogleKeys(forceRefresh);
                    break;

                // Pass on to the rest of the code, we don't want that to be 
                // raised as an error (this is 100% certain to fail) 
                // It should not get there anyway but in case I forgot something, 
                // won't be a problem.
                case IdentityProviderKind.Facebook:
                case IdentityProviderKind.Github:
                case IdentityProviderKind.Unknown:
                default:
                    break;
            }
        }

        return result;
    }

    /// <summary>
    /// Retrieve keys for a single identity provider.
    /// Will trigger keys retrieval in case of first call or keys are expired.
    /// </summary>
    /// <param name="identityProviderKind"></param>
    /// <returns></returns>
    public async Task<IdentityProviderSigningKeyContainer?> GetKeys(IdentityProviderKind identityProviderKind)
    {
        if (!_availableProviders.Contains(identityProviderKind))
        {
            return null;
        }

        // Triggers keys retrieval if first call or keys are expired
        if (!_storedKeys.ContainsKey(identityProviderKind) || _storedKeys[identityProviderKind].ExpirationDate <= DateTime.Now)
        {
            var result = await RefreshKeysSingleProvider(identityProviderKind);
            if (!result)
            {
                // Whoops !
                return null;
            }
        }

        // Should not
        return _storedKeys[identityProviderKind];
    }

    /// <summary>
    /// Retrieve a signing keys container using the KeyId as the search criteria.
    /// The KeyId is used in JWT tokens in order to find the appropriate public signature that was used to
    /// sign the JWT token. This is a crucial part for JWT signature validation, and only the registered IdentityProviders can be verified.
    /// </summary>
    /// <param name="keyId">Key Id as used in the authorization bearer header</param>
    /// <returns></returns>
    public IdentityProviderSigningKeyContainer? FindKeysFromKeyId(string keyId)
    {
        IdentityProviderSigningKeyContainer? target = null;
        foreach (var keysContainer in _storedKeys)
        {
            if (keysContainer.Value.KeysDictionary.ContainsKey(keyId))
            {
                target = keysContainer.Value;
                break;
            }
        }

        return target;
    }
}