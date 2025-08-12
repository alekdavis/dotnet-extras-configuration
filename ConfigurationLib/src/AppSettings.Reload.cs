using Microsoft.Extensions.Configuration;

namespace DotNetExtras.Configuration;
public static partial class AppSettings
{
    /// <summary>
    /// Forces configuration settings to be reloaded from the provider.
    /// </summary>
    /// <remarks>
    /// This method is useful when configuration settings are stored in a location that can change at runtime, 
    /// such as Azure Key Vault, password safe, and so on.
    /// Fore details, see
    /// <see href="https://learn.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-8.0#reload-secrets"/>.
    /// </remarks>
    /// <param name="configuration">
    /// Application configuration settings.
    /// </param>
    public static void Reload
    (
        ref IConfiguration configuration
    )
    {
        ((IConfigurationRoot)configuration).Reload();
    }
}
