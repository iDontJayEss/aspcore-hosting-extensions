using Microsoft.Extensions.DependencyInjection;
using ServiceExtensions.Hosting;

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigAccessExtensions
    {
        private static IConfiguration Configuration
            => HostAccess.ServiceProvider.GetRequiredService<IConfiguration>();

        public static string ConnString(string key)
            => Configuration.GetConnectionString(key);

        public static TValue AppSetting<TValue>(string key, TValue defaultValue)
            => Configuration.GetSection("AppSettings").GetValue(key, defaultValue);

        public static TValue GetConfig<TValue>(string sectionName)
            => Configuration.GetSection(sectionName).Get<TValue>();

        public static TValue GetConfig<TValue>(string sectionName, string key, TValue defaultValue)
            => Configuration.GetSection(sectionName).GetValue(key, defaultValue);
    }
}
