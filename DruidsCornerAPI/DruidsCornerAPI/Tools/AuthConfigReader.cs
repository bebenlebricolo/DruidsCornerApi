using System.Runtime.InteropServices;
using System.Text.Json;
using DruidsCornerAPI.Models.Config;

namespace DruidsCornerAPI.Tools;

/// <summary>
/// Reads a the AuthenticationConfig.json file from disk.
/// This file is used upon program startup in order to configure JWT Authentication
/// Settings (amongst other settings) and this drives how the authentication middleware validates tokens.
/// </summary>
public class AuthConfigReader
{
    public const string ConfigurationDirEnvVarName = "DRUIDSCORNERAPI_CONFIG_DIR";
    public const string BaseConfigDirName = "DruidsCornerApi";
    public const string DefaultConfigFilename = "AuthenticationConfig.json";
    
    /// <summary>
    /// Opens the configuration folder of the local machine
    /// First tries to read the DRUIDSCORNERAPI_CONFIG_DIR environment variable.
    /// Then if it does not exist, it'll try to fetch the $HOME/.config/DruidsCornerApi folder
    ///
    /// By default, on Linux it'll resort to :
    ///  $HOME/.config/DruidsCornerApi/AuthenticationConfig.json
    /// And on Windows :
    ///  %APPDATA%/DruidsCornerApi/AuthenticationConfig.json
    /// </summary>
    /// <returns></returns>
    public static async Task<AuthenticationConfig> ReadFromConfigFolderAsync(string filename = DefaultConfigFilename)
    {
        var outputConfig = new AuthenticationConfig();
        var configDir = System.Environment.GetEnvironmentVariable(ConfigurationDirEnvVarName);
        if (configDir == null  || !Directory.Exists(configDir))
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                configDir = Path.Join(System.Environment.GetEnvironmentVariable("HOME"), $".config/{BaseConfigDirName}");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                configDir = Path.Join(Environment.SpecialFolder.ApplicationData.ToString(), BaseConfigDirName);
            }
            else
            {
                return outputConfig;
            }

            // Check if there's something here
            if (!Directory.Exists(configDir))
            {
                return outputConfig;
            }
        }

        // Config file should be deployed in the execution environment
        var expectedConfigFilePath = Path.Join(configDir, filename);
        if (!File.Exists(expectedConfigFilePath))
        {
            return outputConfig;
        }

        var file = new FileStream(expectedConfigFilePath, FileMode.Open);
        try
        {
            var deserialized = await JsonSerializer.DeserializeAsync<AuthenticationConfig>(file); 
            outputConfig =  deserialized ?? outputConfig;
            file.Close();
        }
        catch
        {
            file.Close();
        }

        return outputConfig;
    }
}