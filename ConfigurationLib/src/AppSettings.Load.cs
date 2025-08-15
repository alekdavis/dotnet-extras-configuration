// Ignore Spelling: json

using Microsoft.Extensions.Configuration;
using System.Text;

namespace DotNetExtras.Configuration;

public static partial class AppSettings
{
    /// <summary>
    /// Groups methods that load the application settings form different sources.
    /// </summary>
    public static class Load
    {
        /// <summary>
        /// Loads the application configuration settings from a dictionary.
        /// </summary>
        /// <param name="dictionary">
        /// A dictionary containing the application settings.
        /// </param>
        /// <returns>
        /// Loaded application configuration settings.
        /// </returns>
        /// <remarks>
        /// You can pass array items by appending zero-based index to the key as illustrated
        /// in the example (e.g."ServiceA:ArraySetting1:0", "ServiceA:ArraySetting1:1", etc.).
        /// See
        /// <see href="https://stackoverflow.com/questions/37825107/net-core-use-configuration-to-bind-to-options-with-array"/>
        /// for details.
        /// </remarks>
        /// <example>
        /// <code>
        /// IConfiguration config = AppConfiguration.Load.FromDictionary(
        ///     new Dictionary&lt;string,string?&gt;
        ///     {
        ///         {"ValueSettingA", "Value1"},
        ///         {"SectionB:ValueSettingX", "ValueX"},
        ///         {"SectionB:ValueSettingY", "ValueY"},
        ///         {"SectionB:ValueSettingZ", "ValueZ"},
        ///         {"SectionC:ArraySetting1:0", "Value0"},
        ///         {"SectionC:ArraySetting1:1", "Value1"},
        ///         {"SectionC:ArraySetting1:2", "Value2"},
        ///     }
        /// );
        /// </code>
        /// </example>
        public static IConfiguration FromDictionary
        (
            IEnumerable<KeyValuePair<string, string?>> dictionary
        )
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(dictionary)
                .Build();
        }

        /// <summary>
        /// Loads the application configuration settings from a JSON string.
        /// </summary>
        /// <param name="json">
        /// A JSON string containing the application settings.
        /// </param>
        /// <returns>
        /// Loaded application configuration settings.
        /// </returns>
        public static IConfiguration FromJsonString
        (
            string json
        )
        {
            using MemoryStream stream = new (Encoding.UTF8.GetBytes(json));
            return new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();
        }

        /// <summary>
        /// Loads the application configuration settings from a JSON file.
        /// </summary>
        /// <param name="filePath">
        /// Path to the JSON file containing the application settings.
        /// </param>
        /// <returns>
        /// Loaded application configuration settings.
        /// </returns>
        public static IConfiguration FromJsonFile
        (
            string filePath
        )
        {
            return new ConfigurationBuilder()
                .SetBasePath(new FileInfo(filePath).Directory?.FullName ?? Path.GetFullPath("."))
                .AddJsonFile(Path.GetFileName(filePath), optional: false, reloadOnChange: false)
                .Build();
        }
    }
}
