using Microsoft.Extensions.DependencyInjection;
using ServiceExtensions.Hosting;
using System;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// Extension methods for exposing ambient services to the globally accessible service container.
    /// </summary>
    public static class HostAccessExtensions
    {
        /// <summary>
        /// Registers the provided <paramref name="services"/> for global access.
        /// </summary>
        /// <param name="services">Instance of Startup services.</param>
        /// <returns>The input services.</returns>
        public static IServiceCollection ConfigureHosting(this IServiceCollection services)
        {
            HostAccess.AmbientServices = services;
            return services;
        }

        /// <summary>
        /// Registers the <paramref name="provider"/> for global access.
        /// </summary>
        /// <param name="provider">Instance of a provider exposed during Startup.</param>
        /// <returns>The input provider.</returns>
        public static IServiceProvider ConfigureHosting(this IServiceProvider provider)
        {
            HostAccess.AmbientProvider = provider;
            return provider;
        }

        /// <summary>
        /// Registers the <paramref name="host"/> for global access.
        /// </summary>
        /// <param name="host">Instance of a Host exposed in Main.</param>
        /// <returns>The input host.</returns>
        public static IHost ConfigureHosting(this IHost host)
        {
            HostAccess.AmbientHost = host;
            return host;
        }
    }
}
