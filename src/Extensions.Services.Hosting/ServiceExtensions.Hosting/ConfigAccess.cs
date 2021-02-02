using Microsoft.Extensions.DependencyInjection;
using ServiceExtensions.Hosting;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Methods exposing global access to configuration.
    /// </summary>
    public static class ConfigAccess
    {
        private static IConfiguration Configuration
            => HostAccess.ServiceProvider.GetRequiredService<IConfiguration>();

        /// <summary>
        /// Retrieves a Connection String with the provided <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the Connection String.</param>
        /// <returns>The matching Connection String, if it exists.</returns>
        public static string ConnString(string name)
            => Configuration.GetConnectionString(name);

        /// <summary>
        /// Retrieves a value from the AppSettings section of configuration matching the provided <paramref name="key"/>.
        /// </summary>
        /// <typeparam name="TValue">Type to convert the value to.</typeparam>
        /// <param name="key">The key to retrieve a value for.</param>
        /// <param name="defaultValue">Value to return if the key doesn't exist.</param>
        /// <returns>The resulting value, if it exists. Otherwise, the provided <paramref name="defaultValue"/>.</returns>
        public static TValue AppSetting<TValue>(string key, TValue defaultValue)
            => Configuration.GetSection("AppSettings").GetValue(key, defaultValue);

        /// <summary>
        /// Retrieves a configuration section, binding it to the provided Type.
        /// </summary>
        /// <typeparam name="TValue">The type to bind.</typeparam>
        /// <param name="sectionName">The name of the configuration section.</param>
        /// <returns>The configuration, or the default <typeparamref name="TValue"/> if it doesn't exist.</returns>
        public static TValue GetConfig<TValue>(string sectionName)
            where TValue : new()
            => Configuration.GetSection(sectionName).Get<TValue>() ?? new TValue();

        /// <summary>
        /// Retrieves a value from a subsection of the configuration.
        /// </summary>
        /// <typeparam name="TValue">Type to bind the configuration to.</typeparam>
        /// <param name="sectionName">The configuration section.</param>
        /// <param name="key">The key to retrieve a value for.</param>
        /// <param name="defaultValue">Value to return if the key doesn't exist.</param>
        /// <returns>The resulting value, if it exists. Otherwise, the provided <paramref name="defaultValue"/>.</returns>
        public static TValue GetConfig<TValue>(string sectionName, string key, TValue defaultValue)
            => Configuration.GetSection(sectionName).GetValue(key, defaultValue);
    }
}
