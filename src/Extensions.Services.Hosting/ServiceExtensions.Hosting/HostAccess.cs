using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ServiceExtensions.Hosting
{
    public static class HostAccess
    {
        public static void AddDefaultLoggingProvider(Action<ILoggingBuilder> provider)
        {
            DefaultLoggingProviders.Add(provider);
            ResetDefaultHost();
        }

        public static void AddDefaultConfigProvider(Action<IConfigurationBuilder> provider)
        {
            DefaultConfigProviders.Add(provider);
            ResetDefaultHost();
        }

        private static void ResetDefaultHost()
        {
            if (defaultHost.IsValueCreated)
            {
                var oldHost = DefaultHost;
                defaultHost = new Lazy<IHost>(CreateDefaultHost);
                oldHost.Dispose();
            }
        }

        private static IList<Action<ILoggingBuilder>> DefaultLoggingProviders { get; }
            = new List<Action<ILoggingBuilder>>();

        private static IList<Action<IConfigurationBuilder>> DefaultConfigProviders { get; }
            = new List<Action<IConfigurationBuilder>>();

        private static IHost CreateDefaultHost()
            => Host.CreateDefaultBuilder()
                   .ConfigureAppConfiguration(configBuilder =>
                   {
                       foreach (var provider in DefaultConfigProviders)
                           provider?.Invoke(configBuilder);
                   })
                   .ConfigureLogging(loggingBuilder =>
                   {
                       foreach (var provider in DefaultLoggingProviders)
                           provider?.Invoke(loggingBuilder);
                   })
                   .Build();

        private static Lazy<IHost> defaultHost
            = new Lazy<IHost>(CreateDefaultHost);

        private static IHost DefaultHost => defaultHost.Value;

        internal static IServiceProvider AmbientProvider { get; set; }

        public static IServiceProvider ServiceProvider => AmbientProvider ?? DefaultHost.Services;
    }
}
