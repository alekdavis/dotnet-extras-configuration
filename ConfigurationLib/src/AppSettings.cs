using Microsoft.Extensions.Configuration;
using System.Text;

namespace DotNetExtras.Configuration;
/// <summary>
/// Implements methods for setting and retrieving application configuration settings.
/// </summary>
/// <remarks>
/// </remarks>
public static partial class AppSettings
{
    /// <summary>
    /// Returns a primitive application setting value for the given key.
    /// </summary>
    /// <param name="configuration">
    /// Application configuration settings.
    /// </param>
    /// <typeparam name="T">
    /// Data type of the value.
    /// </typeparam>
    /// <param name="key">
    /// Name of the configuration setting key.
    /// </param>
    /// <returns>
    /// Configuration setting holding a single value.
    /// </returns>
    public static T? GetValue<T>
    (
        IConfiguration configuration,
        string key
    )
    {
        return configuration.GetValue<T?>(key);
    }

    /// <summary>
    /// Returns a configuration setting array value for the given key.
    /// </summary>
    /// <typeparam name="T">
    /// Data type of the list elements.
    /// </typeparam>
    /// <param name="configuration">
    /// Application configuration settings.
    /// </param>
    /// <param name="key">
    /// Name of the configuration setting key.
    /// </param>
    /// <returns>
    /// Configuration setting holding an array.
    /// </returns>
    public static T[]? GetArrayValue<T>
    (
        IConfiguration configuration,
        string key
    )
    {
        IConfigurationSection section = configuration.GetSection(key);

        return section?.Get<T[]>();
    }

    /// <summary>
    /// Returns a configuration setting list value for the given key.
    /// </summary>
    /// <typeparam name="T">
    /// Data type of the list elements.
    /// </typeparam>
    /// <param name="configuration">
    /// Application configuration settings.
    /// </param>
    /// <param name="key">
    /// Name of the configuration setting key.
    /// </param>
    /// <returns>
    /// Configuration setting holding a list.
    /// </returns>
    public static List<T>? GetListValue<T>
    (
        IConfiguration configuration,
        string key
    )
    {
        IConfigurationSection section = configuration.GetSection(key);

        return section?.Get<List<T>>();
    }

    /// <summary>
    /// Returns a configuration setting hash set value for the given key.
    /// </summary>
    /// <typeparam name="T">
    /// Data type of the hash set elements.
    /// </typeparam>
    /// <param name="configuration">
    /// Application configuration settings.
    /// </param>
    /// <param name="key">
    /// Name of the configuration setting key.
    /// </param>
    /// <returns>
    /// Configuration setting holding a hash set.
    /// </returns>
    public static HashSet<T>? GetHashSetValue<T>
    (
        IConfiguration configuration,
        string key
    )
    {
        List<T>? list = GetListValue<T>(configuration, key);
        
        return list == null 
            ? null
            : [.. list.Distinct()];
    }

    /// <summary>
    /// Returns a configuration setting dictionary value for the given key.
    /// </summary>
    /// <typeparam name="TKey">
    /// Data type of the dictionary keys (currently, only string keys are supported).
    /// </typeparam>
    /// <typeparam name="TValue">
    /// Data type of the dictionary values.
    /// </typeparam>
    /// <param name="configuration">
    /// Application configuration settings.
    /// </param>
    /// <param name="key">
    /// Name of the configuration setting key.
    /// </param>
    /// <returns>
    /// Configuration setting holding a dictionary.
    /// </returns>
    public static Dictionary<TKey,TValue?>? ToDictionary<TKey,TValue>
    (
        IConfiguration configuration,
        string key
    )
    where TKey : notnull
    {
        IConfigurationSection section = configuration.GetSection(key);

        return section?.Get<Dictionary<TKey,TValue?>>();
    }
}
