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
    /// <returns></returns>
    private async Task<bool> RefreshKeysSingleProvider(IdentityProviderKind provider)
    {
        if (!_availableProviders.Contains(provider))
        {
            return false;
        }

        // Reroute to the appropriate Keys provider
        switch (provider)
        {
            case IdentityProviderKind.Google:
                return await RefreshGoogleKeys();

            case IdentityProviderKind.Firebase:
                return await RefreshFirebaseKeys();

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
    /// Calls Google Identity provider and loads the new keys in memory
    /// </summary>
    /// <returns></returns>
    private async Task<bool> RefreshFirebaseKeys()
    {
        var response = await _httpClient.GetAsync(FirebaseKeysUrl);
        var storedKey = await HandleGoogleFirebaseResponseData(response, IdentityProviderKind.Firebase);

        if (storedKey != null)
        {
            // Add the new keys to the stored keys container
            _storedKeys[IdentityProviderKind.Firebase] = storedKey;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Calls Google Identity provider and loads the new keys in memory
    /// </summary>
    /// <returns></returns>
    private async Task<bool> RefreshGoogleKeys()
    {
        var response = await _httpClient.GetAsync(GoogleKeysUrl);
        var storedKey = await HandleGoogleFirebaseResponseData(response, IdentityProviderKind.Google);

        if (storedKey != null)
        {
            // Add the new keys to the stored keys container
            _storedKeys[IdentityProviderKind.Google] = storedKey;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Refresh all keys at once
    /// </summary>
    /// <returns></returns>
    public async Task<bool> RefreshKeys()
    {
        bool result = true;
        foreach (var provider in _availableProviders)
        {
            switch (provider)
            {
                case IdentityProviderKind.Firebase:
                    result &= await RefreshFirebaseKeys();
                    break;

                case IdentityProviderKind.Google:
                    result &= await RefreshGoogleKeys();
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
}