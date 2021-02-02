using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ServiceExtensions.Hosting
{
    /// <summary>
    /// Manages global access to Hosting concerns.
    /// </summary>
    public static class HostAccess
    {
        /// <summary>
        /// Adds a delegate for configuring the default Host's logging functionality.
        /// </summary>
        /// <param name="loggingDelegate">The delegate for configuring the <see cref="ILoggingBuilder"/> used to construct he default Host.</param>
        public static void AddDefaultLoggingProvider(Action<ILoggingBuilder> loggingDelegate)
        {
            DefaultLoggingProviders.Add(loggingDelegate);
            ResetDefaultHost();
        }

        /// <summary>
        /// Adds a delegate for configuring the default Host's configuration functionality.
        /// </summary>
        /// <param name="configDelegate">The delegate for configuring the <see cref="IConfigurationBuilder"/> used to construct the default Host.</param>
        public static void AddDefaultConfigProvider(Action<IConfigurationBuilder> configDelegate)
        {
            DefaultConfigProviders.Add(configDelegate);
            ResetDefaultHost();
        }

        /// <summary>
        /// Adds a delegate for configuring the default Host's registered services.
        /// </summary>
        /// <param name="serviceDelegate">The delegate for configuring the <see cref="IServiceCollection"/> used to construct the default Host.</param>
        public static void AddDefaultServices(Action<IServiceCollection> serviceDelegate)
        {
            DefaultServicesProviders.Add(serviceDelegate);
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

        private static IList<Action<IServiceCollection>> DefaultServicesProviders { get; }
            = new List<Action<IServiceCollection>>();

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
                   .ConfigureServices(services =>
                   {
                       foreach (var provider in DefaultServicesProviders)
                           provider?.Invoke(services);
                   })
                   .Build();

        private static Lazy<IHost> defaultHost
            = new Lazy<IHost>(CreateDefaultHost);

        private static IHost DefaultHost => defaultHost.Value;

        private static Lazy<IServiceProvider> providerBuilder
            = new Lazy<IServiceProvider>(() => default);

        internal static IServiceCollection AmbientServices
        {
            set => providerBuilder = new Lazy<IServiceProvider>(() => value.BuildServiceProvider());
        }

        internal static IServiceProvider AmbientProvider
        {
            get => providerBuilder.Value;
            set => providerBuilder = new Lazy<IServiceProvider>(() => value);
        }

        /// <summary>
        /// The global Service Provider.
        /// </summary>
        public static IServiceProvider ServiceProvider => AmbientProvider ?? DefaultHost.Services;
    }
}
