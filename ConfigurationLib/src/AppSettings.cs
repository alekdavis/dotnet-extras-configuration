using Microsoft.Extensions.Configuration;

namespace DotNetExtras.Configuration;
/// <summary>
/// Implements methods for setting and retrieving application configuration settings.
/// </summary>
public static partial class AppSettings
{
    private static readonly string _refKeyName = "$ref";

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
        T? value = configuration.GetValue<T?>(key);

        // For some reason, GetValue returns an empty string if the value is null,
        // so for strings, we need special handling.
        if ((typeof(T) == typeof(string) && string.IsNullOrEmpty(value?.ToString())) || 
            Equals(value, default(T)))
        {
            string? refKey = GetRefKey(configuration, key);

            if (refKey != null)
            {
                value = configuration.GetValue<T?>(refKey);
            }
        }

        return value;
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
        T[]? value = section?.Get<T[]>();

        if (value == null)
        {
            string? refKey = GetRefKey(configuration, key);

            if (refKey != null)
            {
                section = configuration.GetSection(refKey);
                value = section?.Get<T[]>();
            }
        }

        return value;
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
        List<T>? value = section?.Get<List<T>>();

        if (value == null)
        {
            string? refKey = GetRefKey(configuration, key);

            if (refKey != null)
            {
                section = configuration.GetSection(refKey);
                value = section?.Get<List<T>>();
            }
        }

        return value;
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
    public static Dictionary<TKey,TValue?>? GetDictionaryValue<TKey,TValue>
    (
        IConfiguration configuration,
        string key
    )
    where TKey : notnull
    {
        IConfigurationSection section = configuration.GetSection(key);
        Dictionary<TKey,TValue?>? value = section?.Get<Dictionary<TKey,TValue?>>();

        if (value == null)
        {
            string? refKey = GetRefKey(configuration, key);

            if (refKey != null)
            {
                section = configuration.GetSection(refKey);
                value = section?.Get<Dictionary<TKey,TValue?>>();
            }
        }

        return value;
    }

    /// <summary>
    /// Gets the redirection key from the $ref configuration setting at the same level as the specified key.
    /// </summary>
    /// <param name="configuration">
    /// Application configuration settings.
    /// </param>
    /// <param name="key">
    /// Name of the configuration setting key.
    /// </param>
    /// <returns>
    /// Redirection key if found; otherwise, null.
    /// </returns>
    private static string? GetRefKey
    (
        IConfiguration configuration,
        string key
    )
    {
        int lastSeparatorIndex = key.LastIndexOf(':');

        string keyName = lastSeparatorIndex >= 0
            ? key[(lastSeparatorIndex + 1)..]
            : key;
        
        string refKeyPath = lastSeparatorIndex >= 0
            ? key[..lastSeparatorIndex] + $":{_refKeyName}:" + keyName
            : $"{_refKeyName}:" + keyName;

        string? refKey = configuration.GetValue<string?>(refKeyPath);

        return refKey;
    }
}
