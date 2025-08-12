using Microsoft.Extensions.Configuration;

namespace DotNetExtras.Configuration;

public static partial class AppSettings
{
    /// <summary>
    /// Groups methods that allow the application 
    /// to set the configuration settings once during startup
    /// and then get them from anywhere in the code.
    /// </summary>
    public static class Global
    {
        private static IConfiguration? _configuration = null;

        /// <summary>
        /// Returns application configuration settings initialized by the 
        /// <see cref="Set(IConfiguration)"/> method during application startup.
        /// </summary>
        /// <remarks>
        /// To make it work, call the static <see cref="Set(IConfiguration)"/> method
        /// in the application startup sequence, e.g.:
        /// <code>
        /// AppSettings.Global.Set(builder.Configuration);
        /// </code>
        /// If the <see cref="Set(IConfiguration)"/> method has not been called,
        /// this method will always return <c>null</c>.
        /// </remarks>
        /// <returns>
        /// The application configuration settings initialized by the <see cref="Set(IConfiguration)"/> method.
        /// </returns>
        public static IConfiguration? Get()
        {
            return _configuration;
        }

        /// <summary>
        /// Initializes the configuration object,
        /// so that it can be accessed from anywhere in the code.
        /// </summary>
        /// <param name="configuration">
        /// Application configuration settings.
        /// </param>
        /// <remarks>
        /// This method should be called once during the application startup, e.g.:
        /// <code>
        /// AppSettings.Global.Set(builder.Configuration);
        /// </code>
        /// Once the application settings are initialized,
        /// they can be accessed from anywhere in the application
        /// via the static <see cref="Configuration"/> property:
        /// <code>
        /// string? someValue = AppSettings.Global.Get()?["key"];
        /// </code>
        /// </remarks>
        public static void Set
        (
            IConfiguration configuration
        )
        {
            _configuration = configuration;
        }
    }
}
