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
                if (typeof(T) == typeof(string))
                {
                    string? resolvedValue = ResolveReference(configuration, refKey);
                    value = resolvedValue != null ? (T)(object)resolvedValue : default;
                }
                else
                {
                    value = configuration.GetValue<T?>(refKey);
                }
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
    /// Resolves a reference which can be either a simple key path or a template with placeholders.
    /// If the reference contains {...} placeholders, they are resolved.
    /// Otherwise, the reference is treated as a simple key path.
    /// </summary>
    /// <param name="configuration">
    /// Application configuration settings.
    /// </param>
    /// <param name="reference">
    /// Reference string that can be a key path or a template.
    /// </param>
    /// <returns>
    /// Resolved string value, or null if the reference points to a missing/null value with no literals.
    /// </returns>
    private static string? ResolveReference
    (
        IConfiguration configuration,
        string reference
    )
    {
        if (string.IsNullOrEmpty(reference))
        {
            return reference;
        }

        // Check if the reference contains valid placeholders
        if (ContainsPlaceholder(reference))
        {
            return ResolvePlaceholders(configuration, reference);
        }

        // Treat as a simple key path
        string? value = configuration[reference];

        return value;
    }

    /// <summary>
    /// Checks if a string contains valid (properly closed) {...} placeholders.
    /// </summary>
    /// <param name="text">
    /// Text to check.
    /// </param>
    /// <returns>
    /// True if the text contains at least one valid placeholder; otherwise, false.
    /// </returns>
    private static bool ContainsPlaceholder
    (
        string text
    )
    {
        int i = 0;

        while (i < text.Length)
        {
            if (text[i] == '{')
            {
                // Check for escaped {{ 
                if (i + 1 < text.Length && text[i + 1] == '{')
                {
                    i += 2;
                    continue;
                }

                // Found unescaped opening brace, check if it has a closing brace
                int j = i + 1;
                while (j < text.Length)
                {
                    if (text[j] == '}')
                    {
                        // Check for escaped }}
                        if (j + 1 < text.Length && text[j + 1] == '}')
                        {
                            j += 2;
                            continue;
                        }

                        // Found a valid, properly closed placeholder
                        return true;
                    }

                    if (text[j] == '{')
                    {
                        // Found another opening brace, just skip it
                        j++;
                        continue;
                    }

                    j++;
                }

                // No closing brace found for this opening brace
                // Treat entire string as literal key path
                return false;
            }

            i++;
        }

        return false;
    }

    /// <summary>
    /// Resolves placeholders in the format {key} with their corresponding configuration values.
    /// Use {{ to escape { and }} to escape } in literal text.
    /// </summary>
    /// <param name="configuration">
    /// Application configuration settings.
    /// </param>
    /// <param name="template">
    /// Template string containing placeholders.
    /// </param>
    /// <returns>
    /// String with placeholders replaced by their configuration values, or null if all placeholders
    /// are missing/null and there are no literal characters.
    /// </returns>
    private static string? ResolvePlaceholders
    (
        IConfiguration configuration,
        string template
    )
    {
        if (string.IsNullOrEmpty(template))
        {
            return template;
        }

        System.Text.StringBuilder result = new();
        bool hasLiteralContent = false;
        bool hasAnyContent = false;
        int i = 0;

        while (i < template.Length)
        {
            if (template[i] == '{')
            {
                // Check for escaped {{
                if (i + 1 < template.Length && template[i + 1] == '{')
                {
                    result.Append('{');
                    hasLiteralContent = true;
                    hasAnyContent = true;
                    i += 2;
                    continue;
                }

                // Start of placeholder
                i++; // Skip {
                System.Text.StringBuilder keyBuilder = new();
                bool foundClosing = false;

                while (i < template.Length)
                {
                    if (template[i] == '}')
                    {
                        // Check for escaped }} within placeholder
                        if (i + 1 < template.Length && template[i + 1] == '}')
                        {
                            keyBuilder.Append('}');
                            i += 2;
                            continue;
                        }

                        foundClosing = true;
                        i++; // Skip }
                        break;
                    }

                    if (template[i] == '{')
                    {
                        // Found another opening brace within placeholder, treat as literal
                        keyBuilder.Append(template[i]);
                        i++;
                        continue;
                    }

                    keyBuilder.Append(template[i]);
                    i++;
                }

                if (foundClosing)
                {
                    string configKey = keyBuilder.ToString();
                    string? configValue = configuration[configKey];

                    if (configValue != null)
                    {
                        result.Append(configValue);
                        hasAnyContent = true;
                    }
                }
                else
                {
                    // Malformed placeholder, treat as literal
                    result.Append("{").Append(keyBuilder.ToString());
                    hasLiteralContent = true;
                    hasAnyContent = true;
                }

                continue;
            }

            if (template[i] == '}' && i + 1 < template.Length && template[i + 1] == '}')
            {
                // Escaped }} outside placeholder
                result.Append('}');
                hasLiteralContent = true;
                hasAnyContent = true;
                i += 2;
                continue;
            }

            result.Append(template[i]);
            hasLiteralContent = true;
            hasAnyContent = true;
            i++;
        }

        // If there's no content at all, or only null placeholders without literals, return null
        return !hasAnyContent || (!hasLiteralContent && result.Length == 0) 
            ? null 
            : result.ToString();
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
